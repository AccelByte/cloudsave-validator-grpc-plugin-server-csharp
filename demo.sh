#!/usr/bin/env bash

# cloudsave validator demo script

# Requires: bash curl jq

set -e
set -o pipefail

test -n "$AB_CLIENT_ID" || (echo "AB_CLIENT_ID is not set"; exit 1)
test -n "$AB_CLIENT_SECRET" || (echo "AB_CLIENT_SECRET is not set"; exit 1)
test -n "$AB_NAMESPACE" || (echo "AB_NAMESPACE is not set"; exit 1)

DEMO_PREFIX='cloudsave_grpc_demo'
GRPC_SERVER_URL="$(echo "$GRPC_SERVER_URL" | sed 's@^.*/@@')"   # Remove leading tcp:// if any

get_code_verifier() 
{
  echo $RANDOM | sha256sum | cut -d ' ' -f 1   # For demo only: In reality, it needs to be secure random
}

get_code_challenge()
{
  echo -n "$1" | sha256sum | xxd -r -p | base64 -w 0 | sed -e 's/\+/-/g' -e 's/\//\_/g' -e 's/=//g'
}

clean_up()
{
  echo Deleting player ...
  curl -X DELETE "${AB_BASE_URL}/iam/v3/admin/namespaces/$AB_NAMESPACE/users/$USER_ID/information" -H "Authorization: Bearer $ACCESS_TOKEN"

  echo Resetting cloudsave validator ...

  curl -X DELETE -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/plugins" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' >/dev/null
}

trap clean_up EXIT

echo Logging in client ...

ACCESS_TOKEN="$(curl -s ${AB_BASE_URL}/iam/v3/oauth/token -H 'Content-Type: application/x-www-form-urlencoded' -u "$AB_CLIENT_ID:$AB_CLIENT_SECRET" -d "grant_type=client_credentials" | jq --raw-output .access_token)"

if [ -n "$GRPC_SERVER_URL" ]; then
  echo Registering cloudsave validator \(replace exising\) $GRPC_SERVER_URL ...

  curl -X DELETE -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/plugins" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -H 'Content-Type: application/json' >/dev/null     # Ignore delete error

  curl -X POST -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/plugins" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -H 'Content-Type: application/json' \
      -d "{\"customConfig\":{\"GRPCAddress\":\"${GRPC_SERVER_URL}\"},\"customFunction\":{\"afterReadGameRecord\":true,\"beforeWritePlayerRecord\":true},\"extendType\":\"CUSTOM\"}" >/dev/null
elif [ -n "$EXTEND_APP_NAME" ]; then
  echo Registering cloudsave validator \(replace exising\) $EXTEND_APP_NAME ...

  curl -X DELETE -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/plugins" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -H 'Content-Type: application/json' >/dev/null     # Ignore delete error

  curl -X POST -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/plugins" \
      -H "Authorization: Bearer $ACCESS_TOKEN" \
      -H 'Content-Type: application/json' \
      -d "{\"appConfig\":{\"appName\":\"$EXTEND_APP_NAME\"},\"customFunction\":{\"afterReadGameRecord\":true,\"beforeWritePlayerRecord\":true},\"extendType\":\"APP\"}" >/dev/null
else
  echo "GRPC_SERVER_URL or EXTEND_APP_NAME is not set"
  exit 1
fi
echo Creating PLAYER ...

USER_ID="$(curl -s "${AB_BASE_URL}/iam/v4/public/namespaces/$AB_NAMESPACE/users" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d "{\"authType\":\"EMAILPASSWD\",\"country\":\"ID\",\"dateOfBirth\":\"1995-01-10\",\"displayName\":\"Cloudsave gRPC Player\",\"emailAddress\":\"${DEMO_PREFIX}_player@test.com\",\"password\":\"GFPPlmdb2-\",\"username\":\"${DEMO_PREFIX}_player\"}" | jq --raw-output .userId)"

if [ "$USER_ID" == "null" ]; then
  echo "Failed to create player with email ${DEMO_PREFIX}_player@test.com, please delete existing first!"
  exit 1
fi

echo Test BeforeWritePlayerRecord an VALID payload ... 

curl -X PUT -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/users/$USER_ID/records/favourite_weapon" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d '{"userId": "1e076bcee6d14c849ffb121c0e0135be", "favouriteWeaponType": "SWORD", "favouriteWeapon": "excalibur"}'
echo

echo Test BeforeWritePlayerRecord an INVALID payload ... 

curl -X PUT -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/users/$USER_ID/records/favourite_weapon" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d '{"foo":"bar"}' || true
echo

# echo Logging in PLAYER ...

# CODE_VERIFIER="$(get_code_verifier)" 

# REQUEST_ID="$(curl -s -D - "${AB_BASE_URL}/iam/v3/oauth/authorize?scope=commerce+account+social+publishing+analytics&response_type=code&code_challenge_method=S256&code_challenge=$(get_code_challenge "$CODE_VERIFIER")&client_id=$AB_CLIENT_ID" | grep -o 'request_id=[a-f0-9]\+' | cut -d= -f2)"

# CODE="$(curl -s -D - ${AB_BASE_URL}/iam/v3/authenticate -H 'Content-Type: application/x-www-form-urlencoded' -d "password=GFPPlmdb2-&user_name=${DEMO_PREFIX}_player_$PLAYER_NUMBER@test.com&request_id=$REQUEST_ID&client_id=$AB_CLIENT_ID" | grep -o 'code=[a-f0-9]\+' | cut -d= -f2)"

# PLAYER_TOKEN_RESPONSE="$(curl -s ${AB_BASE_URL}/iam/v3/oauth/token -H 'Content-Type: application/x-www-form-urlencoded' -u "$AB_CLIENT_ID:$AB_CLIENT_SECRET" -d "code=$CODE&grant_type=authorization_code&client_id=$AB_CLIENT_ID&code_verifier=$CODE_VERIFIER")"

# PLAYER_ACCESS_TOKEN="$(echo "$PLAYER_TOKEN_RESPONSE" | jq --raw-output .access_token)"

# echo Test AfterReadGameRecord an INVALID payload ... 

# curl -X PUT -s "${AB_BASE_URL}/cloudsave/v1/admin/namespaces/$AB_NAMESPACE/records/daily_msg" -H "Authorization: Bearer $ACCESS_TOKEN" -H 'Content-Type: application/json' -d '{"message":"mymsg","title":"mytitle","availableOn":"2023-08-13T00:00:00.000Z"}' >/dev/null

# curl -s "${AB_BASE_URL}/cloudsave/v1/namespaces/$AB_NAMESPACE/records/daily_msg" -H "Authorization: Bearer $PLAYER_ACCESS_TOKEN" -H 'Content-Type: application/json' || true
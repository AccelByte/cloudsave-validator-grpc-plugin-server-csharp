// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Text.Json.Serialization;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Model
{
    public class CustomPlayerRecord
    {
        [JsonPropertyName("userId")]
        [JsonRequired]
        public string UserID { get; set; } = String.Empty;

        [JsonPropertyName("favouriteWeaponType")]
        [JsonRequired]
        public string FavouriteWeaponType { get; set; } = String.Empty;

        [JsonPropertyName("favouriteWeapon")]
        [JsonRequired]
        public string FavouriteWeapon { get; set; } = String.Empty;

        public bool Validate()
        {
            if (UserID == "")
                throw new Exception("userId cannot be empty.");
            if (FavouriteWeaponType == "")
                throw new Exception("favouriteWeaponType cannot be empty.");
            if (FavouriteWeapon == "")
                throw new Exception("favouriteWeapon cannot be empty.");
            return true;
        }
    }
}

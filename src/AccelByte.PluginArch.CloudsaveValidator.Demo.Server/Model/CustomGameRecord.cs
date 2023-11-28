// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Text.Json.Serialization;

namespace AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Model
{
    public class CustomGameRecord
    {
        [JsonPropertyName("locationId")]
        [JsonRequired]
        public string LocationID { get; set; } = String.Empty;

        [JsonPropertyName("name")]
        [JsonRequired]
        public string Name { get; set; } = String.Empty;

        [JsonPropertyName("totalResources")]
        [JsonRequired]
        public int TotalResources { get; set; } = 0;

        [JsonPropertyName("totalEnemy")]
        [JsonRequired]
        public int TotalEnemy { get; set; } = 0;

        public bool Validate()
        {
            if (LocationID == "")
                throw new Exception("locationId cannot be empty.");
            if (Name == "")
                throw new Exception("name cannot be empty");

            return true;
        }
    }
}

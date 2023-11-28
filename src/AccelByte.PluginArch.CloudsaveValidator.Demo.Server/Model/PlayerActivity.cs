// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Text.Json.Serialization;

namespace AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Model
{
    public class PlayerActivity
    {
        [JsonPropertyName("userId")]
        [JsonRequired]
        public string UserID { get; set; } = String.Empty;

        [JsonPropertyName("activity")]
        [JsonRequired]
        public string Activity { get; set; } = String.Empty;

        public bool Validate()
        {
            if (UserID == "")
                throw new Exception("userId cannot be empty.");
            if (Activity == "")
                throw new Exception("activity cannot be empty.");
            return true;
        }
    }
}

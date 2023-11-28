// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Text.Json.Serialization;

namespace AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Model
{
    public class DailyMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = String.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = String.Empty;

        [JsonPropertyName("availableOn")]
        public DateTime? AvailableOn { get; set; } = null;
    }
}

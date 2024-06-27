// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Grpc.Core;
using AccelByte.Cloudsave.Validator;
using AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Model;

namespace AccelByte.PluginArch.CloudsaveValidator.Demo.Server.Services
{
    public class CloudSaveValidatorService : CloudsaveValidatorService.CloudsaveValidatorServiceBase
    {
        private readonly ILogger<CloudSaveValidatorService> _Logger;        

        private static readonly HttpClient _HttpClient = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = false
        });

        private long _MaxSizeEventBannerInKB = 100;

        public CloudSaveValidatorService(ILogger<CloudSaveValidatorService> logger)
        {
            _Logger = logger;
        }

        public override Task<GameRecordValidationResult> BeforeWriteGameRecord(GameRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("map", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string jsonPayload = request.Payload.ToString(Encoding.UTF8)!;
                    CustomGameRecord? customGameRecord = JsonSerializer.Deserialize<CustomGameRecord>(jsonPayload);
                    if (customGameRecord == null)
                        throw new Exception("Payload deserialization failed. Null result.");

                    customGameRecord.Validate();
                }
                catch (Exception x)
                {
                    return Task.FromResult(new GameRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new GameRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key
            });
        }

        public override Task<PlayerRecordValidationResult> BeforeWritePlayerRecord(PlayerRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("favourite_weapon", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string jsonPayload = request.Payload.ToString(Encoding.UTF8)!;
                    CustomPlayerRecord? customPlayerRecord = JsonSerializer.Deserialize<CustomPlayerRecord>(jsonPayload);
                    if (customPlayerRecord == null)
                        throw new Exception("Payload deserialization failed. Null result.");

                    customPlayerRecord.Validate();
                }
                catch (Exception x)
                {
                    return Task.FromResult(new PlayerRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        UserId = request.UserId,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new PlayerRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key,
                UserId = request.UserId
            });
        }

        public override Task<GameRecordValidationResult> BeforeWriteAdminGameRecord(AdminGameRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("map", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string jsonPayload = request.Payload.ToString(Encoding.UTF8)!;
                    CustomGameRecord? customGameRecord = JsonSerializer.Deserialize<CustomGameRecord>(jsonPayload);
                    if (customGameRecord == null)
                        throw new Exception("Payload deserialization failed. Null result.");

                    customGameRecord.Validate();
                }
                catch (Exception x)
                {
                    return Task.FromResult(new GameRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new GameRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key
            });
        }

        public override Task<PlayerRecordValidationResult> BeforeWriteAdminPlayerRecord(AdminPlayerRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("player_activity", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string jsonPayload = request.Payload.ToString(Encoding.UTF8)!;
                    PlayerActivity? playerActivity = JsonSerializer.Deserialize<PlayerActivity>(jsonPayload);
                    if (playerActivity == null)
                        throw new Exception("Payload deserialization failed. Null result.");

                    playerActivity.Validate();
                }
                catch (Exception x)
                {
                    return Task.FromResult(new PlayerRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        UserId = request.UserId,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new PlayerRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key,
                UserId = request.UserId
            });
        }

        public override Task<GameRecordValidationResult> AfterReadGameRecord(GameRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("daily_msg",StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string jsonPayload = request.Payload.ToString(Encoding.UTF8)!;
                    DailyMessage? dailyMessage = JsonSerializer.Deserialize<DailyMessage>(jsonPayload);
                    if (dailyMessage == null)
                        throw new Exception("Payload deserialization failed. Null result.");                   

                    if (dailyMessage.AvailableOn != null)
                    {
                        if (DateTime.Now < dailyMessage.AvailableOn.Value)
                        {
                            return Task.FromResult(new GameRecordValidationResult()
                            {
                                IsSuccess = false,
                                Key = request.Key,
                                Error = new Error()
                                {
                                    ErrorCode = 2,
                                    ErrorMessage = "not accessible yet"
                                }
                            });
                        }
                    }
                }
                catch (Exception x)
                {
                    return Task.FromResult(new GameRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new GameRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key
            });
        }

        public override Task<PlayerRecordValidationResult> AfterReadPlayerRecord(PlayerRecord request, ServerCallContext context)
        {
            return Task.FromResult(new PlayerRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key,
                UserId = request.UserId
            });
        }

        public override Task<BulkGameRecordValidationResult> AfterBulkReadGameRecord(BulkGameRecord request, ServerCallContext context)
        {
            BulkGameRecordValidationResult result = new BulkGameRecordValidationResult();
            foreach (var gameRecord in request.GameRecords)
            {
                if (gameRecord.Key.EndsWith("daily_msg", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string jsonPayload = gameRecord.Payload.ToString(Encoding.UTF8)!;
                        DailyMessage? dailyMessage = JsonSerializer.Deserialize<DailyMessage>(jsonPayload);
                        if (dailyMessage == null)
                            throw new Exception("Payload deserialization failed. Null result.");

                        if (dailyMessage.AvailableOn != null)
                        {
                            if (DateTime.Now < dailyMessage.AvailableOn.Value)
                            {
                                result.ValidationResults.Add(new GameRecordValidationResult()
                                {
                                    IsSuccess = false,
                                    Key = gameRecord.Key,
                                    Error = new Error()
                                    {
                                        ErrorCode = 2,
                                        ErrorMessage = "not accessible yet"
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        result.ValidationResults.Add(new GameRecordValidationResult()
                        {
                            IsSuccess = false,
                            Key = gameRecord.Key,
                            Error = new Error()
                            {
                                ErrorCode = 1,
                                ErrorMessage = x.Message
                            }
                        });
                    }
                }

                result.ValidationResults.Add(new GameRecordValidationResult()
                {
                    IsSuccess = true,
                    Key = gameRecord.Key
                });
            }

            return Task.FromResult(result);
        }

        public override Task<BulkPlayerRecordValidationResult> AfterBulkReadPlayerRecord(BulkPlayerRecord request, ServerCallContext context)
        {
            BulkPlayerRecordValidationResult result = new BulkPlayerRecordValidationResult();
            foreach (var playerRecord in request.PlayerRecords)
            {
                result.ValidationResults.Add(new PlayerRecordValidationResult()
                {
                    IsSuccess = true,
                    Key = playerRecord.Key,
                    UserId = playerRecord.UserId
                });
            }

            return Task.FromResult(result);
        }

        public override async Task<GameRecordValidationResult> BeforeWriteGameBinaryRecord(GameBinaryRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("event_banner", StringComparison.OrdinalIgnoreCase)
                && request.BinaryInfo != null)
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, request.BinaryInfo.Url);
                var resp = await _HttpClient.SendAsync(req);

                var contentLengthInKB = resp.Content.Headers.ContentLength ?? 0 / 1000;
                if (contentLengthInKB > _MaxSizeEventBannerInKB)
                    throw new Exception($"maximum size for event banner is {_MaxSizeEventBannerInKB} kB");
            }

            return await Task.FromResult(new GameRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key
            });
        }

        public override Task<GameRecordValidationResult> AfterReadGameBinaryRecord(GameBinaryRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("daily_event_stage", StringComparison.OrdinalIgnoreCase)
                && request.BinaryInfo != null)
            {
                try
                {
                    var binaryDto = request.BinaryInfo.UpdatedAt.ToDateTime();
                    var currentDto = DateTime.UtcNow;

                    if (binaryDto.ToShortDateString() != currentDto.ToShortDateString())
                        throw new Exception($"Today's {request.Key} is not ready yet");
                }
                catch (Exception x)
                {
                    return Task.FromResult(new GameRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new GameRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key
            });
        }

        public override Task<BulkGameRecordValidationResult> AfterBulkReadGameBinaryRecord(BulkGameBinaryRecord request, ServerCallContext context)
        {
            BulkGameRecordValidationResult result = new BulkGameRecordValidationResult();
            foreach (var record in request.GameBinaryRecords)
            {
                if (record.Key.EndsWith("daily_event_stage", StringComparison.OrdinalIgnoreCase)
                    && record.BinaryInfo != null)
                {
                    try
                    {
                        var binaryDto = record.BinaryInfo.UpdatedAt.ToDateTime();
                        var currentDto = DateTime.UtcNow;

                        if (binaryDto.ToShortDateString() != currentDto.ToShortDateString())
                            throw new Exception($"Today's {record.Key} is not ready yet");
                    }
                    catch (Exception x)
                    {
                        result.ValidationResults.Add(new GameRecordValidationResult()
                        {
                            IsSuccess = false,
                            Key = record.Key,
                            Error = new Error()
                            {
                                ErrorCode = 1,
                                ErrorMessage = x.Message
                            }
                        });
                    }
                }

                result.ValidationResults.Add(new GameRecordValidationResult()
                {
                    IsSuccess = true,
                    Key = record.Key
                });
            }

            return Task.FromResult(result);
        }

        public override Task<PlayerRecordValidationResult> BeforeWritePlayerBinaryRecord(PlayerBinaryRecord request, ServerCallContext context)
        {
            if (request.Key.EndsWith("id_card", StringComparison.OrdinalIgnoreCase)
                && (request.BinaryInfo != null))
            {
                try
                {
                    if (request.BinaryInfo.Version > 1)
                        throw new Exception("id card can only be created once");
                }
                catch (Exception x)
                {
                    return Task.FromResult(new PlayerRecordValidationResult()
                    {
                        IsSuccess = false,
                        Key = request.Key,
                        UserId = request.UserId,
                        Error = new Error()
                        {
                            ErrorCode = 1,
                            ErrorMessage = x.Message
                        }
                    });
                }
            }

            return Task.FromResult(new PlayerRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key,
                UserId = request.UserId
            });
        }

        public override Task<PlayerRecordValidationResult> AfterReadPlayerBinaryRecord(PlayerBinaryRecord request, ServerCallContext context)
        {
            return Task.FromResult(new PlayerRecordValidationResult()
            {
                IsSuccess = true,
                Key = request.Key,
                UserId = request.UserId
            });
        }

        public override Task<BulkPlayerRecordValidationResult> AfterBulkReadPlayerBinaryRecord(BulkPlayerBinaryRecord request, ServerCallContext context)
        {
            BulkPlayerRecordValidationResult result = new BulkPlayerRecordValidationResult();
            foreach (var playerRecord in request.PlayerBinaryRecords)
            {
                result.ValidationResults.Add(new PlayerRecordValidationResult()
                {
                    IsSuccess = true,
                    Key = playerRecord.Key,
                    UserId = playerRecord.UserId
                });
            }

            return Task.FromResult(result);
        }
    }
}

// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    }
}

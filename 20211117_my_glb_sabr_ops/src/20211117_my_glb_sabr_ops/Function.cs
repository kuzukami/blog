using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211117_my_glb_sabr_ops
{
    public class Function
    {
        public GlbResponse FunctionHandler(object input, ILambdaContext context)
        {
            try
            {
                GlbRequest glbRequest               = JsonSerializer.Deserialize<GlbRequest>(input.ToString(), GlbUtil.GetJsonSerializerOptionsDefault());
                GlbRequestHeader glbRequestHeader   = glbRequest.Header;
                GlbRequestBody glbRequestBody       = glbRequest.Body;

                GlbResponse glbResponse             = new GlbResponse();

                GlbResponseBody getActionResponse = GetAction(glbRequestBody);

                GlbResponseHeader glbResponseHeader = new GlbResponseHeader();
                glbResponseHeader.ResultCode        = GlbUtil.RESULT_CODE_SUCCESS;
                glbResponseHeader.ResultMessage     = GlbUtil.GetResultCodeDictionary()[GlbUtil.RESULT_CODE_SUCCESS];
                glbResponse.Header                  = JsonSerializer.Serialize(glbResponseHeader);
                glbResponse.Body                    = JsonSerializer.Serialize(getActionResponse);

                return glbResponse;
            }
            catch (System.Exception e)
            {
                GlbResponse glbResponse             = new GlbResponse();

                GlbResponseHeader glbResponseHeader = new GlbResponseHeader();
                glbResponseHeader.ResultCode        = GlbUtil.RESULT_CODE_ERROR;
                glbResponseHeader.ResultMessage     = GlbUtil.GetResultCodeDictionary()[GlbUtil.RESULT_CODE_ERROR] + "::" + e.Message + "::" + e.StackTrace;
                glbResponse.Header                  = JsonSerializer.Serialize(glbResponseHeader);
                glbResponse.Body                    = "";

                return glbResponse;
            }
        }

        public GlbResponseBody GetAction(GlbRequestBody glbRequestBody)
        {
            try
            {
                int argAtBat        = int.Parse(glbRequestBody.AtBat);
                int argSacrificeFly = int.Parse(glbRequestBody.SacrificeFly);
                int argWalks        = int.Parse(glbRequestBody.Walks);
                int argDeadBall     = int.Parse(glbRequestBody.DeadBall);
                int argSingle       = int.Parse(glbRequestBody.Single);
                int argDouble       = int.Parse(glbRequestBody.Double);
                int argTriple       = int.Parse(glbRequestBody.Triple);
                int argHomeRun      = int.Parse(glbRequestBody.HomeRun);

                int hits       = argSingle + argDouble + argTriple + argHomeRun;
                int totalBases = argSingle * 1 + argDouble * 2 + argTriple * 3 + argHomeRun * 4;

                double obp = 1.0 * (hits + argWalks + argDeadBall) / (argAtBat + argWalks + argDeadBall + argSacrificeFly);
                double slg = 1.0 * totalBases / argAtBat;

                GlbResponseBody glbResponseBody = new GlbResponseBody();

                // calc ops
                glbResponseBody.Ops = (obp + slg).ToString("F3");

                return glbResponseBody;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }

    #region utils

    public static class GlbUtil
    {
        #region result code

        public const string RESULT_CODE_SUCCESS    = "0000";
        public const string RESULT_CODE_ERROR      = "9999";
        public const string RESULT_MESSAGE_SUCCESS = "SUCCESS";
        public const string RESULT_MESSAGE_ERROR   = "ERROR OCCURED";

        public static ReadOnlyDictionary<string, string> GetResultCodeDictionary()
        {
            try
            {
                var resultCodeDictionary = new Dictionary<string, string>
                {
                    { RESULT_CODE_SUCCESS, RESULT_MESSAGE_SUCCESS },
                    { RESULT_CODE_ERROR,   RESULT_MESSAGE_ERROR },
                };

                var resultCodeDictionaryRo = new ReadOnlyDictionary<string, string>( resultCodeDictionary );

                return resultCodeDictionaryRo;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        #endregion result code  

        #region serializer

        public static JsonSerializerOptions GetJsonSerializerOptionsDefault()
        {
            try
            {
                return new JsonSerializerOptions()
                {
                    IgnoreNullValues            = true,
                    PropertyNameCaseInsensitive = true
                };
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        #endregion serializer
    }

    #endregion utils

    #region glb request

    public class GlbRequest
    {
        [JsonPropertyName("header")]
        public GlbRequestHeader Header { get; set; }

        [JsonPropertyName("body")]
        public GlbRequestBody Body { get; set; }
    }

    public class GlbRequestHeader
    {
        [JsonPropertyName("header_item")]
        public string HeaderItem { get; set; }
    }

    public class GlbRequestBody
    {
        [JsonPropertyName("at_bat")]
        public string AtBat { get; set; }

        [JsonPropertyName("sacrifice_fly")]
        public string SacrificeFly { get; set; }

        [JsonPropertyName("walks")]
        public string Walks { get; set; }

        [JsonPropertyName("dead_ball")]
        public string DeadBall { get; set; }

        [JsonPropertyName("single")]
        public string Single { get; set; }

        [JsonPropertyName("double")]
        public string Double { get; set; }

        [JsonPropertyName("triple")]
        public string Triple { get; set; }

        [JsonPropertyName("home_run")]
        public string HomeRun { get; set; }
    }

    #endregion glb request

    #region glb response

    public class GlbResponse
    {
        [JsonPropertyName("header")]
        public string Header { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public class GlbResponseHeader
    {
        [JsonPropertyName("result_code")]
        public string ResultCode { get; set; }

        [JsonPropertyName("result_message")]
        public string ResultMessage { get; set; }
    }

    public class GlbResponseBody
    {
        [JsonPropertyName("ops")]
        public string Ops { get; set; }
    }

    #endregion glb response
}
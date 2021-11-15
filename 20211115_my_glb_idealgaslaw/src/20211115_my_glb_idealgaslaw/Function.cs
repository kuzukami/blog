using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

using Amazon.Lambda.Core;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211115_my_glb_idealgaslaw
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

                GlbResponseHeader glbResponseHeader = new GlbResponseHeader();
                glbResponseHeader.ResultCode        = GlbUtil.RESULT_CODE_SUCCESS;
                glbResponse.Header                  = JsonSerializer.Serialize(glbResponseHeader);

                GlbResponseBody glbResponseBody     = GetAction(glbRequestBody);
                glbResponse.Body                    = JsonSerializer.Serialize(glbResponseBody);

                return glbResponse;
            }
            catch (System.Exception e)
            {
                GlbResponse glbResponse             = new GlbResponse();

                GlbResponseHeader glbResponseHeader = new GlbResponseHeader();
                glbResponseHeader.ResultCode        = GlbUtil.RESULT_CODE_ERROR;
                glbResponse.Header                  = JsonSerializer.Serialize(glbResponseHeader);

                GlbResponseBody glbResponseBody     = new GlbResponseBody();
                glbResponseBody.Message             = GlbUtil.GetResultCodeDictionary()[GlbUtil.RESULT_CODE_ERROR] + "::" + e.Message + "::" + e.StackTrace;
                glbResponse.Body                    = JsonSerializer.Serialize(glbResponseBody);

                return glbResponse;
            }
        }

        public GlbResponseBody GetAction(GlbRequestBody glbRequestBody)
        {
            try
            {
                string argPressure   = glbRequestBody.Pressure;
                string argVokume     = glbRequestBody.Volume;
                string argTemprtures = glbRequestBody.Temperture;

                GlbResponseBody glbResponseBody = new GlbResponseBody();

                glbResponseBody.Message = "I'm trying calculation.";

                // calc ideal gas raw
                glbResponseBody.Mol = ((double.Parse(argPressure) * double.Parse(argVokume)) / (GlbUtil.R * double.Parse(argVokume))).ToString("F2");

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
        #region const

        public const double R = 8.31446261815324;

        #endregion const

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
                    { RESULT_CODE_ERROR,   RESULT_MESSAGE_ERROR   },
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
        [JsonPropertyName("pressure")]
        public string Pressure { get; set; }

        [JsonPropertyName("volume")]
        public string Volume { get; set; }

        [JsonPropertyName("temperture")]
        public string Temperture { get; set; }
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
    }

    public class GlbResponseBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("mol")]
        public string Mol { get; set; }
    }

    #endregion glb response
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211117_my_glb_s3_signedurl
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
                var s3Client = new AmazonS3Client(RegionEndpoint.APNortheast1);
                var request  = new Amazon.S3.Model.GetPreSignedUrlRequest 
                {
                    BucketName  = GlbUtil.S3_NOMURABBIT_BLOG_XXX,
                    Key         = glbRequestBody.Key,
                    Verb        = HttpVerb.GET,
                    //Verb        = HttpVerb.PUT,
                    Expires     = DateTime.UtcNow.AddMinutes(5)
                };

                string response = s3Client.GetPreSignedURL(request);

                GlbResponseBody glbResponseBody = new GlbResponseBody();

                glbResponseBody.SignedUrl       = response;

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
        #region s3 backet
        public const string S3_NOMURABBIT_BLOG_XXX = "nomurabbit-blog-xxx";

        #endregion s3 backet

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
        [JsonPropertyName("key")]
        public string Key { get; set; }
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
        [JsonPropertyName("signed_url")]
        public string SignedUrl { get; set; }
    }

    #endregion glb response
}
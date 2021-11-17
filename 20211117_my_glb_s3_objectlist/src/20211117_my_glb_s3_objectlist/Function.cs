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

namespace _20211117_my_glb_s3_objectlist
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

                List<GlbResponseBody> getActionResponse = GetAction(glbRequestBody);

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

        public List<GlbResponseBody> GetAction(GlbRequestBody glbRequestBody)
        {
            try
            {
                var s3Client = new AmazonS3Client(RegionEndpoint.APNortheast1);
                var request  = new Amazon.S3.Model.ListObjectsV2Request
                {
                    BucketName  = GlbUtil.S3_NOMURABBIT_BLOG_XXX,
                    Prefix      = glbRequestBody.Prefix
                };

                Amazon.S3.Model.ListObjectsV2Response response = s3Client.ListObjectsV2Async(request).Result;
                List<Amazon.S3.Model.S3Object> s3Objects = response.S3Objects;

                List<GlbResponseBody> glbResponseBodyList = new List<GlbResponseBody>();
                GlbResponseBody glbResponseBody = new GlbResponseBody();

                foreach(Amazon.S3.Model.S3Object s3Object in s3Objects)
                {
                    glbResponseBody = new GlbResponseBody();
                    glbResponseBody.BucketName       = s3Object.BucketName;
                    glbResponseBody.ETag             = s3Object.ETag;
                    glbResponseBody.Key              = s3Object.Key;
                    glbResponseBody.LastModified     = s3Object.LastModified.ToString("yyyyMMddHHmmssfff");
                    glbResponseBody.OwnerDisplayName = (s3Object.Owner == null) ? " " : s3Object.Owner.DisplayName;
                    glbResponseBody.OwnerId          = (s3Object.Owner == null) ? " " : s3Object.Owner.Id;
                    glbResponseBody.Size             = s3Object.Size.ToString();
                    glbResponseBody.StorageClass     = s3Object.StorageClass.Value;
                    glbResponseBodyList.Add(glbResponseBody);
                }

                return glbResponseBodyList;

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
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }
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
        [JsonPropertyName("bucketName")]
        public string BucketName { get; set; }

        [JsonPropertyName("etag")]
        public string ETag { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("last_modified")]
        public string LastModified { get; set; }

        [JsonPropertyName("owner_display_name")]
        public string OwnerDisplayName { get; set; }
        
        [JsonPropertyName("owner_id")]
        public string OwnerId { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("storage_class")]
        public string StorageClass { get; set; }
    }

    #endregion glb response
}
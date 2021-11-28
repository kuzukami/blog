using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Amazon.Lambda;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211127_my_api_getqueryparams
{
    public class Function
    {
        public ApiResponse FunctionHandler(object input, ILambdaContext context)
        {
            try
            {
                ApiRequest apiRequest         = JsonSerializer.Deserialize<ApiRequest>(input.ToString(), ApiUtil.GetJsonSerializerOptionsDefault());
                ApiRequestBody apiRequestBody = (apiRequest.Body == null) ? new ApiRequestBody() : JsonSerializer.Deserialize<ApiRequestBody>(apiRequest.Body, ApiUtil.GetJsonSerializerOptionsDefault());

                ApiResponse apiResponse                   = new ApiResponse();
                ApiResponseBody apiResponseBody           = new ApiResponseBody();

                string targetParamKey = "user_id";

                Dictionary<string, string>.KeyCollection paramKeys = apiRequest.QueryStringParameters.Keys;
                foreach( string paramKey in paramKeys )
                {
                    if(paramKey == targetParamKey)
                    {
                        apiResponseBody.Message = apiRequest.QueryStringParameters[targetParamKey] ;
                    }
                }

                Dictionary<string, string> apiResonseHeaders             = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "POST"}, {"Lambda-Result-Code", ApiUtil.RESULT_CODE_SUCCESS}, {"Lambda-Result-Message", ApiUtil.GetResultCodeDictionary()[ApiUtil.RESULT_CODE_SUCCESS]}};
                Dictionary<string, string[]> apiResonseMultiValueHeaders = new Dictionary<string, string[]>{{"Set-Cookie", new string[] {"KEY1=VALUE1; SameSite=None", "KEY2=VALUE2; SameSite=None"}}};

                apiResponse.IsBase64Encoded     = false;
                apiResponse.StatusCode          = HttpStatusCode.OK;
                apiResponse.Headers             = apiResonseHeaders;
                apiResponse.MultiValueHeaders   = apiResonseMultiValueHeaders;
                apiResponse.Body                = JsonSerializer.Serialize(apiResponseBody);

                return apiResponse;
            }
            catch (System.Exception e)
            {
                ApiResponse apiResponse         = new ApiResponse();
                ApiResponseBody apiResponseBody = new ApiResponseBody();
                apiResponseBody.Message = "aaa";

                Dictionary<string, string> apiResonseHeaders             = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "POST"}, {"Lambda-Result-Code", ApiUtil.RESULT_CODE_ERROR}, {"Lambda-Result-Message", e.Message + "::" + e.StackTrace}};
                Dictionary<string, string[]> apiResonseMultiValueHeaders = new Dictionary<string, string[]>{{"Set-Cookie", new string[] {"KEY1=VALUE1; SameSite=None", "KEY2=VALUE2; SameSite=None"}}};
                
                apiResponse.IsBase64Encoded     = false;
                apiResponse.StatusCode          = HttpStatusCode.OK;
                apiResponse.Headers             = apiResonseHeaders;
                apiResponse.MultiValueHeaders   = apiResonseMultiValueHeaders;
                apiResponse.Body                = JsonSerializer.Serialize(apiResponseBody);

                return apiResponse;
            }
        }
    }

    #region utils

    public static class ApiUtil
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

    #region api request

    public class ApiRequest
    {
        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("headers")]
        public ApiRequestHeaders Headers { get; set; }

        [JsonPropertyName("multiValueHeaders")]
        public ApiRequestMultiValueHeaders MultiValueHeaders { get; set; }

        [JsonPropertyName("queryStringParameters")]
        public Dictionary<string, string> QueryStringParameters { get; set; }

        [JsonPropertyName("multiValueQueryStringParameters")]
        public Dictionary<string, string[]> MultiValueQueryStringParameters { get; set; }

        [JsonPropertyName("pathParameters")]
        public Dictionary<string, string[]> PathParameters { get; set; }

        [JsonPropertyName("stageVariables")]
        public Dictionary<string, string[]> StageVariables { get; set; }

        [JsonPropertyName("requestContext")]
        public ApiRequestRequestContext RequestContext { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("isBase64Encoded")]
        public bool IsBase64Encoded { get; set; }
    }

    public class ApiRequestHeaders
    {
        [JsonPropertyName("Accept-Encoding")]
        public string AcceptEncoding { get; set; }

        [JsonPropertyName("Host")]
        public string Host { get; set; }

        [JsonPropertyName("User-Agent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("Via")]
        public string Via { get; set; }

        [JsonPropertyName("X-Amz-Cf-Id")]
        public string XAmzCfId { get; set; }

        [JsonPropertyName("X-Amzn-Trace-Id")]
        public string XAmznTraceId { get; set; }

        [JsonPropertyName("X-Forwarded-For")]
        public string XForwardedFor { get; set; }

        [JsonPropertyName("X-Forwarded-Port")]
        public string XForwardedPort { get; set; }

        [JsonPropertyName("X-Forwarded-Proto")]
        public string XForwardedProto { get; set; }
    }

    public class ApiRequestMultiValueHeaders
    {
        [JsonPropertyName("Accept-Encoding")]
        public string[] AcceptEncoding { get; set; }

        [JsonPropertyName("Host")]
        public string[] Host { get; set; }

        [JsonPropertyName("User-Agent")]
        public string[] UserAgent { get; set; }

        [JsonPropertyName("Via")]
        public string[] Via { get; set; }

        [JsonPropertyName("X-Amz-Cf-Id")]
        public string[] XAmzCfId { get; set; }

        [JsonPropertyName("X-Amzn-Trace-Id")]
        public string[] XAmznTraceId { get; set; }

        [JsonPropertyName("X-Forwarded-For")]
        public string[] XForwardedFor { get; set; }

        [JsonPropertyName("X-Forwarded-Port")]
        public string[] XForwardedPort { get; set; }

        [JsonPropertyName("X-Forwarded-Proto")]
        public string[] XForwardedProto { get; set; }
    }

    public class ApiRequestRequestContext
    {
        [JsonPropertyName("resourceId")]
        public string ResourceId { get; set; }

        [JsonPropertyName("resourcePath")]
        public string ResourcePath { get; set; }

        [JsonPropertyName("httpMethod")]
        public string HttpMethod { get; set; }

        [JsonPropertyName("extendedRequestId")]
        public string ExtendedRequestId { get; set; }

        [JsonPropertyName("requestTime")]
        public string RequestTime { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("stage")]
        public string Stage { get; set; }

        [JsonPropertyName("domainPrefix")]
        public string DomainPrefix { get; set; }

        [JsonPropertyName("requestTimeEpoch")]
        public long RequestTimeEpoch { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("identity")]
        public ApiRequestRequestContextIdentity Identity { get; set; }

        [JsonPropertyName("domainName")]
        public string DomainName { get; set; }

        [JsonPropertyName("apiId")]
        public string ApiId { get; set; }
    }

    public class ApiRequestRequestContextIdentity
    {
        [JsonPropertyName("cognitoIdentityPoolId")]
        public string ResourceId { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("cognitoIdentityId")]
        public string CognitoIdentityId { get; set; }

        [JsonPropertyName("caller")]
        public string Caller { get; set; }

        [JsonPropertyName("sourceIp")]
        public string SourceIp { get; set; }

        [JsonPropertyName("principalOrgId")]
        public string PrincipalOrgId { get; set; }

        [JsonPropertyName("accessKey")]
        public string AccessKey { get; set; }

        [JsonPropertyName("cognitoAuthenticationType")]
        public string CognitoAuthenticationType { get; set; }

        [JsonPropertyName("cognitoAuthenticationProvider")]
        public string CognitoAuthenticationProvider { get; set; }

        [JsonPropertyName("userArn")]
        public string UserArn { get; set; }

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }
    }

    public class ApiRequestBody
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

    #endregion api request

    #region api response

    public class ApiResponse
    {
        [JsonPropertyName("isBase64Encoded")]
        public bool IsBase64Encoded { get; set; }

        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonPropertyName("multiValueHeaders")]
        public Dictionary<string, string[]> MultiValueHeaders { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public class ApiResponseBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    #endregion api response
}
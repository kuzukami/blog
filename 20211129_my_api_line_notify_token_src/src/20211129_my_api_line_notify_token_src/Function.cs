using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
// [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211129_my_api_line_notify_token_src
{

    public class Function
    {
        public ApiResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            try
            {
                // ApiRequest apiRequest         = input.Body JsonSerializer.Deserialize<ApiRequest>(input.ToString(), ApiUtil.GetJsonSerializerOptionsDefault());
                // ApiRequestBody apiRequestBody = (apiRequest.Body == null) ? new ApiRequestBody() : JsonSerializer.Deserialize<ApiRequestBody>(apiRequest.Body, ApiUtil.GetJsonSerializerOptionsDefault());


                string baseurlWoSL = Helper.baseURLWoTailingSlash( input );
                context.Logger.LogInformation($"request base url: {baseurlWoSL}");

                ApiResponse apiResponse                   = new ApiResponse();
                List<ApiResponseBody> apiResponseBodyList = new List<ApiResponseBody>();
                ApiResponseBody apiResponseBody           = new ApiResponseBody();
                var redirectURL = LineNotifyRedirect(input);
                Dictionary<string, string> apiResonseHeaders             = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "GET"}, {"Location", redirectURL} , {"Content-Type", "application/json; charset=UTF-8"}};
                Dictionary<string, string[]> apiResonseMultiValueHeaders = new Dictionary<string, string[]>{{"Set-Cookie", new string[] {"KEY1=VALUE1; SameSite=None", "KEY2=VALUE2; SameSite=None"}}};
                
                apiResponse.IsBase64Encoded   = false;
                apiResponse.StatusCode        = HttpStatusCode.Redirect;
                apiResponse.Headers           = apiResonseHeaders;
                apiResponse.MultiValueHeaders = apiResonseMultiValueHeaders;

                apiResponse.Body = JsonSerializer.Serialize(apiResponseBody);

                return apiResponse;
            }
            catch (System.Exception e)
            {
                ApiResponse apiResponse         = new ApiResponse();
                ApiResponseBody apiResponseBody = new ApiResponseBody();
                Dictionary<string, string> apiResonseHeaders             = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "GET"}, {"Content-Type", "application/json; charset=UTF-8"}};
                Dictionary<string, string[]> apiResonseMultiValueHeaders = new Dictionary<string, string[]>{{"Set-Cookie", new string[] {"KEY1=VALUE1; SameSite=None", "KEY2=VALUE2; SameSite=None"}}};
                
                apiResponse.IsBase64Encoded   = false;
                apiResponse.StatusCode        = HttpStatusCode.InternalServerError;
                apiResponse.Headers           = apiResonseHeaders;
                apiResponse.MultiValueHeaders = apiResonseMultiValueHeaders;

                apiResponseBody.Message       = e.Message /* + e.StackTrace */;
                apiResponse.Body              = JsonSerializer.Serialize(apiResponseBody);

                return apiResponse;
            }
        }

        public static byte[] utf8bytes(String val ){
            return Encoding.UTF8.GetBytes(val);
        }
        public static string computeHMACForUTF8String( string targetString, string hmacKey ){
                using (var hmacSha512 = new System.Security.Cryptography.HMACSHA512( utf8bytes(hmacKey)) )
                {
                    byte[] hashValue = hmacSha512.ComputeHash( utf8bytes(targetString));
                    // Console.WriteLine(BitConverter.ToString(hashValue).Replace("-", "").ToLower());
                    var hash = new StringBuilder();
                    foreach (var theByte in hashValue)
                    {
                        hash.Append(theByte.ToString("x2"));
                    }
                    return  hash.ToString();
                }
        }

        public static string computeSignedState(LineNotifyManagerJson manid){
                //JWEの暗号化署名キー
                var stateSignKey = EnvVar.LINNOAX_STATE_KEY_HEXA.getEnforcedAsHexa();
                //enforceEnvVar("LINNOAX_STATE_VALID_SECONDS");//署名の有効期間を秒数で指定する
                var stateValidPeriod_in_secs =  Double.Parse( EnvVar.LINNOAX_STATE_VALID_SECONDS.getEnforced() );

                return manid.buildJWE(stateSignKey, TimeSpan.FromSeconds( stateValidPeriod_in_secs ));
        }

        private string LineNotifyRedirect( APIGatewayProxyRequest req)
        {
            try
            {
                LineNotifyManagerJson manid = LineAPI.API1.extractVerifiedManagementId(req);
                string client_id    =  EnvVar.LINNOAX_CLIENT_ID.getEnforced(); // enforceEnvVar("LINNOAX_CLIENT_ID");//"管理画面から取得してね！";
                string redirect_uri = LineAPI.API2.apiURL( req ); // enforceEnvVar("LINNOA1_REDIRECT_URI"); //"API2のURLを指定してね！"
                string state        = computeSignedState(manid); //"LINE notify APIからAPI2にリダイレクトされるときに渡してくれるパラメータだよ。送信元の認証に使えるかも！"

                var parameters = new Dictionary<string, string>()
                {
                    { "response_type", "code" },
                    { "client_id"    , client_id },
                    { "redirect_uri" , redirect_uri },
                    { "scope"        , "notify" },
                    { "state"        , state },
                };
                string lineNotifyRequestString = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;

                string returnString = "https://notify-bot.line.me/oauth/authorize?" + lineNotifyRequestString;

                return returnString;
            }
            catch (System.Exception e)
            {
                throw e;
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
                    { RESULT_CODE_ERROR, RESULT_MESSAGE_ERROR },
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
        [JsonPropertyName("message")]
        public string Message { get; set; }
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

using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyFunction
{
    public class Function
    {
        public ApiResponse FunctionHandler(object input, ILambdaContext context)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions() {
                    IgnoreNullValues            = true,
                    PropertyNameCaseInsensitive = true
                };

                ApiRequest apiRequest = JsonSerializer.Deserialize<ApiRequest>(input.ToString(), options);

                ApiResponse apiResponse = new ApiResponse();
                Dictionary<string, string> apiResonseHeaders             = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "GET"}};
                Dictionary<string, string[]> apiResonseMultiValueHeaders = new Dictionary<string, string[]>{{"Set-Cookie", new string[] {"KEY1=VALUE1; SameSite=None", "KEY2=VALUE2; SameSite=None"}}};
                ApiResponseBody apiResponseBody = new ApiResponseBody();
                apiResponseBody.Message         = apiRequest.Path;

                apiResponse.IsBase64Encoded   = false;
                apiResponse.StatusCode        = HttpStatusCode.OK;
                apiResponse.Headers           = apiResonseHeaders;
                apiResponse.MultiValueHeaders = apiResonseMultiValueHeaders;
                apiResponse.Body              = JsonSerializer.Serialize(apiResponseBody, options);

                return apiResponse;
            }
            catch (System.Exception e)
            {
                JsonSerializerOptions options = new JsonSerializerOptions() {
                    IgnoreNullValues            = true,
                    PropertyNameCaseInsensitive = true
                };

                ApiResponse apiResponse         = new ApiResponse();
                ApiResponseBody apiResponseBody = new ApiResponseBody();
                apiResponseBody.Message         = e.Message;

                apiResponse.IsBase64Encoded   = false;
                apiResponse.StatusCode        = HttpStatusCode.OK;
                apiResponse.Headers           = new Dictionary<string, string>();
                apiResponse.MultiValueHeaders = new Dictionary<string, string[]>();
                apiResponse.Body              = JsonSerializer.Serialize(apiResponseBody, options);

                return apiResponse;
            }
        }
    }
}

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
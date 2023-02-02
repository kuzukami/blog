using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211129_my_api_line_notify_token_src;


    public static class DictionaryExtensions {
    public static TValue GetValueOrDefault<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    TKey key,
    TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
    }

    public class EnvVar{
        private String keyword;


        private static string enforceEnvVar( String envkey ){
                var environmentvalNullable = Environment.GetEnvironmentVariable(envkey);

                if ( environmentvalNullable == null ){
                    throw new Exception("必要な環境変数が設定されていません" + envkey);
                }


                return environmentvalNullable;
        }

        public String getOrDefault( String defaultValue ){
            var environmentvalNullable = Environment.GetEnvironmentVariable(this.keyword);

            if ( environmentvalNullable == null ){
                return defaultValue;
            }
            return environmentvalNullable;
        }

        public String getEnforced(){
            return enforceEnvVar( this.keyword );
        }
        ///SHA512でデッドラインSignを行う
        public static EnvVar LINNOAX_STATE_SIGN_KEY = new EnvVar(){ keyword = "LINNOAX_STATE_SIGN_KEY" };
        ///署名の有効期間を秒数で指定する
        public static EnvVar LINNOAX_STATE_VALID_SECONDS = new EnvVar() { keyword = "LINNOAX_STATE_VALID_SECONDS" };

        ///管理画面から取得してね！ line_client_id
        public static EnvVar LINNOAX_CLIENT_ID = new EnvVar(){ keyword = "LINNOAX_CLIENT_ID" };

        //  string client_secret =  _20211129_my_api_line_notify_token_src.Function.enforceEnvVar("LINNOAX_CLIENT_SECRET");
        ///管理画面から取得してね！ line_client_secret
        public static EnvVar LINNOAX_CLIENT_SECRET = new EnvVar(){ keyword = "LINNOAX_CLIENT_SECRET" };

    }
    public static class Helper{
        public static String headerValueOrDefault(String httpHeader__keyword, APIGatewayProxyRequest req , String if__null_Default){
            return req.Headers.GetValueOrDefault(httpHeader__keyword, if__null_Default);
        }

        public static String queryParameterValueOrDefault(String queryParameter__keyword, APIGatewayProxyRequest req , String if__null_Default){
            return req.QueryStringParameters.GetValueOrDefault(queryParameter__keyword, if__null_Default);
        }
        ///https://53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com/Prod を計算する
        public static String baseURLWoTailingSlash( APIGatewayProxyRequest input ){
                // String hostHeaderVal = Helper.headerValueOrDefault( "Host",  input, "**ZeroString**");
                // foreach ( var headerKW in  input.Headers.Keys ){
                //     context.Logger.LogDebug($"header keyword: {headerKW} value: {input.Headers[headerKW]}");
                // }
                // context.Logger.LogInformation($"host header  value: {hostHeaderVal}"); // => host header value: 53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com
                // context.Logger.LogInformation($"stage value: {input.RequestContext.Stage}"); // => stage value: Prod


                String hostHeaderVal = headerValueOrDefault( "Host",  input, "**ZeroString**");
                String stage = input.RequestContext.Stage;

                return $"https://{hostHeaderVal}/{stage}";
        }
        ///https://53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com/Prod/api1
        ///https://53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com/Prod/api2
        public static String apiPath( APIGatewayProxyRequest input , String slash_apiName ){
            return baseURLWoTailingSlash(input) +  slash_apiName.ToString();
        }
        public static String accessPath( APIGatewayProxyRequest input){
            return apiPath( input, input.Path );
        }
    }

    public class LineAPI{
        private String apiNameAsRelativePath;
        private String httpMethod;
        public Func<APIGatewayProxyRequest,ILambdaContext,APIGatewayProxyResponse> apiRoutine;


        public String apiURL( APIGatewayProxyRequest req ){
            string apiPath = Helper.apiPath( req , apiNameAsRelativePath);
            return apiPath;
        }

        public bool IsTarget(  APIGatewayProxyRequest req, ILambdaLogger log ){
            string accessPath = Helper.accessPath( req );
            string targetCheckAPIPath = apiURL(req);
            bool pathMatch = targetCheckAPIPath == accessPath;
            bool methodMatch = req.HttpMethod == httpMethod;

            LogLevel lv = LogLevel.Debug;
            log.Log( lv, $"IsTarget: access  Path{accessPath}, Method{req.HttpMethod}");
            log.Log( lv, $"IsTarget: API  Path{targetCheckAPIPath}, Method{httpMethod}");
            log.Log( lv, $"IsTarget: Match.path {pathMatch}, Match.method {methodMatch}");


            return pathMatch && methodMatch;
        }

        private static APIGatewayProxyResponse refineResponse( ApiResponse a  ){
            
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)a.StatusCode, //(int)HttpStatusCode.OK,
                Body =  a.Body, // "Hello AWS Serverless",
                Headers = a.Headers, // new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                IsBase64Encoded = a.IsBase64Encoded,
                MultiValueHeaders = a.MultiValueHeaders.ToDictionary( kvp => kvp.Key, kvp => (IList<string>)new List<string>(kvp.Value) )
            };
            return response;
        }

        public static LineAPI API1 = new LineAPI(){
            apiNameAsRelativePath = "/api1",
            httpMethod = "GET",
            apiRoutine = ( req, lamcon ) => { return  refineResponse( new Function().FunctionHandler(req, lamcon ) );},
        };

        public static LineAPI API2 = new LineAPI(){
            apiNameAsRelativePath = "/api2",
            httpMethod = "POST",
            apiRoutine = ( req, lamcon ) => { return  refineResponse( new _20211129_my_api_line_notify_token_dst.Function().FunctionHandler(req, lamcon ) );},
        };

        public static List<LineAPI> apiSeries = new List<LineAPI>(){
            API1,
            API2,
        };

        public static LineAPI LineRedirectURL = LineAPI.API2;


    }


public class Functions
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
    }



    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The API Gateway response.</returns>
    public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        // context.Logger.LogInformation("Get Request\n");

        string path = request.Path;
        string method = request.HttpMethod;

        // Function f = new Function();
        // ApiResponse a = f.FunctionHandler(request, context);

        foreach ( var api in LineAPI.apiSeries ){
            if ( api.IsTarget( request, context.Logger ) ) {
                return api.apiRoutine( request, context);
            }
        }

        {
            //unkonwn api call
            APIGatewayProxyResponse apiResponse         = new APIGatewayProxyResponse(){
                StatusCode = (int)HttpStatusCode.InternalServerError, //(int)HttpStatusCode.OK,
                Body =  $"unknown api invokation. path {path} and method {method}", // "Hello AWS Serverless",
                Headers = new Dictionary<string, string>{{"Access-Control-Allow-Origin", "*"},{"Access-Control-Allow-Headers", "Content-Type"}, {"Access-Control-Allow-Methods", "GET"}, {"Content-Type", "text/plain; charset=UTF-8"}},
                IsBase64Encoded   = false,
            };

            return apiResponse;
        }

    }
}
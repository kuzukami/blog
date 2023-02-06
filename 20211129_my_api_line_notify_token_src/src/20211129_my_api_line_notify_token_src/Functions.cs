using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
// using JWT.Algorithms;
// using JWT.Builder;
// using JWT;
// using JWT.Algorithms;
// using JWT.Builder;
using Newtonsoft.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Buffers.Binary;
using System.Text;

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

        // public String getOrDefault( String defaultValue ){
        //     var environmentvalNullable = Environment.GetEnvironmentVariable(this.keyword);

        //     if ( environmentvalNullable == null ){
        //         return defaultValue;
        //     }
        //     return environmentvalNullable;
        // }

        public String getEnforced(){
            return byInitializedVariable( this );
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public byte[] getEnforcedAsHexa(){
            return FromHex(getEnforced());
        }
        ///ラインとユーザーエージェント経由で行って戻ってくるSTATE-TOKENはJWE+JWTで暗号化(HMAC256,AES256-GCM)なので、256bitのhexa(64桁)指定
        public static EnvVar LINNOAX_STATE_KEY_HEXA = new EnvVar(){ keyword = "LINNOAX_STATE_KEY_HEXA" };
        ///TOKENの有効期間を秒数で指定する
        public static EnvVar LINNOAX_STATE_VALID_SECONDS = new EnvVar() { keyword = "LINNOAX_STATE_VALID_SECONDS" };

        public static EnvVar LINOAX_IDTOKEN_SIGN_KEY_HEXA = new EnvVar(){ keyword = "LINOAX_IDTOKEN_SIGN_KEY_HEXA" };

        ///管理画面から取得してね！ line_client_id
        public static EnvVar LINNOAX_CLIENT_ID = new EnvVar(){ keyword = "LINNOAX_CLIENT_ID" };

        //  string client_secret =  _20211129_my_api_line_notify_token_src.Function.enforceEnvVar("LINNOAX_CLIENT_SECRET");
        ///管理画面から取得してね！ line_client_secret
        public static EnvVar LINNOAX_CLIENT_SECRET = new EnvVar(){ keyword = "LINNOAX_CLIENT_SECRET" };

        private static Dictionary<EnvVar,string> varValues = null;

        private static string byInitializedVariable( EnvVar envkey ){
            if ( varValues == null ){
                initializeByEnviromentVariables();
            }
            if ( varValues.ContainsKey(envkey ) ){
                return varValues[envkey];
            }else{
                throw new Exception("必要な環境変数が設定されていません" + envkey.keyword);
            }
        }


        public static Dictionary<EnvVar,string> initializeByEnviromentVariables(){
            var dic = new Dictionary<EnvVar, string>();
            var vars = new List<EnvVar>(){ LINNOAX_STATE_KEY_HEXA, LINNOAX_STATE_VALID_SECONDS, LINOAX_IDTOKEN_SIGN_KEY_HEXA, LINNOAX_CLIENT_ID, LINNOAX_CLIENT_SECRET };

            foreach ( var varx in vars ){
                dic.Add(varx,  enforceEnvVar( varx.keyword ) );
            }
            return dic;
        }

        public static void setVarValues( Dictionary<EnvVar,string> values ){
            varValues = values;
        }

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


    public class LineNotifyManagerJson{
        [JsonProperty("managmentid")]   
        public string ManagementId { get; set; }

        public string buildJWT( byte[] secret, TimeSpan seconds_to_deadline){
            return buildIDJWT( ManagementId, secret, seconds_to_deadline );
        }

        public string buildJWE( byte[] secret, TimeSpan seconds_to_deadline){
            return buildIDJWE( ManagementId, secret, seconds_to_deadline );
        }

        public static byte[] aesgcmNonce(){
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // MaxSize = 12
            RandomNumberGenerator.Fill(nonce);
            var epochSecLong = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            BinaryPrimitives.WriteInt32BigEndian( nonce.AsSpan(0, 4), (int)epochSecLong );
            return nonce;
        }

        public static string buildIDJWE( string managementId, byte[] keySecret, TimeSpan seconds_to_deadline ){

            var jwt = buildIDJWT(managementId,  keySecret, seconds_to_deadline );

            //see https://www.scottbrady91.com/c-sharp/aes-gcm-dotnet
            using var aes = new AesGcm(keySecret);
            var nonce = aesgcmNonce();

            var plaintextBytes = Encoding.UTF8.GetBytes(jwt);
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // MaxSize = 16

            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
          
            //Nonce (12B) | Ciphertext (*B) | Tag (16B)
            return  Convert.ToHexString( nonce.Concat( ciphertext ).Concat( tag ).ToArray() );

            //実装失敗、AES直接のケースで動かない
            // var handler = new JsonWebTokenHandler();

            // var symkey = new SymmetricSecurityKey(keySecret);

            // string token = handler.CreateToken(new SecurityTokenDescriptor
            // {
            //     Audience = "api2",
            //     // Issuer = "https://idp.example.com",
            //     // Claims = new Dictionary<string, object> { { "sub", "811e790749a24d8a8f766e1a44dca28a" } },
            //     Expires = expirationEpoch( seconds_to_deadline ), 

            //     // private key for signing
            //     SigningCredentials = new SigningCredentials(symkey, SecurityAlgorithms.HmacSha256),

            //     // public key for encryption
            //     EncryptingCredentials = new EncryptingCredentials( symkey, SecurityAlgorithms.Aes128CbcHmacSha256, SecurityAlgorithms.Aes256Gcm),

            //     Claims =  myClaimDic(managementId),
            // });

            // return token;
        }

        public static LineNotifyManagerJson fromIDJWE( string jwe_token, byte[] keySecret ){
            byte[] encryptedRAW = Convert.FromHexString( jwe_token );
            //see https://www.scottbrady91.com/c-sharp/aes-gcm-dotnet
            using var aes = new AesGcm(keySecret);

            var nonce =         encryptedRAW.AsSpan(0,                                       aesgcmNonce().Length);
            var ciphertext =    encryptedRAW.AsSpan(nonce.Length,                            encryptedRAW.Length - AesGcm.TagByteSizes.MaxSize - nonce.Length );
            var tag =           encryptedRAW.AsSpan(nonce.Length + ciphertext.Length,        AesGcm.TagByteSizes.MaxSize ); // MaxSize = 16

            var plaintextBytes = new byte[ciphertext.Length];
            aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

            string jwt =  Encoding.UTF8.GetString(plaintextBytes);

            return fromIDJWT(jwt, keySecret);

            //実装失敗、AES直接のケースで動かない
            // var handler = new JsonWebTokenHandler();
            // var symkey = new SymmetricSecurityKey(keySecret);

            // var algorithmSpec =new List<string>() { SecurityAlgorithms.Aes256Gcm };
            // TokenValidationResult result = handler.ValidateToken(
            //     jwe_token,
            //     new TokenValidationParameters
            //     {
            //         ValidAudience = "api2",
            //         // ValidIssuer = "https://idp.example.com",
            //         ValidateIssuer = false,
            //         ValidateLifetime = true,
            //         RequireExpirationTime = true,
            //         // private key for encryption
            //         ValidAlgorithms = algorithmSpec,
            //         IssuerSigningKey = symkey,
            //         TokenDecryptionKey = symkey,
            //         ClockSkew = TimeSpan.Zero,
            //     });


            // if ( result.IsValid ) {
            //     return fromPlainClaimDic( result.Claims );
            // }else{
            //     Console.WriteLine(result.ToString());
            //     throw new Exception("JWEトークンの検証に失敗しました");
            // }
        }

        ///JWT内でユーザー名を表すカスタムクレームキー
        private static string id_keyword = "managementid";
        private static  IDictionary<string, object> myClaimDic( string magementId ){
            return new Dictionary<string, object> { { id_keyword , magementId } }; } 

        private static LineNotifyManagerJson fromPlainClaimDic( IDictionary<string, object> claimdic ){
            if ( claimdic.ContainsKey( id_keyword ) ){
                return new LineNotifyManagerJson{
                    ManagementId = claimdic[ id_keyword ].ToString()
                };
            }else{
                throw new Exception($"E002.Tokenに{id_keyword}キーが入っていません");
            }
        }

        private static DateTime expirationEpoch( TimeSpan seconds_to_deadline  ){
            return DateTime.Now.AddMilliseconds(seconds_to_deadline.TotalMilliseconds);
        }

        public static string buildIDJWT( string managementId, byte[] signSecret, TimeSpan seconds_to_deadline ){

            var handler = new JsonWebTokenHandler();

            var symkey = new SymmetricSecurityKey(signSecret);

            string token = handler.CreateToken(new SecurityTokenDescriptor
            {
                // Issuer = "https://idp.example.com",
                // Claims = new Dictionary<string, object> { { "sub", "811e790749a24d8a8f766e1a44dca28a" } },
                Expires = expirationEpoch( seconds_to_deadline ), 

                // private key for signing
                SigningCredentials = new SigningCredentials(symkey, SecurityAlgorithms.HmacSha256),

                Claims =  myClaimDic(managementId),
            });

            return token;

            // LineNotifyManagerJson l = new LineNotifyManagerJson{
            //     ManagementId = managementId
            // };

            // var jwtToken = new JwtBuilder()
            //     .WithAlgorithm(new HMACSHA256Algorithm())
            //     .WithSecret(secret)
            //     .ExpirationTime( expirationEpoch(seconds_to_deadline).ToUnixTimeSeconds() )
            //     // .AddClaim("linenotifymanid", id)
            //     // .AddClaim("data1", "hello!")
            //     .Encode( l );

            // // Console.WriteLine(jwtToken);
            // return jwtToken;
        }

        public static LineNotifyManagerJson fromIDJWT( string jwtToken, byte[] signSecret ){
            var handler = new JsonWebTokenHandler();
            var symkey = new SymmetricSecurityKey(signSecret);

            TokenValidationResult result = handler.ValidateToken(
                jwtToken,
                new TokenValidationParameters
                {
                    // ValidAudience = "api2",
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    // ValidIssuer = "https://idp.example.com",
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    // private key for encryption
                    ValidAlgorithms = Enumerable.Repeat( SecurityAlgorithms.HmacSha256 ,1),
                    IssuerSigningKey = symkey,
                    // TokenDecryptionKey = symkey,
                    ClockSkew = TimeSpan.Zero,
                });



            if ( result.IsValid ) {
                return fromPlainClaimDic( result.Claims );
            }else{
                // Console.WriteLine(result.ToString());
                throw new Exception("E001.JWTトークンの検証に失敗しました", result.Exception);
            }

        //     //仕様によればexp検証は強制
        // var jsonObj = new JwtBuilder()
        // .WithSecret(secret)
        // .WithAlgorithm(new HMACSHA256Algorithm())
        // .MustVerifySignature()
        // .Decode<LineNotifyManagerJson>(jwtToken)
        // ;

        // return jsonObj;

        }



    }

    public class LineAPI{
        private String apiNameAsRelativePath;
        private String httpMethod;

        public Func<APIGatewayProxyRequest,ILambdaContext,APIGatewayProxyResponse> apiRoutine;

        public Func<APIGatewayProxyRequest,LineNotifyManagerJson> extractVerifiedManagementId;

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

            extractVerifiedManagementId = ( req ) => {
                //idtokenクエリパラメータ中のJWTトークンを検証してIDとする
                return
                    LineNotifyManagerJson.fromIDJWT(
                            Helper.queryParameterValueOrDefault("idtoken", req, "_not_specified_"),
                            EnvVar.LINOAX_IDTOKEN_SIGN_KEY_HEXA.getEnforcedAsHexa()
                            ); },

        };

        public static LineAPI API2 = new LineAPI(){
            apiNameAsRelativePath = "/api2",
            httpMethod = "GET",
            apiRoutine = ( req, lamcon ) => { return  refineResponse( new _20211129_my_api_line_notify_token_dst.Function().FunctionHandler(req, lamcon ) );},

            extractVerifiedManagementId = ( req ) => {
                //stateクエリパラメータ中のJWEトークンを検証してIDとする
                return
                    LineNotifyManagerJson.fromIDJWE(
                            Helper.queryParameterValueOrDefault("state", req, "_not_specified_"),
                            EnvVar.LINNOAX_STATE_KEY_HEXA.getEnforcedAsHexa()
                            ); },

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
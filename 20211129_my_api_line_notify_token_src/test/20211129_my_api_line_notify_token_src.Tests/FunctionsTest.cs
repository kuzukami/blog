using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Buffers.Binary;
using System.Text.RegularExpressions;
using System.Web;

namespace _20211129_my_api_line_notify_token_src.Tests;

public class FunctionTest
{
    public FunctionTest()
    {
    }

    public static APIGatewayProxyRequest standardRequestAPI1( String stage, String path, String host_header, string jwtUserToken ){
        return standardRequest( HttpMethod.Get.ToString(), stage, path, host_header, new Dictionary<string,string> {
            ["idtoken"] = jwtUserToken,
        });
    }
    public static APIGatewayProxyRequest standardRequestAPI1(  string jwtUserToken ){
        return standardRequestAPI1("UnitTestStage", "/api1", "example.com", jwtUserToken );
    }

    public static APIGatewayProxyRequest standardRequestAPI2( String stage, String path, String host_header, string lineAccessCode, string line_statecode ){
        return standardRequest( HttpMethod.Get.ToString(), stage, path, host_header, new Dictionary<string,string> {
            ["code"] = lineAccessCode,
            ["state"] = line_statecode,
        });
    }
    public static APIGatewayProxyRequest standardRequestAPI2(  string lineAccessCode, string line_statecode ){
        return standardRequestAPI2("UnitTestStage", "/api2", "example.com", lineAccessCode, line_statecode );
    }

    public static APIGatewayProxyRequest standardRequest( string httpMethod, String stage, String path, String host_header, Dictionary<string,string> queryParameter ){
        APIGatewayProxyRequest request;

        request = new APIGatewayProxyRequest();
        request.HttpMethod = httpMethod;
        request.Path = path;
        request.Headers = new Dictionary<string,string>{{ "Host", host_header}};
        request.QueryStringParameters = queryParameter;
        request.RequestContext = new APIGatewayProxyRequest.ProxyRequestContext(){
            Stage = stage,
        };
        return request;
    }

    public static TestLambdaContext standardContext(){
        var context = new TestLambdaContext();
        return context;
    }
    private string hexa( byte[] x ){
        return Convert.ToHexString(x);
    }

    public APIGatewayProxyResponse WalkThruAPI1( String id, byte[] secret_on_sign, byte[] secret_on_verify , byte[] secret_on_state )
    {
        TestLambdaContext context;
        APIGatewayProxyRequest request;
        APIGatewayProxyResponse response;

        Functions functions = new Functions();


        string jwtIdToken ;
        jwtIdToken = LineNotifyManagerJson.buildIDJWT( id,  secret_on_sign, TimeSpan.FromSeconds( 100/*secs*/ ) );
        request = standardRequestAPI1(  jwtIdToken);
        context = standardContext();

        EnvVar.setVarValues(new Dictionary<EnvVar, string>{
            { EnvVar.LINNOAX_CLIENT_ID, "abc" },
            { EnvVar.LINNOAX_CLIENT_SECRET, "abc" },
            { EnvVar.LINNOAX_STATE_KEY_HEXA, hexa(secret_on_state) },
            { EnvVar.LINNOAX_STATE_VALID_SECONDS, "10" },
            { EnvVar.LINOAX_IDTOKEN_SIGN_KEY_HEXA, hexa(secret_on_verify) },
        });

        response = functions.Get(request, context);
        // Console.WriteLine( Regex.Unescape(response.Body));
        // Assert.Equal("Hello AWS Serverless", response.Body);
        // Assert.Equal((int)System.Net.HttpStatusCode.Redirect, response.StatusCode);//check to redirect
        return response;
    }


    [Fact]
    public void TestAPI1_walkthru()
    {
        {
            var response = WalkThruAPI1( "hoGe", correct_secret, correct_secret, correct_secret );
            Assert.Equal((int)System.Net.HttpStatusCode.Redirect, response.StatusCode);//check to redirect

            string redirectUrl = response.Headers["Location"];
            Uri redirectUri = new Uri(redirectUrl);
            var state_jwe = HttpUtility.ParseQueryString(redirectUri.Query).Get("state");

            Assert.Equal( "hoGe", LineNotifyManagerJson.fromIDJWE( state_jwe, correct_secret ).ManagementId );

        }
        {
            var response = WalkThruAPI1( "hoGe", correct_secret, correct_secret, wrong_secret ); //第三引数は直接の動作に関係ない
            Assert.Equal((int)System.Net.HttpStatusCode.Redirect, response.StatusCode);//check to redirect

            string redirectUrl = response.Headers["Location"];
            Uri redirectUri = new Uri(redirectUrl);
            var state_jwe = HttpUtility.ParseQueryString(redirectUri.Query).Get("state");

            Assert.Equal( "hoGe", LineNotifyManagerJson.fromIDJWE( state_jwe, wrong_secret ).ManagementId );
            Assert.Throws<System.Security.Cryptography.CryptographicException>( () =>  LineNotifyManagerJson.fromIDJWE( state_jwe, correct_secret ).ManagementId ); //誤ったキーでは展開できない

        }
    }




    [Fact]
    public void TestAPI1_WrongSignature()
    {
        APIGatewayProxyResponse resp = WalkThruAPI1( "hogehoge", correct_secret, wrong_secret, correct_secret );
        Assert.Equal( (int)System.Net.HttpStatusCode.InternalServerError, resp.StatusCode   );
        Assert.Contains( "E001.JWT", Regex.Unescape( resp.Body)   );
        // Assert.Contains( "E001.JWX", Regex.Unescape( resp.Body)   ); //test of test
    }



    public APIGatewayProxyResponse WalkThruAPI2( String id, byte[] secret_on_encrypt, byte[] secret_on_decrypt , byte[] secret_on_init_sign )
    {
        TestLambdaContext context;
        APIGatewayProxyRequest request;
        APIGatewayProxyResponse response;

        Functions functions = new Functions();


        string manid = "hoge";
        string lineAccessCode = "aBcEX" ;
        string line_statecode = LineNotifyManagerJson.buildIDJWE( manid, secret_on_encrypt, TimeSpan.FromSeconds(100) );
        request = standardRequestAPI2( lineAccessCode, line_statecode);
        context = standardContext();

        EnvVar.setVarValues(new Dictionary<EnvVar, string>{
            { EnvVar.LINNOAX_CLIENT_ID, "abc" },
            { EnvVar.LINNOAX_CLIENT_SECRET, "abc" },
            { EnvVar.LINNOAX_STATE_KEY_HEXA, hexa(secret_on_decrypt) },
            { EnvVar.LINNOAX_STATE_VALID_SECONDS, "10" },
            { EnvVar.LINOAX_IDTOKEN_SIGN_KEY_HEXA, hexa(secret_on_init_sign) },
        });

        response = functions.Get(request, context);
        // Console.WriteLine( Regex.Unescape(response.Body));
        // Assert.Equal("Hello AWS Serverless", response.Body);
        // Assert.Equal((int)System.Net.HttpStatusCode.Redirect, response.StatusCode);//check to redirect
        return response;
    }



    [Fact]
    public void TestAPI2_walkthru()
    {
        {
            var response = WalkThruAPI2( "hoGe", correct_secret, correct_secret, correct_secret );
            //lineへの問い合わせのところまで貫通している
            Assert.Contains("invalid code", Regex.Unescape(response.Body));
            Assert.Contains("message", Regex.Unescape(response.Body));
        }
        {
            var response = WalkThruAPI2( "hoGe", correct_secret, correct_secret, wrong_secret ); //関係のないシークレットを変更しても影響無い
            //lineへの問い合わせのところまで貫通している
            Assert.Contains("invalid code", Regex.Unescape(response.Body));
            Assert.Contains("message", Regex.Unescape(response.Body));
        }


        
    }

    [Fact]
    public void TestAPI2_wrongecnryption()
    {
        {
            var response = WalkThruAPI2( "hoGe", wrong_secret, correct_secret, correct_secret );
            //lineへの問い合わせのところまで貫通していない(暗号化でこけている)
            Assert.Contains("The computed authentication tag did not match the input authentication tag", Regex.Unescape(response.Body));
        }
        
    }


    private static byte[] bytes32( byte firstValue ){
        var sec = new byte[32];//256bit
        sec[0] = firstValue;
        return sec;
    }
    static byte[] correct_secret = bytes32(1);//256bit
    static byte[] wrong_secret = bytes32(2);//256bit


    private static LineNotifyManagerJson walkThruJWT( string expectedId, byte[] secret_sign, byte[] secret_verify, double seconds_to_deadline,  int sleepMilliSec){
        // string expectedId= "abXc";
        string jwt = LineNotifyManagerJson.buildIDJWT(expectedId, secret_sign, TimeSpan.FromSeconds( seconds_to_deadline) /*sec*/);
        Thread.Sleep(sleepMilliSec);
        LineNotifyManagerJson manj = LineNotifyManagerJson.fromIDJWT(jwt, secret_verify);
        return manj;
    }

    string stdExpectedId= "solich0-0001";
    [Fact]
    public void TestWalkThruJWT()
    {
        {
        LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, correct_secret, 2, 0 );
        Assert.Equal( stdExpectedId, manj.ManagementId );
        }

        {
        LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, correct_secret, 2, 500 ); //sleep 500millisecs
        Assert.Equal( stdExpectedId, manj.ManagementId );
        }
    }


    [Fact]
    public void TestJWT_FailSecret()
    {

        //test of test
        //     "正常状態なので例外はあってはいけない"
        // var excTest = Assert.Throws<Exception>(  () => {
        //     LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, correct_secret, 1, 0 );
        // });
        var exc = Assert.Throws<Exception>(  () => {
            LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, wrong_secret, 2, 0 );
        });

        if ( exc != null ){
            Assert.Contains("Signature validation failed",exc.ToString());
        }
    }

    [Fact]
    public void TestJWT_FailDeadline()
    {
        var exc = Assert.Throws<Exception>(  () => {
            LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, correct_secret, 2, 2100 );
        });

        if ( exc != null ){
            Assert.Contains("Lifetime validation failed",exc.ToString());
        }
    }


    private static LineNotifyManagerJson walkThruJWE( string expectedId, byte[] secret_encsign, byte[] secret_decverify, int seconds_to_deadline,  int sleepMilliSec){
        // string expectedId= "abXc";
        string jwe = LineNotifyManagerJson.buildIDJWE(expectedId, secret_encsign, TimeSpan.FromSeconds( seconds_to_deadline ) /*sec*/);
        Thread.Sleep(sleepMilliSec);
        Console.WriteLine($"jwe:{jwe}");
        LineNotifyManagerJson manj = LineNotifyManagerJson.fromIDJWE(jwe, secret_decverify);
        return manj;
    }


    [Fact]
    public void TestWalkThruJWE()
    {
        {
        LineNotifyManagerJson manj = walkThruJWE( stdExpectedId, correct_secret, correct_secret, 2, 0 );
        Assert.Equal( stdExpectedId, manj.ManagementId );
        }

        {
        LineNotifyManagerJson manj = walkThruJWE( stdExpectedId, correct_secret, correct_secret, 2, 500 ); //sleep 500millisecs
        Assert.Equal( stdExpectedId, manj.ManagementId );
        }
    }

    [Fact]
    public void TestAESGCMNonce()
    {
        byte[] nonce1 = LineNotifyManagerJson.aesgcmNonce();
        byte[] nonce_sute1 = LineNotifyManagerJson.aesgcmNonce();
        byte[] nonce_sute2 = LineNotifyManagerJson.aesgcmNonce();
        Thread.Sleep(1000); //sleep 1sec
        byte[] nonce2 = LineNotifyManagerJson.aesgcmNonce();

        var time1 = BinaryPrimitives.ReadInt32BigEndian(nonce1.AsSpan(0, 4));
        var time2 = BinaryPrimitives.ReadInt32BigEndian(nonce2.AsSpan(0, 4));


        //桁数は必ず同じ(96bit)でなければならない
        Assert.Equal( 96/8, nonce1.Length );
        Assert.Equal( 96/8, nonce2.Length );
        // Assert.Equal( 96/8 + 1, nonce2.Length ); //test of test
// "何回生成しようと、発行エポック秒が先頭の4桁に入っていなければならない",
        Assert.Equal(   time1 * 1.0 , time2 * 1.0, 2 * 1.0 );
        // Assert.Equal(   time1 * 1.0 , time2 * 1.0, 0 * 1.0 ); //test of test
        //残りの桁はランダムでなければならない
        Assert.NotEqual(
             nonce1.AsSpan(4,nonce1.Length -4).ToArray(),
             nonce2.AsSpan(4,nonce2.Length -4).ToArray()
        );
    }

    [Fact]
    public void TestJWE_FailSecret()
    {

        //test of test
        //     "正常状態なので例外はあってはいけない"
        // var excTest = Assert.Throws<Exception>(  () => {
        //     LineNotifyManagerJson manj = walkThruJWT( stdExpectedId, correct_secret, correct_secret, 1, 0 );
        // });
        var exc = Assert.Throws<System.Security.Cryptography.CryptographicException>(  () => {
            LineNotifyManagerJson manj = walkThruJWE( stdExpectedId, correct_secret, wrong_secret, 2, 0 );
        });

        if ( exc != null ){
            Assert.Contains("did not match the input authentication tag",exc.ToString());
        }
    }

    [Fact]
    public void TestJWE_FailDeadline()
    {
        var exc = Assert.Throws<Exception>(  () => {
            LineNotifyManagerJson manj = walkThruJWE( stdExpectedId, correct_secret, correct_secret, 2, 2100 );
        });

        if ( exc != null ){
            Assert.Contains("Lifetime validation failed",exc.ToString());
        }
    }





}
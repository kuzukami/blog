using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;


namespace _20211129_my_api_line_notify_token_src.Tests;

public class FunctionTest
{
    public FunctionTest()
    {
    }

    [Fact]
    public void TestGetMethod()
    {
        TestLambdaContext context;
        APIGatewayProxyRequest request;
        APIGatewayProxyResponse response;

        Functions functions = new Functions();


        request = new APIGatewayProxyRequest();
        context = new TestLambdaContext();
        response = functions.Get(request, context);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("Hello AWS Serverless", response.Body);
    }
}
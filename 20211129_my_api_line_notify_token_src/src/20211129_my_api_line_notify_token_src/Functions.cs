using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace _20211129_my_api_line_notify_token_src;

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
        context.Logger.LogInformation("Get Request\n");

        Function f = new Function();
        ApiResponse a = f.FunctionHandler(request, context);

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)a.StatusCode, //(int)HttpStatusCode.OK,
            Body =  a.Body, // "Hello AWS Serverless",
            Headers = a.Headers, // new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}
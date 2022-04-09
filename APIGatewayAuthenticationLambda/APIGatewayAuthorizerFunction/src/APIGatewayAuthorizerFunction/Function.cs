using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace APIGatewayAuthorizerFunction
{
    public class Function
    {
        public object FunctionHandler(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            if (request.QueryStringParameters.TryGetValue("authorized", out var authValue) && string.Equals("true", authValue))
            {
                return new
                {
                    isAuthorized = true,
                    context = new
                    {
                        userId = "andy"
                    }
                };
            }

            return new
            {
                isAuthorized = false
            };
        }
    }
}
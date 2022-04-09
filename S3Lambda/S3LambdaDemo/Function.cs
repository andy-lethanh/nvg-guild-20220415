using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.S3.Model;
using Aspose.Words;
using Microsoft.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace S3LambdaDemo;

public class Function
{
    private static readonly RecyclableMemoryStreamManager StreamManager = new();

    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        foreach (var record in evnt.Records)
        {
            context.Logger.LogInformation(JsonSerializer.Serialize(record));
            var bucketName = record.S3.Bucket.Name;
            var key = record.S3.Object.Key;
            var wordObject = await S3Client.GetObjectAsync(bucketName, key);

            await using var hashStream = wordObject.ResponseStream;
            await using var readStream = StreamManager.GetStream();
            await hashStream.CopyToAsync(readStream);
            readStream.Seek(0, SeekOrigin.Begin);
            var doc = new Document(readStream);

            await using var writeStream = StreamManager.GetStream();
            doc.Save(writeStream, SaveFormat.Pdf);
            await S3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                AutoCloseStream = true,
                InputStream = writeStream,
                Key = "file/pdf/" + GetPdfFileName(key)
            });
        }
    }

    private static string GetPdfFileName(string objectKey)
    {
        return objectKey.Split("/")
            .Last()
            .Replace(".docx", ".pdf")
            .Replace(".doc", ".pdf");
    }
}
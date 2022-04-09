// See https://aka.ms/new-console-template for more information

using Amazon.S3;
using Amazon.S3.Model;

var s3Client = new AmazonS3Client();

Console.Write("PreSigned URL:");

var request = new GetPreSignedUrlRequest
{
    BucketName = "aws-serverless-demo",
    Key = "test",
    Expires = DateTimeOffset.UtcNow.DateTime.AddDays(7)
};
var preSignedUrl = s3Client.GetPreSignedURL(request);

Console.WriteLine(preSignedUrl);
using System.Text.Json.Serialization;
using Amazon.Lambda.SQSEvents;

namespace SimpleApp.AvatarResize;

[JsonSerializable(typeof(SQSEvent))]
[JsonSerializable(typeof(SQSBatchResponse))]
[JsonSerializable(typeof(SQSBatchResponse.BatchItemFailure))]
[JsonSerializable(typeof(SQSEvent.SQSMessage))]
public partial class AvatarResizeJsonContext : JsonSerializerContext
{
}

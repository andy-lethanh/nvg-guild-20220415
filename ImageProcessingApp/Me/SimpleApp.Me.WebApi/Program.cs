using System.Reflection;
using SimpleApp.Me.Infra.FileHelper;
using SimpleApp.Me.WebApi;
using Adl.ModelBinding;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Transfer;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleApp.Me.Application.Requests;
using SimpleApp.Me.Application.Session;
using SimpleApp.Me.Domain.Configuration;
using SimpleApp.Me.Domain.FileHelper;
using ISession = SimpleApp.Me.Application.Session.ISession;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ImageConfiguration>(builder.Configuration.GetSection("ImageConfiguration"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
	options.Filters.Add<RestApiExceptionFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
	options.InvalidModelStateResponseFactory = ErrorModelStateResponseFactory.Instance;
}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.AddContext<MeJsonSerializeContext>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = JwtConfigurations.GetTokenValidationParameters(builder.Configuration);
	});

builder.Services.AddAuthorization(options =>
{
	options.FallbackPolicy = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build();
});

builder.Services.AddScoped<ISession, Session>();
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
	var imageConfiguration = sp.GetRequiredService<IOptions<ImageConfiguration>>().Value;
	return string.IsNullOrWhiteSpace(imageConfiguration.S3CustomEndpoint)
		? new AmazonS3Client()
		: new AmazonS3Client(new AmazonS3Config {ServiceURL = imageConfiguration.S3CustomEndpoint});
});
builder.Services.AddSingleton<ITransferUtility>(sp => new TransferUtility(sp.GetRequiredService<IAmazonS3>()));
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IImageTypeVerifier>(_ => ImageTypeVerifier.CreateDefault());
builder.Services.AddSingleton<IImageValidator, ImageValidator>();
builder.Services.AddSingleton<IImageStore, ImageStore>();
builder.Services.AddSingleton<IImageUrlBuilder, ImageUrlBuilder>();
builder.Services.AddMediatR(typeof(UploadAvatarRequest).Assembly);
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	options.IncludeXmlComments(xmlPath);
});
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = ApiVersion.Default;
});
builder.Services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");

var app = builder.Build();

app.UseHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

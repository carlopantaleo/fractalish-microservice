using System.Text.Json;
using System.Text.Json.Serialization;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Api.Models;
using FractalishMicroservice.Implementation.Aws.Configuration;
using FractalishMicroservice.Implementation.Aws.Vm;
using FractalishMicroservice.Infrastructure.Middlewares;
using FractalishMicroservice.Infrastructure.Osb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.KebabCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IOsbService, OsbService>()
    .AddScoped<IVmInstanceService, AwsVmInstanceService>()
    .AddAwsServices();

// Configure Catalog and AWS services
builder.Services.Configure<CatalogConfiguration>(builder.Configuration.GetSection("Catalog"));
builder.Services.Configure<AwsConfiguration>(builder.Configuration.GetSection("AwsConfiguration"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app
    .UseSwagger()
    .UseSwaggerUI()
    .UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();


await app.RunAsync();


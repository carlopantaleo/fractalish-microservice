using System.Text.Json;
using System.Text.Json.Serialization;
using FractalishMicroservice.Abstractions.Vm;
using FractalishMicroservice.Api.Models;
using FractalishMicroservice.Implementation.Aws.Configuration;
using FractalishMicroservice.Implementation.Aws.Vm;
using FractalishMicroservice.Infrastructure.Middlewares;
using FractalishMicroservice.Infrastructure.Osb;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.KebabCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fractalish Microservice OSB API", Version = "v2" });

        // Include xml documentation for Swagger
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var xmlFile = $"{assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        }
    });

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

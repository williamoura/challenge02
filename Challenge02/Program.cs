using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Challenge02.Api.Middlewares;
using Challenge02.Api.Modules;
using Challenge02.Domain.Queries;
using Challenge02.Infraestructure.AutoMapper;
using Challenge02.Infraestructure.Handlers.Queries;
using Serilog;
using System.Net.Mime;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
    .Enrich.WithProperty("ServiceName", Assembly.GetExecutingAssembly().GetName().Name)
    .WriteTo.Async(x => x.MongoDB(builder.Configuration.GetConnectionString("Log")))
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(Program)));
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetAllDevsQuery)));
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetAllDevsHandler)));

builder.Services.AddDomain();
builder.Services.AddInfraestructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "challenge Desafio Api", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});

builder.Services.AddHealthChecks()
    .AddMongoDb(builder.Configuration.GetConnectionString("Log"), name: "Log DB", failureStatus: HealthStatus.Unhealthy)
    .AddMongoDb(builder.Configuration.GetSection("MongoDB").GetValue<string>("ConnectionString"), name: "MongoDB", failureStatus: HealthStatus.Unhealthy)
    .AddSqlServer(builder.Configuration.GetConnectionString("SqlDatabase"), name: "SQL Database", failureStatus: HealthStatus.Unhealthy)
    .AddAzureServiceBusQueue(builder.Configuration.GetValue<string>("AzureServiceBus:ConnectionString"), "desafio-queue", name: "Service Bus Queue", failureStatus: HealthStatus.Unhealthy)
    .AddUrlGroup(new Uri("https://mockapi.io"), name: "Mock API", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddCors();

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = JsonConvert.SerializeObject(
                new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        exception = x.Value.Exception != null ? x.Value.Exception.Message : "none",
                        duration = x.Value.Duration.ToString()
                    })
                });
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    });
});

app.Run();

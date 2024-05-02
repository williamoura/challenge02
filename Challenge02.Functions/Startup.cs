using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Domain.Queries;
using Challenge02.Domain.Services;
using HttpClientService;
using HttpClientService.Interfaces;
using Challenge02.Infraestructure.AutoMapper;
using Challenge02.Infraestructure.Clients;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Handlers.Queries;
using Challenge02.Infraestructure.Messaging;
using Challenge02.Infraestructure.Repository;
using Challenge02.Infraestructure.Repository.Factories;
using Challenge02.Infraestructure.Serializer;
using Challenge02.Infraestructure.Services;
using Serilog;
using System.Net.Http;
using System.Reflection;
using ILogger = Serilog.ILogger;

[assembly: FunctionsStartup(typeof(Challenge02.Functions.Startup))]
namespace Challenge02.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();

            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            var defaultConfig = serviceProvider.GetRequiredService<IConfiguration>();

            var configuration = builder.GetContext().Configuration;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
                .Enrich.WithProperty("ServiceName", Assembly.GetExecutingAssembly().GetName().Name)
                .WriteTo.Async(x => x.MongoDB(configuration.GetValue<string>("LogConnection")))
                .WriteTo.Console()
                .CreateLogger();

            builder.Services.AddSingleton<ILogger>(Log.Logger);

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddMediatR(Assembly.GetAssembly(typeof(Challenge02.Functions.Startup)));
            builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetAllDevsQuery)));
            builder.Services.AddMediatR(Assembly.GetAssembly(typeof(GetAllDevsHandler)));

            builder.Services.AddScoped<IMongoClient>(s =>
            new MongoClient(configuration.GetValue<string>("MongoDatabaseConnection")));

            builder.Services.AddScoped<IMongoCollection<Dev>>(s =>
            {
                var mongoClient = s.GetRequiredService<IMongoClient>();
                var database = mongoClient.GetDatabase(configuration.GetValue<string>("MongoDatabaseName"));
                return database.GetCollection<Dev>(configuration.GetValue<string>("MongoDatabaseCollection"));
            });

            builder.Services.AddDbContext<DesafioContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetValue<string>("SqlDatabaseConnection"));
            }, ServiceLifetime.Transient);

            builder.Services.AddScoped<IReadDevRepository, MongoDevRepository>();
            builder.Services.AddScoped<IReadDevRepository, SqlDevRepository>();
            builder.Services.AddScoped<IWriteDevRepository, MongoDevRepository>();
            builder.Services.AddScoped<IWriteDevRepository, SqlDevRepository>();

            builder.Services.AddScoped<IWriteRepositoryFactory, WriteRepositoryFactory>();
            builder.Services.AddScoped<IReadRepositoryFactory, ReadRepositoryFactory>();

            builder.Services.AddScoped<IDevIdProvider, DevIdProvider>();
            builder.Services.AddSingleton<IEmailNormalizationService, EmailNormalizationService>();

            builder.Services.AddSingleton<IServiceBusMessageSender>(s => new ServiceBusMessageSender
            (
                configuration.GetValue<string>("ServiceBusConnection"),
                configuration.GetValue<string>("ServiceBusQueueName"),
                s.GetService<ILogger>()
            ));

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ISerialization, CustomSerializer>();

            builder.Services.AddSingleton<HttpClientWrapper>(
                             (s) => new HttpClientWrapper(
                                 s.GetService<HttpClient>(),
                                 s.GetService<ISerialization>()));

            builder.Services.AddSingleton<HttpClientWithErrorHandling>(
                s => new HttpClientWithErrorHandling(s.GetService<HttpClientWrapper>()));

            builder.Services.AddSingleton<IHttpClientWrapper>(
                s => new HttpClientWithLog(
                    s.GetService<HttpClientWithErrorHandling>(),
                    s.GetService<ILogger>()));

            builder.Services.AddSingleton<IMockApiClient>(s =>
                new MockApiClient(
                    configuration.GetValue<string>("MockApiBaseUrl"),
                    s.GetService<IHttpClientWrapper>()));
        }
    }
}
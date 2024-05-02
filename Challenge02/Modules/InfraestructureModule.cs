using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using HttpClientService;
using HttpClientService.Interfaces;
using Challenge02.Infraestructure.Clients;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Configuration;
using Challenge02.Infraestructure.Messaging;
using Challenge02.Infraestructure.Repository;
using Challenge02.Infraestructure.Repository.Factories;
using Challenge02.Infraestructure.Serializer;
using Challenge02.Infraestructure.Services;
using ILogger = Serilog.ILogger;

namespace Challenge02.Api.Modules
{
    public static class InfraestructureModule
    {
        public static IServiceCollection AddInfraestructure(
             this IServiceCollection services, IConfiguration config)
        {
            AddDatabase(services, config);

            services.AddScoped<IDevIdProvider, DevIdProvider>();

            services.AddSingleton<IServiceBusMessageSender>(s => new ServiceBusMessageSender
            (
                config.GetValue<string>("AzureServiceBus:ConnectionString"),
                config.GetValue<string>("AzureServiceBus:QueueName"),
                s.GetService<ILogger>()
            ));

            services.AddHostedService<PollingService>(s => new PollingService(
                s,
                s.GetService<IMockApiClient>(),
                s.GetService<ILogger>(),
                config.GetValue<int>("PollingIntervalInSeconds", 60)));

            services.AddHttpClient();
            services.AddSingleton<ISerialization, CustomSerializer>();

            services.AddSingleton<HttpClientWrapper>(
                             (s) => new HttpClientWrapper(
                                 s.GetService<HttpClient>(),
                                 s.GetService<ISerialization>()));

            services.AddSingleton<HttpClientWithErrorHandling>(
                s => new HttpClientWithErrorHandling(s.GetService<HttpClientWrapper>()));

            services.AddSingleton<IHttpClientWrapper>(
                s => new HttpClientWithLog(
                    s.GetService<HttpClientWithErrorHandling>(),
                    s.GetService<ILogger>()));

            services.AddSingleton<IMockApiClient>(s =>
                new MockApiClient(
                    config.GetValue<string>("MockApiBaseUrl"),
                    s.GetService<IHttpClientWrapper>()));


            return services;
        }

        private static void AddDatabase(IServiceCollection services, IConfiguration config)
        {
            var mongoDBSettings = config.GetSection("MongoDB").Get<MongoDBSettings>();

            services.AddSingleton(mongoDBSettings);
            services.AddScoped<IMongoClient>(s => new MongoClient(mongoDBSettings.ConnectionString));

            services.AddScoped<IMongoCollection<Dev>>(s =>
            {
                var mongoClient = s.GetRequiredService<IMongoClient>();
                var database = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
                return database.GetCollection<Dev>(mongoDBSettings.CollectionName);
            });

            services.AddDbContext<DesafioContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("SqlDatabase"));
            });

            services.AddScoped<IReadDevRepository, MongoDevRepository>();
            services.AddScoped<IReadDevRepository, SqlDevRepository>();
            services.AddScoped<IWriteDevRepository, MongoDevRepository>();
            services.AddScoped<IWriteDevRepository, SqlDevRepository>();

            services.AddScoped<IWriteRepositoryFactory, WriteRepositoryFactory>();
            services.AddScoped<IReadRepositoryFactory, ReadRepositoryFactory>();
        }
    }
}

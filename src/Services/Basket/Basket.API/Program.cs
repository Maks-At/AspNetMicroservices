using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Redis Config
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            //General Config
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //gRPC Config
            builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
                o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
            builder.Services.AddScoped<DiscountGrpcService>();

            //MassTransit-RabbitMQ Configuration
            builder.Services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
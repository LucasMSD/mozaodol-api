
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProjetoTelegram.Api.Hubs;
using ProjetoTelegram.Application.Implementations.AuthImplementations;
using ProjetoTelegram.Application.Implementations.ChatImplementations;
using ProjetoTelegram.Application.Implementations.UserImplementations;
using ProjetoTelegram.Application.Interfaces.AuthInterfaces;
using ProjetoTelegram.Application.Interfaces.ChatInterfaces;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Domain.Config.JsonConverters;
using ProjetoTelegram.Domain.Config.JwtConfig;
using ProjetoTelegram.Domain.Config.MongoSettings;
using ProjetoTelegram.Domain.Repositories;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using StackExchange.Redis;
using System.Text;

namespace ProjetoTelegram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            MongoSettngs.ConfigureDateSerialization();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                //options.Configuration = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>().ConnectionString;

                var config = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>();
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints = { config.ConnectionString },
                    User = config.User,
                    Password = config.Password
                };
            });

            // Add services to the container.
            builder.Services.AddSignalR(options =>
            {
                options.DisableImplicitFromServicesParameters = true;
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new ObjectIdToStringConverter());
            }); ;

            // MongoDB
            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
            builder.Services.AddScoped<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Jwt
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chatHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new ObjectIdToStringConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<DbContext, DbContext>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}

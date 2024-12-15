
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Mozaodol.Api.Hubs;
using Mozaodol.Application.Services.StorageServices;
using Mozaodol.Application.UseCases.Auth.AuthUseCases;
using Mozaodol.Application.UseCases.ChatUseCases;
using Mozaodol.Application.UseCases.UserUseCases;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Repositories.StorageRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;
using Mozaodol.Infrastructure.Config.Serialization.JsonSerialization;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;
using Mozaodol.Infrastructure.Contexts.RedisDBContexts;
using Mozaodol.Infrastructure.Repositories.ChatRepositories;
using Mozaodol.Infrastructure.Repositories.MessageRepositories;
using Mozaodol.Infrastructure.Repositories.StorageRepositories;
using Mozaodol.Infrastructure.Repositories.UserRepositories;
using Mozaodol.Infrastructure.Services.NotificationServices.ExpoPushNotificationServices;
using Mozaodol.Infrastructure.Services.NotificationServices.SignalRNotificationServices;
using Mozaodol.Infrastructure.Services.StorageProviderServices.GoogleStorageService;
using Mozaodol.Infrastructure.Services.TokenServices.JwtTokenServices;
using StackExchange.Redis;
using System.Text;

namespace Mozaodol
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            MongoDBSettings.ConfigureDateSerialization();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
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

            builder.Services.Configure<GoogleStorageSettings>(builder.Configuration.GetSection(nameof(GoogleStorageSettings)));
            builder.Services.AddSingleton<IStorageProviderService, GoogleStorageService>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new ObjectIdToStringConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<MongoDBContext, MongoDBContext>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IStorageRepository, StorageRepository>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();
            
            builder.Services.AddScoped<IRealTimeNotificationService, SignalRNotificationService<ChatHub>>();
            builder.Services.AddScoped<IPushNotificationService, ExpoPushNotificationService>();


            builder.Services.AddScoped<IGetCurrentUserDtoUseCase, GetCurrentUserDtoUseCase>();
            builder.Services.AddScoped<IGetUserDtoUseByIdUseCase, GetUserDtoUseByIdUseCase>();
            builder.Services.AddScoped<IUpdatePushTokenUseCase, UpdatePushTokenUseCase>();
            builder.Services.AddScoped<IListUserChatsUseCase, ListUserChatsUseCase>();
            builder.Services.AddScoped<IListChatMessagesUseCase, ListChatMessagesUseCase>();

            builder.Services.AddScoped<IOnConnectedUseCase, OnConnectedUseCase>();
            builder.Services.AddScoped<IOnDisconnectedUseCase, OnDisconnectedUseCase>();
            builder.Services.AddScoped<IOnLeftChatUseCase, OnLeftChatUseCase>();
            builder.Services.AddScoped<IOnOpenedChatUseCase, OnOpenedChatUseCase>();
            builder.Services.AddScoped<IOnSeenMessageUseCase, OnSeenMessageUseCase>();
            builder.Services.AddScoped<IOnSendMessageUseCase, OnSendMessageUseCase>();


            builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>();
            builder.Services.AddScoped<IAuthSignupUseCase, AuthSignupUseCase>();
            builder.Services.AddScoped<IOnTypingUseCase, OnTypingUseCase>();

            builder.Services.AddScoped<IStorageService, StorageService>();

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

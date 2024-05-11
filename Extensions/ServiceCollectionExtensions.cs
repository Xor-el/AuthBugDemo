using AuthBugDemo.Authorization;
using AuthBugDemo.Authorization.Policy.RequirementHandlers.ApiKey;
using AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.ApiKey;
using AuthBugDemo.Authorization.Policy.Requirements.ApiKey;
using AuthBugDemo.Models.Authentication.ApiKey;
using AuthBugDemo.Models.Authorization.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthBugDemo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureHost(this WebApplicationBuilder builder)
        {
            builder.AddConfigurationProviders();

            builder.AddLoggingProviders();
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureCoreServices();

            services.ConfigureAuthentication(configuration);
            services.ConfigureAuthorization();

            return services;
        }

        private static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }

        private static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
            })
            .AddApiKey(options =>
            {
                options.HeaderName = configuration.GetValue<string>("Authentication:ApiKey:HeaderName");
                options.Key = configuration.GetValue<string>("Authentication:ApiKey:Key");
            });

            return services;
        }

        private static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationPolicies();

            services.AddPolicyHandlers();

            services.AddScoped<IApiKeyRequirementHandlerValidator, ApiRequirementHandlerValidator>();

            return services;
        }

        private static void AddPolicyHandlers(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, ApiKeyImplicitFailRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ApiKeyExplicitFailRequirementHandler>();
        }

        private static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(config =>
            {
                config.AddPolicy(ApiKeyPolicies.ImplicitFail, options =>
                {
                    options.RequireAuthenticatedUser();
                    options.AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme);
                    options.AddRequirements(new ApiKeyImplicitFailRequirement());
                });

                config.AddPolicy(ApiKeyPolicies.ExplicitFail, options =>
                {
                    options.RequireAuthenticatedUser();
                    options.AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme);
                    options.AddRequirements(new ApiKeyExplicitFailRequirement());
                });

                //Set the fallback and default authorization policies
                config.FallbackPolicy = config.GetPolicy(ApiKeyPolicies.ImplicitFail);
                config.DefaultPolicy = config.GetPolicy(ApiKeyPolicies.ImplicitFail)!;

            });

            services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationResultTransformer>();
        }

        private static void AddConfigurationProviders(this WebApplicationBuilder builder)
        {
            builder.Host.ConfigureServices((hostingContext, _) =>
            {
                var hostEnvironment = hostingContext.HostingEnvironment;
                var environmentName = hostEnvironment.EnvironmentName.ToLower();
                builder.Configuration.Sources.Clear();
                builder.Configuration
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            });
        }

        private static void AddLoggingProviders(this WebApplicationBuilder builder)
        {
            builder.Host.ConfigureServices((hostingContext, _) =>
            {
                builder.Logging.ClearProviders()
                               .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                               .AddConsole()
                               .AddDebug()
                               .AddEventSourceLogger();
            });
        }
    }
}


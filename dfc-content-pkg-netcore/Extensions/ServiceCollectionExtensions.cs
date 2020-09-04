using dfc_cmsapi_pkg_netcore.Data.models;
using dfc_content_pkg_netcore.ApiProcessorService;
using dfc_content_pkg_netcore.CmsApiProcessorService;
using dfc_content_pkg_netcore.contracts;
using dfc_content_pkg_netcore.models.clientOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Net.Http;


namespace dfc_cmsapi_pkg_netcore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApiServices(this ServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            var policyRegistry = services.AddPolicyRegistry();

            services
               .AddPolicies(policyRegistry, nameof(CmsApiClientOptions), policyOptions)
               .AddHttpClient<ICmsApiService, CmsApiService, CmsApiClientOptions>(configuration, nameof(CmsApiClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

        }

        public static IServiceCollection AddPolicies(
           this IServiceCollection services,
           IPolicyRegistry<string> policyRegistry,
           string keyPrefix,
           PolicyOptions policyOptions)
        {
            _ = policyOptions ?? throw new ArgumentNullException(nameof(policyOptions));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpRetry)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpCircuitBreaker)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }
        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
                    this IServiceCollection services,
                    IConfiguration configuration,
                    string configurationSectionName,
                    string retryPolicyName,
                    string circuitBreakerPolicyName)
                    where TClient : class
                    where TImplementation : class, TClient
                    where TClientOptions : ClientOptionsModel, new() =>
                    services
                        .Configure<TClientOptions>(options => configuration?.GetSection(configurationSectionName))
                        .AddHttpClient<TClient, TImplementation>()
                        .ConfigureHttpClient((sp, options) =>
                        {
                            var httpClientOptions = sp
                                .GetRequiredService<IOptions<TClientOptions>>()
                                .Value;
                            options.BaseAddress = httpClientOptions.BaseAddress;
                            options.Timeout = httpClientOptions.Timeout;

                            if (!string.IsNullOrWhiteSpace(httpClientOptions.ApiKey))
                            {
                                options.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", httpClientOptions.ApiKey);
                            }
                        })
                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                        {
                            AllowAutoRedirect = false,
                        })
                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
                        .Services;
    }

}


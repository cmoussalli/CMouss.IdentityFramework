using Microsoft.Extensions.DependencyInjection;

namespace CMouss.IdentityFramework.BlazorUI
{
    /// <summary>
    /// Extension methods for registering CMouss.IdentityFramework.BlazorUI services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the CMouss.IdentityFramework.BlazorUI services to the service collection.
        /// This includes the cookie-based authentication service and any other required dependencies.
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddIdentityFrameworkBlazorUI(this IServiceCollection services)
        {
            // Register cookie authentication service
            services.AddScoped<CookieAuthService>();

            // Add any other future services here as the library grows
            // services.AddScoped<OtherService>();

            return services;
        }
    }
}

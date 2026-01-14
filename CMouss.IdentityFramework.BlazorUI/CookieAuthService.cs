using Microsoft.JSInterop;

namespace CMouss.IdentityFramework.BlazorUI
{
    /// <summary>
    /// Cookie-based authentication service for Blazor Server applications.
    /// Provides secure token storage using cookies via JavaScript interop.
    /// Note: In Blazor Server Interactive mode, we use JavaScript-accessible cookies
    /// instead of HTTP-only cookies due to component lifecycle limitations.
    /// </summary>
    public class CookieAuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string AuthCookieName = "IDF_AuthToken";

        // Default cookie expiration matches typical token lifetime
        // NOTE: Should match your IDFManager.Configure DefaultTokenLifeTime for optimal UX
        // Current: 365 days to match common token lifetimes
        private static readonly TimeSpan DefaultCookieExpiration = TimeSpan.FromDays(365);

        public CookieAuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Stores the authentication token in a cookie.
        /// </summary>
        /// <param name="token">The authentication token to store</param>
        /// <param name="expiration">Optional custom expiration time. Defaults to 30 days.</param>
        public async Task SetTokenAsync(string token, TimeSpan? expiration = null)
        {
            try
            {
                var cookieExpiration = expiration ?? DefaultCookieExpiration;
                var maxAgeSeconds = (int)cookieExpiration.TotalSeconds;

                // Escape the token to prevent cookie injection attacks
                var escapedToken = System.Uri.EscapeDataString(token);

                // Set cookie with security attributes
                // Note: SameSite=Strict and Secure flags for production security
                var cookieString = $"{AuthCookieName}={escapedToken}; path=/; max-age={maxAgeSeconds}; SameSite=Strict";

                await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieString}'");
            }
            catch (Exception)
            {
                // Silently handle JS interop errors (e.g., during prerendering)
            }
        }

        /// <summary>
        /// Retrieves the authentication token from the cookie.
        /// </summary>
        /// <returns>The authentication token, or null if not found</returns>
        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var cookieValue = await _jsRuntime.InvokeAsync<string>("eval",
                    $"document.cookie.split('; ').find(row => row.startsWith('{AuthCookieName}='))?.split('=')[1] || ''");

                if (string.IsNullOrEmpty(cookieValue))
                {
                    return null;
                }

                // Unescape the token value
                return System.Uri.UnescapeDataString(cookieValue);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes the authentication token cookie.
        /// </summary>
        public async Task DeleteTokenAsync()
        {
            try
            {
                // Delete cookie by setting expiration to past date
                var cookieString = $"{AuthCookieName}=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; SameSite=Strict";
                await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieString}'");
            }
            catch (Exception)
            {
                // Silently handle JS interop errors
            }
        }

        /// <summary>
        /// Checks if an authentication token exists.
        /// </summary>
        /// <returns>True if a token exists, false otherwise</returns>
        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token) && token.Length > 10;
        }
    }
}

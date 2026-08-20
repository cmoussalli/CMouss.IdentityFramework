using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CMouss.IdentityFramework.BlazorUI.Admin
{
    /// <summary>
    /// Base class of every Identity Framework management component.
    /// It resolves the signed in user (from the AuthLayout cascading model or from the auth cookie),
    /// checks that this user is allowed to manage the identity framework, and centralizes
    /// the error / success message handling.
    /// </summary>
    public abstract class IdfAdminComponentBase : ComponentBase
    {
        [Inject] protected CookieAuthService CookieAuth { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected NavigationManager NavManager { get; set; } = default!;

        [CascadingParameter] protected AuthLayoutModel? AuthModel { get; set; }

        /// <summary>
        /// When true (default) the component is only rendered for users holding one of
        /// IDFBlazorUIAdminConfig.AdminRoleIds. Set it to false when the hosting page already
        /// handles the authorization.
        /// </summary>
        [Parameter] public bool RequireAdminRole { get; set; } = true;

        /// <summary>
        /// Raised whenever the component changed something, so the hosting page can refresh itself.
        /// </summary>
        [Parameter] public EventCallback OnChanged { get; set; }

        protected List<string> Errors { get; set; } = new();
        protected string SuccessMessage { get; set; } = "";

        protected bool IsLoading { get; set; } = true;
        protected bool IsReady { get; set; }
        protected bool IsAuthorized { get; set; }

        protected User? CurrentUser { get; set; }
        protected string CurrentToken { get; set; } = "";

        protected string CurrentUserId
        {
            get { return CurrentUser is null ? "" : CurrentUser.Id; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ResolveAccessAsync();
                if (IsAuthorized)
                {
                    await SafeLoadAsync();
                }
                IsReady = true;
                IsLoading = false;
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Loads the data displayed by the component, called once the access has been granted.
        /// </summary>
        protected virtual Task LoadDataAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task SafeLoadAsync()
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
        }

        /// <summary>
        /// Reloads the component data and clears the messages.
        /// </summary>
        public async Task RefreshAsync()
        {
            ClearMessages();
            IsLoading = true;
            StateHasChanged();
            await SafeLoadAsync();
            IsLoading = false;
            StateHasChanged();
        }

        #region Access

        protected async Task ResolveAccessAsync()
        {
            try
            {
                if (AuthModel is not null && AuthModel.IsAuthenticated && AuthModel.User is not null)
                {
                    CurrentUser = AuthModel.User;
                    CurrentToken = AuthModel.Token;
                }
                else
                {
                    string? token = await CookieAuth.GetTokenAsync();
                    if (!String.IsNullOrEmpty(token) && token.Length > 10)
                    {
                        UserToken? userToken = IDFManager.userTokenService.Validate(token, TokenValidationMode.UseDefault);
                        if (userToken is not null)
                        {
                            CurrentUser = userToken.User;
                            CurrentToken = token;
                        }
                    }
                }

                IsAuthorized = ValidateAdminAccess();
            }
            catch (Exception ex)
            {
                IsAuthorized = false;
                AddError(ex);
            }
        }

        /// <summary>
        /// False for the components a signed in user runs on its own account (profile, own tokens),
        /// true for the management screens.
        /// </summary>
        protected virtual bool IsAdminScreen
        {
            get { return true; }
        }

        private bool ValidateAdminAccess()
        {
            if (CurrentUser is null) { return false; }
            if (!IsAdminScreen) { return true; }
            if (!RequireAdminRole) { return true; }

            List<string> adminRoles = IDFBlazorUIAdminConfig.GetAdminRoleIds();
            if (adminRoles.Count == 0) { return true; }

            //The database is the reference, the token claims are only used as a fallback
            try
            {
                if (IDFManager.userService.ValidateUserRole(CurrentUser.Id, adminRoles))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                //Fall through to the claim based validation
            }

            if (CurrentUser.Roles is not null)
            {
                return CurrentUser.Roles.Any(o => adminRoles.Any(a => String.Equals(a, o.Id, StringComparison.OrdinalIgnoreCase)));
            }
            return false;
        }

        #endregion

        #region Messages

        protected void ClearMessages()
        {
            Errors = new();
            SuccessMessage = "";
        }

        protected void AddError(string message)
        {
            if (!String.IsNullOrEmpty(message)) { Errors.Add(message); }
        }

        protected void AddError(Exception ex)
        {
            Exception e = ex;
            while (e.InnerException is not null) { e = e.InnerException; }
            AddError(e.Message);
        }

        protected void SetSuccess(string message)
        {
            SuccessMessage = message;
        }

        /// <summary>
        /// Runs a management action, turns any exception into a displayed error and
        /// notifies the hosting page when the action succeeded.
        /// </summary>
        protected async Task<bool> ExecuteAsync(Func<Task> action, string successMessage)
        {
            ClearMessages();
            IsLoading = true;
            StateHasChanged();
            bool result = false;
            try
            {
                await action();
                SetSuccess(successMessage);
                result = true;
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            IsLoading = false;

            if (result)
            {
                await SafeLoadAsync();
                if (OnChanged.HasDelegate) { await OnChanged.InvokeAsync(); }
            }
            StateHasChanged();
            return result;
        }

        protected Task<bool> ExecuteAsync(Action action, string successMessage)
        {
            return ExecuteAsync(() => { action(); return Task.CompletedTask; }, successMessage);
        }

        #endregion

        #region Helpers

        protected async Task CopyToClipboardAsync(string value)
        {
            try
            {
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", value);
                SetSuccess("Copied to clipboard");
            }
            catch (Exception)
            {
                //Clipboard is not available (http or unsupported browser), the value stays selectable
            }
        }

        protected static string Mask(string value)
        {
            if (String.IsNullOrEmpty(value)) { return ""; }
            if (value.Length <= 8) { return new string('*', value.Length); }
            return value.Substring(0, 4) + new string('*', 8) + value.Substring(value.Length - 4);
        }

        protected static string Shorten(string value, int length)
        {
            if (String.IsNullOrEmpty(value)) { return ""; }
            if (value.Length <= length) { return value; }
            return value.Substring(0, length) + "...";
        }

        #endregion
    }
}

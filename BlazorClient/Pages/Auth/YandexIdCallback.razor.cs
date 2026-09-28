using BlazorClient.Auth;
using Blazored.Toast.Services;

namespace BlazorClient.Pages.Auth
{
    public partial class YandexIdCallback : ComponentBase
    {
        [SupplyParameterFromQuery(Name = "Token")]
        public string? Token { get; set; }

        [SupplyParameterFromQuery(Name = "RefreshToken")]
        public string? RefreshToken { get; set; }

        [Inject]
        public IToastService ToastService { get; set; } = default!;

        [Inject]
        public IAuthService AuthService { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private bool _isProcessing = true;
        private string? _error;

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                _error = "Токен не получен от Yandex ID";
                _isProcessing = false;
                return;
            }

            try
            {
                // 1. Сохраняем access token
                await AuthService.StoreAccessTokenAsync(Token);

                // 2. Сохраняем refresh token, если он есть
                if (!string.IsNullOrWhiteSpace(RefreshToken))
                {
                    await AuthService.StoreRefreshTokenAsync(RefreshToken);
                }

                // 3. Настраиваем HttpClient
                AuthService.AddDefaultRequestHeaderBearer(Token);

                ToastService.ShowSuccess("Вы успешно вошли через Yandex ID");

                // 4. Редирект на главную
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                _isProcessing = false;
            }
        }

        private void GoToLogin()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}

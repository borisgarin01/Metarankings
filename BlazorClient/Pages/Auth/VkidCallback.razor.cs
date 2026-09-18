using BlazorClient.Auth;
using Blazored.Toast.Services;

namespace BlazorClient.Pages.Auth;

public partial class VkidCallback : ComponentBase
{
    [SupplyParameterFromQuery(Name = "Token")]
    public string? Token { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public IAuthService AuthService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private bool _isProcessing = true;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            _error = "Токен не получен от VK ID";
            _isProcessing = false;
            return;
        }

        try
        {
            // 1. Сохраняем access token
            await AuthService.StoreAccessTokenAsync(Token);

            // 2. Настраиваем HttpClient
            AuthService.AddDefaultRequestHeaderBearer(Token);

            // 3. Опционально: получаем refresh token через отдельный запрос
            // (если бэкенд возвращает его отдельно — см. ниже)

            // 4. Опционально: загружаем данные пользователя
            // await AuthService.LoadCurrentUserAsync();

            ToastService.ShowSuccess("Вы успешно вошли через VK ID");

            // 5. Редирект на главную
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

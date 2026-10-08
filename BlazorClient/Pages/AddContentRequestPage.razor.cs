using Blazored.Toast.Services;
using Domain.ContentRequests;
using Domain.RequestsModels.ContentRequests;
using WebManagers.Derived;

namespace BlazorClient.Pages;

public partial class AddContentRequestPage : CancellableComponentBase
{
    [Inject]
    public ContentRequestsWebManager ContentRequestsWebManager { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private AddContentRequestModel Form { get; set; } = new();

    private IEnumerable<ContentRequest> MyRequests { get; set; } = Enumerable.Empty<ContentRequest>();

    private bool IsLoading { get; set; }

    private bool IsSubmitting { get; set; }

    private bool IsLoginOpen { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Перезагружаем список после входа/выхода (в том числе через модалку на этой странице)
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

        await LoadMyRequestsAsync();
    }

    private async Task LoadMyRequestsAsync()
    {
        AuthenticationState authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authenticationState.User.Identity?.IsAuthenticated != true)
        {
            MyRequests = Enumerable.Empty<ContentRequest>();
            return;
        }

        IsLoading = true;
        try
        {
            MyRequests = await ContentRequestsWebManager.GetMineAsync(DisposalToken);
        }
        catch (Exception) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError("Не удалось загрузить ваши заявки");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SubmitAsync()
    {
        IsSubmitting = true;
        try
        {
            HttpResponseMessage response = await ContentRequestsWebManager.AddAsync(Form, DisposalToken);

            if (!response.IsSuccessStatusCode)
            {
                ToastService.ShowWarning(await ReadErrorMessageAsync(response) ?? "Не удалось отправить заявку");
                return;
            }

            ToastService.ShowSuccess("Заявка отправлена. Спасибо!");
            Form = new AddContentRequestModel();
            await LoadMyRequestsAsync();
        }
        catch (Exception) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError("Не удалось отправить заявку");
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task WithdrawAsync(ContentRequest contentRequest)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Отозвать заявку «{contentRequest.Title}»?");
        if (!confirmed)
            return;

        HttpResponseMessage response = await ContentRequestsWebManager.DeleteAsync(contentRequest.Id, DisposalToken);
        if (!response.IsSuccessStatusCode)
        {
            ToastService.ShowWarning("Не удалось отозвать заявку");
            return;
        }

        ToastService.ShowSuccess("Заявка отозвана");
        await LoadMyRequestsAsync();
    }

    /// <summary>
    /// Текст ошибки от API: строка приходит в JSON-кавычках, ProblemDetails (валидация) не показываем.
    /// </summary>
    private static async Task<string?> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        string body = (await response.Content.ReadAsStringAsync()).Trim();

        if (body.StartsWith('"'))
            return JsonSerializer.Deserialize<string>(body);

        return string.IsNullOrEmpty(body) || body.StartsWith('{') ? null : body;
    }

    private void OpenLogin() => IsLoginOpen = true;

    private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
    {
        _ = InvokeAsync(async () =>
        {
            await LoadMyRequestsAsync();
            StateHasChanged();
        });
    }

    protected override void Dispose(bool disposing)
    {
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}

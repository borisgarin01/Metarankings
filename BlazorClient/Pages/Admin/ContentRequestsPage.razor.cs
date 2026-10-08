using BlazorClient.Components.PagesComponents.ContentRequests;
using Blazored.Toast.Services;
using Domain.ContentRequests;
using Domain.RequestsModels.ContentRequests;
using WebManagers.Derived;

namespace BlazorClient.Pages.Admin;

public partial class ContentRequestsPage : CancellableComponentBase
{
    [Inject]
    public ContentRequestsWebManager ContentRequestsWebManager { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<ContentRequest> Requests { get; set; } = Enumerable.Empty<ContentRequest>();

    /// <summary>
    /// Черновики ответов администратора по Id заявки.
    /// </summary>
    private Dictionary<long, string?> Comments { get; set; } = new();

    private ContentRequestStatus? StatusFilter { get; set; } = ContentRequestStatus.Pending;

    private bool IsLoading { get; set; }

    private bool IsBusy { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (!authenticationState.User.IsInRole("Admin"))
            return;

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            Requests = (await ContentRequestsWebManager.GetAllAsync(StatusFilter, DisposalToken)).ToList();
            Comments = Requests.ToDictionary(contentRequest => contentRequest.Id, contentRequest => contentRequest.AdminComment);
        }
        catch (Exception) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError("Не удалось загрузить заявки");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SetFilterAsync(ContentRequestStatus? status)
    {
        StatusFilter = status;
        await LoadAsync();
    }

    private async Task SetStatusAsync(ContentRequest contentRequest, ContentRequestStatus status)
    {
        await UpdateAsync(contentRequest, status, $"Статус заявки: {ContentRequestLabels.GetStatusLabel(status)}");
    }

    private async Task SaveCommentAsync(ContentRequest contentRequest)
    {
        await UpdateAsync(contentRequest, contentRequest.Status, "Ответ сохранён");
    }

    private async Task UpdateAsync(ContentRequest contentRequest, ContentRequestStatus status, string successMessage)
    {
        IsBusy = true;
        try
        {
            HttpResponseMessage response = await ContentRequestsWebManager.UpdateStatusAsync(contentRequest.Id, new UpdateContentRequestStatusModel
            {
                Status = status,
                AdminComment = Comments.GetValueOrDefault(contentRequest.Id)
            }, DisposalToken);

            if (!response.IsSuccessStatusCode)
            {
                ToastService.ShowWarning("Не удалось обновить заявку");
                return;
            }

            ToastService.ShowSuccess(successMessage);
            await LoadAsync();
        }
        catch (Exception) when (!DisposalToken.IsCancellationRequested)
        {
            ToastService.ShowError("Не удалось обновить заявку");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync(ContentRequest contentRequest)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Удалить заявку «{contentRequest.Title}»?");
        if (!confirmed)
            return;

        HttpResponseMessage response = await ContentRequestsWebManager.DeleteAsync(contentRequest.Id, DisposalToken);
        if (!response.IsSuccessStatusCode)
        {
            ToastService.ShowWarning("Не удалось удалить заявку");
            return;
        }

        ToastService.ShowSuccess("Заявка удалена");
        await LoadAsync();
    }
}

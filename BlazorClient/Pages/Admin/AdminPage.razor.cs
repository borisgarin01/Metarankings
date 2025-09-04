using Domain.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace BlazorClient.Pages.Admin;

[Authorize(Policy = "Admin")]
public partial class AdminPage : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    NavigationManager NavigationManager { get; set; }

    private string FutureAdminEmail { get; set; }

    public async Task AssignToAdminAsync()
    {
        var userAssignToRoleModel = new UserAssignToRoleModel(FutureAdminEmail);

        var response = await HttpClient.PostAsJsonAsync("/api/Auth/AssignToAdmin", userAssignToRoleModel);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Вы не авторизованы для выполнения этого действия");
            return;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Пользователь не найден");
            return;
        }
        // You might want to handle the response
        if (!response.IsSuccessStatusCode)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Ошибка назначения пользователя администратором");
            return;
        }

        await JSRuntime.InvokeVoidAsync("alert", "Пользователь успешно назначен администратором");

        NavigationManager.NavigateTo("/");
    }
}

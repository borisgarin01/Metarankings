using Domain.Games;
using Domain.Movies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class Headerer : ComponentBase
{
    // ===== ПАРАМЕТРЫ =====
    [Parameter]
    public IEnumerable<MovieGenre>? MoviesGenres { get; set; }

    [Parameter]
    public IEnumerable<Platform>? Platforms { get; set; }

    [Parameter]
    public IEnumerable<Genre>? GamesGenres { get; set; }

    // ===== СОСТОЯНИЕ =====
    private bool isMenuOpen = false;
    private bool isLoginOpen = false;
    private bool isSticky = false;
    private string searchQuery = "";
    private ElementReference headerBottomRef;

    // ===== INJECT =====
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    // ===== ЖИЗНЕННЫЙ ЦИКЛ =====
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Инициализируем скролл и клики
            await JS.InvokeVoidAsync("initHeader", headerBottomRef);
        }
    }

    // ===== МЕТОДЫ =====
    private async Task ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (isMenuOpen)
        {
            await JS.InvokeVoidAsync("document.body.classList.add", "menu-open");
        }
        else
        {
            await JS.InvokeVoidAsync("document.body.classList.remove", "menu-open");
        }
    }

    private void OpenLogin()
    {
        isLoginOpen = true;
        // Закрываем меню если открыто
        if (isMenuOpen)
        {
            _ = ToggleMenu();
        }
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            Search();
        }
    }

    private async Task OnLoginSuccessHandler()
    {
        isLoginOpen = false;
        await AuthProvider.GetAuthenticationStateAsync();
        StateHasChanged();
    }

    private void Search()
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            NavigationManager.NavigateTo($"/search/?SearchText={Uri.EscapeDataString(searchQuery)}");
        }
    }
}
using Domain.Common.News;
using WebManagers.Derived;

namespace BlazorClient.Pages.Admin;

public partial class NewsPage : ComponentBase
{
    [Inject] 
    public NewsWebManager NewsWebManager { get; set; } = default!;
    
    [Inject] 
    public IJSRuntime JSRuntime { get; set; } = default!;

    public IEnumerable<NewsItem> NewsItems { get; set; } = new List<NewsItem>();

    private long? _editingId;
    private bool _isSaving;
    private bool _isError;
    private string? _message;

    // Форма редактирования/создания — UserId здесь не нужен,
    // он берётся на сервере из JWT-claims.
    private UpdateNewsItemModel _form = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadNewsAsync();
    }

    private async Task LoadNewsAsync()
    {
        try
        {
            NewsItems = await NewsWebManager.GetAllAsync() ?? new List<NewsItem>();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка загрузки: {ex.Message}");
        }
    }

    private async Task SaveAsync()
    {
        _isSaving = true;
        var wasEditing = _editingId is not null;
        try
        {
            if (!wasEditing)
            {
                // Маппинг формы -> AddNewsItemModel (frontend, без UserId)
                var addModel = new AddNewsItemModel
                {
                    Title = _form.Title,
                    TextContent = _form.TextContent,
                    ImageSource = _form.ImageSource
                };

                var response = await NewsWebManager.AddAsync(addModel);
                if (!response.IsSuccessStatusCode)
                {
                    ShowError($"Не удалось добавить: {response.StatusCode}");
                    return;
                }
            }
            else
            {
                // Маппинг формы -> UpdateNewsItemModel (frontend, без UserId)
                var updateModel = new UpdateNewsItemModel
                {
                    Title = _form.Title,
                    TextContent = _form.TextContent,
                    ImageSource = _form.ImageSource
                };

                var updated = await NewsWebManager.UpdateAsync(_editingId!.Value, updateModel);
                if (updated is null)
                {
                    ShowError("Не удалось обновить новость.");
                    return;
                }
            }

            ResetForm();
            ShowSuccess(wasEditing ? "Новость обновлена." : "Новость добавлена.");
            await LoadNewsAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private void StartEdit(NewsItem news)
    {
        _editingId = news.Id;
        _form = new UpdateNewsItemModel
        {
            Title = news.Title,
            TextContent = news.TextContent,
            ImageSource = news.ImageSource
            // UserId НЕ присваиваем — его на сервере возьмут из JWT-claims
        };
        _message = null;
    }

    private void CancelEdit()
    {
        ResetForm();
        _message = null;
    }

    private async Task DeleteAsync(long id)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Удалить новость?");
        if (!confirmed) return;

        try
        {
            var response = await NewsWebManager.DeleteAsync(id);
            if (!response.IsSuccessStatusCode)
            {
                ShowError($"Не удалось удалить: {response.StatusCode}");
                return;
            }

            if (_editingId == id) ResetForm();

            ShowSuccess("Новость удалена.");
            await LoadNewsAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка удаления: {ex.Message}");
        }
    }

    private void ResetForm()
    {
        _editingId = null;
        _form = new UpdateNewsItemModel();
    }

    private void ShowSuccess(string text)
    {
        _isError = false;
        _message = text;
    }

    private void ShowError(string text)
    {
        _isError = true;
        _message = text;
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }
}
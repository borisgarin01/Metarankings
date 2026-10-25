using Domain.Movies;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class DragDropCrewEditor : ComponentBase, IAsyncDisposable
{
    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public Dictionary<MovieCrewRole, string> Texts { get; set; } = new();

    private ElementReference _listElement;
    private DotNetObjectReference<DragDropCrewEditor>? _selfRef;
    private bool _sortableInitialized;
    private int _lastRenderedCount = -1;
    private MovieCrewRole _newRole = MovieCrewRole.Actor;   // ← было Director, такого значения нет
    private string? _newName;

    private List<CrewEntry> Members { get; set; } = new();

    public sealed record CrewEntry(string Key, string Name, MovieCrewRole Role);

    protected override void OnParametersSet()
    {
        // Если Texts поменялся извне (например, после загрузки фильма) и компонент ещё не рендерился —
        // строим Members заново. После первого рендера Members — источник истины.
        if (!_sortableInitialized)
            RebuildMembersFromTexts();
    }

    private void RebuildMembersFromTexts()
    {
        Members = Texts
            .Where(kv => Enum.IsDefined(kv.Key))   // отсекаем неизвестные роли
            .OrderBy(kv => (int)kv.Key)
            .SelectMany(kv => kv.Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(name => new CrewEntry($"{kv.Key}:{name.Trim()}", name.Trim(), kv.Key)))
            .GroupBy(m => m.Key)
            .Select(g => g.First())                // защита от дублей
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Members.Count > 0)
        {
            _selfRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("sortableInterop.create", _listElement, _selfRef, nameof(OnOrderChanged));
            _sortableInitialized = true;
        }
        else if (_sortableInitialized)
        {
            if (_lastRenderedCount <= 0 && Members.Count > 0)
            {
                _selfRef ??= DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("sortableInterop.create", _listElement, _selfRef, nameof(OnOrderChanged));
            }

            await JS.InvokeVoidAsync("sortableInterop.refresh", _listElement);
        }

        _lastRenderedCount = Members.Count;
    }

    [JSInvokable]
    public async Task OnOrderChanged(string[] orderedIds)
    {
        Dictionary<string, CrewEntry> byKey = new();
        foreach (CrewEntry m in Members)
            byKey.TryAdd(m.Key, m);

        List<CrewEntry> reordered = orderedIds
            .Select(id => byKey.TryGetValue(id, out CrewEntry? m) ? m : null)
            .Where(m => m is not null)
            .Cast<CrewEntry>()
            .ToList();

        if (reordered.Count != Members.Count)
            return;

        Members = reordered;
        RebuildTexts();
        await InvokeAsync(StateHasChanged);
    }

    private void AddMember()
    {
        if (string.IsNullOrWhiteSpace(_newName)) return;
        string name = _newName.Trim();
        string key = $"{_newRole}:{name}#{Guid.NewGuid():N}";
        Members.Add(new CrewEntry(key, name, _newRole));
        _newName = null;
        RebuildTexts();
    }

    private void RemoveMember(CrewEntry member)
    {
        Members.RemoveAll(m => m.Key == member.Key);
        RebuildTexts();
    }

    private void RebuildTexts()
    {
        foreach (MovieCrewRole role in Enum.GetValues<MovieCrewRole>())
            Texts[role] = string.Join('\n', Members.Where(m => m.Role == role).Select(m => m.Name));
    }

    public async ValueTask DisposeAsync()
    {
        if (_sortableInitialized)
        {
            try { await JS.InvokeVoidAsync("sortableInterop.destroy", _listElement); }
            catch (JSDisconnectedException) { }
        }

        _selfRef?.Dispose();
    }
}
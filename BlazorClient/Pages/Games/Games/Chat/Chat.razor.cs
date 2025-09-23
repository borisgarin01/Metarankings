using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;

namespace BlazorClient.Pages.Games.Games.Chat;

public partial class Chat : ComponentBase, IAsyncDisposable
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter, EditorRequired]
    public long? GameId { get; set; }

    private HubConnection? hubConnection;
    private List<string> messages = [];
    private string? messageInput;

    private string joinRoomStyle = "display:block";
    private string sendMessageStyle = "display:none";
    private string connectionStatus = "Disconnected";

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            var encodedMsg = $"{user}: {message}";
            messages.Add(encodedMsg);
            InvokeAsync(StateHasChanged);
        });

        hubConnection.Reconnecting += ex =>
        {
            connectionStatus = "Reconnecting...";
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        };

        hubConnection.Reconnected += connectionId =>
        {
            connectionStatus = "Connected";
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        };

        hubConnection.Closed += ex =>
        {
            connectionStatus = "Disconnected";
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        };

        try
        {
            await hubConnection.StartAsync();
            connectionStatus = "Connected";
        }
        catch (Exception ex)
        {
            messages.Add($"Connection error: {ex.Message}");
        }
    }

    [CascadingParameter]
    private Task<AuthenticationState>? authenticationState { get; set; }

    private ClaimsPrincipal currentUser;

    protected override async Task OnParametersSetAsync()
    {
        if (authenticationState is not null)
        {
            AuthenticationState authState = await authenticationState;
            currentUser = authState?.User;
        }
        await JoinRoom();
    }

    private async Task JoinRoom()
    {
        if (hubConnection is not null)
        {
            try
            {
                await hubConnection.SendAsync("AddToGroup", GameId.ToString(), currentUser.Claims.First(b => b.Type == ClaimTypes.NameIdentifier).Value);
                joinRoomStyle = "display:none";
                sendMessageStyle = "display:block";
                StateHasChanged();
            }
            catch (Exception ex)
            {
                messages.Add($"Error joining room: {ex.Message}");
            }
        }
    }

    private async Task SendMessage()
    {
        if (hubConnection is not null && !string.IsNullOrEmpty(messageInput))
        {
            try
            {
                await hubConnection.SendAsync("SendMessage", GameId.ToString(), 
                    currentUser.Claims.First(b => b.Type == ClaimTypes.NameIdentifier).Value, messageInput);
                messageInput = string.Empty; // Clear input after sending
            }
            catch (Exception ex)
            {
                messages.Add($"Error sending message: {ex.Message}");
            }
        }
    }

    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            // Leave group before disposing
            if (GameId is not null && currentUser is not null)
            {
                try
                {
                    await hubConnection.SendAsync("RemoveFromGroup", GameId.ToString(), currentUser.Claims.Single(b => b.Type == ClaimTypes.NameIdentifier).Value);
                }
                catch
                {
                    // Ignore errors during disposal
                }
            }
            await hubConnection.DisposeAsync();
        }
    }
}
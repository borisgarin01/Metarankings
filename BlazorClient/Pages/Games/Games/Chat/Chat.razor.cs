using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorClient.Pages.Games.Games.Chat;

public partial class Chat : ComponentBase, IAsyncDisposable
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private HubConnection? hubConnection;
    private List<string> messages = [];
    private string? chatInput;
    private string? userInput;
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

    private async Task JoinRoom()
    {
        if (hubConnection is not null && !string.IsNullOrEmpty(chatInput) && !string.IsNullOrEmpty(userInput))
        {
            try
            {
                await hubConnection.SendAsync("AddToGroup", chatInput, userInput);
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
                await hubConnection.SendAsync("SendMessage", chatInput, userInput, messageInput);
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
            if (!string.IsNullOrEmpty(chatInput) && !string.IsNullOrEmpty(userInput))
            {
                try
                {
                    await hubConnection.SendAsync("RemoveFromGroup", chatInput, userInput);
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
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Serialization;

namespace BlazorClient.PagesModels.Games.Chat
{
    public sealed class ChatComponentModel : ComponentBase, IAsyncDisposable
    {
        private HubConnection? hubConnection;
        private List<string> messages = [];
        private string? messageInput;
        private string connectionStatus = "Disconnected";
        private ClaimsPrincipal currentUser;

        private string joinRoomStyle = "display:block";
        private string sendMessageStyle = "display:none";

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Parameter, EditorRequired]
        public long? GameId { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MaxLength(255, ErrorMessage = "Max length is 255")]
        [MinLength(1, ErrorMessage = "Message should be not empty")]
        [JsonPropertyName("message")]
        public string MessageInput { get; set; }

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



        protected override async Task OnParametersSetAsync()
        {
            if (AuthenticationState is not null)
            {
                AuthenticationState authState = await AuthenticationState;
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

        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;
    }
}

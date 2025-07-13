namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class SystemRequirements : ComponentBase
{
    private bool showRequirements = false;

    private void ToggleRequirements()
    {
        showRequirements = !showRequirements;
    }
}

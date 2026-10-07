using Domain.Games;
using Domain.RequestsModels.Games.Localizations;

namespace BlazorClient.Pages.Admin.Games.Localizations;

public partial class RemoveLocalizationPage : RemoveEntityPageBase<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    protected override string ListUrl => "/admin/Games/localizations/list-localizations";
}

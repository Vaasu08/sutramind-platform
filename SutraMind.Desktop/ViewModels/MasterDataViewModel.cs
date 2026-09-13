using System.Collections.ObjectModel;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.ViewModels;

public sealed class MasterDataViewModel : SectionViewModelBase
{
    private readonly IMasterDataReadService _masterDataService;

    public MasterDataViewModel(IMasterDataReadService masterDataService) => _masterDataService = masterDataService;

    public override WorkspaceSection Section => WorkspaceSection.MasterData;

    public ObservableCollection<MasterTermListItem> Terms { get; } = [];

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _masterDataService.ListActiveAsync(category: null, cancellationToken);
        Terms.Clear();
        foreach (var row in rows)
            Terms.Add(row);
    }
}

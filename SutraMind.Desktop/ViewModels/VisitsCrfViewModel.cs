using System.Collections.ObjectModel;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.ViewModels;

public sealed class VisitsCrfViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IVisitCrfReadService _visitService;

    public VisitsCrfViewModel(ClinicalSession session, IVisitCrfReadService visitService)
    {
        _session = session;
        _visitService = visitService;
    }

    public override WorkspaceSection Section => WorkspaceSection.VisitsAndCrf;

    public ObservableCollection<VisitListItem> Visits { get; } = [];

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _visitService.ListByStudyAsync(_session.ActiveStudyId, cancellationToken);
        Visits.Clear();
        foreach (var row in rows)
            Visits.Add(row);
    }
}

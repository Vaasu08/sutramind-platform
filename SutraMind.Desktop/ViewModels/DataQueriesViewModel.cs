using System.Collections.ObjectModel;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.ViewModels;

public sealed class DataQueriesViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IQueryReadService _queryService;
    private bool openOnly;

    public DataQueriesViewModel(ClinicalSession session, IQueryReadService queryService)
    {
        _session = session;
        _queryService = queryService;
    }

    public override WorkspaceSection Section => WorkspaceSection.DataQueries;

    public bool OpenOnly
    {
        get => openOnly;
        set
        {
            if (!SetProperty(ref openOnly, value))
                return;
            _ = LoadAsync();
        }
    }

    public ObservableCollection<QueryListItem> Queries { get; } = [];

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _queryService.ListByStudyAsync(_session.ActiveStudyId, OpenOnly, cancellationToken);
        Queries.Clear();
        foreach (var row in rows)
            Queries.Add(row);
    }
}

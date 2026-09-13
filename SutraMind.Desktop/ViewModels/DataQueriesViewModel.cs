using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Desktop.Windows;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.ViewModels;

public sealed class DataQueriesViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IQueryReadService _queryService;
    private readonly IShellNavigation _navigation;
    private bool openOnly;
    private QueryListItem? selectedQuery;

    public DataQueriesViewModel(ClinicalSession session, IQueryReadService queryService, IShellNavigation navigation)
    {
        _session = session;
        _queryService = queryService;
        _navigation = navigation;
        RaiseQueryCommand = new PermissionCommand(() => session.Role, Permission.RaiseQuery, () => _navigation.StartQueryAction(QueryActionMode.Raise));
        AnswerQueryCommand = new PermissionCommand(() => session.Role, Permission.AnswerQuery, () => AnswerSelected(), () => SelectedQuery is not null);
        CloseQueryCommand = new PermissionCommand(() => session.Role, Permission.CloseQuery, () => CloseSelected(), () => SelectedQuery is not null);
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

    public QueryListItem? SelectedQuery
    {
        get => selectedQuery;
        set => SetProperty(ref selectedQuery, value);
    }

    public ObservableCollection<QueryListItem> Queries { get; } = [];

    public ICommand RaiseQueryCommand { get; }
    public ICommand AnswerQueryCommand { get; }
    public ICommand CloseQueryCommand { get; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _queryService.ListByStudyAsync(_session.ActiveStudyId, OpenOnly, cancellationToken);
        Queries.Clear();
        foreach (var row in rows)
            Queries.Add(row);
    }

    private void AnswerSelected()
    {
        if (SelectedQuery is null) return;
        _navigation.StartQueryAction(QueryActionMode.Answer, SelectedQuery, SelectedQuery.Id);
    }

    private void CloseSelected()
    {
        if (SelectedQuery is null) return;
        _navigation.StartQueryAction(QueryActionMode.Close, SelectedQuery, SelectedQuery.Id);
    }
}

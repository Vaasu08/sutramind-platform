using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SutraMind.Desktop.ViewModels;

public sealed class MasterDataViewModel : SectionViewModelBase
{
    private readonly IMasterDataReadService _masterDataService;
    private readonly IShellNavigation _navigation;
    private readonly ClinicalSession _session;
    private readonly LocalDbContext _context;
    private MasterTermListItem? selectedTerm;

    public MasterDataViewModel(IMasterDataReadService masterDataService, IShellNavigation navigation, ClinicalSession session, LocalDbContext context)
    {
        _masterDataService = masterDataService;
        _navigation = navigation;
        _session = session;
        _context = context;
        AddTermCommand = new PermissionCommand(() => session.Role, Permission.ManageMasterTerms, () => _navigation.StartAddMasterTerm());
        EditTermCommand = new PermissionCommand(() => session.Role, Permission.ManageMasterTerms, () => OpenEditTerm(), () => SelectedTerm is not null);
    }

    public override WorkspaceSection Section => WorkspaceSection.MasterData;

    public ObservableCollection<MasterTermListItem> Terms { get; } = [];

    public MasterTermListItem? SelectedTerm
    {
        get => selectedTerm;
        set => SetProperty(ref selectedTerm, value);
    }

    public ICommand AddTermCommand { get; }
    public ICommand EditTermCommand { get; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _masterDataService.ListActiveAsync(category: null, cancellationToken);
        Terms.Clear();
        foreach (var row in rows)
            Terms.Add(row);
    }

    private async void OpenEditTerm()
    {
        if (SelectedTerm is null) return;
        var term = await _context.MasterTerms.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == SelectedTerm.Code && t.Category == SelectedTerm.Category);
        if (term is null) return;
        _navigation.StartEditMasterTerm(SelectedTerm, term.Id);
    }
}

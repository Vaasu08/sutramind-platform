using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.ViewModels;

public sealed class StudiesViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IStudyWorkspaceService _studyService;
    private readonly IShellNavigation _navigation;
    private StudyWorkspaceData? workspace;

    public StudiesViewModel(ClinicalSession session, IStudyWorkspaceService studyService, IShellNavigation navigation)
    {
        _session = session;
        _studyService = studyService;
        _navigation = navigation;
        CreateStudyCommand = new PermissionCommand(() => session.Role, Permission.CreateStudy, () => _navigation.StartCreateStudy());
    }

    public override WorkspaceSection Section => WorkspaceSection.Studies;

    public ObservableCollection<StudyListItem> StudyOptions { get; } = [];

    public StudyWorkspaceData? Workspace
    {
        get => workspace;
        private set => SetProperty(ref workspace, value);
    }

    public ICommand CreateStudyCommand { get; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var studies = await _studyService.ListStudiesAsync(cancellationToken);
        StudyOptions.Clear();
        foreach (var study in studies)
            StudyOptions.Add(study);

        Workspace = await _studyService.GetStudyWorkspaceAsync(_session.ActiveStudyId, cancellationToken);
    }
}

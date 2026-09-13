using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.ViewModels;

public sealed class ParticipantsViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IParticipantReadService _participantReadService;
    private readonly IShellNavigation _navigation;
    private string searchText = string.Empty;

    public ParticipantsViewModel(ClinicalSession session, IParticipantReadService participantReadService, IShellNavigation navigation)
    {
        _session = session;
        _participantReadService = participantReadService;
        _navigation = navigation;
        EnrollCommand = new PermissionCommand(() => session.Role, Permission.CreateParticipant, navigation.StartEnrollment);
        SearchCommand = new RelayCommand(async () => await LoadAsync());
    }

    public override WorkspaceSection Section => WorkspaceSection.Participants;

    public ObservableCollection<ParticipantListItem> Participants { get; } = [];

    public string SearchText
    {
        get => searchText;
        set => SetProperty(ref searchText, value);
    }

    public ICommand EnrollCommand { get; }
    public ICommand SearchCommand { get; }

    public string EnrollTooltip => PermissionPolicy.Allows(_session.Role, Permission.CreateParticipant)
        ? "Enroll a new participant in the active study."
        : $"Your role ({ClinicalRoleMapper.ToDisplayName(_session.Role)}) cannot enroll participants.";

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _participantReadService.ListByStudyAsync(_session.ActiveStudyId, SearchText, cancellationToken);
        Participants.Clear();
        foreach (var row in rows)
            Participants.Add(row);
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Desktop.Windows;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SutraMind.Desktop.ViewModels;

public sealed class VisitsCrfViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IVisitCrfReadService _visitService;
    private readonly IShellNavigation _navigation;
    private readonly LocalDbContext _context;
    private VisitListItem? selectedVisit;

    public VisitsCrfViewModel(ClinicalSession session, IVisitCrfReadService visitService, IShellNavigation navigation, LocalDbContext context)
    {
        _session = session;
        _visitService = visitService;
        _navigation = navigation;
        _context = context;
        RecordCrfCommand = new PermissionCommand(() => session.Role, Permission.RecordCrf, () => OpenRecordCrf(), () => SelectedVisit is not null);
        CompleteVisitCommand = new PermissionCommand(() => session.Role, Permission.ScheduleVisit, () => OpenCompleteVisit(), () => SelectedVisit is not null);
        ScheduleVisitCommand = new PermissionCommand(() => session.Role, Permission.ScheduleVisit, () => navigation.StartScheduleVisit());
    }

    public override WorkspaceSection Section => WorkspaceSection.VisitsAndCrf;

    public ObservableCollection<VisitListItem> Visits { get; } = [];

    public VisitListItem? SelectedVisit
    {
        get => selectedVisit;
        set => SetProperty(ref selectedVisit, value);
    }

    public ICommand RecordCrfCommand { get; }
    public ICommand CompleteVisitCommand { get; }
    public ICommand ScheduleVisitCommand { get; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _visitService.ListByStudyAsync(_session.ActiveStudyId, cancellationToken);
        Visits.Clear();
        foreach (var row in rows)
            Visits.Add(row);
    }

    private async void OpenRecordCrf()
    {
        if (SelectedVisit is null) return;
        var visit = await _context.Visits.AsNoTracking()
            .Join(_context.Participants.AsNoTracking(), v => v.ParticipantId, p => p.Id, (v, p) => new { v, p })
            .FirstOrDefaultAsync(x => x.p.ParticipantCode == SelectedVisit.ParticipantCode && x.v.VisitNumber == SelectedVisit.VisitNumber);
        if (visit is null) return;
        _navigation.StartRecordCrf(visit.v.Id, $"{SelectedVisit.ParticipantCode} — {SelectedVisit.VisitLabel}");
    }

    private async void OpenCompleteVisit()
    {
        if (SelectedVisit is null) return;
        var visit = await _context.Visits.AsNoTracking()
            .Join(_context.Participants.AsNoTracking(), v => v.ParticipantId, p => p.Id, (v, p) => new { v, p })
            .FirstOrDefaultAsync(x => x.p.ParticipantCode == SelectedVisit.ParticipantCode && x.v.VisitNumber == SelectedVisit.VisitNumber);
        if (visit is null) return;
        _navigation.StartCompleteVisit(visit.v.Id, $"{SelectedVisit.ParticipantCode} — {SelectedVisit.VisitLabel}");
    }
}

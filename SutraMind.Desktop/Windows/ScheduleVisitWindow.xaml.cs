using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Services;
using SutraMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SutraMind.Desktop.Windows;

public partial class ScheduleVisitWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IParticipantReadService _participantReadService;
    private readonly IVisitScheduleService _visitScheduleService;
    private readonly LocalDbContext _context;
    private List<(Guid Id, string Code)> _participants = [];

    public ScheduleVisitWindow(
        ClinicalSession session,
        IParticipantReadService participantReadService,
        IVisitScheduleService visitScheduleService,
        LocalDbContext context)
    {
        _session = session;
        _participantReadService = participantReadService;
        _visitScheduleService = visitScheduleService;
        _context = context;
        InitializeComponent();
        DateTextBox.Text = DateOnly.FromDateTime(DateTime.Today.AddDays(7)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var rows = await _participantReadService.ListByStudyAsync(_session.ActiveStudyId, search: null);
        var raw = await _context.Participants.AsNoTracking()
            .Where(item => item.StudyId == _session.ActiveStudyId)
            .OrderBy(item => item.ParticipantCode)
            .Select(item => new { item.Id, item.ParticipantCode })
            .ToListAsync();
        _participants = raw.Select(item => (item.Id, Code: item.ParticipantCode)).ToList();

        ParticipantCombo.ItemsSource = rows;
        if (rows.Count > 0)
            ParticipantCombo.SelectedIndex = 0;
    }

    private async void Schedule_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        if (ParticipantCombo.SelectedItem is not ParticipantListItem selected)
        {
            ErrorText.Text = "Select a participant.";
            return;
        }

        if (!int.TryParse(VisitNumberTextBox.Text.Trim(), out var visitNumber))
        {
            ErrorText.Text = "Enter a valid visit number.";
            return;
        }

        if (!DateOnly.TryParseExact(DateTextBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var scheduledDate))
        {
            ErrorText.Text = "Enter the date as yyyy-MM-dd.";
            return;
        }

        var match = _participants.FirstOrDefault(item => item.Code == selected.Code);
        var participantId = match.Code == selected.Code ? match.Id : Guid.Empty;
        if (participantId == Guid.Empty)
        {
            ErrorText.Text = "Participant record could not be resolved.";
            return;
        }

        try
        {
            await _visitScheduleService.ScheduleAsync(
                new ScheduleVisitRequest(participantId, visitNumber, scheduledDate, _session.ActorUserId),
                _session.ActorUserId);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }

    private void FillDummyData_Click(object sender, RoutedEventArgs e)
    {
        if (ParticipantCombo.Items.Count > 0) ParticipantCombo.SelectedIndex = 0;
        VisitNumberTextBox.Text = "1";
        DateTextBox.Text = DateOnly.FromDateTime(DateTime.Today.AddDays(30)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}

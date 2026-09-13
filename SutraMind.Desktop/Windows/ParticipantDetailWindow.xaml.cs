using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Services;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SutraMind.Desktop.Windows;

public partial class ParticipantDetailWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IParticipantWriteService _participantWriteService;
    private readonly LocalDbContext _context;
    private readonly string _participantCode;
    private Guid _participantId;

    public ParticipantDetailWindow(
        ClinicalSession session,
        IParticipantWriteService participantWriteService,
        LocalDbContext context,
        string participantCode)
    {
        _session = session;
        _participantWriteService = participantWriteService;
        _context = context;
        _participantCode = participantCode;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var participant = await _context.Participants.AsNoTracking()
            .FirstOrDefaultAsync(p => p.ParticipantCode == _participantCode && p.StudyId == _session.ActiveStudyId);

        if (participant is null)
        {
            ErrorText.Text = "Participant not found.";
            return;
        }

        _participantId = participant.Id;
        CodeText.Text = participant.ParticipantCode;
        DemographicText.Text = $"Age: {participant.Age}  |  Gender: {participant.Gender}";
        DiagnosisText.Text = $"Diagnosis: {participant.ModernDiagnosis} ({participant.VyadhiCode})";
        RandomizationText.Text = $"Randomization: {participant.RandomizationId ?? "—"}  |  Enrolled: {participant.EnrollmentDate:d}";
        StatusText.Text = $"Status: {participant.Status}";
        StatusText.Foreground = participant.Status == ParticipantStatus.Withdrawn
            ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xA4, 0x4F, 0x3E))
            : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x28, 0x70, 0x4B));

        var baseline = await _context.AyurvedaBaselines.AsNoTracking()
            .FirstOrDefaultAsync(b => b.ParticipantId == participant.Id);

        var terms = await _context.MasterTerms.AsNoTracking().ToListAsync();
        string Label(string? code) => code is null ? "—" : terms.FirstOrDefault(t => t.Code == code)?.LabelEn ?? code;

        PrakritiText.Text = $"Prakriti: {Label(baseline?.PrakritiCode)}";
        AgniText.Text = $"Agni: {Label(baseline?.AgniCode)}";
        BalaText.Text = $"Bala: {Label(baseline?.BalaCode)}";
        SatvaText.Text = $"Satva: {Label(baseline?.SatvaCode)}";

        var canTransition = participant.Status == ParticipantStatus.Enrolled;
        WithdrawButton.IsEnabled = canTransition;
        CompleteButton.IsEnabled = canTransition;
    }

    private async void Withdraw_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        var result = MessageBox.Show("Are you sure you want to withdraw this participant?", "Confirm withdrawal",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _participantWriteService.WithdrawAsync(_participantId, _session.ActorUserId);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }

    private async void Complete_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        try
        {
            await _participantWriteService.CompleteAsync(_participantId, _session.ActorUserId);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }
}

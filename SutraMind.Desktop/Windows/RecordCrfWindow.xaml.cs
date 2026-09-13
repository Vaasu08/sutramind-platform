using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Services;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.Windows;

public partial class RecordCrfWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly ICrfService _crfService;
    private readonly IMasterDataReadService _masterDataService;
    private readonly Guid _visitId;
    private readonly string _visitLabel;

    public RecordCrfWindow(
        ClinicalSession session,
        ICrfService crfService,
        IMasterDataReadService masterDataService,
        Guid visitId,
        string visitLabel)
    {
        _session = session;
        _crfService = crfService;
        _masterDataService = masterDataService;
        _visitId = visitId;
        _visitLabel = visitLabel;
        InitializeComponent();
        VisitHeader.Text = visitLabel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var terms = await _masterDataService.ListActiveAsync(category: null);
        AgniCombo.ItemsSource = terms.Where(t => t.Category == "Agni").ToList();
        BalaCombo.ItemsSource = terms.Where(t => t.Category == "Bala").ToList();
        AgniCombo.SelectedIndex = 0;
        BalaCombo.SelectedIndex = 0;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;

        var status = StatusCombo.SelectedIndex == 1 ? CrfStatus.Completed : CrfStatus.Draft;

        try
        {
            var request = new SaveCrfRequest(
                _visitId,
                DateOnly.FromDateTime(DateTime.Today),
                _session.ActorUserId,
                ParseInt(SystolicBox.Text),
                ParseInt(DiastolicBox.Text),
                ParseInt(PulseBox.Text),
                ParseDecimal(WeightBox.Text),
                ParseDecimal(TempBox.Text),
                AgniCombo.SelectedValue?.ToString(),
                BalaCombo.SelectedValue?.ToString(),
                NullIfEmpty(SymptomsBox.Text),
                null,
                NullIfEmpty(DoseBox.Text),
                NullIfEmpty(FrequencyBox.Text),
                NullIfEmpty(ComplianceBox.Text),
                NullIfEmpty(RemarksBox.Text),
                status);

            await _crfService.SaveAsync(request, _session.ActorUserId);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }

    private static int? ParseInt(string text) =>
        int.TryParse(text.Trim(), out var v) ? v : null;

    private static decimal? ParseDecimal(string text) =>
        decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : null;

    private static string? NullIfEmpty(string text) =>
        string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}

using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.Windows;

public partial class CompleteVisitWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IVisitWriteService _visitWriteService;
    private readonly Guid _visitId;

    public CompleteVisitWindow(
        ClinicalSession session,
        IVisitWriteService visitWriteService,
        Guid visitId,
        string visitLabel)
    {
        _session = session;
        _visitWriteService = visitWriteService;
        _visitId = visitId;
        InitializeComponent();
        VisitHeader.Text = visitLabel;
        DateTextBox.Text = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        var isMissed = OutcomeCombo.SelectedIndex == 1;

        try
        {
            if (isMissed)
            {
                await _visitWriteService.MarkMissedAsync(_visitId, _session.ActorUserId);
            }
            else
            {
                if (!DateOnly.TryParseExact(DateTextBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var visitDate))
                {
                    ErrorText.Text = "Enter the date as yyyy-MM-dd.";
                    return;
                }
                await _visitWriteService.CompleteAsync(_visitId, visitDate, _session.ActorUserId);
            }
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }

    private void FillDummyData_Click(object sender, RoutedEventArgs e)
    {
        OutcomeCombo.SelectedIndex = 0; // Completed
        DateTextBox.Text = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}

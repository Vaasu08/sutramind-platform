using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Desktop.Services;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.Windows;

public partial class UpdateEthicsWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IEthicsWriteService _ethicsWriteService;
    private readonly Guid _ethicsId;

    public UpdateEthicsWindow(
        ClinicalSession session,
        IEthicsWriteService ethicsWriteService,
        Guid ethicsId,
        string iecNumber,
        string currentStatus)
    {
        _session = session;
        _ethicsWriteService = ethicsWriteService;
        _ethicsId = ethicsId;
        InitializeComponent();
        IecLabel.Text = iecNumber;
        CurrentStatusLabel.Text = $"Current status: {currentStatus}";
    }

    private async void Update_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;

        if (StatusCombo.SelectedItem is not System.Windows.Controls.ComboBoxItem selectedItem)
        {
            ErrorText.Text = "Select a status.";
            return;
        }

        var statusText = selectedItem.Content?.ToString() ?? "Pending";
        if (!Enum.TryParse<EthicsStatus>(statusText, out var newStatus))
        {
            ErrorText.Text = "Invalid status.";
            return;
        }

        DateOnly? approvalDate = null;
        if (!string.IsNullOrWhiteSpace(ApprovalDateBox.Text))
        {
            if (!DateOnly.TryParseExact(ApprovalDateBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ad))
            {
                ErrorText.Text = "Enter approval date as yyyy-MM-dd.";
                return;
            }
            approvalDate = ad;
        }

        DateOnly? expiryDate = null;
        if (!string.IsNullOrWhiteSpace(ExpiryDateBox.Text))
        {
            if (!DateOnly.TryParseExact(ExpiryDateBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ed))
            {
                ErrorText.Text = "Enter expiry date as yyyy-MM-dd.";
                return;
            }
            expiryDate = ed;
        }

        try
        {
            await _ethicsWriteService.UpdateStatusAsync(
                _ethicsId,
                newStatus,
                approvalDate,
                expiryDate,
                string.IsNullOrWhiteSpace(RemarksBox.Text) ? null : RemarksBox.Text.Trim(),
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
        StatusCombo.SelectedIndex = 2; // Approved
        ApprovalDateBox.Text = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        ExpiryDateBox.Text = DateOnly.FromDateTime(DateTime.Today.AddYears(1)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        RemarksBox.Text = "Approved with minor remarks.";
    }
}

using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Desktop.Services;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.Windows;

public partial class CreateStudyWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IStudyWriteService _studyWriteService;

    public CreateStudyWindow(ClinicalSession session, IStudyWriteService studyWriteService)
    {
        _session = session;
        _studyWriteService = studyWriteService;
        InitializeComponent();
        StartDateBox.Text = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        EndDateBox.Text = DateOnly.FromDateTime(DateTime.Today.AddYears(1)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(StudyCodeBox.Text) || string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            ErrorText.Text = "Study code and title are required.";
            return;
        }

        if (!int.TryParse(SampleSizeBox.Text.Trim(), out var sampleSize) || sampleSize <= 0)
        {
            ErrorText.Text = "Enter a valid sample size.";
            return;
        }

        if (!DateOnly.TryParseExact(StartDateBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) ||
            !DateOnly.TryParseExact(EndDateBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
        {
            ErrorText.Text = "Enter dates as yyyy-MM-dd.";
            return;
        }

        if (!int.TryParse(DurationBox.Text.Trim(), out var duration) || duration <= 0)
        {
            ErrorText.Text = "Enter a valid treatment duration.";
            return;
        }

        try
        {
            var request = new CreateStudyRequest(
                StudyCodeBox.Text.Trim(),
                TitleBox.Text.Trim(),
                InstitutionBox.Text.Trim(),
                sampleSize,
                startDate,
                endDate,
                new ProtocolInput(
                    "1.0",
                    DiagnosisBox.Text.Trim(),
                    VyadhiCodeBox.Text.Trim(),
                    InterventionBox.Text.Trim(),
                    null,
                    DosageFormBox.Text.Trim(),
                    AnupanaBox.Text.Trim(),
                    duration),
                new EthicsInput(
                    IecNumberBox.Text.Trim(),
                    EthicsStatus.Pending));

            await _studyWriteService.CreateAsync(request, _session.ActorUserId);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }
}

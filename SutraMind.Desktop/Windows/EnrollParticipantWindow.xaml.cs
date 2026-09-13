using System.Globalization;
using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Application.ReadModels;
using SutraMind.Application.Services;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.Windows;

public partial class EnrollParticipantWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly ParticipantService _participantService;
    private readonly IMasterDataReadService _masterDataReadService;

    public EnrollParticipantWindow(
        ClinicalSession session,
        ParticipantService participantService,
        IMasterDataReadService masterDataReadService)
    {
        _session = session;
        _participantService = participantService;
        _masterDataReadService = masterDataReadService;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var terms = await _masterDataReadService.ListActiveAsync(category: null);
        PrakritiCombo.ItemsSource = Filter(terms, "Prakriti");
        AgniCombo.ItemsSource = Filter(terms, "Agni");
        BalaCombo.ItemsSource = Filter(terms, "Bala");
        SatvaCombo.ItemsSource = Filter(terms, "Satva");
        PrakritiCombo.SelectedIndex = 0;
        AgniCombo.SelectedIndex = 0;
        BalaCombo.SelectedIndex = 0;
        if (SatvaCombo.Items.Count > 0) SatvaCombo.SelectedIndex = 0;
    }

    private static List<MasterTermListItem> Filter(IReadOnlyList<MasterTermListItem> terms, string category) =>
        terms.Where(item => item.Category == category).ToList();

    private async void Enroll_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        if (!int.TryParse(AgeTextBox.Text.Trim(), out var age) || age <= 0)
        {
            ErrorText.Text = "Enter a valid age.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CodeTextBox.Text) || string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            ErrorText.Text = "Participant code and name are required.";
            return;
        }

        try
        {
            var request = new EnrollParticipantRequest(
                CodeTextBox.Text.Trim(),
                _session.ActiveStudyId,
                _session.ActiveSiteId,
                NameTextBox.Text.Trim(),
                age,
                GenderTextBox.Text.Trim(),
                "Rheumatoid Arthritis",
                "V001",
                null,
                string.IsNullOrWhiteSpace(RandomizationTextBox.Text) ? null : RandomizationTextBox.Text.Trim(),
                DateOnly.FromDateTime(DateTime.UtcNow),
                new AyurvedaBaselineInput(
                    PrakritiCombo.SelectedValue?.ToString() ?? "P6",
                    null,
                    AgniCombo.SelectedValue?.ToString() ?? "A2",
                    BalaCombo.SelectedValue?.ToString() ?? "B2",
                    SatvaCombo.SelectedValue?.ToString() ?? "S2"),
                _session.ActorUserId);

            await _participantService.EnrollAsync(request);
            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
    }
}

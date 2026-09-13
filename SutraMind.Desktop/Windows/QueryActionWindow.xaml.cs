using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Services;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.Windows;

public enum QueryActionMode { Raise, Answer, Close }

public partial class QueryActionWindow : Window
{
    private readonly ClinicalSession _session;
    private readonly IQueryWriteService _queryWriteService;
    private readonly IParticipantReadService _participantReadService;
    private readonly QueryActionMode _mode;
    private readonly QueryListItem? _queryItem;
    private readonly Guid? _queryEntityId;

    public QueryActionWindow(
        ClinicalSession session,
        IQueryWriteService queryWriteService,
        IParticipantReadService participantReadService,
        QueryActionMode mode,
        QueryListItem? queryItem = null,
        Guid? queryEntityId = null)
    {
        _session = session;
        _queryWriteService = queryWriteService;
        _participantReadService = participantReadService;
        _mode = mode;
        _queryItem = queryItem;
        _queryEntityId = queryEntityId;
        InitializeComponent();
        ConfigureMode();
        Loaded += OnLoaded;
    }

    private void ConfigureMode()
    {
        switch (_mode)
        {
            case QueryActionMode.Raise:
                TitleText.Text = "Raise a data query";
                InputLabel.Text = "Message";
                SubmitButton.Content = "Raise query";
                RaisePanel.Visibility = Visibility.Visible;
                ContextPanel.Visibility = Visibility.Collapsed;
                break;

            case QueryActionMode.Answer:
                TitleText.Text = "Answer query";
                InputLabel.Text = "Answer";
                SubmitButton.Content = "Submit answer";
                RaisePanel.Visibility = Visibility.Collapsed;
                ContextPanel.Visibility = Visibility.Visible;
                if (_queryItem is not null)
                {
                    OriginalSubject.Text = $"Subject: {_queryItem.SubjectCode}";
                    OriginalField.Text = $"Field: {_queryItem.FieldName}";
                    OriginalMessage.Text = _queryItem.Message;
                }
                break;

            case QueryActionMode.Close:
                TitleText.Text = "Close query";
                InputLabel.Text = "Resolution reason";
                SubmitButton.Content = "Close query";
                RaisePanel.Visibility = Visibility.Collapsed;
                ContextPanel.Visibility = Visibility.Visible;
                if (_queryItem is not null)
                {
                    OriginalSubject.Text = $"Subject: {_queryItem.SubjectCode}";
                    OriginalField.Text = $"Field: {_queryItem.FieldName}";
                    OriginalMessage.Text = _queryItem.Message;
                    OriginalAnswer.Text = $"Answer: (see record)";
                }
                break;
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_mode != QueryActionMode.Raise)
            return;

        var participants = await _participantReadService.ListByStudyAsync(_session.ActiveStudyId, search: null);
        ParticipantCombo.ItemsSource = participants;
        if (participants.Count > 0)
            ParticipantCombo.SelectedIndex = 0;
    }

    private async void Submit_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        var text = InputBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            ErrorText.Text = "Please enter the required text.";
            return;
        }

        try
        {
            switch (_mode)
            {
                case QueryActionMode.Raise:
                {
                    if (ParticipantCombo.SelectedItem is not ParticipantListItem selected)
                    {
                        ErrorText.Text = "Select a participant.";
                        return;
                    }
                    var request = new RaiseQueryRequest(
                        _session.ActiveStudyId,
                        QueryTargetType.Participant,
                        selected.Id,
                        string.IsNullOrWhiteSpace(FieldNameBox.Text) ? null : FieldNameBox.Text.Trim(),
                        text);
                    await _queryWriteService.RaiseAsync(request, _session.ActorUserId);
                    break;
                }
                case QueryActionMode.Answer:
                {
                    if (_queryEntityId is null) return;
                    await _queryWriteService.AnswerAsync(_queryEntityId.Value, new AnswerQueryRequest(text), _session.ActorUserId);
                    break;
                }
                case QueryActionMode.Close:
                {
                    if (_queryEntityId is null) return;
                    await _queryWriteService.CloseAsync(_queryEntityId.Value, new CloseQueryRequest(text), _session.ActorUserId);
                    break;
                }
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
        if (_mode == QueryActionMode.Raise)
        {
            if (ParticipantCombo.Items.Count > 0) ParticipantCombo.SelectedIndex = 0;
            FieldNameBox.Text = "Systolic BP";
            InputBox.Text = "Value seems abnormally high. Please confirm.";
        }
        else if (_mode == QueryActionMode.Answer)
        {
            InputBox.Text = "Data entry error, correct value is 120.";
        }
        else if (_mode == QueryActionMode.Close)
        {
            InputBox.Text = "Resolved, corrected in source document.";
        }
    }
}

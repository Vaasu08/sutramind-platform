using System.Windows;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.Windows;

public partial class EditMasterTermWindow : Window
{
    private readonly IMasterDataWriteService _masterDataWriteService;
    private readonly Guid? _editTermId;

    public EditMasterTermWindow(IMasterDataWriteService masterDataWriteService, MasterTermListItem? existing = null, Guid? editTermId = null)
    {
        _masterDataWriteService = masterDataWriteService;
        _editTermId = editTermId;
        InitializeComponent();

        if (existing is not null && editTermId.HasValue)
        {
            TitleText.Text = "Edit Master Term";
            SaveButton.Content = "Save changes";
            CategoryBox.Text = existing.Category;
            CategoryBox.IsReadOnly = true;
            CodeBox.Text = existing.Code;
            CodeBox.IsReadOnly = true;
            LabelEnBox.Text = existing.LabelEn;
            LabelHiBox.Text = existing.LabelHi;
            ModernMappingBox.Text = existing.ModernMappingEn ?? string.Empty;
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(LabelEnBox.Text) || string.IsNullOrWhiteSpace(LabelHiBox.Text))
        {
            ErrorText.Text = "English and Hindi labels are required.";
            return;
        }

        if (!int.TryParse(SortOrderBox.Text.Trim(), out var sortOrder))
        {
            ErrorText.Text = "Enter a valid sort order number.";
            return;
        }

        try
        {
            if (_editTermId.HasValue)
            {
                await _masterDataWriteService.UpdateAsync(
                    _editTermId.Value,
                    LabelEnBox.Text.Trim(),
                    LabelHiBox.Text.Trim(),
                    string.IsNullOrWhiteSpace(ModernMappingBox.Text) ? null : ModernMappingBox.Text.Trim(),
                    sortOrder,
                    ActiveCheck.IsChecked == true);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(CategoryBox.Text) || string.IsNullOrWhiteSpace(CodeBox.Text))
                {
                    ErrorText.Text = "Category and code are required.";
                    return;
                }

                await _masterDataWriteService.AddAsync(
                    CategoryBox.Text.Trim(),
                    CodeBox.Text.Trim(),
                    LabelEnBox.Text.Trim(),
                    LabelHiBox.Text.Trim(),
                    string.IsNullOrWhiteSpace(ModernMappingBox.Text) ? null : ModernMappingBox.Text.Trim(),
                    sortOrder);
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
        if (!_editTermId.HasValue)
        {
            CategoryBox.Text = "ANUPANA";
            CodeBox.Text = "KSHEERA";
        }
        LabelEnBox.Text = "Milk";
        LabelHiBox.Text = "दूध";
        ModernMappingBox.Text = "Milk vehicle";
        SortOrderBox.Text = "10";
    }
}

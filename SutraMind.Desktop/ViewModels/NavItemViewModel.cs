using System.Windows.Input;
using SutraMind.Desktop.Navigation;

namespace SutraMind.Desktop.ViewModels;

public sealed class NavItemViewModel : ViewModelBase
{
    private bool isSelected;

    public NavItemViewModel(WorkspaceSection section, string label, ICommand navigateCommand)
    {
        Section = section;
        Label = label;
        NavigateCommand = navigateCommand;
    }

    public WorkspaceSection Section { get; }
    public string Label { get; }
    public ICommand NavigateCommand { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }
}

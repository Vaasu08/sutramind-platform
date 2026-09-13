using SutraMind.Desktop.Navigation;

namespace SutraMind.Desktop.ViewModels;

public abstract class SectionViewModelBase : ViewModelBase
{
    public abstract WorkspaceSection Section { get; }
    public abstract Task LoadAsync(CancellationToken cancellationToken = default);
}

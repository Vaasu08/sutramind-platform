using SutraMind.Desktop.Navigation;

namespace SutraMind.Desktop.Services;

public interface IShellNavigation
{
    void NavigateTo(WorkspaceSection section);
    void StartEnrollment();
    void StartScheduleVisit();
    void OpenQueries(bool openOnly);
    Task SyncNowAsync();
    Task RefreshShellAsync();
}

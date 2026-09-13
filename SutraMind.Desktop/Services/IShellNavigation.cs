using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Windows;

namespace SutraMind.Desktop.Services;

public interface IShellNavigation
{
    void NavigateTo(WorkspaceSection section);
    void StartEnrollment();
    void StartScheduleVisit();
    void OpenQueries(bool openOnly);
    Task SyncNowAsync();
    Task RefreshShellAsync();
    void StartRecordCrf(Guid visitId, string visitLabel);
    void StartCompleteVisit(Guid visitId, string visitLabel);
    void StartQueryAction(QueryActionMode mode, QueryListItem? item = null, Guid? entityId = null);
    void StartCreateStudy();
    void StartParticipantDetail(string participantCode);
    void StartUpdateEthics(Guid ethicsId, string iecNumber, string currentStatus);
    void StartAddMasterTerm();
    void StartEditMasterTerm(MasterTermListItem existing, Guid termId);
}

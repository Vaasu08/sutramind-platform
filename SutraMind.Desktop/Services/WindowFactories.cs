using Microsoft.Extensions.DependencyInjection;
using SutraMind.Desktop.Windows;

namespace SutraMind.Desktop.Services;

public sealed class EnrollParticipantWindowFactory(IServiceProvider services)
{
    public EnrollParticipantWindow Create() => ActivatorUtilities.CreateInstance<EnrollParticipantWindow>(services);
}

public sealed class ScheduleVisitWindowFactory(IServiceProvider services)
{
    public ScheduleVisitWindow Create() => ActivatorUtilities.CreateInstance<ScheduleVisitWindow>(services);
}

public sealed class RecordCrfWindowFactory(IServiceProvider services)
{
    public RecordCrfWindow Create(Guid visitId, string visitLabel) => ActivatorUtilities.CreateInstance<RecordCrfWindow>(services, visitId, visitLabel);
}

public sealed class CompleteVisitWindowFactory(IServiceProvider services)
{
    public CompleteVisitWindow Create(Guid visitId, string visitLabel) => ActivatorUtilities.CreateInstance<CompleteVisitWindow>(services, visitId, visitLabel);
}

public sealed class QueryActionWindowFactory(IServiceProvider services)
{
    public QueryActionWindow Create(QueryActionMode mode, Application.ReadModels.QueryListItem? item = null, Guid? entityId = null) =>
        new QueryActionWindow(
            services.GetRequiredService<ClinicalSession>(),
            services.GetRequiredService<SutraMind.Application.Abstractions.IQueryWriteService>(),
            services.GetRequiredService<SutraMind.Application.Abstractions.IParticipantReadService>(),
            mode,
            item,
            entityId);
}

public sealed class CreateStudyWindowFactory(IServiceProvider services)
{
    public CreateStudyWindow Create() => ActivatorUtilities.CreateInstance<CreateStudyWindow>(services);
}

public sealed class ParticipantDetailWindowFactory(IServiceProvider services)
{
    public ParticipantDetailWindow Create(string participantCode) => ActivatorUtilities.CreateInstance<ParticipantDetailWindow>(services, participantCode);
}

public sealed class UpdateEthicsWindowFactory(IServiceProvider services)
{
    public UpdateEthicsWindow Create(Guid ethicsId, string iecNumber, string currentStatus) =>
        ActivatorUtilities.CreateInstance<UpdateEthicsWindow>(services, ethicsId, iecNumber, currentStatus);
}

public sealed class EditMasterTermWindowFactory(IServiceProvider services)
{
    public EditMasterTermWindow CreateAdd() => ActivatorUtilities.CreateInstance<EditMasterTermWindow>(services);
    public EditMasterTermWindow CreateEdit(Application.ReadModels.MasterTermListItem existing, Guid termId) =>
        ActivatorUtilities.CreateInstance<EditMasterTermWindow>(services, existing, termId);
}

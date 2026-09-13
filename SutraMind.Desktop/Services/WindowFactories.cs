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

using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Services;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using SutraMind.Infrastructure.Repositories;
using SutraMind.Infrastructure.Seed;
using SutraMind.Infrastructure.Services;
using SutraMind.Desktop.ViewModels;
using SutraMind.Desktop.Windows;

namespace SutraMind.Desktop.Services;

public static class DesktopBootstrap
{
    public static async Task<IServiceProvider> BuildAsync()
    {
        var databaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SutraMind");
        Directory.CreateDirectory(databaseDirectory);
        var databasePath = Path.Combine(databaseDirectory, "local.db");
        RemoveCorruptDatabaseFiles(databasePath);
        var encryptionKey = Environment.GetEnvironmentVariable("SUTRAMIND_DB_KEY") ?? "dev-local-demo-key-change-me";

        var context = LocalDatabase.CreateContext(databasePath, encryptionKey);
        await LocalDatabase.InitializeAsync(context);
        await LocalDemoSeed.EnsureSeededAsync(context);

        var snapshot = await new ClinicalContextService(context).GetDefaultContextAsync()
            ?? throw new InvalidOperationException("Clinical context is unavailable.");

        var services = new ServiceCollection();
        services.AddSingleton(context);
        services.AddSingleton(new ClinicalSession
        {
            Context = snapshot,
            Email = "coordinator@sutramind.local",
            Role = UserRole.StudyCoordinator
        });

        // Read services
        services.AddSingleton<IClinicalContextService, ClinicalContextService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IStudyWorkspaceService, StudyWorkspaceService>();
        services.AddSingleton<IParticipantReadService, ParticipantReadService>();
        services.AddSingleton<IVisitCrfReadService, VisitCrfReadService>();
        services.AddSingleton<IQueryReadService, QueryReadService>();
        services.AddSingleton<IEthicsReadService, EthicsReadService>();
        services.AddSingleton<IMasterDataReadService, MasterDataReadService>();
        services.AddSingleton<IOutboxService, OutboxService>();
        services.AddSingleton<IVisitScheduleService, VisitScheduleService>();

        // Write services
        services.AddSingleton<ICrfService, CrfService>();
        services.AddSingleton<IQueryWriteService, QueryWriteService>();
        services.AddSingleton<IVisitWriteService, VisitWriteService>();
        services.AddSingleton<IStudyWriteService, StudyWriteService>();
        services.AddSingleton<IParticipantWriteService, ParticipantWriteService>();
        services.AddSingleton<IEthicsWriteService, EthicsWriteService>();
        services.AddSingleton<IMasterDataWriteService, MasterDataWriteService>();

        // Repositories
        services.AddTransient<IParticipantRepository>(sp =>
        {
            var session = sp.GetRequiredService<ClinicalSession>();
            return new ParticipantRepository(sp.GetRequiredService<LocalDbContext>(), session.ActorUserId, session.DeviceId);
        });
        services.AddTransient<ParticipantService>();

        // Windows
        services.AddTransient<EnrollParticipantWindow>();
        services.AddTransient<ScheduleVisitWindow>();
        services.AddTransient<RecordCrfWindow>();
        services.AddTransient<CompleteVisitWindow>();
        services.AddTransient<QueryActionWindow>();
        services.AddTransient<CreateStudyWindow>();
        services.AddTransient<ParticipantDetailWindow>();
        services.AddTransient<UpdateEthicsWindow>();
        services.AddTransient<EditMasterTermWindow>();

        // Window factories
        services.AddSingleton<EnrollParticipantWindowFactory>();
        services.AddSingleton<ScheduleVisitWindowFactory>();
        services.AddSingleton<RecordCrfWindowFactory>();
        services.AddSingleton<CompleteVisitWindowFactory>();
        services.AddSingleton<QueryActionWindowFactory>();
        services.AddSingleton<CreateStudyWindowFactory>();
        services.AddSingleton<ParticipantDetailWindowFactory>();
        services.AddSingleton<UpdateEthicsWindowFactory>();
        services.AddSingleton<EditMasterTermWindowFactory>();

        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }

    public static async Task UpdateSessionAsync(IServiceProvider services, string email)
    {
        var session = services.GetRequiredService<ClinicalSession>();
        var snapshot = await services.GetRequiredService<IClinicalContextService>().GetDefaultContextAsync()
            ?? throw new InvalidOperationException("Clinical context is unavailable.");
        session.Email = email;
        session.Role = ClinicalRoleMapper.FromEmail(email);
        session.Context = snapshot with { ActorUserId = ClinicalRoleMapper.ActorUserIdFromEmail(email) };
    }

    private static void RemoveCorruptDatabaseFiles(string databasePath)
    {
        if (File.Exists(databasePath) && new FileInfo(databasePath).Length == 0)
        {
            File.Delete(databasePath);
            var journal = $"{databasePath}-journal";
            if (File.Exists(journal)) File.Delete(journal);
            var wal = $"{databasePath}-wal";
            if (File.Exists(wal)) File.Delete(wal);
            var shm = $"{databasePath}-shm";
            if (File.Exists(shm)) File.Delete(shm);
        }
    }
}

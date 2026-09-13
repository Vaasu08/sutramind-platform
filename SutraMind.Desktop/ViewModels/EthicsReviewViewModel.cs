using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SutraMind.Desktop.ViewModels;

public sealed class EthicsReviewViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IEthicsReadService _ethicsService;
    private readonly IShellNavigation _navigation;
    private readonly LocalDbContext _context;
    private EthicsReviewData? review;

    public EthicsReviewViewModel(ClinicalSession session, IEthicsReadService ethicsService, IShellNavigation navigation, LocalDbContext context)
    {
        _session = session;
        _ethicsService = ethicsService;
        _navigation = navigation;
        _context = context;
        UpdateStatusCommand = new PermissionCommand(() => session.Role, Permission.UpdateEthics, () => OpenUpdateEthics());
    }

    public override WorkspaceSection Section => WorkspaceSection.EthicsReview;

    public EthicsReviewData? Review
    {
        get => review;
        private set => SetProperty(ref review, value);
    }

    public ICommand UpdateStatusCommand { get; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Review = await _ethicsService.GetByStudyAsync(_session.ActiveStudyId, cancellationToken);
    }

    private async void OpenUpdateEthics()
    {
        if (Review is null) return;
        var ethics = await _context.EthicsReviews.AsNoTracking()
            .FirstOrDefaultAsync(e => e.StudyId == _session.ActiveStudyId);
        if (ethics is null) return;
        _navigation.StartUpdateEthics(ethics.Id, Review.IecNumber, Review.Decision);
    }
}

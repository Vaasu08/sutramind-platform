using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;

namespace SutraMind.Desktop.ViewModels;

public sealed class EthicsReviewViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IEthicsReadService _ethicsService;
    private EthicsReviewData? review;

    public EthicsReviewViewModel(ClinicalSession session, IEthicsReadService ethicsService)
    {
        _session = session;
        _ethicsService = ethicsService;
    }

    public override WorkspaceSection Section => WorkspaceSection.EthicsReview;

    public EthicsReviewData? Review
    {
        get => review;
        private set => SetProperty(ref review, value);
    }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Review = await _ethicsService.GetByStudyAsync(_session.ActiveStudyId, cancellationToken);
    }
}

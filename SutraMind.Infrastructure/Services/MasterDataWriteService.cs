using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Domain.Entities;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class MasterDataWriteService(LocalDbContext context) : IMasterDataWriteService
{
    public async Task AddAsync(string category, string code, string labelEn, string labelHi, string? modernMapping, int sortOrder, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Category and code are required.");

        var exists = await context.MasterTerms.AsNoTracking()
            .AnyAsync(t => t.Category == category && t.Code == code, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"A term with code '{code}' already exists in category '{category}'.");

        context.MasterTerms.Add(new MasterTerm
        {
            Category = category.Trim(),
            Code = code.Trim(),
            LabelEn = labelEn.Trim(),
            LabelHi = labelHi.Trim(),
            ModernMappingEn = modernMapping?.Trim(),
            SortOrder = sortOrder,
            Active = true
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Guid termId, string labelEn, string labelHi, string? modernMapping, int sortOrder, bool active, CancellationToken cancellationToken = default)
    {
        var term = await context.MasterTerms.SingleAsync(t => t.Id == termId, cancellationToken);

        term.LabelEn = labelEn.Trim();
        term.LabelHi = labelHi.Trim();
        term.ModernMappingEn = modernMapping?.Trim();
        term.SortOrder = sortOrder;
        term.Active = active;
        term.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}

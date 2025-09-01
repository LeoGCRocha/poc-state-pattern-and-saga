using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class SagaRepository : ISagaRepository
{
    private SagaContext Context { get; set; }

    public SagaRepository(SagaContext context)
    {
        Context = context;
    }

    public async Task<SagaEntity?> GetByCorrelationId(Guid correlationId)
    {
        return await Context.Sagas.FirstOrDefaultAsync(s => s.CorrelationId == correlationId);
    }

    public async Task<SagaEntity?> GetSagaOnReferenceDate(DateTime referenceDate)
    {
        return await Context.Sagas.FirstOrDefaultAsync(s =>
            s.ReferenceDate == referenceDate);
    }

    public async Task Add(SagaEntity sagaEntity)
    {
        await Context.Sagas.AddAsync(sagaEntity);
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}
namespace Domain;

public interface ISagaRepository
{
    public Task<SagaEntity?> GetByCorrelationId(Guid correlationId);
    public Task<SagaEntity?> GetSagaOnReferenceDate(DateTime referenceDate);
    public Task Add(SagaEntity sagaEntity);
    public Task SaveChangesAsync();
}
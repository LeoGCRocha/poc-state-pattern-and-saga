namespace Domain;

public class SagaEntity
{
    public Guid CorrelationId { get; set; }
    public DateTime ReferenceDate { get; set; }
    public SagaStatus Status { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public List<SagaEvent> SagaEvents { get; set; } = new();
    public SagaEntity() {}
}

public class SagaEvent {
    public SagaStatus Status { get; set; }
    public string Step { get; set; }
    public DateTime OccuredAt { get; set; }

    public SagaEvent(SagaStatus status, string step)
    {
        Status = status;
        Step = step;
    }
}

public enum SagaStatus
{
    Pending = 0,
    Running = 1,
    Stopped = 2,
    Finished = 3,
    Failed = 4,
    Retrying = 5
}
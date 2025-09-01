namespace Domain.Events;

public abstract class BaseEvent
{
    public  string EventType { get; set; }
    public  Guid CorrelationId { get; set; }
    public  DateTime ReferenceDate { get; set; }

    protected BaseEvent(string eventType, Guid correlationId, DateTime referenceDate)
    {
        EventType = eventType;
        CorrelationId = correlationId;
        ReferenceDate = referenceDate;
    }
}
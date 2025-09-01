namespace Domain.Events;

public class EventExecutionFinished : BaseEvent
{
    public EventExecutionFinished(string eventType, Guid correlationId, DateTime referenceDate) : base(eventType, correlationId, referenceDate)
    {
    }
}
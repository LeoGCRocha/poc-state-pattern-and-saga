using Domain;

namespace Application.SagaOrchestration.Core;

public interface IContextSagaOrchestrator
{
    Task RunAsync();
    Task UpdateStepStatus(Type currentStep, SagaStatus status, ExceptionPayload? exception = null);
}
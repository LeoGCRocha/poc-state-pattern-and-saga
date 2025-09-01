
using Application.SagaOrchestration.Core;

namespace Application.SagaOrchestration.States;

public interface IState
{
    public Task ExecuteRoutine(ContextSagaOrchestrator contextSagaOrchestrator);
}
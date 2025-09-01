using System.Text.Json;
using Application.SagaOrchestration.Core;
using Domain;
using Application.Services;

namespace Application.SagaOrchestration.States;

public class StateBase : IState
{
    private Type CurrentStep { get; set; }
    private IState? NextState { get; set; }
    private string UrlRequest { get; }
    private ISimpleApiService SimpleApiService { get; }
    
    protected StateBase(ISimpleApiService simpleApiService, string urlRequest, IState? nextState)
    {
        CurrentStep = GetType();
        NextState = nextState;
        SimpleApiService = simpleApiService;
        UrlRequest = urlRequest;
    }

    public virtual async Task ExecuteRoutine(ContextSagaOrchestrator contextSagaOrchestrator)
    {
        await contextSagaOrchestrator.UpdateStepStatus(CurrentStep, SagaStatus.Running);

        try
        {
            var response = await SimpleApiService.CallServiceEndpoint(UrlRequest);

            if (!response.IsSuccessStatusCode)
            {
                var customException = new ExceptionPayload($"[{response.StatusCode}]Failed to call service endpoint {UrlRequest}",
                    JsonSerializer.Serialize(response.Content));
                
                await contextSagaOrchestrator.UpdateStepStatus(CurrentStep, SagaStatus.Failed, customException);
                return;
            }

            contextSagaOrchestrator.Saga.SagaEvents.Add(new SagaEvent(SagaStatus.Finished, CurrentStep.Name));
            TransitToNextState(contextSagaOrchestrator);
        }
        catch (Exception ex)
        {
            await contextSagaOrchestrator.UpdateStepStatus(CurrentStep, SagaStatus.Failed, new ExceptionPayload(ex.Message));
            throw;
        }
    }

    protected virtual void TransitToNextState(ContextSagaOrchestrator contextSagaOrchestrator)
    {
        if (NextState is null)
        {
            contextSagaOrchestrator.Saga.Status = SagaStatus.Finished;
            return;
        }
        contextSagaOrchestrator.SetCurrentState(NextState);
    }
}
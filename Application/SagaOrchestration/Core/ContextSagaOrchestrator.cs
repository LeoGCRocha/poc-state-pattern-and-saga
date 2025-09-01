using System.Text.Json;
using Application.SagaOrchestration.States;
using Application.SagaOrchestration.Workflows;
using Application.Services;
using Domain;

namespace Application.SagaOrchestration.Core;

public sealed class ContextSagaOrchestrator
    : IContextSagaOrchestrator
{
    private IState CurrentState { get; set; }
    private DateTime ReferenceDate { get; }
    public SagaEntity Saga { get; private set; }
    
    private readonly ISagaRepository _sagaRepository;

    private ContextSagaOrchestrator(ISagaRepository sagaRepository, DateTime referenceDate, IState currentState, SagaEntity saga)
    {
        _sagaRepository = sagaRepository;
        ReferenceDate = referenceDate;
        CurrentState = currentState;
        Saga = saga;
    }

    // Factory method
    public static async Task<ContextSagaOrchestrator> CreateOrGetAsync(DateTime referenceDate,
        ISimpleApiService simpleApiService, ISagaRepository sagaRepository, WorkflowFactory? factory = null)
    {
        var alreadyExistedSaga = await sagaRepository.GetSagaOnReferenceDate(referenceDate);

        SagaEntity saga;
        IState state;

        var workflowFactory = factory ?? new WorkflowFactory(simpleApiService);
        if (alreadyExistedSaga is not null)
        {
            saga = alreadyExistedSaga;
            state = workflowFactory.CreateOrGetMainWorkflow(saga.CurrentStep);

            if (saga.Status == SagaStatus.Finished)
                throw new Exception($"Day {referenceDate} already processed.");
            
            saga.Status = SagaStatus.Retrying;
            saga.SagaEvents.Add(new SagaEvent(SagaStatus.Retrying, state.GetType().Name));
            saga.Payload = JsonSerializer.Serialize(new object(){});
            await sagaRepository.SaveChangesAsync();
        }
        else
        {
            var initialState = workflowFactory.CreateOrGetMainWorkflow();

            saga = new SagaEntity
            {
                CorrelationId = Guid.NewGuid(),
                ReferenceDate = referenceDate,
                Status = SagaStatus.Pending,
                CurrentStep = initialState.GetType().Name
            };
            
            saga.SagaEvents.Add(new SagaEvent(SagaStatus.Pending, initialState.GetType().Name));

            state = initialState;
            await sagaRepository.Add(saga);
        }

        return new ContextSagaOrchestrator(sagaRepository, referenceDate, state, saga);
    }

    public async Task RunAsync()
    {
        await _sagaRepository.SaveChangesAsync();
        while (Saga.Status != SagaStatus.Finished && Saga.Status != SagaStatus.Failed)
        {
            try
            {
                await CurrentState.ExecuteRoutine(this);
                await _sagaRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
                break;
            }
        }
    }

    public void SetCurrentState(IState newState)
    {
        CurrentState = newState;
        Saga.CurrentStep = newState.GetType().Name;
        Saga.SagaEvents.Add(new SagaEvent(SagaStatus.Pending, newState.GetType().Name));
    }

    public async Task UpdateStepStatus(Type currentStep, SagaStatus status, ExceptionPayload? exception = null)
    {
        var sagaEvent = new SagaEvent(status, currentStep.Name);

        if (exception is not null)
        {
            Saga.Payload = JsonSerializer.Serialize(exception, new JsonSerializerOptions()
            {
                WriteIndented = false
            });
        }

        Saga.SagaEvents.Add(sagaEvent);
        Saga.Status = status;

        await _sagaRepository.SaveChangesAsync();
    }
}
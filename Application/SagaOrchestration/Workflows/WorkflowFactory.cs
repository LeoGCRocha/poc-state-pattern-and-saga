using Application.Services;
using Application.SagaOrchestration.States;

namespace Application.SagaOrchestration.Workflows;

public class WorkflowFactory
{
    private readonly ISimpleApiService _simpleApiService;
    
    public WorkflowFactory(ISimpleApiService simpleApiService)
    {
        _simpleApiService = simpleApiService;
    }

    public IState CreateOrGetMainWorkflow(string? currentStep = null)
    {
        var stateE = new ServiceEState(_simpleApiService); // final state
        var stateD = new ServiceDState(_simpleApiService, stateE);
        var stateC = new ServiceCState(_simpleApiService, stateD);
        var stateB = new ServiceBState(_simpleApiService, stateC);
        var stateA = new ServiceAState(_simpleApiService, stateB);
        
        if (currentStep is null)
            return stateA;

        return currentStep switch
        {
            nameof(ServiceBState) => stateB,
            nameof(ServiceCState) => stateC,
            nameof(ServiceDState) => stateD,
            nameof(ServiceEState) => stateE,
            _ => stateA
        };
    }
}
using Application.SagaOrchestration.States;
using Application.Services;

namespace Application.SagaOrchestration.Workflows;

public class ServiceAState(ISimpleApiService simpleApiService, IState? state = null)
    : StateBase(simpleApiService, "serviceA", state);
    
public class ServiceBState(ISimpleApiService simpleApiService, IState? state = null)
    : StateBase(simpleApiService, "serviceB", state);
    
public class ServiceCState(ISimpleApiService simpleApiService, IState? state = null)
    : StateBase(simpleApiService, "serviceC", state);
public class ServiceDState(ISimpleApiService simpleApiService, IState? state = null)
    : StateBase(simpleApiService, "serviceD", state);
    
public class ServiceEState(ISimpleApiService simpleApiService, IState? state = null)
    : StateBase(simpleApiService, "serviceE", state);
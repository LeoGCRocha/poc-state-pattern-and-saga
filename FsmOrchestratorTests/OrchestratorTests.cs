using Domain;
using NSubstitute;
using Application.Services;
using Application.SagaOrchestration.Core;
using Application.SagaOrchestration.Workflows;
using Refit;

namespace FsmOrchestratorTests;

public class OrchestratorTests
{
    private readonly ISimpleApiService _simpleApiService = Substitute.For<ISimpleApiService>();
    private readonly ISagaRepository _sagaRepository = Substitute.For<ISagaRepository>();

    [Fact(DisplayName = "Should Correctly Iterate From State A to B")]
    public async Task Should_Correctly_Iterate_From_State_A_to_B()
    {
        // Arrange
        var orchestrator = await ContextSagaOrchestrator.CreateOrGetAsync(new DateTime(2025, 05, 10), _simpleApiService,
            _sagaRepository, new WorkflowFactory(_simpleApiService));

        _simpleApiService.CallServiceEndpoint(Arg.Any<string>())
            .Returns(SuccessResponseMock());

        // Act
        await orchestrator.RunAsync();

        // Assert
        await _simpleApiService.Received(1).CallServiceEndpoint("serviceA");
        await _simpleApiService.Received(1).CallServiceEndpoint("serviceB");
    }

    [Fact(DisplayName = "Should get back to last state after a failure")]
    public async Task Should_Get_Back_To_Latest_State_After_A_Failure()
    {
        // Arrange
        var orchestrator =
            await ContextSagaOrchestrator.CreateOrGetAsync(new DateTime(2025, 05, 10), _simpleApiService,
                _sagaRepository, new WorkflowFactory(_simpleApiService));

        // Act
        _simpleApiService.CallServiceEndpoint("serviceA")
            .Returns(SuccessResponseMock());

        _simpleApiService.CallServiceEndpoint("serviceB")
            .Returns(SuccessResponseMock());
        
        _simpleApiService.CallServiceEndpoint("serviceC")
            .Returns(FailedResponseMock());

        await orchestrator.RunAsync();

        // Assert test part 1
        await _simpleApiService.Received(1).CallServiceEndpoint("serviceA");

        _simpleApiService.CallServiceEndpoint("serviceC")
            .Returns(SuccessResponseMock());

        orchestrator.Saga.Status = SagaStatus.Retrying;
        await orchestrator.RunAsync();

        await _simpleApiService.Received(2)
            .CallServiceEndpoint("serviceC");

        await _simpleApiService.Received(1)
            .CallServiceEndpoint("serviceD");
    }

    private static ApiResponse<object> SuccessResponseMock()
    {
        return new ApiResponse<object>(
            new HttpResponseMessage(System.Net.HttpStatusCode.OK),
            new object(),
            null!
        );
    }

    private static ApiResponse<object> FailedResponseMock()
    {
        return new ApiResponse<object>(
            new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest),
            new Object(),
            null!);
    }
}
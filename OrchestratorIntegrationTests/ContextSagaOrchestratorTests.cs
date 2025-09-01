using Application.SagaOrchestration.Core;
using Application.SagaOrchestration.States;
using Application.SagaOrchestration.Workflows;
using Application.Services;
using FluentAssertions;
using Infra.Repository;
using NSubstitute;
using OrchestratorIntegration.Fixtures;
using Refit;

namespace OrchestratorIntegration;

public class ContextSagaOrchestratorTests
{
    private readonly SagaOrchestratorFixture _fixture = new();
    private readonly ISimpleApiService _apiService = Substitute.For<ISimpleApiService>();

    [Fact(DisplayName = "Should correctly rerun a failed saga")]
    public async Task Should_Correctly_Rerun_A_Failed_Saga()
    {
        // Arrange
        var referenceDate = new DateTime(2025, 05, 20);
        await _fixture.InitializeAsync();
        SagaRepository sagaRepository = new SagaRepository(_fixture.GetDbContext());

        _apiService.CallServiceEndpoint("serviceA")
            .Returns(SuccessResponseMock());

        _apiService.CallServiceEndpoint("serviceB")
            .Returns(FailedResponseMock());

        // Act
        var saga = await ContextSagaOrchestrator.CreateOrGetAsync(referenceDate,
            _apiService, sagaRepository);

        await saga.RunAsync();

        // Assert
        saga =  await ContextSagaOrchestrator.CreateOrGetAsync(referenceDate,
            _apiService, sagaRepository);

        saga.Saga.CurrentStep.Should().Be(nameof(ServiceBState));
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
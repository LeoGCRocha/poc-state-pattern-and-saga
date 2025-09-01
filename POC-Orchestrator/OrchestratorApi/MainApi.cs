using Application.SagaOrchestration.Core;
using Application.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using POC_Orchestrator.OrchestratorApi.DTO;

namespace POC_Orchestrator.OrchestratorApi;

[ApiController]
[Route("orchestrator/")]
[Tags("Orchestrator")]
public class MainApi : ControllerBase
{
    private readonly ISagaRepository _sagaRepository;
    private readonly ISimpleApiService _simpleApiService;


    public MainApi(ISagaRepository sagaRepository, ISimpleApiService simpleApiService)
    {
        _sagaRepository = sagaRepository;
        _simpleApiService = simpleApiService;
    }

    [HttpPost("run")]
    public async Task<IActionResult> Run(OrchestratorExecutionDto orchestratorExecutionDto)
    {
        try
        {
            var saga = await ContextSagaOrchestrator.CreateOrGetAsync(orchestratorExecutionDto.ReferenceDate,
                _simpleApiService, _sagaRepository);
            await saga.RunAsync();
            if (saga.Saga.Status == SagaStatus.Finished)
                return Ok($"Finished execution on day {orchestratorExecutionDto.ReferenceDate}");
            else
                return BadRequest($"Failed to run day {orchestratorExecutionDto.ReferenceDate}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
            
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status(OrchestratorExecutionDto orchestratorExecutionDto)
    {
        await Task.Delay(1000);
        return Ok("Status is....");
    }
}
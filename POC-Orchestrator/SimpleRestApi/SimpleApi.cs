using Microsoft.AspNetCore.Mvc;

namespace POC_Orchestrator.SimpleRestApi;

[ApiController]
[Route("start/")]
[Tags("Simple Services")]
public class SimpleApi : ControllerBase
{
    [HttpPost("serviceA")]
    public async Task<IActionResult> ServiceA()
    {
        await Task.Delay(5000);
        return Ok("Finish serviceA processing");
    }
    
    [HttpPost("serviceB")]
    public async Task<IActionResult> ServiceB()
    {
        await Task.Delay(5000);
        return Ok("Finish serviceA processing");
    }
    
    [HttpPost("serviceC")]
    public async Task<IActionResult> ServiceC()
    {
        await Task.Delay(5000);
        return Ok("Finish serviceA processing");
    }
    
    [HttpPost("serviceD")]
    public async Task<IActionResult> ServiceD()
    {
        await Task.Delay(5000);
        // return BadRequest("Failed to run the process");
        return Ok("Finish serviceA processing");
    }
    
    [HttpPost("serviceE")]
    public async Task<IActionResult> ServiceE()
    {
        await Task.Delay(5000);
        return Ok("Finish serviceA processing");
    }
    
    [HttpPost("serviceF")]
    public async Task<IActionResult> ServiceF()
    {
        await Task.Delay(5000);
        return Ok("Finish serviceA processing");
    }
}
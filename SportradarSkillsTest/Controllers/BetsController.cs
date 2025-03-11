using Microsoft.AspNetCore.Mvc;
using SportradarSkillsTest.Domain;
using SportradarSkillsTest.Services;
using System.Text.Json;

namespace SportradarSkillsTest.Controllers;

[ApiController]
[Route("[controller]")]
public class BetsController : ControllerBase
{
    private readonly IBetProcessingService _service;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly string _jsonFilePath = "data\\bets.json";

    public BetsController(IBetProcessingService service, IHostApplicationLifetime applicationLifetime)
    {
        _service = service;
        _applicationLifetime = applicationLifetime;
    }

    [HttpPost]
    public IActionResult AddBet([FromBody] Bet bet)
    {
        _service.AddBet(bet);
        return Ok("Bet received");
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(_service.GetSummary());
    }

    [HttpGet("review")]
    public IActionResult GetBetsToReview()
    {
        return Ok(_service.GetBetsToReview());
    }

    [HttpPost("initialize")]
    public IActionResult Initialize()
    {

        if (!System.IO.File.Exists(_jsonFilePath))
        {
            return NotFound("File 'bets.json' not found.");
        }

        try
        {
            string jsonString = System.IO.File.ReadAllText(_jsonFilePath);
            List<Bet>? bets = JsonSerializer.Deserialize<List<Bet>>(jsonString);

            if (bets == null) throw new Exception("File 'bets.json' format is invalid or not supported.");

            foreach (var bet in bets)
            {
                _service.AddBet(bet);
            }

            return Ok(bets);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing json file: {ex.Message}");
        }
    }

    [HttpPost("shutdown")]
    public IActionResult ShutdownSystem()
    {
        Task.Run(ShutDown);
        return Ok("The system will gracefully shut down after processing all pending bets.");
    }

    private async Task ShutDown()
    {
        _service.Shutdown();
        _applicationLifetime.StopApplication();
    }



}
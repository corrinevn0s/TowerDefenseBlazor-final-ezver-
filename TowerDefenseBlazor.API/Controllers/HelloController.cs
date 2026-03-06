using Microsoft.AspNetCore.Mvc;

namespace TowerDefenseBlazor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello from API!";
}
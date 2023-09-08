using Microsoft.AspNetCore.Mvc;

namespace EzRPC.ServerDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoRpcController : ControllerBase
{
	private readonly ILogger<DemoRpcController> _logger;

	public DemoRpcController(ILogger<DemoRpcController> logger)
	{
		_logger = logger;
	}

	[HttpGet(Name = "RpcDemo")]
	public string Get()
	{
		return "Yoo";
	}
}

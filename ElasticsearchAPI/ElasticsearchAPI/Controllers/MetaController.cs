using ElasticsearchAPI.Converters;
using ElasticsearchAPI.Services;
using ElasticsearchAPI.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ElasticsearchAPI.Controllers;

[ApiController]
[Route("/get_meta")]
public class MetaController : ControllerBase
{
    #region Constructor and parameters

    private readonly IElasticService _elasticService;
    private readonly ILoggerService _loggerService;


    public MetaController(ILoggerService loggerService)
    {
        _elasticService = ElasticService.Instance;
        _loggerService = loggerService;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> GetMetaProperties()
    {
        try
        {
            var data = await _elasticService.GetAllIndicesProperties();
            return Ok(JsonLdConverter.MetaDictionaryToJsonLd(data));
        }
        catch (InvalidOperationException ex)
        {
            await _loggerService.LogToFile(ex);
            return BadRequest(ex.Message);
        }
    }
}
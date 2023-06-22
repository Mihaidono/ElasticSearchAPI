using ElasticsearchAPI.Converters;
using ElasticsearchAPI.Services;
using ElasticsearchAPI.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace ElasticsearchAPI.Controllers;

[ApiController]
[Route("/get_data")]
public class MovieController : ControllerBase
{
    #region Constructor and parameters

    private readonly IElasticService _elasticService;
    private readonly ILoggerService _loggerService;

    public MovieController(ILoggerService loggerService)
    {
        _elasticService = ElasticService.Instance;
        _loggerService = loggerService;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> GetData([FromQuery] string data_type, [FromQuery] bool snippet)
    {
        try
        {
            if (!snippet)
            {
                var data = await _elasticService.GetAllData(data_type);

                //modifying the response to contain only good data
                //ValidationService.ValidateResponseData(ref data);

                return Ok(JsonLdConverter.ObjectToJsonLd(data, data_type));
            }
            else
            {
                var data = await _elasticService.GetSnippetData(data_type);

                //modifying the response to contain only good data
                //ValidationService.ValidateResponseData(ref data);

                return Ok(JsonLdConverter.ObjectToJsonLd(data, data_type));
            }
        }
        catch (InvalidOperationException ex)
        {
            await _loggerService.LogToFile(ex);
            return BadRequest(ex.Message);
        }
    }

    #region Population Purpose

    //------------------------------------------------------------------------------------------------------------
    // Http request used for populating the Elastic Search Database with Movie Objects
    //------------------------------------------------------------------------------------------------------------
    // [HttpPost("populateDB")]
    // public async Task<ActionResult> PopulateDb()
    // {
    //     try
    //     {
    //         await _elasticService.PopulateMovieDb();
    //         
    //         return Ok();
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         await _loggerService.LogToFile(ex);
    //         return BadRequest(ex.Message);
    //     }
    // }
    //------------------------------------------------------------------------------------------------------------

    #endregion
}
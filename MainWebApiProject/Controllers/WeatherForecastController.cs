using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeatherRequests;

namespace MainWebApiProject.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class  WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherClient _weatherClient;
    private readonly IMemoryCache _memoryCache;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherClient weatherClient, IMemoryCache memoryCache)
    {
        _logger = logger;
        _weatherClient = weatherClient;
        _memoryCache = memoryCache;
    }
    
    /// <summary>
    /// Get weather forecast for the specified coordinates.
    /// </summary>
    /// <param name="latitude">Latitude of the location.</param>
    /// <param name="longitude">Longitude of the location.</param>
    /// <returns>Weather information for the specified location.</returns>
    /// <response code="200">Returns the weather information.</response>
    /// <response code="400">If the coordinates are invalid or an error occurred.</response>
    [HttpGet("coordinates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Get(double latitude, double longitude)
    {
        try
        {
            return Ok(await _weatherClient.GetWeatherByCoordinates(latitude, longitude));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting weather forecast by coordinates.");
            return BadRequest("Failed to retrieve weather forecast. Please try again later.");
        }
    }


    /// <summary>
    /// Get weather forecast for the specified city.
    /// </summary>
    /// <param name="cityName">Name of the city.</param>
    /// <returns>Weather information for the specified city.</returns>
    /// <response code="200">Returns the weather information.</response>
    /// <response code="400">If the city name is invalid or an error occurred.</response>
    [HttpGet("{cityName}", Name = "GetWeatherForecastByCity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetByCity(string cityName)
    {
        try
        {
            var weatherForecast = await _weatherClient.GetWeatherByCity(cityName);
            return Ok(weatherForecast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting weather forecast by city.");
            return BadRequest("Failed to retrieve weather forecast. Please try again later.");
        }
    }

    /// <summary>
    /// Set up _defaultCity in cache memory.
    /// </summary>
    /// <param name="city"></param>
    /// <response code="200">Ok</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Post(string city)
    {
    
        _memoryCache.Set("_defaultCity", city);
        return Ok(new Dictionary<string, string>() {{"_defaultCity", city}, {"statusCode", "200"} });
    }
    
    /// <summary>
    /// Returns weather for city if cityName was set earlier by the Post method, 
    /// otherwise 404 error 
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherForecast>> GetByDefault()
    {
        if (!_memoryCache.TryGetValue("_defaultCity", out string city))
            return NotFound(); 
        return await GetByCity(city);
    
    }
    
}

using System.Text.Json.Serialization;

namespace WeatherRequests.Models;

public class WeatherForecast
{
    /// <summary>Information about speed of wind</summary>
    /// <example>"wind: {'speed':'4.47'}"</example>
    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }
    
    /// <summary>General description of the weather</summary>
    /// <example> weather: [{ description: "overcast clouds"}], </example>
    public List<Weather> Weather { get; set; }

    /// <summary>Main information about weather like a temp, pressure and humidity</summary>
    /// <example>" main: {temp: 29.27, pressure: 1007,humidity: 57}</example>
    public MainInfo Main { get; set; }

    /// <summary>Name of the area</summary>
    /// <example> name: "Kizicheskaya" </example>
    public string Name { get; set; }
}

public class Wind
{
    public double Speed { get; set; }
}

public class Weather
{
    public string Description { get; set; }
}

public class MainInfo
{
    public double Temp { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

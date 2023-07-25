using System.Text.Json;
using Microsoft.Extensions.Options;
using WeatherRequests.Models;

namespace WeatherRequests;

public class WeatherClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public ServiceSettings settings { get; }
    public WeatherClient(IOptions<ServiceSettings> options)
    {
        settings = options.Value;
    }
    
    public async Task<WeatherForecast> GetWeatherByCoordinates(double latitude, double longitude)
    {
        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={settings.ApiKey}&units=metric";
        return await MakeResponse(apiUrl);
    }
    
    public async Task<WeatherForecast> GetWeatherByCity(string cityName)
    {
        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={settings.ApiKey}&units=metric";
        return await MakeResponse(apiUrl);
    }
    
    private async Task<WeatherForecast> MakeResponse(string apiUrl)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
        string json = await response.Content.ReadAsStringAsync();
        Console.WriteLine(json);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"GET {apiUrl} - {response.StatusCode}\n{json}");

		WeatherForecast weatherData = JsonSerializer.Deserialize<WeatherForecast>(json, new JsonSerializerOptions 
        {
            PropertyNameCaseInsensitive = true
        });
		var weatherResponse = new WeatherForecast();
		if (weatherData != null)
		{
			weatherResponse.Wind = new Wind { Speed = weatherData.Wind.Speed };
			weatherResponse.Weather = new List<Weather> { new Weather { Description = weatherData.Weather[0].Description } };
			weatherResponse.Main = new MainInfo { Temp = weatherData.Main.Temp, Pressure = weatherData.Main.Pressure, Humidity = weatherData.Main.Humidity };
			weatherResponse.Name = weatherData.Name;
		}
		return weatherResponse;

	}
}

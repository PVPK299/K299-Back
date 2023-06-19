using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace K299_Back.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // GET: api/weather/stations
        [HttpGet("stations")]
        [Produces("application/json")]
        public async Task<IActionResult> stationsAsync()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("https://api.meteo.lt/v1/stations");

            response.EnsureSuccessStatusCode();

            var body = JsonConvert.DeserializeObject<List<Station>>(await response.Content.ReadAsStringAsync());

            return StatusCode(StatusCodes.Status200OK, body);
        }

        // GET: api/weather/GetStationObservations
        [HttpGet("GetStationObservations")]
        public async Task<StationObservations?> GetStationObservations(String? station, String? date)
        {
            _logger.LogInformation($"GetStationObservations: date={date}");

            station ??= "kauno-ams";
            date ??= "latest";

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://api.meteo.lt/v1/stations/{station}/observations/{date}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // opt1
            // var data = System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, object>>(json);
            // return StatusCode(StatusCodes.Status200OK, data["observations"]);

            // opt2
            return JsonConvert.DeserializeObject<StationObservations>(json);
        }

        // GET: api/weather/stations
        [HttpGet("observation")]
        [Produces("application/json")]
        public async Task<IActionResult> observation(String? station, String? date)
        {
            var stationObservations = await GetStationObservations(station, date);
            return StatusCode(StatusCodes.Status200OK, stationObservations.observations);
        }

        // GET: api/weather/current
        [HttpGet("current")]
        [Produces("application/json")]
        public async Task<IActionResult> currentWeather(String? station)
        {
            var stationObservations = await GetStationObservations(station, null);
            return StatusCode(StatusCodes.Status200OK, stationObservations.observations.Last());
        }
    }
}
using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace K299_Back.Controllers
{
    [ApiController]
    [Route("api/weather_observation")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("GetLastNDaysObservanations/{days}")]
        public IActionResult? GetLastNDaysObservanations(int days)
        {
            string connectionString = _configuration.GetConnectionString("SolarData");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DateTime latest30days = DateTime.Now.AddDays(-days);
                string date = latest30days.ToString("yyyy-MM-dd HH:mm:ss.fff");

                string query = @"SELECT * FROM dbo.[weather_observation] 
                                WHERE observationTimeUtc > @date;";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@date", date);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

                    while (reader.Read())
                    {
                        Dictionary<string, object> observation = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
                        data.Add(observation);
                    }

                    return StatusCode(StatusCodes.Status200OK, data);
                }
            }
        }

        [HttpGet("oldest")]
        public Observation? GetOldestObservation()
        {
            string connectionString = _configuration.GetConnectionString("SolarData");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM dbo.[weather_observation] 
                                WHERE observationTimeUtc = ( SELECT MIN(observationTimeUtc) FROM dbo.[weather_observation] );";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        Dictionary<string, object> observation = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);

                        string data = JsonConvert.SerializeObject(observation);

                        return JsonConvert.DeserializeObject<Observation>(data);
                    }

                    return null;
                }
            }
        }



        [HttpGet("latest")]
        public Observation? GetLatestObservation()
        {
            string connectionString = _configuration.GetConnectionString("SolarData");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM dbo.[weather_observation] 
                                WHERE observationTimeUtc = ( SELECT MAX(observationTimeUtc) FROM dbo.[weather_observation] );";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();


                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        Dictionary<string, object> observation = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);

                        string data = JsonConvert.SerializeObject(observation);

                        return JsonConvert.DeserializeObject<Observation>(data);
                    }

                    return null;
                }
            }
        }


        // POST: api/weather_observation
        [HttpPost("")]
        public IActionResult Insert([FromBody] Observation observation)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("SolarData");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string observationTimeUtc = "@observationTimeUtc";
                    string airTemperature = "@airTemperature";
                    string feelsLikeTemperature = "@feelsLikeTemperature";
                    string windSpeed = "@windSpeed";
                    string windGust = "@windGust";
                    string windDirection = "@windDirection";
                    string cloudCover = "@cloudCover";
                    string seaLevelPressure = "@seaLevelPressure";
                    string relativeHumidity = "@relativeHumidity";
                    string precipitation = "@precipitation";
                    string conditionCode = "@conditionCode";

                    string query = $@"INSERT INTO dbo.[weather_observation] (
                        observationTimeUtc,
                        airTemperature,
                        feelsLikeTemperature,
                        windSpeed,
                        windGust,
                        windDirection,
                        cloudCover, seaLevelPressure,
                        relativeHumidity,
                        precipitation,
                        conditionCode)
                    VALUES({observationTimeUtc}, {airTemperature}, {feelsLikeTemperature}, {windSpeed}, {windGust}, {windDirection}, {cloudCover}, {seaLevelPressure}, {relativeHumidity}, {precipitation}, {conditionCode});";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(observationTimeUtc, observation.observationTimeUtc);
                        command.Parameters.AddWithValue(airTemperature, observation.airTemperature);
                        command.Parameters.AddWithValue(feelsLikeTemperature, observation.feelsLikeTemperature);
                        command.Parameters.AddWithValue(windSpeed, observation.windSpeed);
                        command.Parameters.AddWithValue(windGust, observation.windGust);
                        command.Parameters.AddWithValue(windDirection, observation.windDirection);
                        command.Parameters.AddWithValue(cloudCover, observation.cloudCover);
                        command.Parameters.AddWithValue(seaLevelPressure, observation.seaLevelPressure);
                        command.Parameters.AddWithValue(relativeHumidity, observation.relativeHumidity);
                        command.Parameters.AddWithValue(precipitation, observation.precipitation);
                        command.Parameters.AddWithValue(conditionCode, observation.conditionCode);
                        connection.Open();

                        int rowsAffedted = command.ExecuteNonQuery();

                        if (rowsAffedted > 0)
                        {
                            return StatusCode(StatusCodes.Status201Created);
                        }
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> body = new() { { "error", e.ToString() } };
                return StatusCode(StatusCodes.Status400BadRequest, body);
            }
        }

        [HttpDelete("")]
        public IActionResult Delete()
        {
            string connectionString = _configuration.GetConnectionString("SolarData");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"TRUNCATE TABLE dbo.[weather_observation]";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    command.CommandText = @"SELECT CASE WHEN EXISTS(SELECT 1 FROM dbo.[weather_observation]) THEN 0 ELSE 1 END AS IsEmpty";

                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if ((int)reader[0] == 1) // is empty
                    {
                        return StatusCode(StatusCodes.Status200OK);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
        }
    }
}
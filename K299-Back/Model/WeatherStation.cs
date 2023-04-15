using System.Text.Json.Serialization;

namespace K299_Back.Model
{
    public class StationObservations
    {
        public Station station { get; set; }

        public List<Observation> observations { get; set; }
    }

    public class Observation
    {
        //[JsonPropertyName("time")]
        public DateTime observationTimeUtc { get; set; }

        public double airTemperature { get; set; }

        public double feelsLikeTemperature { get; set; }

        public double windSpeed { get; set; }

        public double windGust { get; set; }

        public int windDirection { get; set; }

        public int cloudCover { get; set; }

        public double seaLevelPressure { get; set; }

        public int relativeHumidity { get; set; }

        public double precipitation { get; set; }

        public string conditionCode { get; set; }
    }

    public class Station
    {
        public string code { get; set; }

        public string name { get; set; }

        public Coordinates coordinates { get; set; }
    }

    public class Coordinates
    {
        public double latitude { get; set; }

        public double longitude { get; set; }
    }
}



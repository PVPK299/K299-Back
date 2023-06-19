using K299_Back.Controllers;
using K299_Back.Model;

namespace K299_Back
{
    public class WeatherDataCollectService : IHostedService
    {
        private Timer _timer;



        private readonly WeatherController _weatherController;

        private readonly WeatherForecastController _weatherForecastController;



        public WeatherDataCollectService(WeatherController weatherController, WeatherForecastController weatherForecastController)
        {
            // _logger = logger;
            _weatherController = weatherController;
            _weatherForecastController = weatherForecastController;
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_weatherController.Delete(); // Clears table
            //System.Diagnostics.Debug.WriteLine("Clearing weather observations table");

            CollectOldWeatherObservations();
            CollectWeatherLatestObservations();

            return Task.CompletedTask;
        }

        private async void CollectOldWeatherObservations()
        {
            string date = _weatherController.GetOldestObservation()?.observationTimeUtc.ToString("yyyy-MM-dd") ?? "latest";

            while (true)
            {
                System.Diagnostics.Debug.WriteLine($"CollectOldWeatherObservations collecting {date} observation");


                StationObservations? observations = await _weatherForecastController.GetStationObservations(null, date);

                if (observations == null)
                {
                    System.Diagnostics.Debug.WriteLine("observations null -> retry");
                }
                else
                {
                    foreach (Observation observation in observations.observations)
                    {
                        var result = _weatherController.Insert(observation);
                    }

                    DateTime last = observations.observations.Last().observationTimeUtc;

                    if (last.AddYears(1) < DateTime.Now)
                    {
                        break;
                    }
                    else
                    {
                        date = last.AddDays(-1).ToString("yyyy-MM-dd");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"CollectOldWeatherObservations collecting completed -> going into sleep for 60s");

                await Task.Delay(60000);
            }
        }

        private async void CollectWeatherLatestObservations()
        {
            DateTime? latest = _weatherController.GetLatestObservation().observationTimeUtc;

            System.Diagnostics.Debug.WriteLine($"latest = {latest}");

            string date = "latest";


            if (latest == null)
            {
                return;
            }

            while (true)
            {
                System.Diagnostics.Debug.WriteLine($"CollectWeatherLatestObservations collecting {date} observation");


                StationObservations? observations = await _weatherForecastController.GetStationObservations(null, date);

                if (observations == null)
                {
                    System.Diagnostics.Debug.WriteLine("observations null -> retry");
                }
                else
                {
                    foreach (Observation observation in observations.observations)
                    {
                        var result = _weatherController.Insert(observation);

                        System.Diagnostics.Debug.WriteLine($"result {result}");
                    }

                    if (observations.observations.Last().observationTimeUtc > latest)
                    {
                        date = observations.observations.Last().observationTimeUtc.AddDays(-1).ToString("yyyy-MM-dd");
                        System.Diagnostics.Debug.WriteLine($" CollectWeatherLatestObservations collecting completed -> going into sleep for 30 sec");
                        await Task.Delay(30000);

                    }
                    else
                    {
                        date = "latest";
                        System.Diagnostics.Debug.WriteLine($" CollectWeatherLatestObservations collecting completed -> going into sleep for 1 hour");
                        await Task.Delay(3600000);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        // To detect redundant calls
        private bool _disposedValue;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                }
                _disposedValue = true;
            }
        }
    }
}


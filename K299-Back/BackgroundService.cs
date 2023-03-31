using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K299_Back.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace K299_Back
{
    public class MyBackgroundService : IHostedService
    {
        private Timer _timer;
        private int id = 1;
        private readonly SolarDataController _otherController;

        public MyBackgroundService(SolarDataController otherController)
        {
            _otherController = otherController;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var data = _otherController.GetSolarDataByID(id++);
            if(data != null)
                _otherController.IntervalInsert(data);
        }
    }
}


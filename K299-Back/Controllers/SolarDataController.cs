using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace K299_Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SolarDataController : ControllerBase
    {
        private readonly ILogger<SolarDataController> _logger;

        public SolarDataController(ILogger<SolarDataController> logger)
        {
            _logger = logger;
        }

        private void query(string q)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            
            string connectionString = configuration.GetConnectionString("SolarParkConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                        }
                    }
                }
            }
        }

        [HttpGet(Name = "GetSolarData")]
        public IEnumerable<SolarData> Get()
        {
            throw new NotImplementedException();
            
        }
    }
}



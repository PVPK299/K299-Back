using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace K299_Back.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/auth/register
        [HttpPost("register")]
        public IActionResult register([FromBody] NewUser user)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("SolarData");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string email = "@email";
                    string password = "@password";
                    string first_name = "@first_name";
                    string last_name = "@last_name";

                    string query = $@"INSERT INTO dbo.[user] (
                        email,
                        password,
                        first_name,
                        last_name)
                    OUTPUT INSERTED.*
                    VALUES({email}, {password}, {first_name}, {last_name});";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(email, user.email);
                        command.Parameters.AddWithValue(password, user.password);
                        command.Parameters.AddWithValue(first_name, user.first_name);
                        command.Parameters.AddWithValue(last_name, user.last_name);
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();

                        Dictionary<string, object> registeredUser = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
                        
                        return StatusCode(StatusCodes.Status201Created, registeredUser);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.ToString());
            }

        }
    }
}

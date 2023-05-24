using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        // POST: api/auth/register
        [HttpPost("register")]
        [Produces("application/json")]
        public IActionResult register([FromBody] NewUser user)
        {
            try
            {
                _logger.LogDebug(user.ToString());


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
                        registeredUser.Remove("birth_date");
                        registeredUser.Remove("park_share");

                        return StatusCode(StatusCodes.Status201Created, registeredUser);
                    }
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> body = new() { { "error", e.ToString() } };
                return StatusCode(StatusCodes.Status400BadRequest, body);
            }

        }

        // PATH: api/auth/update
        [HttpPatch("update")]
        [Produces("application/json")]
        public IActionResult update([FromBody] Dictionary<string, object> body)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("SolarData");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string id = "@id";
                    string email = "@email";
                    string password = "@password";
                    string first_name = "@first_name";
                    string last_name = "@last_name";

                    string query = $@"UPDATE dbo.[user] SET 
                            email = ISNULL({email}, email),
                            password = ISNULL({password}, password),
                            first_name = ISNULL({first_name}, first_name),
                            last_name = ISNULL({last_name}, last_name)
                        Where id = CONVERT(uniqueidentifier, {id})";

                    if (!body.ContainsKey("id"))
                    {
                        Dictionary<string, object> err = new() { { "error", "missing 'id' in body" } };
                        return StatusCode(StatusCodes.Status400BadRequest, err);
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(id, body["id"].ToString());
                        command.Parameters.AddWithValue(email, body.ContainsKey("email") ? body["email"].ToString() : DBNull.Value);
                        command.Parameters.AddWithValue(password, body.ContainsKey("password") ? body["password"].ToString() : DBNull.Value);
                        command.Parameters.AddWithValue(first_name, body.ContainsKey("first_name") ? body["first_name"].ToString() : DBNull.Value);
                        command.Parameters.AddWithValue(last_name, body.ContainsKey("last_name") ? body["last_name"].ToString() : DBNull.Value);

                        connection.Open();

                        int rows = command.ExecuteNonQuery();

                        if (rows != 1)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return StatusCode(StatusCodes.Status200OK);
                    }
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> err = new() { { "error", e.ToString() } };
                return StatusCode(StatusCodes.Status400BadRequest, err);
            }

        }
    }


}



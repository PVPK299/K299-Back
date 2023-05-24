using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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
                        // registeredUser.Remove("birth_date");
                        // registeredUser.Remove("park_share");

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

                        command.CommandText = $@"SELECT * FROM dbo.[user] WHERE id = {id}";
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();

                        Dictionary<string, object> user = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);

                        return StatusCode(StatusCodes.Status200OK, user);
                    }
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> err = new() { { "error", e.ToString() } };
                return StatusCode(StatusCodes.Status400BadRequest, err);
            }
        }


        //GET: api/auth/login
        [HttpGet("login/{email}/{password}")]
        public IActionResult login(string email, string password)
        {
            NewUser user = new NewUser();
            try
            {

                string emaill = "@email";
                string passwordd = "@password";

                string query = $@"SELECT id, email, password, first_name, last_name, park_share FROM dbo.[user] WHERE email = {emaill} AND password = {passwordd}";

                string sqlDataSource = _configuration.GetConnectionString("SolarData");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue(emaill, email);
                        myCommand.Parameters.AddWithValue(passwordd, password);
                        myCon.Open();

                        SqlDataReader myreader = myCommand.ExecuteReader();

                        myreader.Read();

                        user.ID = myreader.GetGuid(0);
                        user.email = myreader.GetString(1);
                        user.password = myreader.GetString(2);
                        user.first_name = myreader.GetString(3);
                        user.last_name = myreader.GetString(4);
                        user.park_share = (double)myreader.GetDecimal(5);

                    }

                    myCon.Close();
                    return new JsonResult(user);
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



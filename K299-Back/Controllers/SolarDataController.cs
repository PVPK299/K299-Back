using K299_Back.Model;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.IO;
using System.Text.Json.Nodes;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace K299_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolarDataController : ControllerBase
    {
        private readonly ILogger<SolarDataController> _logger;
        private readonly IConfiguration _configuration;

        public SolarDataController(ILogger<SolarDataController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/SolarData/GetSolarData
        [HttpGet("GetSolarData")]
        public JsonResult Get()
        {
            List<SolarData> Data = new List<SolarData>();

            string query = @"SELECT ID, Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName FROM dbo.Inverter_record";

            
            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myreader = myCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        SolarData solar = new SolarData()
                        {
                            ID                    = (int)myreader.GetInt32(0),
                            Time                  = myreader.GetDateTime(1),
                            Temperature           = (float)myreader.GetDouble(2),
                            PV1_Voltage           = (float)myreader.GetDouble(3),
                            PV2_Voltage           = (float)myreader.GetDouble(4),
                            PV1_Current           = (float)myreader.GetDouble(5),
                            PV2_Current           = (float)myreader.GetDouble(6),
                            Total_Energy          = (float)myreader.GetDouble(7),
                            Total_Operation_Hours = (float)myreader.GetDouble(8),
                            Total_AC_Power        = (float)myreader.GetDouble(9),
                            Daily_Energy          = (float)myreader.GetDouble(10),
                            ControllerName        = myreader.GetString(11)
                        };
                        Data.Add(solar);
                    }
                    myreader.Close();
                }

                myCon.Close();
            } 
            return new JsonResult(Data);
        }

        // GET: api/SolarData/GetSolarDataByID/{ID}
        [HttpGet("GetSolarDataByID/{ID}")]
        public JsonResult GetByID(int ID)
        {
            SolarData Data = new SolarData();

            string query = @"SELECT ID, Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName FROM dbo.Inverter_record WHERE ID =" + ID;


            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myreader = myCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        SolarData solar = new SolarData()
                        {
                            ID = (int)myreader.GetInt32(0),
                            Time = myreader.GetDateTime(1),
                            Temperature = (float)myreader.GetDouble(2),
                            PV1_Voltage = (float)myreader.GetDouble(3),
                            PV2_Voltage = (float)myreader.GetDouble(4),
                            PV1_Current = (float)myreader.GetDouble(5),
                            PV2_Current = (float)myreader.GetDouble(6),
                            Total_Energy = (float)myreader.GetDouble(7),
                            Total_Operation_Hours = (float)myreader.GetDouble(8),
                            Total_AC_Power = (float)myreader.GetDouble(9),
                            Daily_Energy = (float)myreader.GetDouble(10),
                            ControllerName = myreader.GetString(11)
                        };
                        Data = solar;
                    }
                    myreader.Close();
                }

                myCon.Close();
            }
            return new JsonResult(Data);
        }

        // GET: api/SolarData/GetByIDFromTo/{IDFrom}/{IDTo}
        [HttpGet("GetByIDFromTo/{IDFrom}/{IDTo}")]
        public JsonResult GetByIDFromTo(int IDFrom, int IDTo)
        {
            List<SolarData> Data = new List<SolarData>();

            string query = @"SELECT ID, Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName FROM dbo.inverter_record WHERE ID >=" + IDFrom + " AND ID <=" + IDTo;


            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myreader = myCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        SolarData solar = new SolarData()
                        {
                            ID = (int)myreader.GetInt32(0),
                            Time = myreader.GetDateTime(1),
                            Temperature = (float)myreader.GetDouble(2),
                            PV1_Voltage = (float)myreader.GetDouble(3),
                            PV2_Voltage = (float)myreader.GetDouble(4),
                            PV1_Current = (float)myreader.GetDouble(5),
                            PV2_Current = (float)myreader.GetDouble(6),
                            Total_Energy = (float)myreader.GetDouble(7),
                            Total_Operation_Hours = (float)myreader.GetDouble(8),
                            Total_AC_Power = (float)myreader.GetDouble(9),
                            Daily_Energy = (float)myreader.GetDouble(10),
                            ControllerName = myreader.GetString(11)
                        };
                        Data.Add(solar);
                    }
                    myreader.Close();
                }

                myCon.Close();
            }
            return new JsonResult(Data);
        }

        // GET: api/SolarData/GetByDateFromTo/{DateFrom}/{DateTo}
        [HttpGet("GetByDateFromTo/{DateFrom}/{DateTo}")]
        public JsonResult GetByDateFromTo(DateTime startDate, DateTime endDate)
        {
            List<SolarData> Data = new List<SolarData>();

            string query = @"SELECT ID, Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName FROM dbo.inverter_record WHERE Time BETWEEN" + startDate + " AND" + endDate;


            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myreader = myCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        SolarData solar = new SolarData()
                        {
                            ID = (int)myreader.GetInt32(0),
                            Time = myreader.GetDateTime(1),
                            Temperature = (float)myreader.GetDouble(2),
                            PV1_Voltage = (float)myreader.GetDouble(3),
                            PV2_Voltage = (float)myreader.GetDouble(4),
                            PV1_Current = (float)myreader.GetDouble(5),
                            PV2_Current = (float)myreader.GetDouble(6),
                            Total_Energy = (float)myreader.GetDouble(7),
                            Total_Operation_Hours = (float)myreader.GetDouble(8),
                            Total_AC_Power = (float)myreader.GetDouble(9),
                            Daily_Energy = (float)myreader.GetDouble(10),
                            ControllerName = myreader.GetString(11)
                        };
                        Data.Add(solar);
                    }
                    myreader.Close();
                }

                myCon.Close();
            }
            return new JsonResult(Data);
        }

        // GET: api/SolarData/InsertDataFromFile/{filename}
        [HttpGet("InsertDataFromFile/{filename}")]
        public JsonResult ReadCsvAndPutData(string filename)
        {
            //ControllerName T15133M013

            string[] files = Directory.GetFiles("F:\\Repos\\K299-Back\\K299-Back\\csv", filename + @".csv", SearchOption.AllDirectories);

            List<SolarData> Data = new List<SolarData>();

            foreach (var file in files)
            {
                string[] lines = System.IO.File.ReadAllLines(file);

                foreach (string line in lines)
                {
                    if (line != lines.First())
                    {
                        string[] columns = line.Split(',');

                        var solar = new SolarData()
                        {
                            Time                  = DateTime.Parse(columns[0]),
                            Temperature           = float.Parse(columns[1]),
                            PV1_Voltage           = float.Parse(columns[2]),
                            PV2_Voltage           = float.Parse(columns[3]),
                            PV1_Current           = float.Parse(columns[4]),
                            PV2_Current           = float.Parse(columns[5]),
                            Total_Energy          = float.Parse(columns[6]),
                            Total_Operation_Hours = float.Parse(columns[7]),
                            Total_AC_Power        = float.Parse(columns[8]),
                            Daily_Energy          = float.Parse(columns[10]),
                            ControllerName        = (string)filename
                        };

                        Data.Add(solar);

                    }
                }
            }

            //Put(Data);

            return new JsonResult(Data);
        }
        [HttpPut("InsertAllFiles")]
        public void Put(List<SolarData> Data)
        {

            string query = @"INSERT INTO dbo.Inverter_record
                            (Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName)
                            VALUES (@Time, @Temperature, @PV1_Voltage, @PV2_Voltage, @PV1_Current,
                                    @PV2_Current, @Total_Energy, @Total_Operation_Hours, @Total_AC_Power,
                                    @Daily_Energy, @ControllerName)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                foreach (var dat in Data)
                {

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Time",                  dat.Time);
                        myCommand.Parameters.AddWithValue("@Temperature",           dat.Temperature);
                        myCommand.Parameters.AddWithValue("@PV1_Voltage",           dat.PV1_Voltage);
                        myCommand.Parameters.AddWithValue("@PV2_Voltage",           dat.PV2_Voltage);
                        myCommand.Parameters.AddWithValue("@PV1_Current",           dat.PV1_Current);
                        myCommand.Parameters.AddWithValue("@PV2_Current",           dat.PV2_Current);
                        myCommand.Parameters.AddWithValue("@Total_Energy",          dat.Total_Energy);
                        myCommand.Parameters.AddWithValue("@Total_Operation_Hours", dat.Total_Operation_Hours);
                        myCommand.Parameters.AddWithValue("@Total_AC_Power",        dat.Total_AC_Power);
                        myCommand.Parameters.AddWithValue("@Daily_Energy",          dat.Daily_Energy);
                        myCommand.Parameters.AddWithValue("@ControllerName",        dat.ControllerName);

                        myreader = myCommand.ExecuteReader();
                        table.Load(myreader);
                        myreader.Close();

                    }
                }

                myCon.Close();

            }
        }

        //Put: api/SolarData/IsnertOne
        [HttpPut("InsertOne")]
        public void InsertOne([FromBody] SolarData Data)
        {

            string query = @"INSERT INTO dbo.Inverter_record
                            (Time, Temperature, PV1_Voltage, PV2_Voltage, PV1_Current,
                                    PV2_Current, Total_Energy, Total_Operation_Hours, Total_AC_Power,
                                   Daily_Energy, ControllerName)
                            VALUES (@Time, @Temperature, @PV1_Voltage, @PV2_Voltage, @PV1_Current,
                                    @PV2_Current, @Total_Energy, @Total_Operation_Hours, @Total_AC_Power,
                                    @Daily_Energy, @ControllerName)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SolarData");

            SqlDataReader myreader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Time", Data.Time);
                    myCommand.Parameters.AddWithValue("@Temperature", Data.Temperature);
                    myCommand.Parameters.AddWithValue("@PV1_Voltage", Data.PV1_Voltage);
                    myCommand.Parameters.AddWithValue("@PV2_Voltage", Data.PV2_Voltage);
                    myCommand.Parameters.AddWithValue("@PV1_Current", Data.PV1_Current);
                    myCommand.Parameters.AddWithValue("@PV2_Current", Data.PV2_Current);
                    myCommand.Parameters.AddWithValue("@Total_Energy", Data.Total_Energy);
                    myCommand.Parameters.AddWithValue("@Total_Operation_Hours", Data.Total_Operation_Hours);
                    myCommand.Parameters.AddWithValue("@Total_AC_Power", Data.Total_AC_Power);
                    myCommand.Parameters.AddWithValue("@Daily_Energy", Data.Daily_Energy);
                    myCommand.Parameters.AddWithValue("@ControllerName", Data.ControllerName);

                    myreader = myCommand.ExecuteReader();
                    table.Load(myreader);
                    myreader.Close();
                    myCon.Close();

                }

            }
        }

    }
}

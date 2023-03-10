using System.ComponentModel.DataAnnotations;

namespace K299_Back.Model
{
    public class SolarData
    {
        public int ID { get; set; }

        public string? ControllerName { get; set; }
        public DateTime Time { get; set; }

        public float Temperature { get; set; }

        public float PV1_Voltage { get; set; }

        public float PV2_Voltage { get; set; }

        public float PV1_Current { get; set; }

        public float PV2_Current { get; set; }

        public float Total_Energy { get; set; }

        public float Total_Operation_Hours { get; set; }

        public float Total_AC_Power { get; set; }

        public float Daily_Energy { get; set; }

    }
}

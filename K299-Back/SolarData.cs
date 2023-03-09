using System.Text.Json.Serialization;
namespace K299_Back
{
	public class SolarData
	{
        public DateTime time { get; set; }
        public int temperature { get; set; }
		public float pv1V { get; set; }
        public float pv2V { get; set; }
        public float pv1A { get; set; }
        public float pv2A { get; set; }
        public float totalEnergy { get; set; }
        public int totalTime { get; set; }
        public int totalPower { get; set; }
        public string? mode { get; set; }
        public float dailyEnergy { get; set; }
        //public int pv1Input { get; set; }
        //public int pv2Input { get; set; }
        public int heatsinkTemp { get; set; }
        //public float currentR { get; set; }
        //public float voltageR { get; set; }
        //public float frequencyR { get; set; }
        //public float currentS { get; set; }
        //public float voltageS { get; set; }
        //public float frequencyS { get; set; }
        //public float currentT { get; set; }
        //public float voltageT { get; set; }
        //public float frequencyT { get; set; }
        //public float reducedCO2 { get; set; }
        //public float reducedSO2 { get; set; }
        //public float reducedOIL { get; set; }
        //public float reducedCOAL { get; set; }
        

    }
}


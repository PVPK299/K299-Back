namespace K299_Back.Model
{
    public class NewUser
    {
        public Guid ID { get; set; }
        public string? email { get; set; }

        public string? password { get; set; }

        public string? first_name { get; set; }

        public string? last_name { get; set; }

        public double park_share { get; set; }
    }
}
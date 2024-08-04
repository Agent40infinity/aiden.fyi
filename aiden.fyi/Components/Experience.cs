namespace aiden.fyi.Components
{
    public class Experience
    {
        public string project { get; set; }
        public string location { get; set; }
        public string date { get; set; }
        public string logo { get; set; }
        public string description { get; set; }
        public bool columns { get; set; } = true;
        public bool selected { get; set; } = false;
        public List<string> details { get; set; } = new List<string>();
    }
}

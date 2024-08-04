namespace aiden.fyi.Components
{
    public class ExperienceCategory
    {
        public string name { get; set; }
        public string icon { get; set; }    
        public List<Experience> list { get; set; } = new List<Experience>();
    }
}

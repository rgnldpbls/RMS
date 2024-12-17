namespace RemcSys.Models
{
    public class Settings
    {
        public string Id { get; set; }
        public bool isMaintenance { get; set; }
        public bool isUFRApplication { get; set; }
        public bool isEFRApplication { get; set; }
        public bool isUFRLApplication { get; set; }
        public int evaluatorNum { get; set; }
        public int daysEvaluation { get; set; }
        public string tuklasInvolvement { get; set; }
        public List<string>? tuklasFieldOfStudy { get; set; }
        public string lathalaInvolvement { get; set; }
        public List<string>? lathalaFieldOfStudy { get; set; }
    }
}

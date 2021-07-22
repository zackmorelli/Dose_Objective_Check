using System.Collections.Generic;

namespace DoseObjectiveCheck
{
    public class ReportData
    {
        public Patient Patient { get; set; }
        public Hospital Hospital { get; set; }
        public Plan Plan { get; set; }
        public Plansum Plansum { get; set; }
        public string User { get; set; }
        public bool SRSstats { get; set; }
        public bool Convstats { get; set; }
        public SRScoveragestats srsstats {get; set;}
        public ConventionalCoverageStats conventionalstats { get; set; }
        public List<DoseObjective> PROI { get; set; }



    }
}
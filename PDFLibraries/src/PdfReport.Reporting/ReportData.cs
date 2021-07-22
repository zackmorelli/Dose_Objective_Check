using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;



namespace PdfReport.Reporting
{
    public class ReportData
    {
        public Patient Patient { get; set; }
        public StructureSet StructureSet { get; set; }
        public Hospital Hospital { get; set; }
        public Plan Plan { get; set; }
        public Plansum Plansum { get; set; }
        public string User { get; set; }
        public bool SRSstats { get; set; }
        public bool Convstats { get; set; }
        public SRScoveragestats srsstats {get; set;}
        public ConventionalCoverageStats conventionalstats { get; set; }
        public List<ROI.ROI> PROI { get; set; }



    }
}
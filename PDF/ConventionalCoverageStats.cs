using System.Collections.Generic;

namespace DoseObjectiveCheck
{ 
    public class ConventionalCoverageStats
    {
        public List<TargetStructure> TargetStructures { get; set; }
        public bool DosePainted { get; set; }
        public bool Simple { get; set; }
        public bool Manual { get; set; }
        public bool Sequential { get; set; }
        public double CTVRXcoverage { get; set; }   
        public double CTVVolcoverage { get; set; }
        public double PTV1RXcoverage { get; set; }
        public double PTV1Volcoverage { get; set; }
        public double PTV2RXcoverage { get; set; }
        public double PTV2Volcoverage { get; set; }
        public double PTV3RXcoverage { get; set; }
        public double PTV3Volcoverage { get; set; }


        public ConventionalCoverageStats()   //default constructor that specifically instantiates the Targets List. 
        {
            TargetStructures = new List<TargetStructure>();
        }

    }

    public class TargetStructure
    {
        public string StructureNAME { get; set; }
        public int DosePaintedTargetNumber { get; set; }
        public double Dose { get; set; }   //entered in by user, in cGy
        public string plan { get; set; }   // if applicable
        public double vol { get; set; }
        public double CTVXXRX0 { get; set; }   //percent of the volume of the PTV contained within the coverage requirment, or 100% isodose line.
        public double PTVXXRX1 { get; set; }   //percent of the volume of the PTV contained within the coverage requirment, or 100% isodose line.
        public double PTVXXRX2 { get; set; }   //percent of the volume of the PTV contained within the coverage requirment, or 100% isodose line.
        public double PTVXXRX3 { get; set; }   //percent of the volume of the PTV contained within the coverage requirment, or 100% isodose line.
        public double GlobalMaxPointDose { get; set; }  //of the plan/plansum of this target

    }
}
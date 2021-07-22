using System.Collections.Generic;

namespace DoseObjectiveCheck
{
    public class SRScoveragestats
    {
        public double CoverageReqRX { get; set; }
        public double CoverageReqVol { get; set; }
        public List<SRSTargetstats> Targets { get; set; }


        public SRScoveragestats()   //default constructor that specifically instantiates the Targets List. 
        {
            Targets = new List<SRSTargetstats>();
        }


    }


    public class SRSTargetstats
    {
        public string TargetNAME { get; set; }

        public double Targetvol { get; set; }

        public double BodyV100 { get; set; }

        public double BodyV50 { get; set; }

        public double BodyPTV_20_Dose { get; set; }    //This is the max dose (to 0.03 cc) of the Body-PTV_20 structure, if present

        public double Dose { get; set; }

        public double PTVXXRX { get; set; }   //percent of the volume of the PTV contained within the coverage requirment, or 100% isodose line.

        public bool DistanceWarning { get; set; }

        public SRSTargetstats() //default constructor
        {
            DistanceWarning = false;
        }
    }
}
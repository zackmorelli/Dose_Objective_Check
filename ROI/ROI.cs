using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    Lahey RadOnc Dose Objective Checker - ROI
    Copyright (c) 2019 Radiation Oncology Department, Lahey Hospital and Medical Center
    Written by: Zackary T Morelli

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program also requires .NET Framework 4.5.0 to run properly.

    This is the source code for a .NET Framework assembly file, however this functions as an executable file in Eclipse.
    In addition to Varian's APIs and .NET Framework, this program uses the following commonly available libraries:
    MigraDoc
    PdfSharp

    Release 2.1 - 11/19/2019

    Description:
    This is an internal helper class of the Dose Objective Check program. This defines the ROI class, which is a custom class which is used to store all of the information of individual dose objectives.
    This is crucual to the proper and efficient functioning of the overall program.

*/



namespace ROI
{
    public class ROI : IEquatable<ROI>
    {
        public string ROIName { get; set; }

        public int ROIId { get; set; }

        public string Rstruct { get; set; }    // structure id in eclipse

        public double structvol { get; set; }

        public string limit { get; set; }       // like "V80 or max point dose"

        public string limval { get; set; }        // like "25 cGy"

        public string strict { get; set; }

        public string limunit { get; set; }     // cGy, %

        public string status { get; set; }

        public string goal { get; set; }

        public double limdose { get; set; }    // the upper limit of the dose in cGy for this specific ROI, calculated from predcribed dose of plan

        public double actdose { get; set; }    // actual dose from eclipse

        public string[] treatsite { get; set; }

        // these last 3 variables are specifically used with the "V" type dose objectives, which compare the volumes enclosed by specific Isodose lines
        public double limvol { get; set; }
        public double goalvol { get; set; }
        public double actvol { get; set; }
        public string type { get; set; }      // differentiates between absolute and realative V type limits
        public bool applystatus { get; set; }
        public override string ToString()
        {
            return "ID: " + ROIId + "   Name: " + ROIName + "   Limit:" + limit + "   Strictness: " + strict + "   Limit Value: " + limval + "   Limit Unit: " + limunit;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ROI ROI = obj as ROI;
            if (ROI == null) return false;
            else return Equals(ROI);
        }
        public override int GetHashCode()
        {
            return ROIId;
        }
        public bool Equals(ROI other)
        {
            if (other == null) return false;
            return (this.ROIId.Equals(other.ROIId));
        }
        public void Clear()
        {
            ROIName = null;
            ROIId = 0;
            limit = null;
            limval = null;
            strict = null;
            limunit = null;
            status = null;
            goal = null;
            treatsite = null;
        }



    }

}           // Should also override == and != operators.

     

























    

using System;

/*
    Dose Objective Check - Dose Objective Class

    Description:
    This is an internal helper class of the Dose Objective Check program. This defines the Dose Objective class, which is a custom class which is used to store all of the information of individual dose objectives.
    This is crucual to the proper and efficient functioning of the overall program.

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program runs on .NET Framework 4.6.1. It also uses MigraDoc and PDFSharp for the PDF generation, commonly available libraries which can be found on NuGet

    Copyright (C) 2021 Zackary Thomas Ricci Morelli
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    I can be contacted at: zackmorelli@gmail.com


    Release 3.2 - 6/8/2021

*/


namespace DoseObjectiveCheck
{
    public class DoseObjective : IEquatable<DoseObjective>
    {
        public string DoseObjectiveName { get; set; }

        public int DoseObjectiveID { get; set; }

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
            return "ID: " + DoseObjectiveID + "   Name: " + DoseObjectiveName + "   Limit:" + limit + "   Strictness: " + strict + "   Limit Value: " + limval + "   Limit Unit: " + limunit;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DoseObjective ROI = obj as DoseObjective;
            if (ROI == null) return false;
            else return Equals(ROI);
        }
        public override int GetHashCode()
        {
            return DoseObjectiveID;
        }
        public bool Equals(DoseObjective other)
        {
            if (other == null) return false;
            return (this.DoseObjectiveName.Equals(other.DoseObjectiveName));
        }
        public void Clear()
        {
            DoseObjectiveName = null;
            DoseObjectiveID = 0;
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

     

























    

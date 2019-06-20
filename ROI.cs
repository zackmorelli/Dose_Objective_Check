using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Report_Script
{
    public class ROI : IEquatable<ROI>
    {
        public string ROIName { get; set; }

        public int ROIId { get; set; }

        public string Rstruct { get; set; }    // structure id in eclipse

        public double structvol { get; set; }

        public string limit { get; set; }       // like "V80 or max point dose"

        public int limval { get; set; }        // like "25 cGy"

        public string strict { get; set; }

        public string limunit { get; set; }     // cGy, %

        public string status { get; set; }

        public string goal { get; set; }

        public double limdose { get; set; }    // the upper limit of the dose in cGy for this specific ROI, calculated from predcribed dose of plan

        public double actdose { get; set; }    // actual dose from eclipse

        public string[] treatsite { get; set; }

        public override string ToString()
        {
            return "ID: " + ROIId + "   Name: " + ROIName + "   Limit:" + limit + "   Strictness: " + strict + "   Limit Value: " + limval  + "   Limit Unit: " + limunit ;
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
            limval = 0;
            strict = null;
            limunit = null;
            status = null;
            goal = null;
            treatsite = null;
        }



    }
}          
            // Should also override == and != operators.

     

























    

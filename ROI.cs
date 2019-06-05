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

            public string limit { get; set; }

            public int limval { get; set; } 

            public string strict { get; set; }

            public string limunit { get; set; }

            public string status { get; set; }

            public override string ToString()
            {
                return "ID: " + ROIId + "   Name: " + ROIName + "   Current Plan Status: " + status ;
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
            // Should also override == and != operators.

     }

























    
}

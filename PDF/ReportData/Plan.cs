using System;

namespace DoseObjectiveCheck
{
    public class Plan
    {
        public string Id { get; set; }
        public string Course { get; set; }
        public string TreatmentSite { get; set; }           // one of the departments's standardized Treatment Sites (like Head and Neck, Gynecological, Breast23+fx, etc.)
        public string ApprovalStatus { get; set; }

         
        public Nullable<DateTime> CreationDateTime { get; set; }

        public string CreationUser { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string LastModifiedUser { get; set; }
        
        public double TotalPrescribedDose { get; set; }
    }
}
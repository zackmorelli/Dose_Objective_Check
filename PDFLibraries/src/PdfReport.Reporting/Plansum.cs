using System;

namespace PdfReport.Reporting
{
    public class Plansum
    {
        public string Id { get; set; }
        public string Course { get; set; }
        public string[] TreatmentSites { get; set; }         // A collection of the department's standarized treatment sites in this plansum.
        public int Fractions { get; set; }
        public string[] Energies { get; set; }
        public Beam[] Beams { get; set; }
        public string ApprovalStatus { get; set; }

        public Nullable<DateTime> CreationDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string LastModifiedUser { get; set; }

        public double TotalPrescribedDose { get; set; }
    }
}
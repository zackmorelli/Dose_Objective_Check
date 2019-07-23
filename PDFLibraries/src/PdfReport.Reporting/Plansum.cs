namespace PdfReport.Reporting
{
    public class Plansum
    {
        public string Id { get; set; }
        public string Course { get; set; }
        public string Type { get; set; }
        public int Fractions { get; set; }
        public string[] Energies { get; set; }
        public Beam[] Beams { get; set; }
        public string ApprovalStatus { get; set; }
        
        public double TotalPrescribedDose { get; set; }
    }
}
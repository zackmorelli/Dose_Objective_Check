namespace DoseObjectiveCheck
{
    public interface IReport
    {
        void Export(string path, ReportData data);
    }
}

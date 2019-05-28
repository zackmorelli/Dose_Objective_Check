using System.Windows;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfReport.Reporting.MigraDoc.Internal;

namespace PdfReport.Reporting.MigraDoc
{
    public class ReportPdf : IReport
    {
        public void Export(string path, ReportData reportdata)
        {
           // MessageBox.Show("Trigger export");
            ExportPdf(path, CreateReport(reportdata));
        }

        private void ExportPdf(string path, Document report)
        {
            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = report;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(path);
           // MessageBox.Show("Trigger exportpdf");
        }

        private Document CreateReport(ReportData reportdata)
        {
            var doc = new Document();
            CustomStyles.Define(doc);
            doc.Add(CreateMainSection(reportdata));
           // MessageBox.Show("Trigger create report");
            return doc;

        }

        private Section CreateMainSection(ReportData reportdata)
        {
            var section = new Section();
            SetUpPage(section);
            AddHeaderAndFooter(section, reportdata);
            // AddContents(section, data);
           // MessageBox.Show("Trigger main section");
            return section;
        }

        private void SetUpPage(Section section)
        {
            section.PageSetup.PageFormat = PageFormat.Letter;

            section.PageSetup.LeftMargin = Size.LeftRightPageMargin;
            section.PageSetup.TopMargin = Size.TopBottomPageMargin;
            section.PageSetup.RightMargin = Size.LeftRightPageMargin;
            section.PageSetup.BottomMargin = Size.TopBottomPageMargin;

            section.PageSetup.HeaderDistance = Size.HeaderFooterMargin;
            section.PageSetup.FooterDistance = Size.HeaderFooterMargin;
        }

        private void AddHeaderAndFooter(Section section, ReportData reportdata)
        {

          //  MessageBox.Show("Trigger HF");
            new HeaderAndFooter().Add(section, reportdata);      
           // MessageBox.Show("Trigger HF2");
        }

        /*
        private void AddContents(Section section, ReportData data)
        {
            AddPatientInfo(section, data.Patient);
            AddStructureSet(section, data.StructureSet);
        }

        private void AddPatientInfo(Section section, Patient patient)
        {
            new PatientInfo().Add(section, patient);
        }

        private void AddStructureSet(Section section, StructureSet structureSet)
        {
            new StructureSetContent().Add(section, structureSet);
        }

        */

    }
}

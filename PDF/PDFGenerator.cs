using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;



/*
    Lahey RadOnc Dose Objective Checker - PdfReport
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
    PdfReport, and the other files in the Pdf.Reporting.MigraDoc Visual Studio project (as well as the Pdf.Reporting project, which doesn't have a "main" file to put a description like this in), are an internal helper class of the Dose Objective Check program that are involved in the generation of the PDF report.
    All of the code in the Pdf.Reporting namespace was originally developed by Carlos J Anderson and was obtained from him via his website. It was significantly modified by Zackary T Morelli for use with the Dose Objective Check program. 

*/



namespace DoseObjectiveCheck
{
    public class PDFGenerator : IReport
    {
        public void Export(string path, ReportData reportdata)
        {
          //  MessageBox.Show("Trigger export");
            ExportPdf(path, CreateReport(reportdata));
        }

        private void ExportPdf(string path, Document report)
        {
            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = report;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(path);
          //  MessageBox.Show("Trigger exportpdf");
        }

        private Document CreateReport(ReportData reportdata)
        {
            var doc = new Document();
            CustomStyles.Define(doc);
            doc.Add(CreateMainSection(reportdata));
         //   doc.Add(CreateMainSection2(reportdata));
          //  MessageBox.Show("Trigger create report");
            return doc;

        }

        private Section CreateMainSection(ReportData reportdata)
        {
            var section = new Section();
            SetUpPage(section);
            AddHeaderAndFooter(section, reportdata);
            AddFirstPageStuff(section, reportdata);

            //We want the Target Coverage to show up first, if it was run
            if (reportdata.SRSstats == true)
            {
                AddSRSstats(section, reportdata);
            }
            if (reportdata.Convstats == true)
            {
                AddConvstats(section, reportdata);
            }
            AddDoseObjectiveList(section, reportdata);

            //  MessageBox.Show("Trigger main section");
            return section;
        }

     //   private Section CreateMainSection2(ReportData reportdata)
     //   {
      //      var section = new Section();
       //     SetUpPage(section);
        //    AddHeaderAndFooter(section, reportdata);
        //    AddVolDoseObjectiveList(section, reportdata);
          //  MessageBox.Show("Trigger main section2");
         //   return section;
       // }

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
           // MessageBox.Show("Trigger HF");
            new HeaderAndFooter().Add(section, reportdata);      
          //  MessageBox.Show("Trigger HF2");
        }

        private void AddFirstPageStuff(Section section, ReportData data)
        {
            new FirstPageStuff().Add(section, data);
        }

        private void AddDoseObjectiveList(Section section, ReportData data)
        {

           // MessageBox.Show("Trigger Dose objective start");
            new DoseObjectiveList().Add(section, data);
         //   MessageBox.Show("Trigger Dose Objective end");
        }

        private void AddSRSstats(Section section, ReportData data)
        {
            // MessageBox.Show("Trigger Dose objective start");
            new SRSstats().Add(section, data);
            //   MessageBox.Show("Trigger Dose Objective end");
        }

        private void AddConvstats(Section section, ReportData data)
        {
            // MessageBox.Show("Trigger Dose objective start");
            new ConvStats().Add(section, data);
            //   MessageBox.Show("Trigger Dose Objective end");
        }


        //   private void AddVolDoseObjectiveList(Section section, ReportData data)
        //   {

        //  MessageBox.Show("Trigger Vol Dose objective start");
        //       new VolDoseObjectiveList().Add(section, data);
        //  MessageBox.Show("Trigger Vol Dose Objective end");
        //  }




    }
}

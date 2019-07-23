using System;
using System.Windows;
using MigraDoc.DocumentObjectModel;


namespace PdfReport.Reporting.MigraDoc.Internal
{
    internal class HeaderAndFooter
    {
        public void Add(Section section, ReportData data)
        {
           // MessageBox.Show("Trigger main Header ADD1");
            AddHeader(section, data);    
            AddFooter(section);
           // MessageBox.Show("Trigger main Header ADD2");
        }

        private string Format(DateTime birthdate)
        {
            return $"{birthdate:d} (age {Age(birthdate)})";
        }

        // See http://stackoverflow.com/a/1404/1383366
        private int Age(DateTime birthdate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthdate.Year;
            return birthdate.AddYears(age) > today ? age - 1 : age;
        }

        private void AddHeader(Section section, ReportData data)
        {
            var header = section.Headers.Primary.AddParagraph();
            header.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

            /*
            MessageBox.Show(data.Patient.LastName);
            MessageBox.Show(data.Patient.FirstName);
            MessageBox.Show(data.Plan.Id);
            MessageBox.Show(data.Patient.Doctor.Name);
            MessageBox.Show(data.Hospital.Address);
            MessageBox.Show(data.Plan.ApprovalStatus);
            */


            header.AddFormattedText($"{data.Patient.LastName}, {data.Patient.FirstName} (ID: {data.Patient.Id}) - {data.Patient.Sex} " , TextFormat.Bold);
            header.AddSpace(60);
            header.AddText($"Generated {DateTime.Now:g}  By: {data.User}");           //first line  ( g is the General Date Short Time Format Specifier)
            header.AddLineBreak();
            
            header.AddText("Birthdate: ");
            header.AddFormattedText(Format(data.Patient.Birthdate), TextFormat.Bold );    // second line
            header.AddTab();
            header.AddText("Primary Oncologist: ");
            header.AddFormattedText($"{data.Patient.Doctor.Name} ", TextFormat.Bold);
            header.AddLineBreak();

            header.AddText("Plan: ");

            if (data.Plansum == null)
            {
                header.AddFormattedText($"{data.Plan.Id} - {data.Plan.ApprovalStatus}", TextFormat.Bold);    // third line
            }
            else
            {
                header.AddFormattedText($"{data.Plansum.Id} - {data.Plansum.ApprovalStatus}", TextFormat.Bold);    // third line
            }
            header.AddTab();
            header.AddText($"{data.Hospital.Name}, {data.Hospital.Address} ");
            header.AddLineBreak();
            header.AddSpace(60);
        }

       
        private void AddFooter(Section section)
        {
            var footer = section.Footers.Primary.AddParagraph();
            footer.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

            footer.AddTab();
            footer.AddText("Page ");
            footer.AddPageField();
            footer.AddText(" of ");
            footer.AddNumPagesField();
        }

       











    }




}
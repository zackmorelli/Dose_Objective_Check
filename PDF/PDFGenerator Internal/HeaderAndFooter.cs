using System;
using System.Windows;
using MigraDoc.DocumentObjectModel;


namespace DoseObjectiveCheck
{
    internal class HeaderAndFooter
    {
        public void Add(Section section, ReportData data)
        {
           // MessageBox.Show("Trigger main Header ADD1");
            AddHeader(section, data);    
            AddFooter(section, data);
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

            header.AddText($"Generated {DateTime.Now:g}  By: {data.User}");           //first line  ( g is the General Date Short Time Format Specifier)
          
            header.AddTab();
            header.AddText($"{data.Hospital.Name}, {data.Hospital.Address} ");

            header.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };
        }

       
        private void AddFooter(Section section, ReportData data)
        {
            var footer = section.Footers.Primary.AddParagraph();
            footer.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);
            footer.Format.Borders.Top = new Border() { Width = "1pt", Color = Colors.DarkGray };

            footer.AddFormattedText($"{data.Patient.LastName}, {data.Patient.FirstName} (ID: {data.Patient.Id}) - {data.Patient.Sex} ", TextFormat.Bold);

            footer.AddLineBreak();
            if (data.Plansum == null)
            {
                footer.AddText("Course: " + data.Plan.Course);
            }
            else
            {
                footer.AddText("Course: " + data.Plansum.Course);
            }

            footer.AddLineBreak();
            if (data.Plansum == null)
            {
                footer.AddText("Plan: " + data.Plan.Id + " - " + data.Plan.ApprovalStatus);    
            }
            else
            {
                if (data.Plansum.ApprovalStatus == "")
                {
                    footer.AddText("Plansum: " + data.Plansum.Id);
                }
                else
                {
                    footer.AddText("Plansum: " + data.Plansum.Id + " - " + data.Plansum.ApprovalStatus);
                }
            }

            footer.AddTab();
            footer.AddText("Page ");
            footer.AddPageField();
            footer.AddText(" of ");
            footer.AddNumPagesField();

            footer.AddLineBreak();
            if (data.Plansum == null)
            {
                footer.AddText("Treatment Site: " + data.Plan.TreatmentSite);
            }
            else
            {
                footer.AddText("Treatment Site(s): " + data.Plansum.TreatmentSites[0]);
            }

            footer.AddLineBreak();
            if (data.Plansum == null)
            {
                footer.AddText("Plan Created: ");
                footer.AddText(data.Plan.CreationDateTime.ToString() + " By " + data.Plan.CreationUser.ToString());    
            }
            else
            {
                footer.AddText("Plansum Created: ");
                footer.AddText(data.Plansum.CreationDateTime.ToString());    
            }

            footer.AddLineBreak();
            
            if (data.Plansum == null)
            {
                footer.AddText("Plan Last Modified: ");
                footer.AddText(data.Plan.LastModifiedDateTime.ToString() + " By " + data.Plan.LastModifiedUser.ToString());
            }
            else
            {
                footer.AddText("Plansum Last Modified: ");
                footer.AddText(data.Plansum.LastModifiedDateTime.ToString() + " By " + data.Plansum.LastModifiedUser.ToString());
            }


        }
    }
}
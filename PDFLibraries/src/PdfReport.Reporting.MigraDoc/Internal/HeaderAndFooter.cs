using System;
using MigraDoc.DocumentObjectModel;

namespace PdfReport.Reporting.MigraDoc.Internal
{
    internal class HeaderAndFooter
    {
        public void Add(Section section, ReportData data)
        {
            AddHeader(section, data.Patient, data.Hospital, data.Plan);
            AddFooter(section);
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

        private void AddHeader(Section section, Patient patient, Hospital hospital, Plan plan)
        {
            var header = section.Headers.Primary.AddParagraph();
            header.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

            header.AddFormattedText($"{patient.LastName}, {patient.FirstName} (ID: {patient.Id}) - {patient.Sex} \n" , TextFormat.Bold);
            header.AddText("Birthdate: ");
            header.AddFormattedText(Format(patient.Birthdate), TextFormat.Bold );
            header.AddText("\n");
            header.AddText("Plan: ");
            header.AddFormattedText($"{plan.Id} - {plan.ApprovalStatus}", TextFormat.Bold);
            header.AddTab();
            header.AddText($"Generated {DateTime.Now:g}  By: \n");
            header.AddText("Primary Oncologist: ");
            header.AddFormattedText($"{patient.Doctor.Name} ", TextFormat.Bold);
            header.AddText("\n");
            header.AddText($"{hospital.Name}, {hospital.Address} ");


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
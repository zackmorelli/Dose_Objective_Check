using System;
using MigraDoc.DocumentObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfReport.Reporting.MigraDoc.Internal
{
    internal class FirstPageStuff
    {
        public void Add(Section section, ReportData data)
        {
            Paragraph stuff = new Paragraph();
            stuff.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

            stuff.AddFormattedText($"{data.Patient.LastName}, {data.Patient.FirstName} (ID: {data.Patient.Id}) - {data.Patient.Sex} ", TextFormat.Bold);

            stuff.AddTab();

            if (data.Plansum == null)
            {
                stuff.AddText("Plan: ");
                stuff.AddFormattedText(data.Plan.Id + " - " + data.Plan.ApprovalStatus, TextFormat.Bold);
            }
            else
            {
                stuff.AddText("Plansum: ");

                if (data.Plansum.ApprovalStatus == "")
                {
                    stuff.AddFormattedText(data.Plansum.Id);
                }
                else
                {
                    stuff.AddFormattedText(data.Plansum.Id + " - " + data.Plansum.ApprovalStatus, TextFormat.Bold);
                }
            }

            stuff.AddLineBreak();
            stuff.AddText("Primary Oncologist: " + data.Patient.Doctor.Name);

            section.Add(stuff);
        }

    }
}









using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using PdfReport.Reporting;
using PdfReport.Reporting.MigraDoc;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using ROI;

namespace PdfReport.PDFGenerator
{
     public class Program
    {
        public static void PlanMain(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output)
        {

           // MessageBox.Show("Trigger plan main start");

            var reportService = new PdfReport.Reporting.MigraDoc.ReportPdf();
            var reportData = CreateReportDataPlan(patient, course, plan, image3D, structureSet, user, output);
          //  MessageBox.Show("Trigger main middle plan");
            var path = GetTempPdfPath();
           // MessageBox.Show(path);
            reportService.Export(path, reportData);
          //  MessageBox.Show("Trigger after export plan");

            Process.Start(path);
           // MessageBox.Show("Trigger main end");
        }


        public static void PlansumMain(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output)
        {

            // MessageBox.Show("Trigger plansum main start");

            var reportService = new PdfReport.Reporting.MigraDoc.ReportPdf();
            var reportData = CreateReportDataPlansum(patient, course, plansum, image3D, structureSet, user, output);
            // MessageBox.Show("Trigger main middle");
            var path = GetTempPdfPath();
            // MessageBox.Show(path);
            reportService.Export(path, reportData);
           //  MessageBox.Show("Trigger after export plansum");

            Process.Start(path);
            // MessageBox.Show("Trigger main end");
        }


        private static ReportData CreateReportDataPlan(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output)
        {

          //  MessageBox.Show("Trigger report data plan");

            // some variables used to help convert between Varian stuff and the classes for the pdf

            DateTime DOB = (DateTime)patient.DateOfBirth;  //casts nullable DateTime varriable from Varian's API to a normal one


            return new ReportData
            {

                User = user.Name,

                Patient = new PdfReport.Reporting.Patient
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    Sex = patient.Sex,
                    Birthdate = DOB,

                    Doctor = new PdfReport.Reporting.Doctor
                    {
                        Name = patient.PrimaryOncologistId
                    }

                },

                Hospital = new PdfReport.Reporting.Hospital
                {
                    Name = patient.Hospital.Name,
                    Address = patient.Hospital.Location
                },

                Plan = new PdfReport.Reporting.Plan
                {

                    Id = plan.Id,
                    Course = course.Id,
                    ApprovalStatus = plan.ApprovalStatus.ToString(),
                    TotalPrescribedDose = plan.TotalPrescribedDose.Dose

                    //  Type = Enum.GetName(typeof(PlanType),

                },

                PROI = output,

            };
        }


        private static ReportData CreateReportDataPlansum(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output)
        {

           //  MessageBox.Show("Trigger report data sum");

            // some variables used to help convert between Varian stuff and the classes for the pdf

            DateTime DOB = (DateTime)patient.DateOfBirth;  //casts nullable DateTime varriable from Varian's API to a normal one

            double dosesum = 0.0;
            string dunit = null;

            foreach (PlanSetup aplan in plansum.PlanSetups)
            {
                dosesum += aplan.TotalPrescribedDose.Dose;
                dunit = aplan.TotalPrescribedDose.UnitAsString;
            }

            return new ReportData
            {

                User = user.Name,

                Patient = new PdfReport.Reporting.Patient
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    Sex = patient.Sex,
                    Birthdate = DOB,

                    Doctor = new PdfReport.Reporting.Doctor
                    {
                        Name = patient.PrimaryOncologistId
                    }

                },

                Hospital = new PdfReport.Reporting.Hospital
                {
                    Name = patient.Hospital.Name,
                    Address = patient.Hospital.Location
                },

                Plansum = new PdfReport.Reporting.Plansum
                {

                    Id = plansum.Id,
                    Course = course.Id,
                    TotalPrescribedDose = dosesum,
                
                    //  Type = Enum.GetName(typeof(PlanType),

                },

                PROI = output,

            };
        }

        private static string GetTempPdfPath()
        {
            return Path.GetTempFileName() + ".pdf";
        }
     }
}

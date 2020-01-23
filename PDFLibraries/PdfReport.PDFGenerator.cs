using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using PdfReport.Reporting;
using PdfReport.Reporting.MigraDoc;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using ROI;
using System.Collections;


/*
    Lahey RadOnc Dose Objective Checker - PDF Generator
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
    This is an internal helper class that is involved in creating the PDF report made by the Dose Objective Check Program. 
    This code was originally developed by Carlos J Anderson and was obtained from him via his website. It was significatly modified for use with the Dose Objective Check program.

*/


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


        public static void PlansumMain(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output, int dt, double dd)
        {

            // MessageBox.Show("Trigger plansum main start");

            var reportService = new PdfReport.Reporting.MigraDoc.ReportPdf();
            var reportData = CreateReportDataPlansum(patient, course, plansum, image3D, structureSet, user, output, dt, dd);
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

            DateTime DOB;

            try
            {
                 DOB = (DateTime)patient.DateOfBirth;  //casts nullable DateTime variable from Varian's API to a normal one
            }
            catch(InvalidOperationException e)
            {
                // nullable object must have a value. this happens when there is no date of birth for the patient
                System.Windows.Forms.MessageBox.Show("Alert: This patient does not have a Date of Birth that is stored in Eclipse. Today's date will be used as their date of birth so the program can continue.");

                 DOB = DateTime.Today;
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


        private static ReportData CreateReportDataPlansum(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, List<ROI.ROI> output, int dt, double dd)
        {

            //  MessageBox.Show("Trigger report data sum");

            // some variables used to help convert between Varian stuff and the classes for the pdf


            DateTime DOB;

            try
            {
                DOB = (DateTime)patient.DateOfBirth;  //casts nullable DateTime variable from Varian's API to a normal one
            }
            catch (InvalidOperationException e)
            {
                // nullable object must have a value. this happens when there is no date of birth for the patient
                System.Windows.Forms.MessageBox.Show("Alert: This patient does not have a Date of Birth that is stored in Eclipse. Today's date will be used as their date of birth so the program can continue.");

                DOB = DateTime.Today;
            }


            double dosesum = 0.0;
            string dunit = null;

            if (dt == 1)
            {

                foreach (PlanSetup aplan in plansum.PlanSetups)
                {
                    dosesum += aplan.TotalPrescribedDose.Dose;
                    dunit = aplan.TotalPrescribedDose.UnitAsString;
                }
            }
            else if (dt == 2)
            {
                IEnumerator lk = plansum.PlanSetups.GetEnumerator();
                lk.MoveNext();
                PlanSetup PS = (PlanSetup)lk.Current;
                dosesum = PS.TotalPrescribedDose.Dose;
                dunit = PS.TotalPrescribedDose.UnitAsString;
            }
            else if (dt == 3)
            {
                dosesum = dd;
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
                    TotalPrescribedDose = dosesum,   // in cGy
                
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

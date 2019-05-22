﻿using System;
using System.Diagnostics;
using System.IO;
using PdfReport.Reporting;
using PdfReport.Reporting.MigraDoc;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PdfReport.PDFGenerator
{
     public class Program
    {
        public static void Main(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user)
        {
            var reportService = new ReportPdf();
            var reportData = CreateReportData(patient, course, plan, image3D, structureSet, user);

            var path = GetTempPdfPath();
            reportService.Export(path, reportData);

            Process.Start(path);
        }

        private static ReportData CreateReportData(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user)
        {
            // some variables used to help convert between Varian stuff and the classes for the pdf

            DateTime DOB = (DateTime)patient.DateOfBirth;  //casts nullable DateTime varriable from Varian's API to a normal one


            return new ReportData
            {
                Patient = new PdfReport.Reporting.Patient
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Sex = patient.Sex,
                    Birthdate = DOB,

                    Doctor = new Doctor
                    {
                        Name = patient.PrimaryOncologistId
                    }
                },

                Plan = new PdfReport.Reporting.Plan
                {

                    Id = plan.Id,
                    Course = course.Id,
                    ApprovalStatus = plan.ApprovalStatus.ToString()

                    //  Type = Enum.GetName(typeof(PlanType),

                }






               /* 
                StructureSet = new StructureSet
                {
                    Id = "CT",
                    Image = new Image
                    {
                        Id = "CT_SCAN",
                        CreationTime = new DateTime(2017, 9, 2, 15, 56, 12)
                    },
                    Structures = new[]
                    {
                        new Structure
                        {
                            Id = "PTV",
                            VolumeInCc = 153.83,
                            MeanDoseInGy = 47.12
                        },
                        new Structure
                        {
                            Id = "Bladder",
                            VolumeInCc = 96.31,
                            MeanDoseInGy = 38.60
                        },
                        new Structure
                        {
                            Id = "Bowel",
                            VolumeInCc = 1683.98,
                            MeanDoseInGy = 34.71,
                        },
                        new Structure
                        {
                            Id = "Femur, Left",
                            VolumeInCc = 176.33,
                            MeanDoseInGy = 15.45
                        },
                        new Structure
                        {
                            Id = "Femur, Right",
                            VolumeInCc = 174.43,
                            MeanDoseInGy = 16.11
                        },
                        new Structure
                        {
                            Id = "Prostate Bed",
                            VolumeInCc = 76.67,
                            MeanDoseInGy = 46.78,
                        },
                        new Structure
                        {
                            Id = "Rectum",
                            VolumeInCc = 75.41,
                            MeanDoseInGy = 26.39
                        },
                    }
                }   //ends structure set
                */

            };
        }

        private static string GetTempPdfPath()
        {
            return Path.GetTempFileName() + ".pdf";
        }
    }
}

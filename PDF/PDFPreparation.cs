using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;




/*
    Dose Objective Check - PDF Preparation

    Description:
    This is an internal helper class that is involved in creating the PDF report made by the Dose Objective Check Program. 
    A lot of the logic in here is for dealing with the target coverage.

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program runs on .NET Framework 4.6.1. It also uses MigraDoc and PDFSharp for the PDF generation, commonly available libraries which can be found on NuGet

    Copyright (C) 2021 Zackary Thomas Ricci Morelli
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    I can be contacted at: zackmorelli@gmail.com


    Release 3.2 - 6/8/2021



*/


namespace DoseObjectiveCheck
{
     public class PDFPreparation
     {
        public static async void PlanMain(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, string TS, string ptype, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, bool DoseStats, string[] SRScoverageRequirements, string[] ConvCoverageRequirements, List<DoseObjective> output)
        {
           // MessageBox.Show("Trigger plan main start");
            var reportService = new PDFGenerator();
            SRScoveragestats srsstats = new SRScoveragestats();
            ConventionalCoverageStats conventionalCoverageStats = new ConventionalCoverageStats();

            if (DoseStats == true)
            {
                if (ptype == "SRS/SBRT" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("goals=false before SRS create reprot data plan");
                    List<string> SRSTargetlist = new List<string>();
                    SRSTargetlist.Add("NA");
                    List<string> targetdoses = new List<string>();
                    List<string> targetids = new List<string>();
                    string plandose = null;

                    //The doctors are likely to specify doses for different targets in a multi-target SRS plan, so we query the RX to see if that is the case
                    if (plan.RTPrescription.Targets.Count() > 1)
                    {
                        //Multi-target!
                        System.Windows.Forms.MessageBox.Show("The doctor has specified multiple targets in the prescription. The program will use the Ids and doses of these targets to fill out the interface.");
                        foreach (RTPrescriptionTarget targ in plan.RTPrescription.Targets)
                        {
                            targetdoses.Add(Convert.ToString(targ.DosePerFraction.Dose * targ.NumberOfFractions));
                            targetids.Add(targ.TargetId);
                        }
                    }
                    else
                    {
                        plandose = Convert.ToString(plan.TotalDose.Dose);
                    }

                    foreach (VMS.TPS.Common.Model.API.Structure str in plan.StructureSet.Structures)
                    {
                        SRSTargetlist.Add(str.Id);
                    }

                    SRScoveragestats srscoveragestats = await DoseCoverageUtilities.SRSDoseCoverageSelectorGUICALLER(SRSTargetlist, targetdoses, targetids, plandose);
                    srscoveragestats.CoverageReqRX = Convert.ToDouble(SRScoverageRequirements[0]);
                    srscoveragestats.CoverageReqVol = Convert.ToDouble(SRScoverageRequirements[1]);
                    srsstats = srscoveragestats;
                }
                else if (ptype == "Conventional" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("goals=false before Conv. create reprot data plan");
                    List<string> ConvTargetlist = new List<string>();
                    List<DoseCoverageUtilities.conventionalplaninfo> planlist = new List<DoseCoverageUtilities.conventionalplaninfo> ();
                    string planid = plan.Id;
                    ConvTargetlist.Add("NA");
                    planlist.Add(new DoseCoverageUtilities.conventionalplaninfo { id = "NA", dose = "0" });

                    foreach (PlanSetup pla in course.PlanSetups)
                    {
                        planlist.Add(new DoseCoverageUtilities.conventionalplaninfo {id = pla.Id, dose = Convert.ToString(pla.TotalDose.Dose) });
                    }

                    foreach (PlanSum ps in course.PlanSums)
                    {
                        planlist.Add(new DoseCoverageUtilities.conventionalplaninfo {id = ps.Id, dose = "0" });
                    }

                    foreach (VMS.TPS.Common.Model.API.Structure str in plan.StructureSet.Structures)
                    {
                        ConvTargetlist.Add(str.Id);
                    }

                    bool sequentialstatus = false;
                    conventionalCoverageStats.Sequential = false;   //always false for plans

                    List<VMS.TPS.Common.Model.API.Structure> ptvlist = plan.StructureSet.Structures.Where(s => s.Id.StartsWith("_PTV")).ToList();
                    List<VMS.TPS.Common.Model.API.Structure> ctvlist = plan.StructureSet.Structures.Where(s => s.Id.StartsWith("_CTV")).ToList();
                    List<VMS.TPS.Common.Model.API.Structure> gtvlist = plan.StructureSet.Structures.Where(s => s.Id.StartsWith("_GTV")).ToList();
                   
                    if(ptvlist.Count == 1 && ctvlist.Count < 2 && gtvlist.Count < 2)
                    {
                        //seems like a "normal" conventional plan with one PTV and one CTV or GTV.
                        //We want to make a PDF where the target coverage is reproted all in one neat section, instead of multiple sections for each target
                        bool manualstatus = false;     //this bool determines if the user still wants to specify their own structures for target coverage analysis
                        manualstatus = await DoseCoverageUtilities.simpleplanGUICALLER();

                        if (manualstatus == false)
                        {
                            //simple PTV/CTV pair
                            conventionalCoverageStats.Simple = true;
                            conventionalCoverageStats.Manual = false;
                            conventionalCoverageStats.DosePainted = false;

                            conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ptvlist.First().Id, Dose = plan.TotalDose.Dose, plan = plan.Id });
                            if (ctvlist.Count == 1)
                            {
                                conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ctvlist.First().Id, Dose = plan.TotalDose.Dose, plan = plan.Id });
                            }

                            if (gtvlist.Count == 1)
                            {
                                conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = gtvlist.First().Id, Dose = plan.TotalDose.Dose, plan = plan.Id });
                            }
                        }
                        else
                        {
                            //user specified they want to select target structures manually
                            ConventionalCoverageStats CCS = await DoseCoverageUtilities.ConventionalDoseCoverageSelectorGUICALLER(ConvTargetlist, planlist, planid, sequentialstatus);
                            if (CCS.Sequential == false)
                            {
                                foreach (TargetStructure targ in CCS.TargetStructures)
                                {
                                    targ.plan = plan.Id;
                                }
                            }

                            CCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            CCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            CCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            CCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            CCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            CCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            CCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            CCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = CCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = manualstatus;
                            conventionalCoverageStats.DosePainted = false;
                        }
                    }
                    else
                    {
                        // query the user to see if it is a dose painted plan
                        bool DosePainted = false;
                        DosePainted = await DoseCoverageUtilities.DosePaintedPromptGUICALLER();
                        
                        if(DosePainted == true)
                        {
                            //Dose painted plan. A GUI allows the user to specify dose levels and the targets in each level
                             ConventionalCoverageStats DCS = await DoseCoverageUtilities.DosePaintedTargetSelectorGUICALLER(ConvTargetlist);

                            if (DCS.Sequential == false)
                            {
                                foreach (TargetStructure targ in DCS.TargetStructures)
                                {
                                    targ.plan = plan.Id;
                                }
                            }

                            DCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            DCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            DCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            DCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            DCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            DCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            DCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            DCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = DCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = false;
                            conventionalCoverageStats.DosePainted = true;
                        }
                        else if (DosePainted == false)
                        {
                            //Not simple or dose painted, so a manual plan. As in the user uses the GUI below to manuallt select the targets and doses used for evaluation.
                            ConventionalCoverageStats CCS = await DoseCoverageUtilities.ConventionalDoseCoverageSelectorGUICALLER(ConvTargetlist, planlist, planid, sequentialstatus);
                            if (CCS.Sequential == false)
                            {
                                foreach (TargetStructure targ in CCS.TargetStructures)
                                {
                                    targ.plan = plan.Id;
                                }
                            }

                            CCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            CCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            CCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            CCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            CCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            CCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            CCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            CCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = CCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = true;
                            conventionalCoverageStats.DosePainted = false;
                        }
                    }

                    conventionalCoverageStats.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                    conventionalCoverageStats.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                    conventionalCoverageStats.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                    conventionalCoverageStats.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                    conventionalCoverageStats.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                    conventionalCoverageStats.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                    conventionalCoverageStats.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                    conventionalCoverageStats.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                }
            }

            var reportData = CreateReportDataPlan(patient, course, TS, ptype, plan, image3D, structureSet, user, DoseStats, srsstats, conventionalCoverageStats, SRScoverageRequirements, ConvCoverageRequirements, output);
            //  MessageBox.Show("Trigger main middle plan");

            //var path = @"\\ntfs16\TherapyPhysics\LCN Scans\Script_Reports\Dose_Objective_Reports\Dose_Objective_Report_" + patient.Id + "_" + course.Id + "_" + plan.Id + ".pdf";  // Lahey
            var path = @"\\shccorp\commononcology\PHYSICS\New File Structure PHYSICS\Script Reports\Dose Objective Reports\Dose_Objective_Report_" + patient.Id + "_" + course.Id + "_" + plan.Id + ".pdf"; // Winchester

            // MessageBox.Show(path);
            reportService.Export(path, reportData);
           //  MessageBox.Show("Trigger after export plan");

            Process.Start(path);
           // MessageBox.Show("Trigger main end");
        }

        //===================================================================================================================================================================================================================================================================================================================================================================================================================================================================

        public static async void PlansumMain(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, string[] Si, string ptype, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, bool DoseStats, string[] SRScoverageRequirements, string[] ConvCoverageRequirements, List<DoseObjective> output, int dt, double dd)
        {
            // MessageBox.Show("Trigger plansum main start");
            var reportService = new PDFGenerator();
            SRScoveragestats srsstats = new SRScoveragestats();
            ConventionalCoverageStats conventionalCoverageStats = new ConventionalCoverageStats();

            double dosesum = 0.0;
            try
            {
                string dunit = null;
                if (dt == 1)
                {
                    foreach (PlanSetup aplan in plansum.PlanSetups)
                    {
                        dosesum += aplan.TotalDose.Dose;
                        dunit = aplan.TotalDose.UnitAsString;
                    }
                }
                else if (dt == 2)
                {
                    IEnumerator lk = plansum.PlanSetups.GetEnumerator();
                    lk.MoveNext();
                    PlanSetup PS = (PlanSetup)lk.Current;
                    dosesum = PS.TotalDose.Dose;
                    dunit = PS.TotalDose.UnitAsString;
                }
                else if (dt == 3)
                {
                    dosesum = dd;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Something has gone wrong while trying to select the Total Dose of this plansum. Please try running the script again and make sure you select one of the three options for the total dose when prompted.");
            }

            if (DoseStats == true)
            {
                if (ptype == "SRS/SBRT" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("goals=false before SRS create reprot data plan");
                    List<string> SRSTargetlist = new List<string>();
                    SRSTargetlist.Add("NA");
                    List<string> targetdoses = new List<string>();
                    List<string> targetids = new List<string>();
                    targetids[0] = "plansum";
                    string plandose = null;

                    plandose = Convert.ToString(dosesum);

                    foreach (VMS.TPS.Common.Model.API.Structure str in plansum.StructureSet.Structures)
                    {
                        SRSTargetlist.Add(str.Id);
                    }

                    SRScoveragestats srscoveragestats = await DoseCoverageUtilities.SRSDoseCoverageSelectorGUICALLER(SRSTargetlist, targetdoses, targetids, plandose);

                    srscoveragestats.CoverageReqRX = Convert.ToDouble(SRScoverageRequirements[0]);
                    srscoveragestats.CoverageReqVol = Convert.ToDouble(SRScoverageRequirements[1]);
                    srsstats = srscoveragestats;
                }
                else if (ptype == "Conventional" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("goals=false before Conv. create reprot data plan");
                    List<string> ConvTargetlist = new List<string>();
                    List<DoseCoverageUtilities.conventionalplaninfo> planlist = new List<DoseCoverageUtilities.conventionalplaninfo>();
                    string planid = plansum.Id;
                    ConvTargetlist.Add("NA");
                    planlist.Add(new DoseCoverageUtilities.conventionalplaninfo { id = "NA", dose = "0" });

                    foreach (PlanSetup pla in course.PlanSetups)
                    {
                        planlist.Add(new DoseCoverageUtilities.conventionalplaninfo { id = pla.Id, dose = Convert.ToString(pla.TotalDose.Dose) });
                    }

                    foreach (PlanSum ps in course.PlanSums)
                    {
                        planlist.Add(new DoseCoverageUtilities.conventionalplaninfo { id = ps.Id, dose = "0" });
                    }

                    foreach (VMS.TPS.Common.Model.API.Structure str in plansum.StructureSet.Structures)
                    {
                        ConvTargetlist.Add(str.Id);
                    }

                    bool sequentialstatus = false;
                    if (plansum.Id.Contains("PlanSum") || plansum.Id.Contains("Plan Sum") || plansum.Id.Contains("plan sum") || plansum.Id.Contains("plansum"))
                    {
                        sequentialstatus = await DoseCoverageUtilities.sequentialpromptGUICALLER();
                    }
                    conventionalCoverageStats.Sequential = sequentialstatus;

                    List<VMS.TPS.Common.Model.API.Structure> ptvlist = plansum.StructureSet.Structures.Where(s => s.Id.StartsWith("_PTV")).ToList();
                    List<VMS.TPS.Common.Model.API.Structure> ctvlist = plansum.StructureSet.Structures.Where(s => s.Id.StartsWith("_CTV")).ToList();
                    List<VMS.TPS.Common.Model.API.Structure> gtvlist = plansum.StructureSet.Structures.Where(s => s.Id.StartsWith("_GTV")).ToList();
                   

                    if (ptvlist.Count == 1 && gtvlist.Count < 2 && ctvlist.Count < 2)
                    {
                        //seems like a "normal" conventional plan with one PTV and one CTV or GTV.
                        //We want to make a PDF where the target coverage is reproted all in one neat section, instead of multiple sections for each target
                        bool manualstatus = false;     //this bool determines if the user still wants to specify their own structures for target coverage analysis
                        manualstatus = await DoseCoverageUtilities.simpleplanGUICALLER();

                        if (manualstatus == false)
                        {
                            //simple PTV/CTV pair
                            conventionalCoverageStats.Simple = true;
                            conventionalCoverageStats.Manual = false;
                            conventionalCoverageStats.DosePainted = false;

                            if (sequentialstatus == false)
                            {
                                conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ptvlist.First().Id, Dose = dosesum, plan = plansum.Id });
                                if (ctvlist.Count == 1)
                                {
                                    conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ctvlist.First().Id, Dose = dosesum, plan = plansum.Id });
                                }

                                if (gtvlist.Count == 1)
                                {
                                    conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = gtvlist.First().Id, Dose = dosesum, plan = plansum.Id });
                                }
                            }
                            else if (sequentialstatus == true)
                            {
                                //so if the situation is a simple PTV/CTV pair, but for a sequntial course of plans, we need to add other structures to our list 
                                //so we can calculate the neccessary values for the PTV/CTV pair in the other plans, even though its the same structures in all the plans.

                                foreach (PlanSetup ps in course.PlanSetups)
                                {
                                    conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ptvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    if (ctvlist.Count == 1)
                                    {
                                        conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ctvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    }

                                    if (gtvlist.Count == 1)
                                    {
                                        conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = gtvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    }
                                }

                                foreach (PlanSum ps in course.PlanSums)
                                {
                                    conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ptvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    if (ctvlist.Count == 1)
                                    {
                                        conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = ctvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    }

                                    if (gtvlist.Count == 1)
                                    {
                                        conventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = gtvlist.First().Id, Dose = dosesum, plan = ps.Id });
                                    }
                                }
                            }
                        }
                        else
                        {
                            //user specified they want to select target structures manually
                            ConventionalCoverageStats CCS = await DoseCoverageUtilities.ConventionalDoseCoverageSelectorGUICALLER(ConvTargetlist, planlist, planid, sequentialstatus);
                            if (CCS.Sequential == false)
                            {
                                foreach (TargetStructure targ in CCS.TargetStructures)
                                {
                                    targ.plan = plansum.Id;
                                }
                            }

                            CCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            CCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            CCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            CCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            CCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            CCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            CCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            CCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = CCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = manualstatus;
                            conventionalCoverageStats.DosePainted = false;
                        }
                    }
                    else
                    {
                        //if sequential is true, we aren't going to bother asking if it is dose painted
                        // just call the gui where the user can select manual structures
                        // for courses that are dose painted and sequential, the dosimitrists will have to run the program several times to get all the info they need.

                        bool DosePainted = false;

                        if (sequentialstatus == false)
                        {
                            // query the user to see if it is a dose painted plan
                            DosePainted = await DoseCoverageUtilities.DosePaintedPromptGUICALLER();
                        }

                        if (DosePainted == true)
                        {
                            //Dose painted plan. A GUI allows the user to specify dose levels and the targets in each level
                            ConventionalCoverageStats DCS = await DoseCoverageUtilities.DosePaintedTargetSelectorGUICALLER(ConvTargetlist);

                            if (DCS.Sequential == false)
                            {
                                foreach (TargetStructure targ in DCS.TargetStructures)
                                {
                                    targ.plan = plansum.Id;
                                }
                            }

                            DCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            DCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            DCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            DCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            DCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            DCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            DCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            DCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = DCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = false;
                            conventionalCoverageStats.DosePainted = true;
                        }
                        else if (DosePainted == false)
                        {
                            //Not simple or dose painted, so a manual plan. As in the user uses the GUI below to manually select the targets and doses used for evaluation.
                            ConventionalCoverageStats CCS = await DoseCoverageUtilities.ConventionalDoseCoverageSelectorGUICALLER(ConvTargetlist, planlist, planid, sequentialstatus);

                            CCS.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                            CCS.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                            CCS.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                            CCS.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                            CCS.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                            CCS.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                            CCS.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                            CCS.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                            //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + CCS.PTV1RXcoverage);

                            conventionalCoverageStats = CCS;
                            conventionalCoverageStats.Simple = false;
                            conventionalCoverageStats.Sequential = sequentialstatus;
                            conventionalCoverageStats.Manual = true;
                            conventionalCoverageStats.DosePainted = false;
                        }
                    }

                    conventionalCoverageStats.CTVRXcoverage = Convert.ToDouble(ConvCoverageRequirements[0]);
                    conventionalCoverageStats.CTVVolcoverage = Convert.ToDouble(ConvCoverageRequirements[1]);
                    conventionalCoverageStats.PTV1RXcoverage = Convert.ToDouble(ConvCoverageRequirements[2]);
                    conventionalCoverageStats.PTV1Volcoverage = Convert.ToDouble(ConvCoverageRequirements[3]);
                    conventionalCoverageStats.PTV2RXcoverage = Convert.ToDouble(ConvCoverageRequirements[4]);
                    conventionalCoverageStats.PTV2Volcoverage = Convert.ToDouble(ConvCoverageRequirements[5]);
                    conventionalCoverageStats.PTV3RXcoverage = Convert.ToDouble(ConvCoverageRequirements[6]);
                    conventionalCoverageStats.PTV3Volcoverage = Convert.ToDouble(ConvCoverageRequirements[7]);
                }
            }

            var reportData = CreateReportDataPlansum(patient, course, Si, ptype, plansum, image3D, structureSet, user, DoseStats, srsstats, conventionalCoverageStats, SRScoverageRequirements, ConvCoverageRequirements, output, dt, dd);
            // MessageBox.Show("Trigger main middle");
            //var path = @"\\ntfs16\TherapyPhysics\LCN Scans\Script_Reports\Dose_Objective_Reports\Dose_Objective_Report_" + patient.Id + "_" + course.Id + "_" + plansum.Id + ".pdf";   // Lahey
            var path = @"\\shccorp\commononcology\PHYSICS\New File Structure PHYSICS\Script Reports\Dose Objective Reports\Dose_Objective_Report_" + patient.Id + "_" + course.Id + "_" + plansum.Id + ".pdf"; // Winchester

            // MessageBox.Show(path);
            reportService.Export(path, reportData);
           //  MessageBox.Show("Trigger after export plansum");

            Process.Start(path);
            // MessageBox.Show("Trigger main end");
        }

        //====================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================

        private static ReportData CreateReportDataPlan(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, string TS, string ptype, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, bool DoseStats, SRScoveragestats srsstats, ConventionalCoverageStats conventionalCoverageStats, string[] SRScoverageRequirements, string[] ConvCoverageRequirements, List<DoseObjective> output)
        {
            //System.Windows.Forms.MessageBox.Show("Trigger report data plan");
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

            ReportData data = new ReportData
            {
                User = user.Name,

                Patient = new Patient
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    // MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    Sex = patient.Sex,
                    Birthdate = DOB,

                    Doctor = new Doctor
                    {
                        Name = patient.PrimaryOncologistId
                    }
                },

                Hospital = new Hospital
                {
                    Name = patient.Hospital.Name,
                    Address = patient.Hospital.Location,
                },

                Plan = new Plan
                {
                    Id = plan.Id,
                    Course = course.Id,
                    ApprovalStatus = plan.ApprovalStatus.ToString(),
                    TotalPrescribedDose = plan.TotalDose.Dose,
                    CreationDateTime = plan.CreationDateTime,
                    CreationUser = plan.CreationUserName,
                    LastModifiedDateTime = plan.HistoryDateTime,
                    LastModifiedUser = plan.HistoryUserName,
                    TreatmentSite = TS,

                    //  Type = Enum.GetName(typeof(PlanType),
                },
                PROI = output,
                SRSstats = false,
                Convstats = false,
            };

            if (DoseStats == true)
            {
                if (ptype == "SRS/SBRT" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("Planreport - dosestats - SRS");
                    //This list is used to check all the SRS target structures to make sure they aren't too close
                    List<Structure> SRStargets = new List<Structure>();

                    //finds the body structure once here, instead of repeatedly finding it later
                    var Bodystructs = plan.StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                    VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                    //finds the Body-PTV_20 structure. This is not always present, so there is a bool used to control this
                    bool Body_PTV20_exists = false;
                    VMS.TPS.Common.Model.API.Structure Body_PTV_20 = plan.StructureSet.Structures.First();  // just to get this to work;
                    var Body_PTVtemplist = plan.StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body-PTV_20")));
                    if(Body_PTVtemplist.Count() < 1)
                    {
                        Body_PTV20_exists = false;
                    }
                    else if(Body_PTVtemplist.Count() > 0)
                    {
                        Body_PTV20_exists = true;
                        Body_PTV_20 = Body_PTVtemplist.First();
                    }

                    foreach(SRSTargetstats target in srsstats.Targets)
                    {
                        foreach(VMS.TPS.Common.Model.API.Structure str in plan.StructureSet.Structures)
                        {
                            if(str.Id == target.TargetNAME)
                            {
                                SRStargets.Add(str);

                                DoseValue iso100 = new DoseValue(target.Dose, DoseValue.DoseUnit.cGy);
                                target.BodyV100 = plan.GetVolumeAtDose(Bodystruct, iso100, VolumePresentation.AbsoluteCm3);

                                DoseValue iso50 = new DoseValue((target.Dose * 0.5), DoseValue.DoseUnit.cGy);
                                target.BodyV50 = plan.GetVolumeAtDose(Bodystruct, iso50, VolumePresentation.AbsoluteCm3);

                                if(Body_PTV20_exists == true)
                                {
                                    DoseValue BodyPTV_20Dosetemp = plan.GetDoseAtVolume(Body_PTV_20, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                    target.BodyPTV_20_Dose = BodyPTV_20Dosetemp.Dose;
                                }
                                else if(Body_PTV20_exists == false)
                                {
                                    target.BodyPTV_20_Dose = -5000;  //this is a flag that the structure doesn't exist, to be handled later
                                }

                                target.Targetvol = str.Volume;

                                //So we use the dose in cGy that the user typed in, multiplied by the PTV coverage requirement (a %) that the user typed in. 
                                double tempdose = target.Dose * (srsstats.CoverageReqRX / 100.0);
                                DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                target.PTVXXRX = plan.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                            }
                        }
                    }

                    //So this figures out if SRS Targets are too close to each other for normal dosimetric analysis
                    // The user is alerted with a pop-up and a warning is included in the PDF
                    string ovcheck = SRSTargetOverlapCheck.DistanceCheck(SRStargets);
                    if(ovcheck != null)
                    {
                        System.Windows.Forms.MessageBox.Show("Close SRS targets detected! The following target structures are within 50 mm of each other.\n\n=====================================================\n\n" + ovcheck);
                    }

                    foreach (SRSTargetstats target in srsstats.Targets)
                    {
                        if(ovcheck.Contains(target.TargetNAME))
                        {
                            target.DistanceWarning = true;
                        }
                        else
                        {
                            target.DistanceWarning = false;
                        }
                    }

                    data.SRSstats = true;
                    data.srsstats = srsstats;
                }
                else if (ptype == "Conventional" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("Planreport - dosestats - Conventional");

                    if(conventionalCoverageStats.Sequential == false)
                    {
                        foreach(TargetStructure target in conventionalCoverageStats.TargetStructures)
                        {
                            foreach(VMS.TPS.Common.Model.API.Structure str in plan.StructureSet.Structures)
                            {
                                if(str.Id == target.StructureNAME)
                                {
                                    target.vol = str.Volume;

                                    double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                    DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.CTVXXRX0 = plan.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX0: " + target.CTVXXRX0);

                                    tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                    //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + conventionalCoverageStats.PTV1RXcoverage);
                                    //System.Windows.Forms.MessageBox.Show("tempdose: " + tempdose);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX1 = plan.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                   // System.Windows.Forms.MessageBox.Show("RX1: " + target.PTVXXRX1);

                                    tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX2 = plan.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX2: " + target.PTVXXRX2);

                                    //System.Windows.Forms.MessageBox.Show("RX3 tempdose struct: " + target.StructureNAME);
                                    tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                    //System.Windows.Forms.MessageBox.Show("RX3 tempdose: " + tempdose);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX3 = plan.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX3: " + target.PTVXXRX3);

                                    var Bodystructs = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                    VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                    target.GlobalMaxPointDose = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                }
                            }
                        }
                    }
                    else if(conventionalCoverageStats.Sequential == true)
                    {
                        foreach (TargetStructure target in conventionalCoverageStats.TargetStructures)
                        {
                            foreach (VMS.TPS.Common.Model.API.Structure str in plan.StructureSet.Structures)
                            {
                                if (str.Id == target.StructureNAME)
                                {
                                    target.vol = str.Volume;

                                    if(target.plan.Contains("PlanSum") || target.plan.Contains("Plan Sum") || target.plan.Contains("plan sum") || target.plan.Contains("plansum"))
                                    {
                                        double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                        DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.CTVXXRX0 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX1 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX2 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX3 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        var Bodystructs = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                        VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                        target.GlobalMaxPointDose = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                    }
                                    else
                                    {
                                        //a normal plan
                                        double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                        DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.CTVXXRX0 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX1 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX2 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX3 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        var Bodystructs = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                        VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                        target.GlobalMaxPointDose = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                    }
                                }
                            }
                        }
                    }
                    data.Convstats = true;
                    data.conventionalstats = conventionalCoverageStats;
                }
            }
            
            return data;
        }

        //====================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================

        private static ReportData CreateReportDataPlansum(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, string[] Si, string ptype, VMS.TPS.Common.Model.API.PlanSum plansum, VMS.TPS.Common.Model.API.Image image3D, VMS.TPS.Common.Model.API.StructureSet structureSet, VMS.TPS.Common.Model.API.User user, bool DoseStats, SRScoveragestats srsstats, ConventionalCoverageStats conventionalCoverageStats, string[] SRScoverageRequirements, string[] ConvCoverageRequirements, List<DoseObjective> output, int dt, double dd)
        {
            //  MessageBox.Show("Trigger report data sum");
            // some variables used to help convert between Varian stuff and the classes for the pdf
  
            DateTime DOB;
            double dosesum = 0.0;
            string dunit = null;
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

            try
            {
                if (dt == 1)
                {
                    foreach (PlanSetup aplan in plansum.PlanSetups)
                    {
                        dosesum += aplan.TotalDose.Dose;
                        dunit = aplan.TotalDose.UnitAsString;
                    }
                }
                else if (dt == 2)
                {
                    IEnumerator lk = plansum.PlanSetups.GetEnumerator();
                    lk.MoveNext();
                    PlanSetup PS = (PlanSetup)lk.Current;
                    dosesum = PS.TotalDose.Dose;
                    dunit = PS.TotalDose.UnitAsString;
                }
                else if (dt == 3)
                {
                    dosesum = dd;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Something has gone wrong while trying to select the Total Dose of this plansum. Please try running the script again and make sure you select one of the three options for the total dose when prompted.");
            }

            ReportData data = new ReportData
            {
                User = user.Name,

                Patient = new Patient
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    // MiddleName = patient.MiddleName,
                    LastName = patient.LastName,
                    Sex = patient.Sex,
                    Birthdate = DOB,

                    Doctor = new Doctor
                    {
                        Name = patient.PrimaryOncologistId
                    }
                },

                Hospital = new Hospital
                {
                    Name = patient.Hospital.Name,
                    Address = patient.Hospital.Location,
                },

                Plansum = new Plansum
                {
                    Id = plansum.Id,
                    Course = course.Id,
                    TotalPrescribedDose = dosesum,   // in cGy
                    CreationDateTime = plansum.CreationDateTime,
                    LastModifiedDateTime = plansum.HistoryDateTime,
                    LastModifiedUser = plansum.HistoryUserName,
                    TreatmentSites = Si

                    //  Type = Enum.GetName(typeof(PlanType),

                },
                PROI = output,
                SRSstats = false,
                Convstats = false,
            };

            if (DoseStats == true)
            {
                if (ptype == "SRS/SBRT" || ptype == "Both")
                {
                    //System.Windows.Forms.MessageBox.Show("Planreport - dosestats - SRS");
                    //finds the body structure once here, instead of repeatedly finding it later
                    List<Structure> SRStargets = new List<Structure>();

                    var Bodystructs = plansum.StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                    VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                    //finds the Body-PTV_20 structure. This is not always present, so there is a bool used to control this
                    bool Body_PTV20_exists = false;
                    VMS.TPS.Common.Model.API.Structure Body_PTV_20 = plansum.StructureSet.Structures.First();  // just to get this to work;
                    var Body_PTVtemplist = plansum.StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body-PTV_20")));
                    if (Body_PTVtemplist.Count() < 1)
                    {
                        Body_PTV20_exists = false;
                    }
                    else if (Body_PTVtemplist.Count() > 0)
                    {
                        Body_PTV20_exists = true;
                        Body_PTV_20 = Body_PTVtemplist.First();
                    }

                    foreach (SRSTargetstats target in srsstats.Targets)
                    {
                        foreach (VMS.TPS.Common.Model.API.Structure str in plansum.StructureSet.Structures)
                        {
                            if (str.Id == target.TargetNAME)
                            {
                                SRStargets.Add(str);

                                DoseValue iso100 = new DoseValue(target.Dose, DoseValue.DoseUnit.cGy);
                                target.BodyV100 = plansum.GetVolumeAtDose(Bodystruct, iso100, VolumePresentation.AbsoluteCm3);

                                DoseValue iso50 = new DoseValue((target.Dose * 0.5), DoseValue.DoseUnit.cGy);
                                target.BodyV50 = plansum.GetVolumeAtDose(Bodystruct, iso50, VolumePresentation.AbsoluteCm3);

                                if (Body_PTV20_exists == true)
                                {
                                    DoseValue BodyPTV_20Dosetemp = plansum.GetDoseAtVolume(Body_PTV_20, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                    target.BodyPTV_20_Dose = BodyPTV_20Dosetemp.Dose;
                                }
                                else if (Body_PTV20_exists == false)
                                {
                                    target.BodyPTV_20_Dose = -5000;  //this is a flag that the structure doesn't exist, to be handled later
                                }

                                target.Targetvol = str.Volume;

                                //So we use the dose in cGy that the user typed in, multiplied by the PTV coverage requirement (a %) that the user typed in. 
                                double tempdose = target.Dose * (srsstats.CoverageReqRX / 100.0);
                                DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                target.PTVXXRX = plansum.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                            }
                        }
                    }

                    //So this figures out if SRS Targets are too close to each other for normal dosimetric analysis
                    // The user is alerted with a pop-up and a warning is included in the PDF
                    string ovcheck = SRSTargetOverlapCheck.DistanceCheck(SRStargets);
                    if (ovcheck != null)
                    {
                        System.Windows.Forms.MessageBox.Show("Close SRS targets detected! The following target structures are within 50 mm of each other.\n\n=====================================================\n\n" + ovcheck);
                    }

                    foreach (SRSTargetstats target in srsstats.Targets)
                    {
                        if (ovcheck.Contains(target.TargetNAME))
                        {
                            target.DistanceWarning = true;
                        }
                        else
                        {
                            target.DistanceWarning = false;
                        }
                    }

                    data.SRSstats = true;
                    data.srsstats = srsstats;
                }
                else if (ptype == "Conventional" || ptype == "Both")
                {
                    if (conventionalCoverageStats.Sequential == false)
                    {
                        foreach (TargetStructure target in conventionalCoverageStats.TargetStructures)
                        {
                            foreach (VMS.TPS.Common.Model.API.Structure str in plansum.StructureSet.Structures)
                            {
                                if (str.Id == target.StructureNAME)
                                {
                                    target.vol = str.Volume;

                                    double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                    DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.CTVXXRX0 = plansum.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX0: " + target.CTVXXRX0);

                                    tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                    //System.Windows.Forms.MessageBox.Show("PTV1RXcoverage: " + conventionalCoverageStats.PTV1RXcoverage);
                                    //System.Windows.Forms.MessageBox.Show("tempdose: " + tempdose);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX1 = plansum.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX1: " + target.PTVXXRX1);

                                    tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX2 = plansum.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX2: " + target.PTVXXRX2);

                                    tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                    td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                    target.PTVXXRX3 = plansum.GetVolumeAtDose(str, td, VolumePresentation.Relative);
                                    //System.Windows.Forms.MessageBox.Show("RX3: " + target.PTVXXRX3);

                                    var Bodystructs = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                    VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                    target.GlobalMaxPointDose = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                }
                            }
                        }
                    }
                    else if (conventionalCoverageStats.Sequential == true)
                    {
                        foreach (TargetStructure target in conventionalCoverageStats.TargetStructures)
                        {
                            foreach (VMS.TPS.Common.Model.API.Structure str in plansum.StructureSet.Structures)
                            {
                                if (str.Id == target.StructureNAME)
                                {
                                    target.vol = str.Volume;

                                    if (target.plan.Contains("PlanSum") || target.plan.Contains("Plan Sum") || target.plan.Contains("plan sum") || target.plan.Contains("plansum"))
                                    {
                                        double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                        DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.CTVXXRX0 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX1 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX2 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX3 = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        var Bodystructs = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                        VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                        target.GlobalMaxPointDose = course.PlanSums.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                    }
                                    else
                                    {
                                        //a normal plan
                                        double tempdose = target.Dose * (conventionalCoverageStats.CTVRXcoverage / 100.0);
                                        DoseValue td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.CTVXXRX0 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV1RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX1 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV2RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX2 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        tempdose = target.Dose * (conventionalCoverageStats.PTV3RXcoverage / 100.0);
                                        td = new DoseValue(tempdose, DoseValue.DoseUnit.cGy);
                                        target.PTVXXRX3 = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetVolumeAtDose(str, td, VolumePresentation.Relative);

                                        var Bodystructs = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().StructureSet.Structures.Where(bsr => (bsr.Id.Equals("Body") || bsr.Id.Equals("body") || bsr.Id.Equals("BODY")));
                                        VMS.TPS.Common.Model.API.Structure Bodystruct = Bodystructs.First();

                                        target.GlobalMaxPointDose = course.PlanSetups.Where(ps => ps.Id.Equals(target.plan)).First().GetDoseAtVolume(Bodystruct, 0.03, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute).Dose;
                                    }
                                }
                            }
                        }
                    }
                    data.Convstats = true;
                    data.conventionalstats = conventionalCoverageStats;
                }
            }

            return data;
        }

        //private static string GetTempPdfPath()
        //{
        //    return Path.GetTempFileName() + ".pdf";
        //}
     }

    //=============================================================================================================================================================================================

    public class DoseCoverageUtilities
    {
        public static async Task<ConventionalCoverageStats> ConventionalDoseCoverageSelectorGUICALLER(List<string> ConvTargetList, List<DoseCoverageUtilities.conventionalplaninfo> PlanList, string planid, bool sequentialstatus)
        {
            ConventionalDoseCoverageTargetSelector cts = new ConventionalDoseCoverageTargetSelector(ConvTargetList, PlanList, planid, sequentialstatus);
            await Task.Run(() => System.Windows.Forms.Application.Run(cts));
            ConventionalCoverageStats conventionalCoverageStats = cts.conventionalCoverageStats;
            return conventionalCoverageStats;
        }

        public static async Task<SRScoveragestats> SRSDoseCoverageSelectorGUICALLER(List<string> SRSTargetList, List<string> targetdoses, List<string> targetids, string plandose)
        {
            SRSDoseCoverageTargetSelectorGUI tsg = new SRSDoseCoverageTargetSelectorGUI(SRSTargetList, targetdoses, targetids, plandose);
            await Task.Run(() => System.Windows.Forms.Application.Run(tsg));
            SRScoveragestats srscoveragestats = tsg.srscoveragestats;
            return srscoveragestats;
        }

        public static async Task<bool> simpleplanGUICALLER()
        {
            SimplePlanNotifier spn = new SimplePlanNotifier();
            await Task.Run(() => System.Windows.Forms.Application.Run(spn));
            bool manualselection = spn.manualselection;
            return manualselection;
        }

        public static async Task<bool> sequentialpromptGUICALLER()
        {
            PlansumSequentialPrompt psp = new PlansumSequentialPrompt();
            await Task.Run(() => System.Windows.Forms.Application.Run(psp));
            bool sequentialstatus = psp.sequentialprompt;
            return sequentialstatus;
        }

        public static async Task<bool> DosePaintedPromptGUICALLER()
        {
            DosePaintedPrompt dpp = new DosePaintedPrompt();
            await Task.Run(() => System.Windows.Forms.Application.Run(dpp));
            bool dosepainted = dpp.DosePainted;
            return dosepainted;
        }

        public static async Task<ConventionalCoverageStats> DosePaintedTargetSelectorGUICALLER(List<string> ConvTargList)
        {
            DosePaintedTargetSpecifier dpts = new DosePaintedTargetSpecifier(ConvTargList);
            await Task.Run(() => System.Windows.Forms.Application.Run(dpts));
            ConventionalCoverageStats stats = dpts.ConventionalCoverageStats;
            return stats;
        }


        public class conventionalplaninfo
        {
            public string id { get; set; }

            public string dose { get; set; }

        }

    }





}

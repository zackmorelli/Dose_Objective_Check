using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;


using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;



/*
    Dose Objective Check - (Dose Objective analysis class)
    
    Description:
    This class houses the code for the actual dose objective analysis. There are two methods, one for plans and another for plansums. They both return lists of the Dose Objective class which are used to populate the PDF report.

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
    class DoseObjectiveAnalysis
    {
        public static List<DoseObjective> PlanAnalysis(string laterality, string TS, string ptype, User user, VMS.TPS.Common.Model.API.Patient patient, Course course, StructureSet structureSet, PlanSetup Plan, TextBox OuputBox, string gyntype, bool UseGoals)
        {
            // System.Windows.Forms.MessageBox.Show("TS is: " + TS);
            //  System.Windows.Forms.MessageBox.Show("ptype is: " + ptype);
            List<DoseObjective> ROIE = new List<DoseObjective>();     // Expected ROI made from text file list
            List<DoseObjective> ROIA = new List<DoseObjective>();     // Actual ROI list from Eclipse 
            string Ttype = ptype;
            string Tsite = TS;

            string[] Si = new string[10] { "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA" };

            ROIE = LISTMAKER.Listmaker(Ttype, Tsite, Si, laterality, patient.Id);          // separate class with LISTMAKER function which generates a list of Dose Objectives for the given treatment type and site

            // If the User wants the program to evaluate dose objectives they have made using the clinical goals feature in Eclipse, the code below handles that
            // It simply adds their clincial goals to the standard list. If they want to change a dose objective that is already on the list, they can use the PCTPN Document workflow
            // basically it takes in the string representing the clinical goal and breaks it apart to assign the neccessary info to the appropriate parts of the Dose Objective class
            
            //if (UseGoals == true)
            //{
            //    //actually take the clinical goals and add them to the dose objective list
            //    foreach (ClinicalGoal CG in Plan.GetClinicalGoals())
            //    {
            //        string limit = null;
            //        string limunit = null;
            //        string strict = null;
            //        string limitval = null;

            //        if (CG.ObjectiveAsString.StartsWith("Conformity Index") || CG.ObjectiveAsString.StartsWith("Gradient Measure") || CG.ObjectiveAsString.StartsWith("GM") || CG.ObjectiveAsString.StartsWith("CI"))
            //        {
            //            continue;
            //        }

            //        try
            //        {
            //            if (CG.ObjectiveAsString.StartsWith("Dmax"))
            //            {
            //                limit = "Max Pt Dose [voxel]";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Maximum Dose"))
            //            {
            //                limit = "Max Pt Dose [voxel]";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Mean Dose"))
            //            {
            //                limit = "Mean Dose";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Dmean"))
            //            {
            //                limit = "Mean Dose";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("V"))
            //            {
            //                string[] wrdbrk = new string[6];

            //                wrdbrk = CG.ObjectiveAsString.Split(' ');

            //                if (wrdbrk[2] == "%")
            //                {
            //                    limit = wrdbrk[0] + wrdbrk[1] + wrdbrk[2];
            //                }
            //                else
            //                {
            //                    // the clinical goals force the use of cGy, so we need to convert to Gy so this is compatible with the rest of the program
            //                    string gyconv = Convert.ToString((Convert.ToDouble(wrdbrk[1]) / 100.0));

            //                    limit = wrdbrk[0] + gyconv;
            //                }

            //                limunit = wrdbrk[5];
            //                limitval = wrdbrk[4];
            //                strict = wrdbrk[3];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("D "))
            //            {
            //                string[] wrdbrk = new string[6];

            //                wrdbrk = CG.ObjectiveAsString.Split(' ');

            //                if (wrdbrk[2].StartsWith("cm"))
            //                {
            //                    wrdbrk[2] = "cc";
            //                }

            //                limit = wrdbrk[0] + wrdbrk[1] + wrdbrk[2];

            //                limunit = wrdbrk[5];
            //                limitval = wrdbrk[4];
            //                strict = wrdbrk[3];
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            MessageBox.Show("Clincal goals conversion error.\n\n" + e.ToString() + "\n" + e.StackTrace);
            //        }


            //        if (strict == "\u2265")
            //        {
            //            strict = ">=";
            //        }
            //        else if (strict == "\u2264")
            //        {
            //            strict = "<=";
            //        }

            //        string CGname = CG.StructureId + "_" + limit + " " + strict + " " + limitval + limunit;

            //        //MessageBox.Show("Name: " + CGname);
            //        //MessageBox.Show("Struct Id: " + CG.StructureId);
            //        //MessageBox.Show("limit: " + limit);
            //        //MessageBox.Show("strict: " + strict);
            //        //MessageBox.Show("limitval: " + limitval);
            //        //MessageBox.Show("limitunit: " + limunit);

            //        string[] treatsiteclinical = new string[1];
            //        treatsiteclinical[0] = "Clinical Goal";

            //        ROIE.Add(new DoseObjective { DoseObjectiveName = CGname, Rstruct = CG.StructureId, DoseObjectiveID = 5000, limit = limit, limval = limitval, strict = strict, limunit = limunit, status = null, goal = "NA", treatsite = treatsiteclinical, applystatus = true });
            //    }

            //    //ROIL.Add(new ROI.ROI { DoseObjectiveName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite, applystatus = true });
            //}
            
            double pdose = Plan.TotalDose.Dose;       // prescribed dose of the Plan

            int county = 0;

            //The commented out code below is for parsing out diagnosis-specific dose objectives
            //The MDs do not currently assign a diagnosis to a course as part of their workflow, so the code does not work and is commented out to prevent errors.

            //bool lymphomapatient = false;
            //bool oralpatient = false;

            //MessageBox.Show("Course Diagnoses size: " + course.Diagnoses.Count());
            ////MessageBox.Show("Patient Diagnoses size: " + patient.      .Diagnoses.Count());

            //foreach (Diagnosis dg in course.Diagnoses)
            //{
            //    MessageBox.Show("Diagnosis: " + dg.Id + "   " + dg.ClinicalDescription);
            //    if (dg.ClinicalDescription.Contains("lymphoma") || dg.ClinicalDescription.Contains("Lymphoma"))
            //    {
            //        lymphomapatient = true;
            //    }

            //    //long list here to cover all 14 of the "Oral" malignant Neoplasms in ICD-10-CM
            //    if (dg.ClinicalDescription.Contains("lip") || dg.ClinicalDescription.Contains("Lip") || dg.ClinicalDescription.Contains("tongue") || dg.ClinicalDescription.Contains("Tongue") || dg.ClinicalDescription.Contains("gum") || dg.ClinicalDescription.Contains("Gum") || dg.ClinicalDescription.Contains("mouth") || dg.ClinicalDescription.Contains("Mouth") || dg.ClinicalDescription.Contains("palate") || dg.ClinicalDescription.Contains("Palate") || dg.ClinicalDescription.Contains("tonsil") || dg.ClinicalDescription.Contains("Tonsil") || dg.ClinicalDescription.Contains("salivary") || dg.ClinicalDescription.Contains("Salivary") || dg.ClinicalDescription.Contains("oropharynx") || dg.ClinicalDescription.Contains("Oropharynx") || dg.ClinicalDescription.Contains("nasopharynx") || dg.ClinicalDescription.Contains("Nasopharynx") || dg.ClinicalDescription.Contains("hypopharynx") || dg.ClinicalDescription.Contains("Hypopharynx") || dg.ClinicalDescription.Contains("sinus") || dg.ClinicalDescription.Contains("Sinus") || dg.ClinicalDescription.Contains("pharynx") || dg.ClinicalDescription.Contains("Pharynx") || dg.ClinicalDescription.Contains("oral") || dg.ClinicalDescription.Contains("Oral") || dg.ClinicalDescription.Contains("parotid") || dg.ClinicalDescription.Contains("Parotid"))
            //    {
            //        oralpatient = true;
            //        MessageBox.Show("Oral Diagnosis");
            //    }
            //}

            //for(int k = 0; k < ROIE.Count; k++)
            //{
            //    if (lymphomapatient == false && ROIE[k].DoseObjectiveName.Contains("[lymphoma patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //    }

            //    if (oralpatient == true && ROIE[k].DoseObjectiveName.Contains("[non-oral patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //        MessageBox.Show("Removed non-oral: " + ROIE[k].DoseObjectiveName);
            //    }

            //    if (oralpatient == false && ROIE[k].DoseObjectiveName.Contains("[oral patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //        MessageBox.Show("Removed Oral: " + ROIE[k].DoseObjectiveName);
            //    }
            //}

            

            // MessageBox.Show(Tsite + " dose objective list created successfully.");
            // MessageBox.Show("struct -r " + structureSet.Id);

            // This part of code below gets DVH data from Eclipse. The way it works is different for different limit types, like MaxPtDose, V80, D1cc. etc.
            //ROIE.Sort();
            //Need to implement DoseObjective as Icomparable if we want to sort

            foreach (DoseObjective DO in ROIE)
            {
                // System.Windows.Forms.MessageBox.Show("ROI: " + DO.DoseObjectiveName);
                county++;
                //  System.Windows.Forms.MessageBox.Show("Plan A ROI iterate " + county);
                // MessageBox.Show("struct - " + structureSet.Id);

                //  OuputBox.AppendText(Environment.NewLine);
                //  OuputBox.AppendText("Dose Objectives checked: " + county + "/" + ROIE.Count);

                foreach (Structure S in structureSet.Structures)        // iterates through all the structures in the structureset of the current Plan
                {
                    double structvol = S.Volume;

                    //  System.Windows.Forms.MessageBox.Show("Plan A struct iterate");
                    if (S.IsEmpty == true || S.Volume < 0.0)
                    {
                        // MessageBox.Show("The structure " + S.Id + " has been omitted from the DVH analysis because it is not contoured.");
                        continue;
                    }

                    if (S.Id == DO.Rstruct)
                    {

                        //  System.Windows.Forms.MessageBox.Show("Struct match: " + Erika.Rstruct + " Name: " + Erika.DoseObjectiveName);
                        //  Thread.Sleep(3000);

                        if (DO.limit == "Max Pt Dose [voxel]")
                        {
                            string kstatus = null;
                            //  System.Windows.Forms.MessageBox.Show("Plan A Max Dose Voxel");
                            DVHData mDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);
                            double DM = mDVH.MaxDose.Dose;

                            DM = Math.Round(DM, 1, MidpointRounding.AwayFromZero);
                            double vxgoal = -15;
                            if (DO.goal != "NA")
                            {
                                vxgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double vxlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM < vxgoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM < vxlimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (DM < vxlimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM <= vxgoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM <= vxlimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (DM < vxlimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = DM, status = kstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit == "Max Pt Dose")        // MaxPtDose
                        {
                            string kstatus = null;
                            // System.Windows.Forms.MessageBox.Show("Plan A Max Dose");
                            //  Console.WriteLine("\nTRIGGER MAX PT Dose");
                            //   Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //Thread.Sleep(1000);

                            DVHData kDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);

                            //   Console.WriteLine("\n  DVH Point VOLUME UNIT: {0}", kDVH.CurveData[1].VolumeUnit.ToString());
                            //  Console.WriteLine("\n  NORMAL MAXDOSE: {0}", kDVH.MaxDose.ToString());
                            //  Thread.Sleep(10000);

                            double maxdose = -5000;
                            //  DoseValue maxdose = kDVH.MaxDose;

                            foreach (DVHPoint point in kDVH.CurveData)
                            {
                                if (point.Volume < 2.0 && point.Volume > 0.03)
                                {
                                    // Console.WriteLine("\n  DVH Point VOLUME: {0}", point.Volume);

                                    if (maxdose == -5000)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                    if (point.DoseValue.Dose > maxdose)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                }
                            }

                            if (maxdose == -5000)
                            {
                                System.Windows.Forms.MessageBox.Show("The structure " + S.Id + " has a volume less than 0.03 cc. Eclipse cannot reliably estimate dose to a volume this small, so any max point dose objectives for this structure will be marked as -5000 cGy, just to flag it as an issue.");
                            }

                            if ((DO.DoseObjectiveName == "Bladder_Max Pt Dose <= 5000cGy" | DO.DoseObjectiveName == "Rectum_Max Pt Dose < 5000cGy" | DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Dose Painted"))
                            {
                                DO.limval = Convert.ToString(1.15 * pdose);

                                if (DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy")
                                {
                                    DO.applystatus = false;
                                }
                            }
                            else if ((DO.DoseObjectiveName == "Bladder_Max Pt Dose <= 5000cGy" | DO.DoseObjectiveName == "Rectum_Max Pt Dose < 5000cGy" | DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Sequential Courses"))
                            {
                                DO.limval = Convert.ToString(1.10 * pdose);
                            }

                            if (DO.DoseObjectiveName == "Trachea_Bronc_Max Pt Dose <= 105 Percent Rx")
                            {
                                DO.limval = Convert.ToString(1.05 * pdose);
                            }

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                                // System.Windows.Forms.MessageBox.Show("Limval NA Trig");
                            }

                            if (DO.DoseObjectiveName == "Body Max Point Dose (Global Max)")
                            {
                                double bmper = Math.Round((maxdose / pdose) * 100.0, 1, MidpointRounding.AwayFromZero);
                                kstatus = bmper + "% of Rx dose";
                                goto bmperplanlabel;
                            }

                            maxdose = Math.Round(maxdose, 1, MidpointRounding.AwayFromZero);
                            double magoal = -15;
                            if (DO.goal != "NA")
                            {
                                magoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double malimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (maxdose < magoal)
                                        {
                                            kstatus = "PASS";
                                        }
                                        else
                                        {
                                            kstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if (maxdose < magoal)
                                        {
                                            kstatus = "PASS";
                                        }
                                        else if (maxdose < malimval)
                                        {
                                            kstatus = "REVIEW - GOAL";
                                        }
                                        else
                                        {
                                            kstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {
                                    if (maxdose < malimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose <= magoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose <= malimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (maxdose < malimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                        bmperplanlabel:
                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = maxdose, status = kstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;
                            // System.Windows.Forms.MessageBox.Show("Plan A Mean Dose");

                            DVHData jDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);
                            DoseValue meandose = jDVH.MeanDose;

                            double mdose = Math.Round(meandose.Dose, 1, MidpointRounding.AwayFromZero);
                            double mgoal = -15;
                            if (DO.goal != "NA")
                            {
                                mgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double mlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                jstatus = "";
                            }
                            else if (DO.strict == "<")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (mdose < mgoal)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (mdose < mgoal)
                                    {
                                        jstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (mdose < mlimval)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (mdose <= mgoal)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (mdose <= mlimval)
                                    {
                                        jstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (mdose < mlimval)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                            //  System.Windows.Forms.MessageBox.Show("Scorpia 2");
                        }
                        else if (DO.limit.StartsWith("CV"))
                        {
                            // System.Windows.Forms.MessageBox.Show("Plan A CV");
                            string Lstatus = null;
                            double Lcomp = 0.0;    //compare
                            double Lcomp2 = 0.0;
                            double Lvol = 0.0;
                            string type = "cm3";
                            double Llimit = 0.0;
                            double compvol = 0.0;

                            //   System.Windows.Forms.MessageBox.Show("Trig 1");

                            string jerry = DO.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);

                            Lcomp = Convert.ToDouble(DO.limval);  // VOLUME IN CM3

                            // System.Windows.Forms.MessageBox.Show("CV Llimit is: " + Llimit);

                            if (DO.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(DO.goal);   // VOLUME IN CM3
                            }

                            DoseValue Ldose = new DoseValue((Llimit * 100.0), DoseValue.DoseUnit.cGy);

                            //  System.Windows.Forms.MessageBox.Show("CV Ldose is: " + Ldose.Dose + "  Unit: " + Ldose.UnitAsString);

                            Lvol = Plan.GetVolumeAtDose(S, Ldose, VolumePresentation.AbsoluteCm3);

                            // System.Windows.Forms.MessageBox.Show("CV Lvol is: " + Lvol);

                            compvol = S.Volume - Lvol;

                            // System.Windows.Forms.MessageBox.Show("CV compvol is: " + compvol);

                            //   System.Windows.Forms.MessageBox.Show("Trig 3");

                            compvol = Math.Round(compvol, 1, MidpointRounding.AwayFromZero);
                            Lcomp = Math.Round(Lcomp, 1, MidpointRounding.AwayFromZero);
                            Lcomp2 = Math.Round(Lcomp2, 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                Lstatus = "";
                            }
                            else if (DO.strict == ">")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (compvol > Lcomp2)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else if ((compvol < Lcomp2) & (compvol > Lcomp))
                                    {
                                        Lstatus = "REVIEW - GOAL";
                                    }
                                    else if (compvol < Lcomp)
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (compvol > Lcomp)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = Lcomp, strict = DO.strict, goal = DO.goal, actvol = compvol, status = Lstatus, structvol = structvol, type = type, limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;
                            // System.Windows.Forms.MessageBox.Show("Plan A V");

                            if (DO.limit == "Volume")
                            {
                                //THIS IS SPECIFICALLY FOR THE "LIVER-GTV_VOLUME > 700CC" DOSE OBJECTIVE FOR SBRT LIVER PLANS
                                //  System.Windows.Forms.MessageBox.Show("Volume limit fire");

                                if (S.Volume > 700.0)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "REVIEW";
                                }

                                ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = 700.0, strict = DO.strict, goal = "NA", actvol = S.Volume, status = fstatus, structvol = structvol, type = "cm3", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                                continue;
                            }
                            else if (DO.limit == "V100%Rx")
                            {
                                // THIS IS SPECIFICALLY FOR THE "_CTV_V100%Rx>=100%" DOSE OBJECTIVE FOR SBRT LIVER PLANS
                                // System.Windows.Forms.MessageBox.Show("V100%Rx fire");

                                DoseValue tdose = new DoseValue(pdose, DoseValue.DoseUnit.cGy);
                                double ctvvol = Plan.GetVolumeAtDose(S, tdose, VolumePresentation.Relative);

                                if (ctvvol >= 100.0)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "REVIEW";
                                }

                                ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = 100.0, strict = DO.strict, goal = "NA", actvol = ctvvol, status = fstatus, structvol = structvol, type = "percent", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                                continue;
                            }
                            else if (DO.limit == "Veff" || DO.limit == "V60 is NOT Circumferential")
                            {
                                continue;
                            }

                            if (gyntype == "Sequential Courses" & DO.DoseObjectiveName == "SmBowel_Loops_V55 <= 15cc")
                            {
                                DO.applystatus = false;
                            }

                            //  DoseValue fdose = new DoseValue();
                            //  DoseValue gfdose = new DoseValue();
                            double Vgy = 0.0;
                            double gfvol = 0.0;
                            double fvol = 0.0;
                            double comp = 0.0;    //compare
                            double comp2 = 0.0;
                            double Vvol = 0.0;
                            string type = null;

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                            }

                            // System.Windows.Forms.MessageBox.Show("Trig 4");

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    

                            string jerry = DO.limit.Substring(1);
                            //System.Windows.Forms.MessageBox.Show("Plan jerry is: " + jerry);

                            try
                            {
                                if (DO.limit.EndsWith("%"))
                                {
                                    goto Pbreak;
                                }
                                Vgy = Convert.ToDouble(jerry);
                            }
                            catch (FormatException e)
                            {
                                Vgy = 0.0;
                                System.Windows.Forms.MessageBox.Show("An error occurred when attempting to convert the string \"" + DO.limit + "\" to a number for a dose objective with a limit that starts with the character \"V\". This is most likely due to a dose objective that was added to the list that this script has not been modified to handle. \n\n The value of this limit will be set to 0 to allow the program to continue working, however the information given by the program for this dose objective wil not be correct.");
                            }

                        //  System.Windows.Forms.MessageBox.Show("Plan Vgy is : " + Vgy);
                        Pbreak:
                            if (DO.limit.EndsWith("%"))
                            {
                                // This is for the Volume limits which are expressed as a percent of the prescribed dose, instead of in Gy as volume based limits usually are
                                type = "percent";
                                string token = jerry.TrimEnd('%');
                                //MessageBox.Show("before Token trim: " + jerry);
                                //MessageBox.Show("Token: " + token);
                                //MessageBox.Show("Limit ends with %");

                                double pda = Convert.ToDouble(token);
                                // MessageBox.Show("pda: " + pda);
                                pda = pda / 100.0;
                                double absdose = pda * pdose;  // absdose is now a dose in cGy
                                DoseValue dosep = new DoseValue(absdose, DoseValue.DoseUnit.cGy);
                                Vvol = Plan.GetVolumeAtDose(S, dosep, VolumePresentation.Relative);
                                //MessageBox.Show("Erika.limval: " + Erika.limval);
                                comp = Convert.ToDouble(DO.limval);

                                if (DO.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(DO.goal);
                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                goto CompareStart;
                            }
                            else if (DO.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(DO.limval);
                                //MessageBox.Show("Limit UNIT is %");
                                //MessageBox.Show("Comp is: " + comp);
                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the DoseObjective is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                //  System.Windows.Forms.MessageBox.Show("Trig 5");

                                if (DO.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(DO.goal);
                                    //MessageBox.Show("Comp2 is: " + comp2);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));
                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                //MessageBox.Show("Vgy is: " + Vgy);

                                DoseValue Vdose = new DoseValue((Vgy * 100.0), DoseValue.DoseUnit.cGy);

                                //MessageBox.Show("Vdose is: " + Vdose.Dose);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                //MessageBox.Show("Vvol is: " + Vvol);

                            }
                            else if (DO.limunit == "cc")
                            {
                                // System.Windows.Forms.MessageBox.Show("Trig 6");
                                type = "cm3";
                                comp = Convert.ToDouble(DO.limval);

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (DO.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(DO.goal);
                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                DoseValue Vdose = new DoseValue((Vgy * 100.0), DoseValue.DoseUnit.cGy);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);
                            }

                        // System.Windows.Forms.MessageBox.Show("Trig 7");
                        CompareStart:

                            Vvol = Math.Round(Vvol, 1, MidpointRounding.AwayFromZero);
                            comp = Math.Round(comp, 1, MidpointRounding.AwayFromZero);
                            comp2 = Math.Round(comp2, 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                fstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol < comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol > comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol < comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol < comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol < comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol <= comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol >= comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol <= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol <= comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol <= comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol >= comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol <= comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol >= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol >= comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol >= comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol > comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol < comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol > comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol > comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol > comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = comp, strict = DO.strict, goal = DO.goal, actvol = Vvol, status = fstatus, structvol = structvol, type = type, limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                            //  System.Windows.Forms.MessageBox.Show("end v type ");
                        }
                        else if (DO.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {
                            // System.Windows.Forms.MessageBox.Show("Plan A D");
                            string qstatus = null;
                            DoseValue qdose = new DoseValue();

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                            }

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = DO.limit.Substring(1);                     // "V gray" 

                            if (DO.limit.EndsWith("cc"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);

                                qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                // System.Windows.Forms.MessageBox.Show("qdose is: " + qdose);
                                // special case for dynamic Body-PTV D1cc objective (Liver only)
                                if (DO.DoseObjectiveName == "Body-PTV_D1cc <= 115%Rx")
                                {
                                    DO.limval = Convert.ToString(1.15 * pdose);
                                    DO.goal = Convert.ToString(1.10 * pdose);
                                }


                            }
                            else if (DO.limit.EndsWith("%"))
                            {
                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);
                                double qvol = (structvol * ((Convert.ToDouble(q2str)) / 100.0));           // specific volume that the DoseObjective is concerned with. Here, limval is the percent of the volume of the structure

                                qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);
                            }

                            double rqdose = Math.Round(qdose.Dose, 1, MidpointRounding.AwayFromZero);
                            double qgoal = -15;
                            if (DO.goal != "NA")
                            {
                                qgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double qlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                qstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (rqdose < qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (rqdose > qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (rqdose < qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((rqdose < qlimval) && (rqdose > qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (rqdose > qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (rqdose < qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (rqdose > qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (rqdose < qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (rqdose > qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((rqdose > qlimval) && (rqdose < qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (rqdose < qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (rqdose > qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">=")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (rqdose >= qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (rqdose <= qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (rqdose >= qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((rqdose >= qlimval) && (rqdose <= qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (rqdose <= qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (rqdose >= qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (rqdose <= qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (rqdose >= qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if ((rqdose <= qgoal))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((rqdose <= qlimval) && (rqdose >= qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (rqdose <= qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = qdose.Dose, status = qstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
            }// Ends ROIE iterator loop
            return ROIA;
        }



        //=======================================================================================================================================================================================================================================================================================
        //==============================PlanSum Dose Objective Analysis====================================================================================================================================================================================================================================



        public static List<DoseObjective> PlansumAnalysis(string laterality, string[] Si, string ptype, VMS.TPS.Common.Model.API.Patient patient, Course course, StructureSet structureSet, PlanSum Plansum, int dt, double dd, TextBox OuputBox, string gyntype, ProgressBar pBar, bool UseGoals)
        {
            List<DoseObjective> ROIE = new List<DoseObjective>();     // Expected ROI made from text file list
            List<DoseObjective> ROIA = new List<DoseObjective>();     // Actual ROI list from Eclipse 
            string Ttype = ptype;
            string Tsite = null;
            //  string [] Si = new string[50]; 

            // This calls LISTMAKER, which generates a list of Dose Objectives for the given treatment type and site
             ROIE = LISTMAKER.Listmaker(Ttype, Tsite, Si, laterality, patient.Id);

            // If the User wants the program to evaluate dose objectives they have made using the clinical goals feature in Eclipse, the code below handles that
            // It simply adds their clincial goals to the standard list. If they want to change a dose objective that is already on the list, they can use the PCTPN Document workflow
            // basically it takes in the string representing the clinical goal and breaks it apart to assign the neccessary info to the appropriate parts of the Dose Objective class
            
            //if (UseGoals == true)
            //{
            //    //take the clinical goals and add them to the dose objective list
            //    foreach (ClinicalGoal CG in Plansum.GetClinicalGoals())
            //    {
            //        string limit = null;
            //        string limunit = null;
            //        string strict = null;
            //        string limitval = null;

            //        if (CG.ObjectiveAsString.StartsWith("Conformity Index") || CG.ObjectiveAsString.StartsWith("Gradient Measure") || CG.ObjectiveAsString.StartsWith("GM") || CG.ObjectiveAsString.StartsWith("CI"))
            //        {
            //            continue;
            //        }

            //        try
            //        {
            //            if (CG.ObjectiveAsString.StartsWith("Dmax"))
            //            {
            //                limit = "Max Pt Dose [voxel]";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Maximum Dose"))
            //            {
            //                limit = "Max Pt Dose [voxel]";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Mean Dose"))
            //            {
            //                limit = "Mean Dose";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("Dmean"))
            //            {
            //                limit = "Mean Dose";

            //                string[] wrdbrk = new string[4];
            //                wrdbrk = CG.ObjectiveAsString.Split(' ');
            //                limunit = wrdbrk[3];
            //                limitval = wrdbrk[2];
            //                strict = wrdbrk[1];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("V"))
            //            {
            //                string[] wrdbrk = new string[6];

            //                wrdbrk = CG.ObjectiveAsString.Split(' ');

            //                if (wrdbrk[2] == "%")
            //                {
            //                    limit = wrdbrk[0] + wrdbrk[1] + wrdbrk[2];
            //                }
            //                else
            //                {
            //                    limit = wrdbrk[0] + wrdbrk[1];
            //                }

            //                limunit = wrdbrk[5];
            //                limitval = wrdbrk[4];
            //                strict = wrdbrk[3];
            //            }

            //            if (CG.ObjectiveAsString.StartsWith("D "))
            //            {
            //                string[] wrdbrk = new string[6];

            //                wrdbrk = CG.ObjectiveAsString.Split(' ');

            //                limit = wrdbrk[0] + wrdbrk[1] + wrdbrk[2];

            //                limunit = wrdbrk[5];
            //                limitval = wrdbrk[4];
            //                strict = wrdbrk[3];
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            MessageBox.Show("Clincal goals conversion error.\n\n" + e.ToString() + "\n" + e.StackTrace);
            //        }


            //        if (strict == "\u2265")
            //        {
            //            strict = ">=";
            //        }
            //        else if (strict == "\u2264")
            //        {
            //            strict = "<=";
            //        }

            //        string CGname = CG.StructureId + "_" + limit + " " + strict + " " + limitval + limunit;

            //        //MessageBox.Show("Name: " + CGname);
            //        //MessageBox.Show("Struct Id: " + CG.StructureId);
            //        //MessageBox.Show("limit: " + limit);
            //        //MessageBox.Show("strict: " + strict);
            //        //MessageBox.Show("limitval: " + limitval);
            //        //MessageBox.Show("limitunit: " + limunit);

            //        string[] treatsiteclinical = new string[1];
            //        treatsiteclinical[0] = "Clinical Goal";

            //        ROIE.Add(new DoseObjective { DoseObjectiveName = CGname, Rstruct = CG.StructureId, DoseObjectiveID = 5000, limit = limit, limval = limitval, strict = strict, limunit = limunit, status = null, goal = "NA", treatsite = treatsiteclinical, applystatus = true });
            //    }
            //    //ROIL.Add(new DoseObjective { DoseObjectiveName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite, applystatus = true });
            //}
            

            //this gets the progress bar going on the GUI
            pBar.Style = ProgressBarStyle.Continuous;
            pBar.Visible = true;
            pBar.Minimum = 0;
            pBar.Value = 0;
            pBar.Maximum = ROIE.Count;
            pBar.Step = 1;

            double dosesum = 0.0;
            string dunit = null;

            //This figures out what the effective total dose for this plansum is. This matters for dynamic limits with Gynecological plans.
            if (dt == 1)
            {
                foreach (PlanSetup aplan in Plansum.PlanSetups)
                {
                    dosesum += aplan.TotalDose.Dose;
                    dunit = aplan.TotalDose.UnitAsString;
                }
            }
            else if (dt == 2)
            {
                IEnumerator lk = Plansum.PlanSetups.GetEnumerator();
                lk.MoveNext();
                PlanSetup PS = (PlanSetup)lk.Current;
                dosesum = PS.TotalDose.Dose;
                dunit = PS.TotalDose.UnitAsString;
            }
            else if (dt == 3)
            {
                dosesum = dd;
            }

            int county = 0;

            //This is code that makes sure the dose objectives reflect the diagnosis of the patient
            //currently not implemented because the doctor's don't assign the diagnosis to the course, so there is no way for ESAPI to know

            //bool lymphomapatient = false;
            //bool oralpatient = false;
            //foreach (Diagnosis dg in course.Diagnoses)
            //{
            //    if (dg.ClinicalDescription.Contains("lymphoma") || dg.ClinicalDescription.Contains("Lymphoma"))
            //    {
            //        lymphomapatient = true;
            //    }

            //    //long list here to cover all 14 of the "Oral" malignant Neoplasms in ICD-10-CM
            //    if (dg.ClinicalDescription.Contains("lip") || dg.ClinicalDescription.Contains("Lip") || dg.ClinicalDescription.Contains("tongue") || dg.ClinicalDescription.Contains("Tongue") || dg.ClinicalDescription.Contains("gum") || dg.ClinicalDescription.Contains("Gum") || dg.ClinicalDescription.Contains("mouth") || dg.ClinicalDescription.Contains("Mouth") || dg.ClinicalDescription.Contains("palate") || dg.ClinicalDescription.Contains("Palate") || dg.ClinicalDescription.Contains("tonsil") || dg.ClinicalDescription.Contains("Tonsil") || dg.ClinicalDescription.Contains("salivary") || dg.ClinicalDescription.Contains("Salivary") || dg.ClinicalDescription.Contains("oropharynx") || dg.ClinicalDescription.Contains("Oropharynx") || dg.ClinicalDescription.Contains("nasopharynx") || dg.ClinicalDescription.Contains("Nasopharynx") || dg.ClinicalDescription.Contains("hypopharynx") || dg.ClinicalDescription.Contains("Hypopharynx") || dg.ClinicalDescription.Contains("sinus") || dg.ClinicalDescription.Contains("Sinus") || dg.ClinicalDescription.Contains("pharynx") || dg.ClinicalDescription.Contains("Pharynx") || dg.ClinicalDescription.Contains("oral") || dg.ClinicalDescription.Contains("Oral") || dg.ClinicalDescription.Contains("parotid") || dg.ClinicalDescription.Contains("Parotid"))
            //    {
            //        oralpatient = true;
            //    }
            //}

            //for (int k = 0; k < ROIE.Count; k++)
            //{
            //    if (lymphomapatient == false && ROIE[k].DoseObjectiveName.Contains("[lymphoma patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //    }

            //    if (oralpatient == true && ROIE[k].DoseObjectiveName.Contains("[non-oral patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //    }

            //    if (oralpatient == false && ROIE[k].DoseObjectiveName.Contains("[oral patients]"))
            //    {
            //        ROIE.RemoveAt(k);
            //    }
            //}

           // ROIE.Sort();
           // Need to implement IComparable to sort

            //Loops through all the dose objectives from the list generated by LISTMAKER
            foreach (DoseObjective DO in ROIE)
            {
                county++;
                // OuputBox.AppendText(Environment.NewLine);
                // OuputBox.AppendText("Dose Objectives checked: " + county + "/" + ROIE.Count);

                //Loops through each structure for each dose objective until the structure that the objective pertains to is found.
                foreach (Structure S in structureSet.Structures)        // iterates through all the structures in the structureset of the current Plan
                {
                    double structvol = S.Volume;

                    //This is important. This checks to make sure the structure is actually contoured, because empty contours can exist in Eclipse
                    //accessing an empty structure would throw an error
                    if (S.IsEmpty == true || S.Volume < 0.0)
                    {
                        // MessageBox.Show("The structure " + S.Id + " has been omitted from the DVH analysis because it is not contoured.");
                        continue;
                    }

                    //If the structure matches the structure of the current objective, the analysis begins
                    //The dose objective analysis is broken up by the kind of objective
                    //The first is Max Pt Dose [voxel], which is the absolute highest dose for that structure, according to Eclipse
                    if (S.Id == DO.Rstruct)
                    {
                        if (DO.limit == "Max Pt Dose [voxel]")
                        {
                            string kstatus = null;
                            //System.Windows.Forms.MessageBox.Show("Plan A Max Dose Voxel");

                            //This is anachronistic now, but when i first made this the normal DVH methods didn't work with plansums
                            //This method below pulls all the DVH data for a given structure, and puts it into an object of a DVHData class.
                            //This object has basic things like max dose, but for most objectives you need to loop though all the pints in the DVH curve
                            //and find what you are looking for.
                            DVHData mDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);
                            double DM = mDVH.MaxDose.Dose;

                            //This logic here is repeated for the different types of objectives
                            //It determines if the objective passes, fails, or doesn't meet the goal, if applicable

                            DM = Math.Round(DM, 1, MidpointRounding.AwayFromZero);
                            double vxgoal = -15;
                            if (DO.goal != "NA")
                            {
                                vxgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double vxlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM < vxgoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM < vxlimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (DM < vxlimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM <= vxgoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM <= vxlimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (DM < vxlimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            //This line is very important. It adds all the info calculated for this objective to the list of analyized objectives
                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = DM, status = kstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit == "Max Pt Dose")
                        {
                            //This analyizes Max Pt Dose objectives, which unlike the Max Pt Dose [voxel], is limited to the max dose for a volume greater than 0.03 cc, which is the size of a voxel
                            //in Eclipse. The DVH algorithim cannot reliably calclulate dose to volumes this small, so we cut it off.
                            //maxdose is initially set to -5000, so that if a maxdose is not found, because the structure is too small (this does happen rarely), it will trigger the if statement below to alert the user.
                            string kstatus = null;
                            double maxdose = -5000;
                            DVHData kDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);

                            //This loops through the DVH curve to the max dose at a volume greater than 0.03 cc
                            foreach (DVHPoint point in kDVH.CurveData)
                            {
                                if (point.Volume < 2.0 && point.Volume > 0.03)
                                {
                                    if (maxdose == -5000)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                    if (point.DoseValue.Dose > maxdose)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                }
                            }

                            if (maxdose == -5000)
                            {
                                System.Windows.Forms.MessageBox.Show("The structure " + S.Id + " has a volume less than 0.03 cc. Eclipse cannot reliably estimate dose to a volume this small, so any max point dose objectives for this structure will be marked as -5000 cGy, just to flag it as an issue.");
                            }

                            //This code here is for special dynamically calculated dose objectives, for Gynecological plans
                            if ((DO.DoseObjectiveName == "Bladder_Max Pt Dose <= 5000cGy" | DO.DoseObjectiveName == "Rectum_Max Pt Dose < 5000cGy" | DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Dose Painted"))
                            {
                                DO.limval = Convert.ToString(1.15 * dosesum);

                                if (DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy")
                                {
                                    DO.applystatus = false;
                                }
                            }
                            else if ((DO.DoseObjectiveName == "Bladder_Max Pt Dose <= 5000cGy" | DO.DoseObjectiveName == "Rectum_Max Pt Dose < 5000cGy" | DO.DoseObjectiveName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Sequential Courses"))
                            {
                                DO.limval = Convert.ToString(1.10 * dosesum);
                            }

                            //This is a separate dynamic objective that is not Gynecological
                            if (DO.DoseObjectiveName == "Trachea_Bronc_Max Pt Dose <= 105% Rx")
                            {
                                DO.limval = Convert.ToString(1.05 * dosesum);
                            }

                            //This is for the Body Max Point Dose. we want to include the percentage of the RX dose that this max is for reporting purposes, so we put into status, that way it shows up in the PTV
                            //We do this instead of coming up with a way to include it in the Dose coverage statistics, because it was a preexisting objective

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                            }

                            if (DO.DoseObjectiveName == "Body Max Point Dose (Global Max)")
                            {
                                double bmper = Math.Round((maxdose / dosesum) * 100.0, 1, MidpointRounding.AwayFromZero);
                                kstatus = bmper + "% of Rx dose";
                                goto bmperplansumlabel;
                            }

                            maxdose = Math.Round(maxdose, 1, MidpointRounding.AwayFromZero);
                            double magoal = -15;
                            if (DO.goal != "NA")
                            {
                                magoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double malimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (maxdose < magoal)
                                        {
                                            kstatus = "PASS";
                                        }
                                        else
                                        {
                                            kstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if (maxdose < magoal)
                                        {
                                            kstatus = "PASS";
                                        }
                                        else if (maxdose < malimval)
                                        {
                                            kstatus = "REVIEW - GOAL";
                                        }
                                        else
                                        {
                                            kstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {
                                    if (maxdose < malimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose <= magoal)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose <= malimval)
                                    {
                                        kstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (maxdose < malimval)
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                        bmperplansumlabel:
                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = maxdose, status = kstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;
                            DVHData jDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);
                            DoseValue meandose = jDVH.MeanDose;

                            double mdose = Math.Round(meandose.Dose, 1, MidpointRounding.AwayFromZero);
                            double mgoal = -15;
                            if (DO.goal != "NA")
                            {
                                mgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double mlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                jstatus = "";
                            }
                            else if (DO.strict == "<")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (mdose < mgoal)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (mdose < mgoal)
                                    {
                                        jstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (mdose < mlimval)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (mdose <= mgoal)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (mdose <= mlimval)
                                    {
                                        jstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (mdose < mlimval)
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit.StartsWith("CV"))
                        {
                            //This is for Complementary Volume objectives. These are opposite all other objective types, where you are looking to see if the value is above the limit, not below

                            string Lstatus = null;
                            double Lcomp = 0.0;    //compare
                            double Lcomp2 = 0.0;
                            double Lvol = 0.0;
                            double Ldose = 0.0;    //functional dose
                            string type = "cm3";
                            double Llimit = 0.0;
                            double compvol = 0.0;

                            string jerry = DO.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);    // in Gy
                            Lcomp = Convert.ToDouble(DO.limval);  // VOLUME IN CM3

                            if (DO.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(DO.goal);   // VOLUME IN CM3
                            }

                            Ldose = Llimit * 100.0;
                            DVHData Ldvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);
                            foreach (DVHPoint point in Ldvh.CurveData)
                            {
                                if ((point.DoseValue.Dose >= (Ldose - 0.5)) && (point.DoseValue.Dose <= (Ldose + 0.5)))
                                {
                                    Lvol = point.Volume;
                                }
                            }

                            compvol = S.Volume - Lvol;

                            compvol = Math.Round(compvol, 1, MidpointRounding.AwayFromZero);
                            Lcomp = Math.Round(Lcomp, 1, MidpointRounding.AwayFromZero);
                            Lcomp2 = Math.Round(Lcomp2, 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                Lstatus = "";
                            }
                            else if (DO.strict == ">")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {

                                    if (compvol > Lcomp2)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else if ((compvol < Lcomp2) & (compvol > Lcomp))
                                    {
                                        Lstatus = "REVIEW - GOAL";
                                    }
                                    else if (compvol < Lcomp)
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (compvol > Lcomp)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = Lcomp, strict = DO.strict, goal = DO.goal, actvol = compvol, status = Lstatus, structvol = structvol, type = type, limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }
                        else if (DO.limit.StartsWith("V"))
                        {
                            //This is Volume-based objectives, the most complicated. 
                            //This is an example of the syntax of what these objectives mean: V45 - 45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve

                            string fstatus = null;

                            if (DO.limit == "Volume")
                            {
                                //THIS IS SPECIFICALLY FOR THE "LIVER-GTV_VOLUME > 700CC" DOE OBJECTIVE FOR SBRT LIVER PLANS
                                if (S.Volume > 700.0)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "REVIEW";

                                }

                                ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = 700.0, strict = DO.strict, goal = "NA", actvol = S.Volume, status = fstatus, structvol = structvol, type = "cm3", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                                continue;
                            }
                            else if (DO.limit == "V100%Rx")
                            {
                                // THIS IS SPECIFICALLY FOR THE "_CTV_V100%Rx>=100%" DOSE OBJECTIVE FOR SBRT LIVER PLANS
                                double ctvvol = 0.0;
                                DoseValue tdose = new DoseValue(dosesum, DoseValue.DoseUnit.cGy);
                                DVHData ctvdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                foreach (DVHPoint point in ctvdvh.CurveData)
                                {
                                    if ((point.DoseValue.Dose >= (dosesum - 0.2)) && (point.DoseValue.Dose <= (dosesum + 0.2)))
                                    {
                                        ctvvol = point.Volume;
                                    }
                                }

                                if (ctvvol >= 100.0)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "REVIEW";

                                }

                                ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = 100.0, strict = DO.strict, goal = "NA", actvol = ctvvol, status = fstatus, structvol = structvol, type = "percent", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                                continue;
                            }
                            else if (DO.limit == "Veff" || DO.limit == "V60 is NOT Circumferential")
                            {
                                //These are objectives that are outside of the Script's ability to handle. Veff is actually calculated by NTCP calculator script, but not here.
                                continue;
                            }

                            if (gyntype == "Sequential Courses" & DO.DoseObjectiveName == "SmBowel_Loops_V55 <= 15cc")
                            {
                                DO.applystatus = false;
                            }

                            //  DoseValue fdose = new DoseValue();
                            //  DoseValue gfdose = new DoseValue();
                            double Vgy = 0.0;
                            double gfvol = 0.0;
                            double fvol = 0.0;
                            double comp = 0.0;    //compare
                            double comp2 = 0.0;
                            double Vvol = 0.0;
                            double fdose = 0.0;    //functional dose
                            string type = null;

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                            }

                            string jerry = DO.limit.Substring(1);
                            try
                            {
                                if (DO.limit.EndsWith("%"))
                                {
                                    goto Pbreak;
                                }
                                Vgy = Convert.ToDouble(jerry) * 100.0;   //multiplied by 100 to convert to cGy. For plansums we do this here. For the plan analysis it is done in the respective IF statements.
                            }
                            catch (FormatException e)
                            {
                                Vgy = 0.0;
                                System.Windows.Forms.MessageBox.Show("An error occurred when attempting to convert the string \"" + DO.limit + "\" to a number for a dose objective with a limit that starts with the character \"V\". This is most likely due to a dose objective that was added to the list that this script has not been modified to handle. \n\n The value of this limit will be set to 0 to allow the program to continue working, however the information given by the program for this dose objective wil not be correct.");
                            }

                        // System.Windows.Forms.MessageBox.Show("Plan Vgy is : " + Vgy);
                        Pbreak:
                            if (DO.limit.EndsWith("%"))
                            {
                                // This is for the Volume limits which are expressed as a percent of the prescribed dose, instead of in Gy as volume based limits usually are
                                type = "percent";
                                string token = jerry.TrimEnd('%');
                                //MessageBox.Show("before Token trim: " + jerry);
                                //MessageBox.Show("Token: " + token);
                                //MessageBox.Show("Limit ends with %");

                                double pda = Convert.ToDouble(token);
                                // MessageBox.Show("pda: " + pda);
                                pda = pda / 100.0;
                                double absdose = pda * dosesum;  // absdose is now a dose in cGy

                                DVHData specdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
                                foreach (DVHPoint point in specdvh.CurveData)
                                {
                                    if ((point.DoseValue.Dose >= (absdose - 0.2)) && (point.DoseValue.Dose <= (absdose + 0.2)))
                                    {
                                        Vvol = point.Volume;
                                    }
                                }

                                //MessageBox.Show("Erika.limval: " + Erika.limval);
                                comp = Convert.ToDouble(DO.limval);

                                if (DO.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(DO.goal);
                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                goto CompareStart;
                            }
                            if (DO.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(DO.limval);

                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the DoseObjective is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                if (DO.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(DO.goal);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));

                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                //  Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
                                foreach (DVHPoint point in Vdvh.CurveData)
                                {

                                    if ((point.DoseValue.Dose >= (Vgy - 0.2)) && (point.DoseValue.Dose <= (Vgy + 0.2)))
                                    {
                                        Vvol = point.Volume;
                                    }
                                }
                            }
                            else if (DO.limunit == "cc")
                            {
                                type = "cm3";

                                comp = Convert.ToDouble(DO.limval);  // VOLUME IN CM3

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (DO.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(DO.goal);   // VOLUME IN CM3

                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                // Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);
                                foreach (DVHPoint point in Vdvh.CurveData)
                                {
                                    if ((point.DoseValue.Dose >= (Vgy - 0.2)) && (point.DoseValue.Dose <= (Vgy + 0.2)))
                                    {
                                        Vvol = point.Volume;

                                    }
                                }
                            }

                        CompareStart:
                            Vvol = Math.Round(Vvol, 1, MidpointRounding.AwayFromZero);
                            comp = Math.Round(comp, 1, MidpointRounding.AwayFromZero);
                            comp2 = Math.Round(comp2, 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                fstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol < comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol > comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol < comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol < comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol < comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol <= comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol >= comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol <= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol <= comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol <= comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol >= comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol <= comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol >= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol >= comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol >= comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (Vvol > comp2)
                                        {
                                            fstatus = "PASS";
                                        }
                                        else if (Vvol < comp2)
                                        {
                                            fstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (Vvol > comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol > comp)
                                    {
                                        fstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (Vvol > comp)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limvol = comp, strict = DO.strict, goal = DO.goal, actvol = Vvol, status = fstatus, structvol = structvol, type = type, limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });
                        }
                        else if (DO.limit.StartsWith("D"))
                        {
                            //This is for Dosed-based objectives
                            //Syntax example: D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit

                            string qstatus = null;
                            double qdose = 0.0;

                            if (DO.limval == "NA")
                            {
                                DO.limval = "-1";
                            }

                            string qstring = DO.limit.Substring(1);       // "V gray" 

                            if (DO.limit.EndsWith("cc"))
                            {
                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);
                                double qvol = Convert.ToDouble(q2str);
                                // qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);
                                foreach (DVHPoint point in Qdvh.CurveData)
                                {
                                    if ((point.Volume >= (qvol - 0.3)) && (point.Volume <= (qvol + 0.3)))
                                    {
                                        qdose = point.DoseValue.Dose;
                                    }
                                }

                                // special case for dynamic Body-PTV D1cc objective (Liver only)
                                if (DO.DoseObjectiveName == "Body-PTV_D1cc <= 115%Rx")
                                {
                                    DO.limval = Convert.ToString(1.15 * dosesum);
                                    DO.goal = Convert.ToString(1.10 * dosesum);
                                }

                            }
                            else if (DO.limit.EndsWith("%"))
                            {
                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);
                                double qvol = Convert.ToDouble(q2str);

                                // qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
                                foreach (DVHPoint point in Qdvh.CurveData)
                                {
                                    if ((point.Volume >= (qvol - 0.3)) && (point.Volume <= (qvol + 0.3)))
                                    {
                                        qdose = point.DoseValue.Dose;
                                    }
                                }
                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);
                            }

                            qdose = Math.Round(qdose, 1, MidpointRounding.AwayFromZero);
                            double qgoal = -15;
                            if (DO.goal != "NA")
                            {
                                qgoal = Math.Round(Convert.ToDouble(DO.goal), 1, MidpointRounding.AwayFromZero);
                            }
                            double qlimval = Math.Round(Convert.ToDouble(DO.limval), 1, MidpointRounding.AwayFromZero);

                            if (DO.strict == "[record]")
                            {
                                qstatus = "";
                            }
                            else if (DO.strict == "<")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (qdose < qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose > qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose < qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose < qlimval) && (qdose > qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose > qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (qdose < qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (qdose > qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose < qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose > qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose > qlimval) && (qdose < qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose < qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (qdose > qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == ">=")
                            {

                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (qdose >= qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose <= qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose >= qgoal)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose >= qlimval) && (qdose <= qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose <= qlimval)
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (qdose >= qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (DO.strict == "<=")
                            {
                                if (DO.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DO.limval == "-1")
                                    {
                                        if (qdose <= qgoal)
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose >= qgoal)
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if ((qdose <= qgoal))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose <= qlimval) && (qdose >= qgoal))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (qdose <= qlimval)
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, limdose = Convert.ToDouble(DO.limval), strict = DO.strict, goal = DO.goal, actdose = qdose, status = qstatus, structvol = structvol, type = "NV", limunit = DO.limunit, applystatus = DO.applystatus, treatsite = DO.treatsite });

                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
                pBar.PerformStep();
            }// Ends ROIE iterator loop


            // Code which gets data from Eclipse ends here. Below this is the ouput for the DoseObjective comparison.

            return ROIA;
        }
    }
}

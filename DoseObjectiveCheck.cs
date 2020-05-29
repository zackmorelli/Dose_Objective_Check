using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using GUI;
using ROI;
using DoseObjectiveCheck;





/*
    Lahey RadOnc Dose Objective Checker - DoseObjectiveCheck (MAIN PROGRAM)
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
    Uses space-delimited text files to run a dose objective check (using the department's unique criteria) on an external beam Plan (or Plansum) in Eclipse using some user-provided input. 
    This program also generates a report of the dose objective check as a PDF, which includes a standard header used to identify the patient/Plan that the report was made for.
    The code used to generate the PDF is based off a program developed by Carlos J Anderson and obtained from him via his website. The helper classes which originally came from Carlos are explicitly labeled as such.
    Otherwise, everything else was solely written by Zackary T Morelli, including this program (DoseObjectiveCheck), which performs the actual analysis of the "Dose Objective Check" and is the main program which runs in Eclipse when initiated by the User.
    The GUI that is used by the User is called from this program.
*/

namespace VMS.TPS
{
    public class Script  // creates a class called Script within the VMS.TPS Namesapce
    {

        public Script() { }  // instantiates a Script class

        // Assembly Commands

        [MethodImpl(MethodImplOptions.NoInlining)]       // prevents compiler optimization from messing with the program's methods. Protection for eventual Eclipse 15.5 upgrade

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]          // This stuff manually imports the Windows Kernel and then sets up a function that instantiates a cmd line
        static extern bool AllocConsole();

        // Declaration Space for things outside of "Execute" class




        public class TreatSite : IEquatable<TreatSite>        //makes a treatment site class used to make a list of treatment sites 
        {                                                     //the treatment site list is used when the user is prompted to choose the treatment site of the selected Plan
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public int Id { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                TreatSite objAsTreatSite = obj as TreatSite;
                if (objAsTreatSite == null) return false;
                else return Equals(objAsTreatSite);
            }
            public override int GetHashCode()
            {
                return Id;
            }
            public bool Equals(TreatSite other)
            {
                if (other == null) return false;
                return (this.Id.Equals(other.Id));
            }
            // Should also override == and != operators.

        }

        public static List<TreatSite> MakelistConv()           //treatment site list for Conventional plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Abdomen", Name = "Abdomen", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain", Name = "Brain", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain Hypofx", Name = "BrainHypofx", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast 23+fx", Name = "Breast23+fx", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast Hypofx", Name = "BreastHypofx", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "Esophagus", Name = "Esophagus", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "Gynecological", Name = "Gynecological", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Head & Neck", Name = "Head&Neck", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung", Name = "Lung", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis (Other)", Name = "Pelvis(Other)", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 25fx + HDR", Name = "PelivsEBRT25fx+HDR", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 15fx + HDR", Name = "PelivsEBRT15fx+HDR", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate", Name = "Prostate", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Bed", Name = "ProstateBed", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20fx", Name = "ProstateHypo20fx", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28fx", Name = "ProstateHypo28fx", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax (Other)", Name = "Thorax(Other)", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 1", Name = "ProstateNRGProtArm1", Id = 18 });

            return treatsite;
        }

        public static List<TreatSite> MakelistSRS()           //treatment site list for SRS plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Single fraction", Name = "Singlefraction", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "3 fraction", Name = "3fraction", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "4 fraction", Name = "4fraction", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "5 fraction", Name = "5fraction", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "6 fraction", Name = "6fraction", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "8 fraction", Name = "8fraction", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "10 fraction", Name = "10fraction", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Liver", Name = "Liver", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 4 fraction", Name = "Lung4fraction", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 5 fraction", Name = "Lung5fraction", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 8 fraction", Name = "Lung8fraction", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 1 fraction", Name = "Oligomets1fraction", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 3 fractions", Name = "Oligomets3fractions", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 5 fractions", Name = "Oligomets5fractions", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Pancreas", Name = "Pancreas", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 1 fraction", Name = "SRSCranial1fraction", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 3 fraction", Name = "SRSCranial3fraction", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 5 fraction", Name = "SRSCranial5fraction", Id = 18 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial AVM", Name = "SRSCranialAVM", Id = 19 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial Trigeminal Neuralgia", Name = "SRSCranialTrigeminalNeuralgia", Id = 20 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 2", Name = "ProstateNRGProtArm2", Id = 21 });

            return treatsite;
        }

        public static List<TreatSite> MakelistBoth()           //treatment site list for Both
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Abdomen", Name = "Abdomen", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain", Name = "Brain", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain Hypofx", Name = "BrainHypofx", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast 23+fx", Name = "Breast23+fx", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast Hypofx", Name = "BreastHypofx", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "Esophagus", Name = "Esophagus", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "Gynecological", Name = "Gynecological", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Head & Neck", Name = "Head&Neck", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung", Name = "Lung", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis (Other)", Name = "Pelvis(Other)", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 25fx + HDR", Name = "PelivsEBRT25fx+HDR", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 15fx + HDR", Name = "PelivsEBRT+HDR", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate", Name = "Prostate", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Bed", Name = "ProstateBed", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20fx", Name = "ProstateHypo20fx", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28fx", Name = "ProstateHypo28fx", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax (Other)", Name = "Thorax(Other)", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 1", Name = "ProstateNRGProtArm1", Id = 18 });
            treatsite.Add(new TreatSite() { DisplayName = " Single fraction", Name = "Singlefraction", Id = 19 });
            treatsite.Add(new TreatSite() { DisplayName = "3 fraction", Name = "3fraction", Id = 20 });
            treatsite.Add(new TreatSite() { DisplayName = "4 fraction", Name = "4fraction", Id = 21 });
            treatsite.Add(new TreatSite() { DisplayName = "5 fraction", Name = "5fraction", Id = 22 });
            treatsite.Add(new TreatSite() { DisplayName = "6 fraction", Name = "6fraction", Id = 23 });
            treatsite.Add(new TreatSite() { DisplayName = "8 fraction", Name = "8fraction", Id = 24 });
            treatsite.Add(new TreatSite() { DisplayName = "10 fraction", Name = "10fraction", Id = 25 });
            treatsite.Add(new TreatSite() { DisplayName = "Liver", Name = "Liver", Id = 26 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 4 fraction", Name = "Lung4fraction", Id = 27 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 5 fraction", Name = "Lung5fraction", Id = 28 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 8 fraction", Name = "Lung8fraction", Id = 29 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 1 fraction", Name = "Oligomets1fraction", Id = 30 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 3 fractions", Name = "Oligomets3fractions", Id = 31 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 5 fractions", Name = "Oligomets5fractions", Id = 32 });
            treatsite.Add(new TreatSite() { DisplayName = "Pancreas", Name = "Pancreas", Id = 33 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 1 fraction", Name = "SRSCranial1fraction", Id = 34 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 3 fraction", Name = "SRSCranial3fraction", Id = 35 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 5 fraction", Name = "SRSCranial5fraction", Id = 36 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial AVM", Name = "SRSCranialAVM", Id = 37 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial Trigeminal Neuralgia", Name = "SRSCranialTrigeminalNeuralgia", Id = 38 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 2", Name = "ProstateNRGProtArm2", Id = 39 });

            return treatsite;
        }


        static bool Discriminator(IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, User user)
        {

            bool D = false;
            int cnt = 0;
            string input = null;

            AllocConsole();
            Console.Title = "Lahey RadOnc Dose Objective Checker  V 1.0";

            Console.SetWindowSize(200, 75);                                 //these specific values are here for a reason, don't change them
            Console.SetBufferSize(200, 75);

            Console.WriteLine(" Hi {0}, Welcome to the Lahey RadOnc Dose Objective Checker  V 1.0 \n \n", user.Name);

            Thread.Sleep(1000);

            foreach (PlanSum aplansum in Plansums)
            {
                cnt++;
            }

            if (cnt > 0)
            {
                Console.WriteLine("\nA Plan sum has been dectected in your current Scope. \n");
                Thread.Sleep(600);

                Console.WriteLine("Do you want to run an analysis on this Plan sum?\n");
                Console.WriteLine("(Enter Y for Yes or N for NO): ");
                input = Console.ReadLine();

                while (input != "y" & input != "Y" & input != "N" & input != "n")
                {
                    Console.WriteLine("Let's try that again. You must enter either Y for YES or N for NO.");
                    input = Console.ReadLine();
                }
                if (input == "y" | input == "Y")
                {
                    D = true;
                }
                if (input == "n" | input == "N")
                {
                    D = false;
                }
            }
            else
            {
                D = false;
            }
            return D;
        }

        static PlanSum PlansumPick(IEnumerable<PlanSum> Plansums)
        {
            IEnumerator TR = Plansums.GetEnumerator();
            TR.MoveNext();
            PlanSum Plansum = (PlanSum)TR.Current;             //creates an enumerator that selects the first plansum
            return Plansum;
        }

        static PlanSetup PlanPick(IEnumerable<PlanSetup> Plans)
        {
            int nt = 0;
            int count = 0;
            int inp = 0;

            IEnumerator TP = Plans.GetEnumerator();    //this is just here to instantiate "Plan"
            TP.MoveNext();
            PlanSetup Plan = (PlanSetup)TP.Current;            //creates an enumerator that selects the first Plan

            foreach (PlanSetup aplansetup in Plans)
            {
                nt++;
            }

            if (nt == 1)
            {

                // Does nothing. First Plan in enumerator list has already been selected and put into "Plan"

            }
            else if (nt > 1)
            {
                Console.WriteLine("\n\nMultiple plans have been detected in your current Scope.\n");
                foreach (PlanSetup aplansetup in Plans)
                {
                    count++;
                    Console.WriteLine("\n {0}.  {1}", count, aplansetup.Name);
                }

                Console.WriteLine("\n\nWhich Plan would you like to perform an analysis on?");
                Console.WriteLine("\n(Enter the number associated with your Plan): ");
                inp = Convert.ToInt32(Console.ReadLine());

                while ((inp > count) & (inp <= 0))
                {
                    Console.WriteLine("\nPlease enter a valid number:");
                    inp = Convert.ToInt32(Console.ReadLine());

                }
                foreach (PlanSetup aplansetup in Plans)
                {
                    int pcount = 1;

                    if (inp == count)
                    {
                        Plan = aplansetup;

                    }

                    pcount++;
                }
            }

            return Plan;
        }


        public static List<ROI.ROI> PlansumAnalysis(string laterality, string[] Si, string ptype, Patient patient, Course course, StructureSet structureSet, PlanSum Plansum, int dt, double dd, TextBox OuputBox, string gyntype)
        {

            List<ROI.ROI> ROIE = new List<ROI.ROI>();     // Expected ROI made from text file list
            List<ROI.ROI> ROIA = new List<ROI.ROI>();     // Actual ROI list from Eclipse 
            string Ttype = ptype;
            string Tsite = null;
            //  string [] Si = new string[50]; 

            // ROI.ROI is its own custom class
            ROIE = LISTMAKER.Listmaker(Ttype, Tsite, Si, laterality);          // separate class with LISTMAKER function which generates a list of ROIs for the given treatment type and site

            double dosesum = 0.0;
            string dunit = null;

            if (dt == 1)
            {

                foreach (PlanSetup aplan in Plansum.PlanSetups)
                {
                    dosesum += aplan.TotalPrescribedDose.Dose;
                    dunit = aplan.TotalPrescribedDose.UnitAsString;
                }
            }
            else if (dt == 2)
            {
                IEnumerator lk = Plansum.PlanSetups.GetEnumerator();
                lk.MoveNext();
                PlanSetup PS = (PlanSetup)lk.Current;
                dosesum = PS.TotalPrescribedDose.Dose;
                dunit = PS.TotalPrescribedDose.UnitAsString;
            }
            else if (dt == 3)
            {
                dosesum = dd;
            }

            int county = 0;

            foreach (ROI.ROI Erika in ROIE)
            {
                county++;

                // OuputBox.AppendText(Environment.NewLine);
               // OuputBox.AppendText("Dose Objectives checked: " + county + "/" + ROIE.Count);

                //  Console.WriteLine("\nThe current dose of objective is: {0}", morty.ROIName);
                // Thread.Sleep(2000);

                foreach (Structure S in structureSet.Structures)        // iterates through all the structures in the structureset of the current Plan
                {
                    double structvol = S.Volume;

                    if (S.IsEmpty == true || S.Volume < 0.0)
                    {
                        // MessageBox.Show("The structure " + S.Id + " has been omitted from the DVH analysis because it is not contoured.");
                        continue;
                    }

                    if (S.Id == Erika.Rstruct)
                    {
                        // Console.WriteLine("\nThe current structure from the Plan is: {0}", S.Id);
                        //  Console.WriteLine("\nThe current dose of objective has the structure tag: {0}", morty.Rstruct);
                        //  Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {1}", S.Id, S.Volume);
                        //  Thread.Sleep(3000);

                        if (Erika.limit == "Max Pt Dose [voxel]")
                        {
                            string kstatus = null;
                          //  System.Windows.Forms.MessageBox.Show("Plan A Max Dose Voxel");
                            DVHData mDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);
                            double DM = mDVH.MaxDose.Dose;

                            if (Erika.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM < Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM < Convert.ToDouble(Erika.limval))
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
                                    if (DM < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM <= Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM <= Convert.ToDouble(Erika.limval))
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
                                    if (DM < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = DM, status = kstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });
                          //  System.Windows.Forms.MessageBox.Show("Scorpia 0");

                        }
                        else if (Erika.limit == "Max Pt Dose")        // MaxPtDose
                        {
                            string kstatus = null;

                            //  Console.WriteLine("\nTRIGGER MAX PT Dose");
                            //   Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //Thread.Sleep(1000);
                            double maxdose = 0.0;

                            DVHData kDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);

                            // Console.WriteLine("\n  DVH Point VOLUME UNIT: {0}", kDVH.CurveData[1].VolumeUnit.ToString());

                            // Console.WriteLine("\n  NORMAL MAXDOSE: {0}", kDVH.MaxDose.ToString());

                            // Thread.Sleep(10000);

                            foreach (DVHPoint point in kDVH.CurveData)
                            {
                                if (point.Volume < 0.1 && point.Volume > 0.03)
                                {
                                    // Console.WriteLine("\n  DVH Point VOLUME: {0}", point.Volume);

                                    if (maxdose == 0.0)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                    if (point.DoseValue.Dose > maxdose)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                }
                            }

                            if((Erika.ROIName == "Bladder_Max Pt Dose <= 5000cGy" | Erika.ROIName == "Rectum_Max Pt Dose < 5000cGy" | Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Dose Painted"))
                            {
                                Erika.limval = Convert.ToString(1.15 * dosesum);

                                if(Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy")
                                {
                                    Erika.applystatus = false;
                                }
                            }
                            else if((Erika.ROIName == "Bladder_Max Pt Dose <= 5000cGy" | Erika.ROIName == "Rectum_Max Pt Dose < 5000cGy" | Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Sequential Courses"))
                            {
                                Erika.limval = Convert.ToString(1.10 * dosesum);
                            }

                            if (Erika.ROIName == "Trachea_Bronc_Max Pt Dose <= 105% Rx")
                            {
                                Erika.limval = Convert.ToString(1.05 * dosesum);
                            }




                            //  Console.WriteLine("\nDOSE UNIT: {0}", maxdose.Unit.ToString());
                            //   Console.WriteLine("\nDOSE Value: {0}", maxdose.Dose);
                            //  Thread.Sleep(4000);

                            if (Erika.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose < Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose < Convert.ToDouble(Erika.limval))
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
                                    if (maxdose < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose <= Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose <= Convert.ToDouble(Erika.limval))
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
                                    if (maxdose < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = maxdose, status = kstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });

                        }
                        else if (Erika.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;

                            //  Console.WriteLine("\nTRIGGER Mean");
                            //  Console.WriteLine("\nMean Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            DVHData jDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue meandose = jDVH.MeanDose;

                            //   Console.WriteLine("\nDOSE UNIT: {0}", meandose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Vale: {0}", meandose.Dose);
                            //  Thread.Sleep(4000);

                            if (Erika.strict == "[record]")
                            {
                                jstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose < Convert.ToDouble(Erika.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose < Convert.ToDouble(Erika.limval))
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

                                    if (meandose.Dose < Convert.ToDouble(Erika.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose <= Convert.ToDouble(Erika.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose <= Convert.ToDouble(Erika.limval))
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

                                    if (meandose.Dose < Convert.ToDouble(Erika.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });

                        }
                        else if (Erika.limit.StartsWith("CV"))
                        {

                            string Lstatus = null;
                            double Lcomp = 0.0;    //compare
                            double Lcomp2 = 0.0;
                            double Lvol = 0.0;
                            double Ldose = 0.0;    //functional dose
                            string type = "cm3";
                            double Llimit = 0.0;
                            double compvol = 0.0;


                            string jerry = Erika.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);    // in Gy

                            Lcomp = Convert.ToDouble(Erika.limval);  // VOLUME IN CM3

                            if (Erika.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(Erika.goal);   // VOLUME IN CM3
                            }

                            Ldose = Llimit * 100.0;

                            DVHData Ldvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            //  Console.WriteLine("\nDVH Point Curves Volume Unit CM3: {0}", Ldvh.CurveData[1].VolumeUnit);
                            //  Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Ldvh.CurveData[1].DoseValue.UnitAsString);


                            foreach (DVHPoint point in Ldvh.CurveData)
                            {

                                if ((point.DoseValue.Dose >= (Ldose - 0.5)) && (point.DoseValue.Dose <= (Ldose + 0.5)))
                                {
                                    // Console.WriteLine("\nTrigger DVH Point match!!");
                                    Lvol = point.Volume;

                                }
                            }

                            compvol = S.Volume - Lvol;

                            if (Erika.strict == "[record]")
                            {
                                Lstatus = "";
                            }
                            else if (Erika.strict == ">")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
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

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = Lcomp, strict = Erika.strict, goal = Erika.goal, actvol = compvol, status = Lstatus, structvol = structvol, type = type, limunit = Erika.limunit, applystatus = Erika.applystatus });

                        }
                        else if (Erika.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;

                            if (Erika.limit == "Volume")
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

                                ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = 700.0, strict = Erika.strict, goal = "NA", actvol = S.Volume, status = fstatus, structvol = structvol, type = "cm3", limunit = Erika.limunit, applystatus = Erika.applystatus });
                                continue;
                            }
                            else if (Erika.limit == "V100%Rx")
                            {
                                // THIS IS SPECIFICALLY FOR THE "_CTV_V100%Rx>=100%" DOSE OBJECTIVE FOR SBRT LIVER PLANS

                                double ctvvol = 0.0;
                                DoseValue tdose = new DoseValue(dosesum, DoseValue.DoseUnit.cGy);
                                DVHData ctvdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                foreach (DVHPoint point in ctvdvh.CurveData)
                                {
                                    //  Console.WriteLine("\n\n    DVH point UNIT:  ", point.DoseValue.Unit);

                                    if ((point.DoseValue.Dose >= (dosesum - 0.2)) && (point.DoseValue.Dose <= (dosesum + 0.2)))
                                    {
                                        // Console.WriteLine("\nTrigger DVH Point match!!");
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

                                ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = 100.0, strict = Erika.strict, goal = "NA", actvol = ctvvol, status = fstatus, structvol = structvol, type = "percent", limunit = Erika.limunit, applystatus = Erika.applystatus });
                                continue;
                            }
                            else if (Erika.limit == "Veff" || Erika.limit == "V60 is NOT Circumferential")
                            {
                                continue;
                            }

                            if (gyntype == "Sequential Courses" & Erika.ROIName == "SmBowel_Loops_V55 <= 15cc")
                            {
                                Erika.applystatus = false;
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

                            if (Erika.limval == "NA")
                            {
                                Erika.limval = "-1";
                            }


                            //   Console.WriteLine("\nTRIGGER V ");
                            //  Console.WriteLine("\nV Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //  Thread.Sleep(2000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    

                            string jerry = Erika.limit.Substring(1);
                            //  Console.WriteLine("\n After V chop, we have (jerry): {0}", jerry);
                            // Thread.Sleep(2000);

                            try
                            {
                                Vgy = Convert.ToDouble(jerry) * 100.0;   //multiplied by 100 to convert to cGy
                            }
                            catch (FormatException e)
                            {
                                Vgy = 0.0;
                                System.Windows.Forms.MessageBox.Show("An error occurred when attempting to convert the string \"" + Erika.limit + "\" to a number for a dose objective with a limit that starts with the character \"V\". This is most likely due to a dose objective that was added to the list that this script has not been modified to handle. \n\n The value of this limit will be set to 0 to allow the program to continue working, however the information given by the program for this dose objective wil not be correct.");
                            }

                           // System.Windows.Forms.MessageBox.Show("Plan Vgy is : " + Vgy);

                            if (Erika.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(Erika.limval);

                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                if (Erika.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(Erika.goal);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));

                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                //  Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                //  Console.WriteLine("\nDVH Point Curves Volume Unit PERCENT: {0}", Vdvh.CurveData[1].VolumeUnit);
                                //  Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Vdvh.CurveData[1].DoseValue.UnitAsString);

                                foreach (DVHPoint point in Vdvh.CurveData)
                                {

                                    //  Console.WriteLine("\n\n    DVH point UNIT:  ", point.DoseValue.Unit);

                                    if ((point.DoseValue.Dose >= (Vgy - 0.2)) && (point.DoseValue.Dose <= (Vgy + 0.2)))
                                    {
                                        // Console.WriteLine("\nTrigger DVH Point match!!");
                                        Vvol = point.Volume;
                                    }
                                }

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                //  Thread.Sleep(5000);

                            }
                            else if (Erika.limunit == "cc")
                            {
                                type = "cm3";

                                comp = Convert.ToDouble(Erika.limval);  // VOLUME IN CM3

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (Erika.goal != "NA")
                                {
                                    comp2 = Convert.ToDouble(Erika.goal);   // VOLUME IN CM3

                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                }

                                // Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                                // Console.WriteLine("\nDVH Point Curves Volume Unit CM3: {0}", Vdvh.CurveData[1].VolumeUnit);
                                // Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Vdvh.CurveData[1].DoseValue.UnitAsString);

                                //Console.WriteLine("\n\n  DVH point UNIT: {0} ", Vdvh.CurveData[1].DoseValue.Unit.ToString());

                                // Thread.Sleep(3000);

                                foreach (DVHPoint point in Vdvh.CurveData)
                                {

                                    //  Console.WriteLine("\n\n     Point dose value: {0}", point.DoseValue.Dose.ToString());
                                    //  Thread.Sleep(750);

                                    if ((point.DoseValue.Dose >= (Vgy - 0.2)) && (point.DoseValue.Dose <= (Vgy + 0.2)))
                                    {
                                        // Console.WriteLine("\nTrigger DVH Point match!!");
                                        Vvol = point.Volume;

                                    }
                                }
                            }

                            if (Erika.strict == "[record]")
                            {
                                fstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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
                            else if (Erika.strict == ">=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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

                            // add thing to deal with Esophagus circumferential

                            // Console.WriteLine("\nDOSE UNIT: {0}", fdose.Unit.ToString());
                            // Console.WriteLine("\nDOSE Value: {0}", fdose.Dose);
                            //  Thread.Sleep(5000);

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = comp, strict = Erika.strict, goal = Erika.goal, actvol = Vvol, status = fstatus, structvol = structvol, type = type, limunit = Erika.limunit, applystatus = Erika.applystatus });

                        }
                        else if (Erika.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {

                            string qstatus = null;
                            double qdose = 0.0;

                            if (Erika.limval == "NA")
                            {
                                Erika.limval = "-1";
                            }

                            //  Console.WriteLine("\nTRIGGER D ");
                            //  Console.WriteLine("\nD Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = Erika.limit.Substring(1);                     // "V gray" 

                            //  Console.WriteLine("\nqstring after D remove: {0}", qstring);

                            if (Erika.limit.EndsWith("cc"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);

                                double qvol = Convert.ToDouble(q2str);

                                // Console.WriteLine("\n q2str is: {0}", q2str);

                                // qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                                // Console.WriteLine("\nDVH Point Curves volume unit CM3: {0}", Qdvh.CurveData[1].VolumeUnit);

                                foreach (DVHPoint point in Qdvh.CurveData)
                                {

                                    if ((point.Volume >= (qvol - 0.3)) && (point.Volume <= (qvol + 0.3)))
                                    {

                                        qdose = point.DoseValue.Dose;

                                    }
                                }

                                //  Console.WriteLine("\n ABS DOSE: {0}", qdose.Dose);
                                //  Thread.Sleep(4000);

                                // special case for dynamic Body-PTV D1cc objective (Liver only)
                                if (Erika.ROIName == "Body-PTV_D1cc <= 115%Rx")
                                {
                                    Erika.limval = Convert.ToString(1.15 * dosesum);
                                    Erika.goal = Convert.ToString(1.10 * dosesum);
                                }

                            }
                            else if (Erika.limit.EndsWith("%"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);

                                double qvol = Convert.ToDouble(q2str);

                                // qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                // Console.WriteLine("\nDVH Point Curves volume unit PERCENT: {0}", Qdvh.CurveData[1].VolumeUnit);

                                foreach (DVHPoint point in Qdvh.CurveData)
                                {

                                    if ((point.Volume >= (qvol - 0.3)) && (point.Volume <= (qvol + 0.3)))
                                    {

                                        qdose = point.DoseValue.Dose;

                                    }
                                }

                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", qdose.Dose, tqdose.Dose);
                                //  Thread.Sleep(5000);

                            }


                            if (Erika.strict == "[record]")
                            {

                                qstatus = "";

                            }
                            else if (Erika.strict == "<")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if(Erika.limval == "-1")
                                    {
                                        if (qdose < Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose > Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose < Convert.ToDouble(Erika.goal))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose < Convert.ToDouble(Erika.limval)) && (qdose > Convert.ToDouble(Erika.goal)))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose > Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (qdose < Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }
                                }
                            }
                            else if (Erika.strict == ">")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose > Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose < Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose > Convert.ToDouble(Erika.goal))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose > Convert.ToDouble(Erika.limval)) && (qdose < Convert.ToDouble(Erika.goal)))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose < Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (qdose > Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == ">=")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose >= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose <= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if (qdose >= Convert.ToDouble(Erika.goal))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose >= Convert.ToDouble(Erika.limval)) && (qdose <= Convert.ToDouble(Erika.goal)))
                                    {
                                        qstatus = "REVIEW - GOAL";
                                    }
                                    else if (qdose <=Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                                else
                                {
                                    if (qdose >= Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose <= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose >= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }

                                    if ((qdose <= Convert.ToDouble(Erika.goal)))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if ((qdose <= Convert.ToDouble(Erika.limval)) && (qdose >= Convert.ToDouble(Erika.goal)))
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
                                    if (qdose <= Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }

                            //  Console.WriteLine("\nDOSE UNIT: {0}", qdose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Value: {0}", qdose.Dose);
                            //  Thread.Sleep(5000);

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = qdose, status = qstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });
    
                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
            }// Ends ROIE iterator loop


            // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI.ROI comparison.

            return ROIA;
        }

        public static List<ROI.ROI> PlanAnalysis(string laterality, string TS, string ptype, User user, Patient patient, Course course, StructureSet structureSet, PlanSetup Plan, TextBox OuputBox, string gyntype)
        {
            //  System.Windows.Forms.MessageBox.Show("TS is: " + TS);
            //  System.Windows.Forms.MessageBox.Show("ptype is: " + ptype);
            List<ROI.ROI> ROIE = new List<ROI.ROI>();     // Expected ROI made from text file list
            List<ROI.ROI> ROIA = new List<ROI.ROI>();     // Actual ROI list from Eclipse 
            string Ttype = ptype;
            string Tsite = TS;


            string[] Si = new string[10] { "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA" };
            // ROI.ROI is its own custom class
            ROIE = LISTMAKER.Listmaker(Ttype, Tsite, Si, laterality);          // separate class with LISTMAKER function which generates a list of ROIs for the given treatment type and site

           // MessageBox.Show(Tsite + " dose objective list created successfully.");
          //  MessageBox.Show("testr");
           // MessageBox.Show("struct -r " + structureSet.Id);
            // Console.WriteLine("\nThe {0} dose objective list contains {1} unique dose objectives.", Tsite, ROIE.Count);
            // Thread.Sleep(2000);
            // This part of code below gets DVH data from Eclipse. The way it works is different for different limit types, like MaxPtDose, V80, D1cc. etc.

            double pdose = Plan.TotalPrescribedDose.Dose;       // prescribed dose of the Plan

            // Console.WriteLine("\nPRESCRIBED DOSE: {0} {1}", pdose, Plan.TotalPrescribedDose.Unit.ToString());
            // Thread.Sleep(2000);
           // MessageBox.Show("struct -o " + structureSet.Id);
           // MessageBox.Show("Matching dose objectives with contoured structures in current Plan ... ");

            int county = 0;

            foreach (ROI.ROI Erika in ROIE)
            {
               // System.Windows.Forms.MessageBox.Show("ROI: " + Erika.ROIName);
                county++;
                //  System.Windows.Forms.MessageBox.Show("Plan A ROI iterate " + county);
                //  Console.WriteLine("\nThe current dose of objective is: {0}", morty.ROIName);
                // Thread.Sleep(2000);
                // MessageBox.Show("struct - " + structureSet.Id);

               //  OuputBox.AppendText(Environment.NewLine);
               //  OuputBox.AppendText("Dose Objectives checked: " + county + "/" + ROIE.Count);

                foreach (Structure S in structureSet.Structures)        // iterates thriugh all the structures in the structureset of the current Plan
                {
                    double structvol = S.Volume;

                    //  System.Windows.Forms.MessageBox.Show("Plan A struct iterate");
                    if (S.IsEmpty == true || S.Volume < 0.0)
                    {
                       // MessageBox.Show("The structure " + S.Id + " has been omitted from the DVH analysis because it is not contoured.");
                        continue;
                    }


                    if (S.Id == Erika.Rstruct)
                    {

                        // System.Windows.Forms.MessageBox.Show("Plan A struct match");
                        // Console.WriteLine("\nThe current structure from the Plan is: {0}", S.Id);
                        //  Console.WriteLine("\nThe current dose of objective has the structure tag: {0}", morty.Rstruct);
                        //  Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {1}", S.Id, S.Volume);
                        //  Thread.Sleep(3000);

                        if (Erika.limit == "Max Pt Dose [voxel]")
                        {
                            string kstatus = null;
                          //  System.Windows.Forms.MessageBox.Show("Plan A Max Dose Voxel");
                            DVHData mDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.01);
                            double DM = mDVH.MaxDose.Dose;

                            if (Erika.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM < Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM < Convert.ToDouble(Erika.limval))
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
                                    if (DM < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (DM <= Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (DM <= Convert.ToDouble(Erika.limval))
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
                                    if (DM < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = DM, status = kstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });
                           //  System.Windows.Forms.MessageBox.Show("Scorpia 0");

                        }
                        else if (Erika.limit == "Max Pt Dose")        // MaxPtDose
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

                            double maxdose = 0.0;
                            //  DoseValue maxdose = kDVH.MaxDose;

                            foreach (DVHPoint point in kDVH.CurveData)
                            {
                                if (point.Volume < 0.1 && point.Volume > 0.03)
                                {
                                    // Console.WriteLine("\n  DVH Point VOLUME: {0}", point.Volume);

                                    if (maxdose == 0.0)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                    if (point.DoseValue.Dose > maxdose)
                                    {
                                        maxdose = point.DoseValue.Dose;
                                    }
                                }
                            }

                            if ((Erika.ROIName == "Bladder_Max Pt Dose <= 5000cGy" | Erika.ROIName == "Rectum_Max Pt Dose < 5000cGy" | Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Dose Painted"))
                            {
                                Erika.limval = Convert.ToString(1.15 * pdose);

                                if (Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy")
                                {
                                    Erika.applystatus = false;
                                }
                            }
                            else if ((Erika.ROIName == "Bladder_Max Pt Dose <= 5000cGy" | Erika.ROIName == "Rectum_Max Pt Dose < 5000cGy" | Erika.ROIName == "SmBowel_Loops_Max Pt Dose < 5000cGy") & (gyntype == "Sequential Courses"))
                            {
                                Erika.limval = Convert.ToString(1.10 * pdose);
                            }

                            if (Erika.ROIName == "Trachea_Bronc_Max Pt Dose <= 105 Percent Rx")
                            {
                                Erika.limval = Convert.ToString(1.05 * pdose);
                            }



                            if (Erika.strict == "[record]")
                            {
                                kstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose < Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose < Convert.ToDouble(Erika.limval))
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
                                    if (maxdose < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (maxdose <= Convert.ToDouble(Erika.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose <= Convert.ToDouble(Erika.limval))
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
                                    if (maxdose < Convert.ToDouble(Erika.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = maxdose, status = kstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });
                            // System.Windows.Forms.MessageBox.Show("Scorpia 1");
                        }
                        else if (Erika.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;
                            // System.Windows.Forms.MessageBox.Show("Plan A Mean Dose");
                            //  Console.WriteLine("\nTRIGGER Mean");
                            //  Console.WriteLine("\nMean Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            DVHData jDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue meandose = jDVH.MeanDose;

                            //   Console.WriteLine("\nDOSE UNIT: {0}", meandose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Vale: {0}", meandose.Dose);
                            //  Thread.Sleep(4000);


                            if (Erika.strict == "[record]")
                            {
                                jstatus = "";
                            }
                            else if (Erika.strict == "<")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (meandose.Dose < Convert.ToDouble(Erika.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose < Convert.ToDouble(Erika.limval))
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
                                    if (meandose.Dose < Convert.ToDouble(Erika.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (meandose.Dose <= Convert.ToDouble(Erika.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose <= Convert.ToDouble(Erika.limval))
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
                                    if (meandose.Dose < Convert.ToDouble(Erika.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });
                            //  System.Windows.Forms.MessageBox.Show("Scorpia 2");
                        }
                        else if (Erika.limit.StartsWith("CV"))
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

                            string jerry = Erika.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);

                            Lcomp = Convert.ToDouble(Erika.limval);  // VOLUME IN CM3

                           // System.Windows.Forms.MessageBox.Show("CV Llimit is: " + Llimit);

                            if (Erika.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(Erika.goal);   // VOLUME IN CM3
                            }

                            DoseValue Ldose = new DoseValue((Llimit * 100.0), DoseValue.DoseUnit.cGy);

                          //  System.Windows.Forms.MessageBox.Show("CV Ldose is: " + Ldose.Dose + "  Unit: " + Ldose.UnitAsString);

                            Lvol = Plan.GetVolumeAtDose(S, Ldose, VolumePresentation.AbsoluteCm3);

                           // System.Windows.Forms.MessageBox.Show("CV Lvol is: " + Lvol);
                           
                            compvol = S.Volume - Lvol;

                           // System.Windows.Forms.MessageBox.Show("CV compvol is: " + compvol);

                            //   System.Windows.Forms.MessageBox.Show("Trig 3");

                            if (Erika.strict == "[record]")
                            {
                                Lstatus = "";
                            }
                            else if (Erika.strict == ">")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
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

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = Lcomp, strict = Erika.strict, goal = Erika.goal, actvol = compvol, status = Lstatus, structvol = structvol, type = type, limunit = Erika.limunit, applystatus = Erika.applystatus });

                            // System.Windows.Forms.MessageBox.Show("Scorpia 3");
                        }
                        else if (Erika.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;
                            // System.Windows.Forms.MessageBox.Show("Plan A V");

                            if (Erika.limit == "Volume")
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

                                ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = 700.0, strict = Erika.strict, goal = "NA", actvol = S.Volume, status = fstatus, structvol = structvol, type = "cm3", limunit = Erika.limunit, applystatus = Erika.applystatus });
                                continue;
                            }
                            else if (Erika.limit == "V100%Rx")
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

                                ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = 100.0, strict = Erika.strict, goal = "NA" , actvol = ctvvol, status = fstatus, structvol = structvol, type = "percent", limunit = Erika.limunit, applystatus = Erika.applystatus });
                                continue;
                            }
                            else if (Erika.limit == "Veff" || Erika.limit == "V60 is NOT Circumferential")
                            {
                                continue;
                            }

                            if (gyntype == "Sequential Courses" & Erika.ROIName == "SmBowel_Loops_V55 <= 15cc")
                            {
                                Erika.applystatus = false;
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

                            if (Erika.limval == "NA")
                            {
                                Erika.limval = "-1";
                            }

                            //   Console.WriteLine("\nTRIGGER V ");
                            //  Console.WriteLine("\nV Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //  Thread.Sleep(2000);
                            // System.Windows.Forms.MessageBox.Show("Trig 4");

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    

                            string jerry = Erika.limit.Substring(1);
                           //  System.Windows.Forms.MessageBox.Show("Plan jerry is: " + jerry);
                                //  Console.WriteLine("\n After V chop, we have (jerry): {0}", jerry);
                                // Thread.Sleep(2000);

                            try
                            {
                                Vgy = Convert.ToDouble(jerry);
                            }
                            catch(FormatException e)
                            {
                                Vgy = 0.0;
                                System.Windows.Forms.MessageBox.Show("An error occurred when attempting to convert the string \"" + Erika.limit + "\" to a number for a dose objective with a limit that starts with the character \"V\". This is most likely due to a dose objective that was added to the list that this script has not been modified to handle. \n\n The value of this limit will be set to 0 to allow the program to continue working, however the information given by the program for this dose objective wil not be correct.");
                            }

                          //  System.Windows.Forms.MessageBox.Show("Plan Vgy is : " + Vgy);

                            if (Erika.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(Erika.limval);

                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                              //  System.Windows.Forms.MessageBox.Show("Trig 5");

                                if (Erika.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(Erika.goal);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));

                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                DoseValue Vdose = new DoseValue((Vgy * 100.0), DoseValue.DoseUnit.cGy);

                                // Console.WriteLine("\nVDOSE: {0} {1}", Vdose.Dose, Vdose.UnitAsString);

                                // Thread.Sleep(2000);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                //  Console.WriteLine("\nVVOL: {0}", Vvol);

                                // Thread.Sleep(2000);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                //  Thread.Sleep(5000);

                            }
                            else if (Erika.limunit == "cc")
                            {
                               // System.Windows.Forms.MessageBox.Show("Trig 6");
                                type = "cm3";
                                comp = Convert.ToDouble(Erika.limval);

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (Erika.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(Erika.goal);

                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                DoseValue Vdose = new DoseValue((Vgy * 100.0), DoseValue.DoseUnit.cGy);

                                //  Console.WriteLine("\nVDOSE: {0} {1}", Vdose.Dose, Vdose.UnitAsString);

                                //  Thread.Sleep(2000);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);

                                //  Console.WriteLine("\nVVOL: {0}", Vvol);

                                //  Thread.Sleep(2000);

                            }

                           // System.Windows.Forms.MessageBox.Show("Trig 7");

                            if (Erika.strict == "[record]")
                            {

                                fstatus = "";

                            }
                            else if (Erika.strict == "<")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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
                                    else
                                    {
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
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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
                                    else
                                    {
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
                            else if (Erika.strict == ">=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
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
                                    else
                                    {
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

                            // add thing to deal with Esophagus circumferential

                            // Console.WriteLine("\nDOSE UNIT: {0}", fdose.Unit.ToString());
                            // Console.WriteLine("\nDOSE Value: {0}", fdose.Dose);
                            //  Thread.Sleep(5000);

                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limvol = comp, strict = Erika.strict, goal = Erika.goal, actvol = Vvol, status = fstatus, structvol = structvol, type = type, limunit = Erika.limunit, applystatus = Erika.applystatus });
                           //  System.Windows.Forms.MessageBox.Show("end v type ");
                        }
                        else if (Erika.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {
                             // System.Windows.Forms.MessageBox.Show("Plan A D");
                            string qstatus = null;
                            DoseValue qdose = new DoseValue();

                            if (Erika.limval == "NA")
                            {
                                Erika.limval = "-1";
                            }

                            //  Console.WriteLine("\nTRIGGER D ");
                            //  Console.WriteLine("\nD Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = Erika.limit.Substring(1);                     // "V gray" 

                            //  Console.WriteLine("\nqstring after D remove: {0}", qstring);

                            if (Erika.limit.EndsWith("cc"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);

                                // Console.WriteLine("\n q2str is: {0}", q2str);

                                qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n ABS DOSE: {0}", qdose.Dose);
                                // System.Windows.Forms.MessageBox.Show("qdose is: " + qdose);

                                //  Thread.Sleep(4000);

                                // special case for dynamic Body-PTV D1cc objective (Liver only)
                                if (Erika.ROIName == "Body-PTV_D1cc <= 115%Rx")
                                {
                                    Erika.limval = Convert.ToString(1.15 * pdose);
                                    Erika.goal = Convert.ToString(1.10 * pdose);
                                }


                            }
                            else if (Erika.limit.EndsWith("%"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);

                                //  Console.WriteLine("\n q2str is: {0}", q2str);

                                double qvol = (structvol * ((Convert.ToDouble(q2str)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", qdose.Dose, tqdose.Dose);
                                //  Thread.Sleep(5000);

                            }

                            if (Erika.strict == "[record]")
                            {

                                qstatus = "";

                            }
                            else if (Erika.strict == "<")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose.Dose < Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose.Dose > Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if (qdose.Dose < Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if ((qdose.Dose < Convert.ToDouble(Erika.limval)) && (qdose.Dose > Convert.ToDouble(Erika.goal)))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                        else if (qdose.Dose > Convert.ToDouble(Erika.limval))
                                        {
                                            qstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {
                                    if (qdose.Dose < Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }
                                }
                            }
                            else if (Erika.strict == ">")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose.Dose > Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose.Dose < Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if (qdose.Dose > Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if ((qdose.Dose > Convert.ToDouble(Erika.limval)) && (qdose.Dose < Convert.ToDouble(Erika.goal)))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                        else if (qdose.Dose < Convert.ToDouble(Erika.limval))
                                        {
                                            qstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {

                                    if (qdose.Dose > Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }
                                }
                            }
                            else if (Erika.strict == ">=")
                            {

                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose.Dose >= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose.Dose <= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if (qdose.Dose >= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if ((qdose.Dose >= Convert.ToDouble(Erika.limval)) && (qdose.Dose <= Convert.ToDouble(Erika.goal)))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                        else if (qdose.Dose <= Convert.ToDouble(Erika.limval))
                                        {
                                            qstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {
                                    if (qdose.Dose >= Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (Erika.strict == "<=")
                            {
                                if (Erika.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Erika.limval == "-1")
                                    {
                                        if (qdose.Dose <= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if (qdose.Dose >= Convert.ToDouble(Erika.goal))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                    }
                                    else
                                    {
                                        if ((qdose.Dose <= Convert.ToDouble(Erika.goal)))
                                        {
                                            qstatus = "PASS";
                                        }
                                        else if ((qdose.Dose <= Convert.ToDouble(Erika.limval)) && (qdose.Dose >= Convert.ToDouble(Erika.goal)))
                                        {
                                            qstatus = "REVIEW - GOAL";
                                        }
                                        else
                                        {
                                            qstatus = "REVIEW";
                                        }
                                    }
                                }
                                else
                                {
                                    if (qdose.Dose <= Convert.ToDouble(Erika.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";
                                    }
                                }
                            }

                            //  Console.WriteLine("\nDOSE UNIT: {0}", qdose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Value: {0}", qdose.Dose);
                            //  Thread.Sleep(5000);


                            // System.Windows.Forms.MessageBox.Show("Scorpia 5");
                            ROIA.Add(new ROI.ROI { ROIName = Erika.ROIName, limdose = Convert.ToDouble(Erika.limval), strict = Erika.strict, goal = Erika.goal, actdose = qdose.Dose, status = qstatus, structvol = structvol, type = "NV", limunit = Erika.limunit, applystatus = Erika.applystatus });


                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
            }// Ends ROIE iterator loop

            // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI.ROI comparison.

            // Console.WriteLine("\n{0} unique dose objectives matched with structures in the current Plan.", ROIA.Count);


            // Console.WriteLine("\nDose objective check complete.");


            return ROIA;
        }



        /*
Console.WriteLine("\n\n\t\tDOSE OBJECTIVE REPORT FOR {0}'S PLAN {1}", patient.Name, Plan.Name);
Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------");
Console.WriteLine("Name                                                   |  Hard Dose Limit (cGy) | Goal Dose Limit (cGy) | Eclipse Estimated Dose (cGy) |  Status  | Structure Volume  ");

int ct = 0;

foreach(ROI.ROI aroi in ROIA)
{

    if((ct % 6) == 0)
    {

        Console.WriteLine("\n\nName                                                   |  Hard Dose Limit (cGy) | Goal Dose Limit (cGy) | Eclipse Estimated Dose (cGy) |  Status  | Structure Volume  ");

    }

    if(aroi.strict == "[record]")
    {

        Console.WriteLine("\n\n");
        Console.WriteLine("{0}      NA           {1}         {3}         {2}    {3} ", aroi.ROIName, aroi.goal, aroi.actdose, aroi.status, aroi.structvol);
        Thread.Sleep(3000);
    }
    else
    {

        Console.WriteLine("\n\n");
        Console.WriteLine("{0}      {1}         {2}          {3}        {4}   {5} ", aroi.ROIName, aroi.limdose, aroi.goal, aroi.actdose, aroi.status, aroi.structvol);
        Thread.Sleep(3000);
    }
    ct++;
}

*/



        public void Execute(ScriptContext context)     // PROGRAM START - sending a return to Execute will end the program
        {
            //regular variables

            List<ROI.ROI> output = new List<ROI.ROI>();

            // this stuff below is used with the GUI
            //ESAPI variables  NOTE: CANNOT INSTANTIATE ECLIPSE VARIABLES. CAN ONLT GET THEM FROM ECLIPSE.

            Patient patient = context.Patient;   // creates an object of the patient class called patient equal to the active patient open in Eclipse
            Course course = context.Course;
            Image image3D = context.Image;
           // StructureSet structureSet = context.StructureSet;
            User user = context.CurrentUser;
            IEnumerable<PlanSum> Plansums = context.PlanSumsInScope;
            IEnumerable<PlanSetup> Plans = context.PlansInScope;

            if (context.Patient == null)
            {
                System.Windows.MessageBox.Show("Please load a patient with a treatment plan before running this script!");
                return;
            }

            //  course.Diagnoses






            // this area calls outside functions that perform automatic checks on system
            // Starts automatic checks on a separate thread  (Work In Progress)
            // Thread BackCheck = new Thread(() => AutoChecks.CountourChecks.ContoursInBody(structureSet));

            // GUI STARTS HERE
            // settings for windows forms GUI
            System.Windows.Forms.Application.EnableVisualStyles();

            //Starts GUI for Dose objective check in a separate thread
            System.Windows.Forms.Application.Run(new DoseObjectiveCheck.GUI(patient, course, image3D, user, Plansums, Plans));




            // Functions below here are for the dose objective check program

            /*
            Discrim = Discriminator(Plansums, Plans, user);

                if (Discrim == true)
                {

                    PlanSum Plansum = PlansumPick(Plansums);
                    output = PlansumAnalysis(patient, course, structureSet, Plansum);

                    foreach (ROI.ROI aroi in output)
                    {
                        if (aroi.status == "REVIEW")
                        {
                            T = true;
                        }
                    }

                    if (T == true)
                    {
                    System.Windows.MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
                    }

                    Console.WriteLine("\n\n\n");

                    Console.WriteLine("A report of the Dose Objective Check will now open in your default PDF viewer. Please rename it and save it in the 'Dose Objective Checker Reports' folder.");

                    PdfReport.PDFGenerator.Program.PlansumMain(patient, course, Plansum, image3D, structureSet, user, output);
                    
                }
                else if (Discrim == false)
                {

                    PlanSetup Plan = PlanPick(Plans);
                    output = PlanAnalysis(user, patient, course, structureSet, Plan);

                    foreach (ROI.ROI aroi in output)
                    {
                        if (aroi.status == "REVIEW")
                        {
                            T = true;
                        }
                    }

                    if (T == true)
                    {
                    System.Windows.MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
                    }

                    Console.WriteLine("\n\n\n");

                    Console.WriteLine("A report of the Dose Objective Check will now open in your default PDF viewer. Please rename it and save it in the 'Dose Objective Checker Reports' folder.");

                    PdfReport.PDFGenerator.Program.PlanMain(patient, course, Plan, image3D, structureSet, user, output);
                    
                }

            */


            /*
                Console.WriteLine("Would you like to generate a PDF of this dose objective report (Y/N)?  ");
                input = Console.ReadLine();
                if (input == "n" | input == "N")
                {
                    Console.WriteLine("\n \n");
                    Console.WriteLine("This program will now close.");
                    Thread.Sleep(3000);


                }
                else if (input == "y" | input == "Y")
                {
                    Console.WriteLine("\n \n");
                    Console.WriteLine("Your report will now open in your default PDF viewer. \n \n");
                    Console.WriteLine("This Program will now close. \n");
                    Thread.Sleep(2000);
                    PdfReport.PDFGenerator.Program.Main(patient, course, Plan, image3D, structureSet, user, output);

                }
             */


            // code to put images in pdf goes here

            //script code here

            return;

        }  //ends Execute  END OF PROGRAM

    }   //ends Script class
}  //ends namespace

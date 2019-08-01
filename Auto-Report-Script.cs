using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using ContourChecks;
using PdfReport.PDFGenerator;
using ROI;





/*
    Lahey RadOnc Dose Objective Checker
    Copyright (c) 2019 Radiation Oncology Department, Lahey Health

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program also requires .NET Framework 4.5.0 to run properly.

    This is the source code for a .NET Framework assembly file, however this functions as an executable file in Eclipse.
    In addition to Varian's APIs and .NET Framework, this program uses the following commonly available libraries:
    MigraDoc
    PdfSharp

    Release 1.0
    Description:
    Uses space-delimited text files to run an ROI check (using the department's unique criteria) on an external beam Plan in Eclipse using some user-provided input. 
    This program also generates a report of the ROI check as a PDF, which includes a standard header used to identify the patient/Plan that the report was made for

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
                return "ID: " + Id + "   Name: " + DisplayName;
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

        static List<TreatSite> MakelistConv()           //treatment site list for Conventional plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Abdomen", Name = "Abdomen", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain", Name = "Brain", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain Hypofx", Name = "BrainHypofx", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast", Name = "Breast", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "Esophagus", Name = "Esophagus", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "Gynecological", Name = "Gynecological", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "Head & Neck", Name = "Head&Neck", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung", Name = "Lung", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis (Other)", Name = "Pelvis(Other)", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT + HDR", Name = "PelivsEBRT+HDR", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate", Name = "Prostate", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Bed", Name = "ProstateBed", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20fx", Name = "ProstateHypo20fx", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28fx", Name = "ProstateHypo28fx", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax (Other)", Name = "Thorax(Other)", Id = 15 });

            return treatsite;
        }

        static List<TreatSite> MakelistSRS()           //treatment site list for SRS plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = " Single fraction", Name = "Singlefraction", Id = 1 });
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

            return treatsite;
        }

        static bool Discriminator(IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, User user)
        {
            bool D = false;
            int cnt = 0;
            string input = null;

            AllocConsole();
            Console.Title = "Lahey RadOnc Dose Objective Checker  V 1.0";

            Console.SetWindowSize(230, 83);                                 //these specific values are here for a reason, don't change them
            Console.SetBufferSize(230, 83);

            Console.WriteLine(" Hi {0}, Welcome to the Lahey RadOnc Dose Objective Checker  V 1.0 \n \n", user.Name);

            Thread.Sleep(1000);

            foreach (PlanSum aplansum in Plansums)
            {
                cnt++;
            }

            if (cnt > 0)
            {
                Console.WriteLine("\nA Plan sum has been dectected in your current Scope. \n.");
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


        static List<ROI.ROI> PlansumAnalysis(User user, Patient patient, Course course, StructureSet structureSet, PlanSum Plansum)
        {

            List<ROI.ROI> ROIE = new List<ROI.ROI>();     // Expected ROI made from text file list
            List<ROI.ROI> ROIA = new List<ROI.ROI>();     // Actual ROI list from Eclipse 
            string Ttype = null;
            string Tsite = null;
            string input = null;
            int t = 0;
            List<TreatSite> sitelist = null;

            Thread.Sleep(1000);

            Console.WriteLine("You have loaded {0}'s course {1}. The currently selected Plansum of course {1} is {2}.  \n", patient.Name, course.Id, Plansum.Id);
            Console.WriteLine("This program will now check the selected Plansum {0} to determine if the Eclipse-calculated Dose values meet the dose objectives established by the Radiation Oncology\nteam for the relevant treatment site. \n", Plansum.Id);
            Console.WriteLine("In order to do this, you must specify the treatment type and treatment site. Is this a conventionally fractionated Plan or an SRS/SBRT Plan? \n");
            Console.WriteLine("(Enter C for conventional or S for SRS/SBRT): ");
            input = Console.ReadLine();
            Console.WriteLine("\n\n ");

            while (input != "c" & input != "C" & input != "S" & input != "s")
            {
                Console.WriteLine("Let's try that again. You must enter either C for Conventional or S for SRS/SBRT.");
                input = Console.ReadLine();
            }

            if (input == "c" | input == "C")
            {
                Console.WriteLine("You have chosen a Conventional Plan.");
                Ttype = "Conventional";
                sitelist = MakelistConv();
            }

            else if (input == "s" | input == "S")
            {
                Console.WriteLine("You have chosen an SRS/SBRT Plan.");
                Ttype = "SRS/SBRT";
                sitelist = MakelistSRS();
            }

            Console.WriteLine("\n\n These are the following treatment sites associated with {0} plans.", Ttype);
            Console.WriteLine("\n");

            foreach (TreatSite aTreatSite in sitelist)
            {
                Console.WriteLine(aTreatSite);
            }

            Console.WriteLine("\nPlease enter the Id number associated with the treatment site of the selected plansum: ");
            t = Convert.ToInt32(Console.ReadLine());

            foreach (TreatSite aTreatSite in sitelist)
            {

                if (aTreatSite.Id == t)
                {
                    Console.WriteLine("\nYou have chosen the {0} treatment site.", aTreatSite.DisplayName);
                    Tsite = aTreatSite.DisplayName;
                    // Console.WriteLine("\n\nTsite is: {0} \n", Tsite);
                    Thread.Sleep(2000);
                    break;
                }

            }


            Console.WriteLine("\n\nStarting dose objectives check ... \n\n");
            // ROI.ROI is its own custom class
            ROIE = Auto_Report_Script.LISTMAKER.Listmaker(Ttype, Tsite);          // separate class with LISTMAKER function which generates a list of ROIs for the given treeatment type and site


            Console.WriteLine("\n{0} dose objective list created successfully.", Tsite);
            Console.WriteLine("\nThe {0} dose objective list contains {1} unique dose objectives.", Tsite, ROIE.Count);

            double dosesum = 0.0;
            string dunit = null;

            foreach (PlanSetup aplan in Plansum.PlanSetups)
            {
                dosesum += aplan.TotalPrescribedDose.Dose;
                dunit = aplan.TotalPrescribedDose.UnitAsString;
            }

            Console.WriteLine("\nSummed Prescribed doses of component plans: {0} {1}", dosesum, dunit);

            Thread.Sleep(2000);

            Console.WriteLine("\nCorrelating dose objectives with structures in current Plansum ... ");


            foreach (ROI.ROI morty in ROIE)
            {

                //  Console.WriteLine("\nThe current dose of objective is: {0}", morty.ROIName);
                // Thread.Sleep(2000);

                foreach (Structure S in structureSet.Structures)        // iterates thriugh all the structures in the structureset of the current Plan
                {

                    if (S.Id == morty.Rstruct)
                    {
                        // Console.WriteLine("\nThe current structure from the Plan is: {0}", S.Id);
                        //  Console.WriteLine("\nThe current dose of objective has the structure tag: {0}", morty.Rstruct);

                        double structvol = S.Volume;

                        if (structvol == 0.0)
                        {

                            continue;

                        }

                        //  Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {1}", S.Id, S.Volume);
                        //  Thread.Sleep(3000);

                        if (morty.limit == "Max Pt Dose")        // MaxPtDose
                        {
                            string kstatus = null;

                            //  Console.WriteLine("\nTRIGGER MAX PT Dose");
                            //   Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //Thread.Sleep(1000);

                            DVHData kDVH = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue maxdose = kDVH.MaxDose;

                            //  Console.WriteLine("\nDOSE UNIT: {0}", maxdose.Unit.ToString());
                            //   Console.WriteLine("\nDOSE Value: {0}", maxdose.Dose);
                            //  Thread.Sleep(4000);


                            if (morty.strict == "[record]")
                            {

                                kstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {

                                        kstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (maxdose.Dose <= Convert.ToDouble(morty.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose <= Convert.ToDouble(morty.limval))
                                    {

                                        kstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = maxdose.Dose, status = kstatus, structvol = structvol, type = "NV"});

                        }
                        else if (morty.limit == "Mean Dose")        // Mean dose
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

                            if (morty.strict == "[record]")
                            {
                                jstatus = "PASS";
                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "REVIEW";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }
                            else if (morty.strict == "<=")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose <= Convert.ToDouble(morty.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose <= Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "REVIEW";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV" });

                        }
                        else if (morty.limit.StartsWith("CV"))
                        {

                            string Lstatus = null;
                            double Lcomp = 0.0;    //compare
                            double Lcomp2 = 0.0;
                            double Lvol = 0.0;
                            double Ldose = 0.0;    //functional dose
                            string type = "cm3";
                            double Llimit = 0.0;


                            string jerry = morty.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);

                            Lcomp = Convert.ToDouble(morty.limval);  // VOLUME IN CM3

                            if (morty.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(morty.goal);   // VOLUME IN CM3
                            }

                            Ldose = (Llimit / 100.0) * dosesum;    // this calculates an absolute dose from the fractional value of Vgy

                            DVHData Ldvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            Console.WriteLine("\nDVH Point Curves Volume Unit CM3: {0}", Ldvh.CurveData[1].VolumeUnit);
                            Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Ldvh.CurveData[1].DoseValue.UnitAsString);

                            Thread.Sleep(2000);

                            foreach (DVHPoint point in Ldvh.CurveData)
                            {

                                if ((point.DoseValue.Dose >= (Ldose - 0.5)) && (point.DoseValue.Dose <= (Ldose + 0.5)))
                                {
                                    Console.WriteLine("\nTrigger DVH Point match!!");
                                    Lvol = point.Volume;

                                }
                            }

                            if (morty.strict == "[record]")
                            {
                                Lstatus = "PASS";
                            }
                            else if (morty.strict == ">")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (Lvol > Lcomp2)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else if (Lvol < Lcomp)
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (Lvol > Lcomp)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limvol = Lcomp, goalvol = Lcomp2, actvol = Lvol, status = Lstatus, structvol = structvol, type = type });

                        }
                        else if (morty.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;
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

                            //   Console.WriteLine("\nTRIGGER V ");
                            //  Console.WriteLine("\nV Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //  Thread.Sleep(2000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            if (morty.limit != "V60 is NOT Circumferential")                   // This allows the "V" in the limit string to be omitted so we just get the number    
                            {
                                string jerry = morty.limit.Substring(1);
                                //  Console.WriteLine("\n After V chop, we have (jerry): {0}", jerry);
                                // Thread.Sleep(2000);
                                Vgy = Convert.ToDouble(jerry);                                 // "V gray" 

                            }
                            else if (morty.limit == "V100%Rx")
                            {

                                Vgy = 100.0;
                            }
                            else
                            {
                                Vgy = 50000;
                            }

                            if (morty.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(morty.limval);

                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                if (morty.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(morty.goal);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));

                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                fdose = (Vgy / 100.0) * dosesum;    // this calculates an absolute dose from the fractional value of Vgy

                                //  Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                Console.WriteLine("\nDVH Point Curves Volume Unit PERCENT: {0}", Vdvh.CurveData[1].VolumeUnit);
                                Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Vdvh.CurveData[1].DoseValue.UnitAsString);

                                Thread.Sleep(2000);

                                foreach (DVHPoint point in Vdvh.CurveData)
                                {

                                    if ((point.DoseValue.Dose >= (fdose - 0.5)) && (point.DoseValue.Dose <= (fdose + 0.5)))
                                    {
                                        Console.WriteLine("\nTrigger DVH Point match!!");
                                        Vvol = point.Volume;
                                    }
                                }

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                //  Thread.Sleep(5000);

                            }
                            else if (morty.limunit == "cc")
                            {
                                type = "cm3";

                                comp = Convert.ToDouble(morty.limval);  // VOLUME IN CM3

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (morty.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(morty.goal);   // VOLUME IN CM3

                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                fdose = (Vgy / 100.0) * dosesum;    // this calculates an absolute dose from the fractional value of Vgy

                                // Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);

                                DVHData Vdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                                Console.WriteLine("\nDVH Point Curves Volume Unit CM3: {0}", Vdvh.CurveData[1].VolumeUnit);
                                Console.WriteLine("\nDVH Point Curves Dose Unit cGy: {0}", Vdvh.CurveData[1].DoseValue.UnitAsString);

                                Thread.Sleep(2000);

                                foreach (DVHPoint point in Vdvh.CurveData)
                                {

                                    if ((point.DoseValue.Dose >= (fdose - 0.5)) && (point.DoseValue.Dose <= (fdose + 0.5)))
                                    {
                                        Console.WriteLine("\nTrigger DVH Point match!!");
                                        Vvol = point.Volume;

                                    }
                                }
                            }

                            if (morty.strict == "[record]")
                            {
                                fstatus = "PASS";
                            }
                            else if (morty.strict == "<")
                            {
                                if (morty.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Vvol < comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol < comp)
                                    { 
                                        fstatus = "REVIEW";
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
                            else if (morty.strict == "<=")
                            {
                                if (morty.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Vvol <= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol <= comp)
                                    {
                                        fstatus = "REVIEW";
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
                            else if (morty.strict == ">=")
                            {
                                if (morty.goal != "NA")            // meaning there is a goal set
                                {
                                    if (Vvol >= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol >= comp)
                                    {
                                        fstatus = "REVIEW";
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

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limvol = comp, goalvol = comp2, actvol = Vvol, status = fstatus, structvol = structvol, type = type });

                        }
                        else if (morty.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {

                            string qstatus = null;
                            Double qdose = 0.0;

                            //  Console.WriteLine("\nTRIGGER D ");
                            //  Console.WriteLine("\nD Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = morty.limit.Substring(1);                     // "V gray" 

                            //  Console.WriteLine("\nqstring after D remove: {0}", qstring);

                            if (morty.limit.EndsWith("cc"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);

                                double qvol = Convert.ToDouble(q2str);

                                // Console.WriteLine("\n q2str is: {0}", q2str);

                                // qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                                Console.WriteLine("\nDVH Point Curves volume unit CM3: {0}", Qdvh.CurveData[1].VolumeUnit);

                                foreach (DVHPoint point in Qdvh.CurveData)
                                {

                                    if ((point.Volume == qvol))
                                    {

                                        qdose = point.DoseValue.Dose;

                                    }
                                }

                                //  Console.WriteLine("\n ABS DOSE: {0}", qdose.Dose);
                                //  Thread.Sleep(4000);

                            }
                            else if (morty.limit.EndsWith("%"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);

                                double qvol = Convert.ToDouble(q2str);

                                // qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DVHData Qdvh = Plansum.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);

                                Console.WriteLine("\nDVH Point Curves volume unit PERCENT: {0}", Qdvh.CurveData[1].VolumeUnit);

                                foreach (DVHPoint point in Qdvh.CurveData)
                                {

                                    if ((point.Volume == qvol))
                                    {

                                        qdose = point.DoseValue.Dose;

                                    }
                                }

                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", qdose.Dose, tqdose.Dose);
                                //  Thread.Sleep(5000);

                            }


                            if (morty.strict == "[record]")
                            {

                                qstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((qdose < Convert.ToDouble(morty.limval)) && (qdose < Convert.ToDouble(morty.goal)))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if (qdose < Convert.ToDouble(morty.goal))
                                    {

                                        qstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }
                                }
                                else
                                {

                                    if (qdose < Convert.ToDouble(morty.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }
                                }
                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((qdose <= Convert.ToDouble(morty.limval)) && (qdose <= Convert.ToDouble(morty.goal)))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if (qdose <= Convert.ToDouble(morty.goal))
                                    {

                                        qstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        qstatus = "REVIEW";

                                    }

                                }
                                else
                                {

                                    if (qdose < Convert.ToDouble(morty.limval))
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

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = qdose, status = qstatus, structvol = structvol, type = "NV"});


                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
            }// Ends ROIE iterator loop
        

        // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI.ROI comparison.

            Console.WriteLine("\n{0} unique dose objectives matched with structures in the current Plansum.", ROIA.Count);


            Console.WriteLine("\nDose objective check complete.");

            return ROIA;
        }

        static List<ROI.ROI> PlanAnalysis(User user, Patient patient, Course course, StructureSet structureSet, PlanSetup Plan)
        {

            List<ROI.ROI> ROIE = new List<ROI.ROI>();     // Expected ROI made from text file list
            List<ROI.ROI> ROIA = new List<ROI.ROI>();     // Actual ROI list from Eclipse 
            string Ttype = null;
            string Tsite = null;
            string input = null;
            int t = 0;
            List<TreatSite> sitelist = null;

            Thread.Sleep(1000);

            Console.WriteLine("You have loaded {0}'s course {1}. The currently selected Plan of course {1} is {2}.  \n", patient.Name, course.Id, Plan.Id);
            Console.WriteLine("This program will now check the selected Plan {0} to determine if the Eclipse-calculated Dose values meet the dose objectives established by the Radiation Oncology\nteam for the relevant treatment site. \n", Plan.Id);
            Console.WriteLine("In order to do this, you must specify the treatment type and treatment site. Is this a conventionally fractionated Plan or an SRS/SBRT Plan? \n");
            Console.WriteLine("(Enter C for conventional or S for SRS/SBRT): ");
            input = Console.ReadLine();
            Console.WriteLine("\n\n ");

            while (input != "c" & input != "C" & input != "S" & input != "s")
            {
                Console.WriteLine("Let's try that again. You must enter either C for Conventional or S for SRS/SBRT.");
                input = Console.ReadLine();
            }

            if (input == "c" | input == "C")
            {
                Console.WriteLine("You have chosen a Conventional Plan.");
                Ttype = "Conventional";
                sitelist = MakelistConv();
            }

            else if (input == "s" | input == "S")
            {
                Console.WriteLine("You have chosen an SRS/SBRT Plan.");
                Ttype = "SRS/SBRT";
                sitelist = MakelistSRS();
            }

            Console.WriteLine("\n\n These are the following treatment sites associated with {0} plans.", Ttype);
            Console.WriteLine("\n");

            foreach (TreatSite aTreatSite in sitelist)
            {
                Console.WriteLine(aTreatSite);
            }

            Console.WriteLine("\nPlease enter the Id number associated with the treatment site of the selected Plan: ");
            t = Convert.ToInt32(Console.ReadLine());

            foreach (TreatSite aTreatSite in sitelist)
            {

                if (aTreatSite.Id == t)
                {
                    Console.WriteLine("\nYou have chosen the {0} treatment site.", aTreatSite.DisplayName);
                    Tsite = aTreatSite.DisplayName;
                    // Console.WriteLine("\n\nTsite is: {0} \n", Tsite);
                    Thread.Sleep(2000);
                    break;
                }

            }


            Console.WriteLine("\n\nStarting dose objectives check ... \n\n");
            // ROI.ROI is its own custom class
            ROIE = Auto_Report_Script.LISTMAKER.Listmaker(Ttype, Tsite);          // separate class with LISTMAKER function which generates a list of ROIs for the given treeatment type and site


            Console.WriteLine("\n{0} dose objective list created successfully.", Tsite);
            Console.WriteLine("\nThe {0} dose objective list contains {1} unique dose objectives.", Tsite, ROIE.Count);
            Thread.Sleep(2000);

            // This part of code below gets DVH data from Eclipse. The way it works is different for different limit types, like MaxPtDose, V80, D1cc. etc.


            double pdose = Plan.TotalPrescribedDose.Dose;       // prescribed dose of the Plan

            Console.WriteLine("\nPRESCRIBED DOSE: {0} {1}", pdose, Plan.TotalPrescribedDose.Unit.ToString());

            Thread.Sleep(2000);

            Console.WriteLine("\nCorrelating dose objectives with structures in current Plan ... ");

            foreach (ROI.ROI morty in ROIE)
            {

                //  Console.WriteLine("\nThe current dose of objective is: {0}", morty.ROIName);
                // Thread.Sleep(2000);

                foreach (Structure S in structureSet.Structures)        // iterates thriugh all the structures in the structureset of the current Plan
                {

                    if (S.Id == morty.Rstruct)
                    {
                        // Console.WriteLine("\nThe current structure from the Plan is: {0}", S.Id);
                        //  Console.WriteLine("\nThe current dose of objective has the structure tag: {0}", morty.Rstruct);

                        double structvol = S.Volume;

                        if (structvol == 0.0)
                        {

                            continue;

                        }


                        //  Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {1}", S.Id, S.Volume);
                        //  Thread.Sleep(3000);


                        if (morty.limit == "Max Pt Dose")        // MaxPtDose
                        {
                            string kstatus = null;

                            //  Console.WriteLine("\nTRIGGER MAX PT Dose");
                            //   Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //Thread.Sleep(1000);

                            DVHData kDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue maxdose = kDVH.MaxDose;

                            //  Console.WriteLine("\nDOSE UNIT: {0}", maxdose.Unit.ToString());
                            //   Console.WriteLine("\nDOSE Value: {0}", maxdose.Dose);
                            //  Thread.Sleep(4000);


                            if (morty.strict == "[record]")
                            {

                                kstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {

                                        kstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (maxdose.Dose <= Convert.ToDouble(morty.goal))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose <= Convert.ToDouble(morty.limval))
                                    {

                                        kstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (maxdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else
                                    {
                                        kstatus = "REVIEW";

                                    }

                                }


                            }

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = maxdose.Dose, status = kstatus, structvol = structvol, type = "NV" });


                        }
                        else if (morty.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;

                            //  Console.WriteLine("\nTRIGGER Mean");
                            //  Console.WriteLine("\nMean Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            DVHData jDVH = Plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue meandose = jDVH.MeanDose;

                            //   Console.WriteLine("\nDOSE UNIT: {0}", meandose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Vale: {0}", meandose.Dose);
                            //  Thread.Sleep(4000);


                            if (morty.strict == "[record]")
                            {

                                jstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {

                                        jstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (meandose.Dose <= Convert.ToDouble(morty.goal))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose <= Convert.ToDouble(morty.limval))
                                    {

                                        jstatus = "REVIEW";

                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";

                                    }


                                }
                                else
                                {

                                    if (meandose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else
                                    {
                                        jstatus = "REVIEW";

                                    }

                                }


                            }



                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = meandose.Dose, status = jstatus, structvol = structvol, type = "NV" });

                        }
                        else if (morty.limit.StartsWith("CV"))
                        {

                            string Lstatus = null;
                            double Lcomp = 0.0;    //compare
                            double Lcomp2 = 0.0;
                            double Lvol = 0.0;
                            string type = "cm3";
                            double Llimit = 0.0;


                            string jerry = morty.limit.Substring(2);
                            Llimit = Convert.ToDouble(jerry);

                            Lcomp = Convert.ToDouble(morty.limval);  // VOLUME IN CM3

                            if (morty.goal != "NA")
                            {
                                Lcomp2 = Convert.ToDouble(morty.goal);   // VOLUME IN CM3
                            }

                            DoseValue Ldose = new DoseValue(Llimit, DoseValue.DoseUnit.Percent);

                            Lvol = Plan.GetVolumeAtDose(S, Ldose, VolumePresentation.AbsoluteCm3);

                            if (morty.strict == "[record]")
                            {
                                Lstatus = "PASS";
                            }
                            else if (morty.strict == ">")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (Lvol > Lcomp2)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else if (Lvol < Lcomp)
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                                else
                                {

                                    if (Lvol > Lcomp)
                                    {
                                        Lstatus = "PASS";
                                    }
                                    else
                                    {
                                        Lstatus = "REVIEW";
                                    }
                                }
                            }

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limvol = Lcomp, goalvol = Lcomp2, actvol = Lvol, status = Lstatus, structvol = structvol, type = type });

                        }
                        else if (morty.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;
                            //  DoseValue fdose = new DoseValue();
                            //  DoseValue gfdose = new DoseValue();
                            double Vgy = 0.0;
                            double gfvol = 0.0;
                            double fvol = 0.0;
                            double comp = 0.0;    //compare
                            double comp2 = 0.0;
                            double Vvol = 0.0;
                            string type = null;

                            //   Console.WriteLine("\nTRIGGER V ");
                            //  Console.WriteLine("\nV Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            //  Thread.Sleep(2000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            if (morty.limit != "V60 is NOT Circumferential")                   // This allows the "V" in the limit string to be omitted so we just get the number    
                            {
                                string jerry = morty.limit.Substring(1);
                                //  Console.WriteLine("\n After V chop, we have (jerry): {0}", jerry);
                                // Thread.Sleep(2000);
                                Vgy = Convert.ToDouble(jerry);                                  

                            }
                            else if (morty.limit == "V100%Rx")
                            {

                                Vgy = 100.0;
                            }
                            else
                            {

                                Vgy = 50000;

                            }

                            if (morty.limunit == "%")
                            {
                                type = "percent";

                                comp = Convert.ToDouble(morty.limval);

                                // fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                // fdose = Plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tfdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                if (morty.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(morty.goal);

                                    // gfvol = (structvol * ((Convert.ToDouble(morty.goal)) / 100.0));

                                    // gfdose = Plan.GetDoseAtVolume(S, gfvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                DoseValue Vdose = new DoseValue(Vgy, DoseValue.DoseUnit.Percent);

                                // Console.WriteLine("\nVDOSE: {0} {1}", Vdose.Dose, Vdose.UnitAsString);

                                // Thread.Sleep(2000);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.Relative);        //dvolper - dose volume percent

                                //  Console.WriteLine("\nVVOL: {0}", Vvol);

                                // Thread.Sleep(2000);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                //  Thread.Sleep(5000);

                            }
                            else if (morty.limunit == "cc")
                            {

                                type = "cm3";
                                comp = Convert.ToDouble(morty.limval);

                                // fdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                if (morty.goal != "NA")
                                {

                                    comp2 = Convert.ToDouble(morty.goal);

                                    // gfdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                }

                                DoseValue Vdose = new DoseValue(Vgy, DoseValue.DoseUnit.Percent);

                                //  Console.WriteLine("\nVDOSE: {0} {1}", Vdose.Dose, Vdose.UnitAsString);

                                //  Thread.Sleep(2000);

                                Vvol = Plan.GetVolumeAtDose(S, Vdose, VolumePresentation.AbsoluteCm3);

                                //  Console.WriteLine("\nVVOL: {0}", Vvol);

                                //  Thread.Sleep(2000);

                            }

                            if (morty.strict == "[record]")
                            {

                                fstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (Vvol < comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol < comp)
                                    {

                                        fstatus = "REVIEW";

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
                            else if (morty.strict == "<=")
                            {
                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (Vvol <= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol <= comp)
                                    {

                                        fstatus = "REVIEW";

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
                            else if (morty.strict == ">=")
                            {
                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if (Vvol >= comp2)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (Vvol >= comp)
                                    {
                                        fstatus = "REVIEW";
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

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limvol = comp, goalvol = comp2, actvol = Vvol, status = fstatus, structvol = structvol, type = type });

                        }
                        else if (morty.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {

                            string qstatus = null;
                            DoseValue qdose = new DoseValue();

                            //  Console.WriteLine("\nTRIGGER D ");
                            //  Console.WriteLine("\nD Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            // Thread.Sleep(1000);

                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = morty.limit.Substring(1);                     // "V gray" 

                            //  Console.WriteLine("\nqstring after D remove: {0}", qstring);

                            if (morty.limit.EndsWith("cc"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('c'), 2);

                                // Console.WriteLine("\n q2str is: {0}", q2str);

                                qdose = Plan.GetDoseAtVolume(S, Convert.ToDouble(q2str), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n ABS DOSE: {0}", qdose.Dose);
                                //  Thread.Sleep(4000);

                            }
                            else if (morty.limit.EndsWith("%"))
                            {

                                string q2str = qstring.Remove(qstring.IndexOf('%'), 1);

                                //  Console.WriteLine("\n q2str is: {0}", q2str);

                                double qvol = (structvol * ((Convert.ToDouble(q2str)) / 100.0));           // specific volume that the ROI.ROI is concerned with. Here, limval is the percent of the volume of the structure

                                qdose = Plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                //  DoseValue tqdose = Plan.GetDoseAtVolume(S, (Convert.ToDouble(q2str) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                //  Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", qdose.Dose, tqdose.Dose);
                                //  Thread.Sleep(5000);

                            }


                            if (morty.strict == "[record]")
                            {

                                qstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((qdose.Dose < Convert.ToDouble(morty.limval)) && (qdose.Dose < Convert.ToDouble(morty.goal)))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if (qdose.Dose < Convert.ToDouble(morty.goal))
                                    {

                                        qstatus = "WARNING";

                                    }
                                    else
                                    {
                                        qstatus = "FAIL";

                                    }


                                }
                                else
                                {

                                    if (qdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "FAIL";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((qdose.Dose <= Convert.ToDouble(morty.limval)) && (qdose.Dose <= Convert.ToDouble(morty.goal)))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else if (qdose.Dose <= Convert.ToDouble(morty.goal))
                                    {

                                        qstatus = "WARNING";

                                    }
                                    else
                                    {
                                        qstatus = "FAIL";

                                    }


                                }
                                else
                                {

                                    if (qdose.Dose < Convert.ToDouble(morty.limval))
                                    {
                                        qstatus = "PASS";
                                    }
                                    else
                                    {
                                        qstatus = "FAIL";

                                    }

                                }


                            }

                            //  Console.WriteLine("\nDOSE UNIT: {0}", qdose.Unit.ToString());
                            //  Console.WriteLine("\nDOSE Value: {0}", qdose.Dose);
                            //  Thread.Sleep(5000);

                            ROIA.Add(new ROI.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), goal = morty.goal, actdose = qdose.Dose, status = qstatus, structvol = structvol, type = "NV" });


                        }  // ends the D loop
                    }   // ends the if structure match loop
                }   // ends structure iterating through current ROIE loop
            }// Ends ROIE iterator loop

            // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI.ROI comparison.

            Console.WriteLine("\n{0} unique dose objectives matched with structures in the current Plan.", ROIA.Count);


            Console.WriteLine("\nDose objective check complete.");


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



        public void Execute(ScriptContext context )     // PROGRAM START - sending a return to Execute will end the program
        {
            //regular variables

            List<ROI.ROI> output = new List<ROI.ROI>();
            bool T = false;
            bool Discrim;
            
            //ESAPI variables  NOTE: CANNOT INSTANTIATE ECLIPSE VARIABLES. CAN ONLT GET THEM FROM ECLIPSE.

            Patient patient = context.Patient;   // creates an object of the patient class called patient equal to the active patient open in Eclipse
            Course course = context.Course;
            Image image3D = context.Image;
            StructureSet structureSet = context.StructureSet;
            User user = context.CurrentUser;
            IEnumerable<PlanSum> Plansums = context.PlanSumsInScope;
            IEnumerable<PlanSetup> Plans = context.PlansInScope;


            if (context.Patient == null)
            {
                MessageBox.Show("Please load a patient with a treatment Plan before running this script!");
                return;
            }

            // this area calls outside functions that perform automatic checks on system


           // ContourChecks.CountourChecks.ContoursInBody(structureSet);      
                


            // Functions below here are for the dose objective check program

            Discrim = Discriminator(Plansums, Plans, user);
            
            if (Discrim == true)
            {

                PlanSum Plansum = PlansumPick(Plansums);
                output = PlansumAnalysis(user, patient, course, structureSet, Plansum);

                foreach (ROI.ROI aroi in output)
                {
                    if (aroi.status == "REVIEW")
                    {
                        T = true;
                    }
                }

                if (T == true)
                {
                    MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
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
                    MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
                }

                Console.WriteLine("\n\n\n");

                Console.WriteLine("A report of the Dose Objective Check will now open in your default PDF viewer. Please rename it and save it in the 'Dose Objective Checker Reports' folder.");

                PdfReport.PDFGenerator.Program.PlanMain(patient, course, Plan, image3D, structureSet, user, output);

            }
    

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

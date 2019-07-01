using System;
using System.Collections.Generic;
using System.Linq;
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
using PdfReport.PDFGenerator; 


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
    Uses space-delimited text files to run an ROI check (using the department's unique criteria) on an external beam plan in Eclipse using some user-provided input. 
    This program also generates a report of the ROI check as a PDF, which includes a standard header used to identify the patient/plan that the report was made for

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

        // Declaration Space for Functions and Variables
        



        public class TreatSite : IEquatable<TreatSite>        //makes a treatment site class used to make a list of treatment sites 
        {                                                     //the treatment site list is used when the user is prompted to choose the treatment site of the selected plan
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
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20 fx", Name = "ProstateHypo20fx", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28 fx", Name = "ProstateHypo28fx", Id = 14 });
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
                treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial Trigeminal Neuralgia", Name = "SRSCranialTrigeminalNeralgia", Id = 20 });

            return treatsite;
        }


        static int IOConsole(User user, Patient patient, PlanSetup plan, Course course, StructureSet structureSet)   // This "IOConsole" function is the entire I/O interface for the program, and as such it is very long. It runs once and collects the requisite info from the user. 
                                                                                                                        // This is neccessary so we can use the Command Line in what is technically a .DLL assembly file.                        
        {

            List<Auto_Report_Script.ROI> ROIE = new List<Auto_Report_Script.ROI>();     // Expected ROI made from text file list
            List<Auto_Report_Script.ROI> ROIA = new List<Auto_Report_Script.ROI>();     // Actual ROI list from Eclipse 

            string Ttype = null;
            string Tsite = null;
            
            int k = 0;
            string input = null;
            int t = 0;
            List<TreatSite> sitelist = null;

            
            AllocConsole();
            Console.Title = "Lahey RadOnc Dose Objective Checker  V 1.0";

            Console.SetWindowSize(230, 83);                                 //these specific values are here for a reason, don't change them
            Console.SetBufferSize(230, 83);

            Console.WriteLine(" Hi {0}, Welcome to the Lahey RadOnc Automatic Patient Plan Report Generator and ROI Criteria Checker  V 1.0 \n \n", user.Name);
                                                                                                                                                                                                    //this is the size limit for characters on one line of cmd prompt
            Thread.Sleep(2000);

            Console.WriteLine("You have loaded {0}'s course {1}. The currently selected plan of course {1} is {2}.  \n", patient.Name, course.Name, plan.Name);
            Console.WriteLine("This program will now check the selected plan {0} to determine if the Eclipse-calculated DVH values meet the dose objectives established by the Radiation Oncology\nteam for the relevant treatment site. \n", plan.Name);
            Console.WriteLine("In order to do this, you must specify the treatment type and treatment site. Is this a conventionally fractionated plan or an SRS/SBRT plan? \n");
            Console.WriteLine("(Enter C for conventional or S for SRS/SBRT): ");
            input = Console.ReadLine();
            Console.WriteLine("\n\n ");
          
            while (input != "c" & input != "C" & input != "S" & input != "s")
            {
                Console.WriteLine("Let's try that again. You must enter either C for Conventional or S for SRS/SBRT.");
                input = Console.ReadLine();
            }

            if (input == "c" | input =="C")
            {
                Console.WriteLine("You have chosen a Conventional plan.");
                Ttype = "Conventional";
                sitelist = MakelistConv();
            }

            else if (input == "s" | input == "S")
            {
                Console.WriteLine("You have chosen an SRS/SBRT plan.");
                Ttype = "SRS/SBRT";
                sitelist = MakelistSRS();
            }

            Console.WriteLine("\n\n These are the following treatment sites associated with {0} plans.", Ttype);
            Console.WriteLine("\n");

            foreach (TreatSite aTreatSite in sitelist)
            {
                Console.WriteLine(aTreatSite);
            }

            Console.WriteLine("\nPlease enter the Id number associated with the treatment site of the selected plan: ");
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
                                                                                   // ROI is its own custom class
            ROIE = Auto_Report_Script.LISTMAKER.Listmaker(Ttype, Tsite);          // separate class with LISTMAKER function which generates a list of ROIs for the given treeatment type and site

         
            Console.WriteLine("\n{0} dose objective list created successfully.", Tsite);
            Console.WriteLine("\nThe {0} dose objective list contains {1} unique dose objectives.", Tsite, ROIE.Count);
            Thread.Sleep(2000);
            

            // This part of code below gets DVH data from Eclipse. The way it works is different for different limit types, like MaxPtDose, V80, D1cc. etc.

 
            double pdose = plan.TotalPrescribedDose.Dose;       // prescribed dose of the plan

            Console.WriteLine("\nPRESCRIBED DOSE: {0} {1}", pdose, plan.TotalPrescribedDose.Unit.ToString());

            Thread.Sleep(4000);

            Console.WriteLine("\nCorrelating dose objectives with structures in current plan ... ");

            foreach (Auto_Report_Script.ROI morty in ROIE)
            {

                foreach (Structure S in structureSet.Structures)        // iterates thriugh all the structures in the structureset of the current plan
                {

                    if (S.Id == morty.Rstruct)
                    {

                        double structvol = S.Volume;
                        Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {1}", S.Id, S.Volume);
                        Thread.Sleep(1000);



                        if (morty.limit == "Max Pt Dose")        // MaxPtDose
                        {
                            string kstatus = null;
 
                            Console.WriteLine("\nTRIGGER MAX PT Dose");
                            Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            Thread.Sleep(1000);

                            DVHData kDVH = plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue maxdose = kDVH.MaxDose;
                            
                            Console.WriteLine("\nDOSE UNIT: {0}", maxdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", maxdose.Dose);
                            Thread.Sleep(5000);


                            if(morty.strict == "[record]")
                            {

                                kstatus = "PASS";

                            }
                            else if(morty.strict == "<")
                            {

                                if(morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((maxdose.Dose < Convert.ToDouble(morty.limval)) && (maxdose.Dose < Convert.ToDouble(morty.goal)))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose < Convert.ToDouble(morty.goal))
                                    {

                                        kstatus = "WARNING";

                                    }
                                    else
                                    {
                                        kstatus = "FAIL";

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
                                        kstatus = "FAIL";

                                    }

                                }


                            }
                            else if(morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((maxdose.Dose <= Convert.ToDouble(morty.limval)) && (maxdose.Dose <= Convert.ToDouble(morty.goal)))
                                    {
                                        kstatus = "PASS";
                                    }
                                    else if (maxdose.Dose <= Convert.ToDouble(morty.goal))
                                    {

                                        kstatus = "WARNING";

                                    }
                                    else
                                    {
                                        kstatus = "FAIL";

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
                                        kstatus = "FAIL";

                                    }

                                }


                            }

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval) , actdose = maxdose.Dose, status = kstatus, structvol = structvol });


                        }
                        else if (morty.limit == "Mean Dose")        // Mean dose
                        {
                            string jstatus = null;

                            Console.WriteLine("\nTRIGGER Mean");
                            Console.WriteLine("\nMean Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            Thread.Sleep(1000);

                            DVHData jDVH = plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue meandose = jDVH.MaxDose;

                            Console.WriteLine("\nDOSE UNIT: {0}", meandose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", meandose.Dose);
                            Thread.Sleep(5000);


                            if (morty.strict == "[record]")
                            {

                                jstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((meandose.Dose < Convert.ToDouble(morty.limval)) && (meandose.Dose < Convert.ToDouble(morty.goal)))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose < Convert.ToDouble(morty.goal))
                                    {

                                        jstatus = "WARNING";

                                    }
                                    else
                                    {
                                        jstatus = "FAIL";

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
                                        jstatus = "FAIL";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((meandose.Dose <= Convert.ToDouble(morty.limval)) && (meandose.Dose <= Convert.ToDouble(morty.goal)))
                                    {
                                        jstatus = "PASS";
                                    }
                                    else if (meandose.Dose <= Convert.ToDouble(morty.goal))
                                    {

                                        jstatus = "WARNING";

                                    }
                                    else
                                    {
                                        jstatus = "FAIL";

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
                                        jstatus = "FAIL";

                                    }

                                }


                            }



                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), actdose = meandose.Dose, status = jstatus, structvol = structvol});


                        }
                        else if (morty.limit.StartsWith("V"))         // V45   45 is the dose in cGy which specifies a maximum dose that a specific amount (percentage or absolute amount) of the volume of a structure can recieve
                        {
                            string fstatus = null;
                            DoseValue fdose = new DoseValue();
                            DoseValue gfdose = new DoseValue();
                            double Vgy = 0.0;

                            Console.WriteLine("\nTRIGGER V ");
                            Console.WriteLine("\nV Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            Thread.Sleep(1000);

                                                                                              // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                            if(morty.limit != "V60 is NOT Circumferential")                   // This allows the "V" in the limit string to be omitted so we just get the number    
                            {

                                 Vgy = Convert.ToDouble(morty.limit.Substring(1));       // "V gray" 

                            }
                            else
                            {

                                Vgy = 50000;

                            }

                            if(morty.limunit == "%")
                            {

                                double fvol = (structvol * ((Convert.ToDouble(morty.limval)) / 100.0));           // specific volume that the ROI is concerned with. Here, limval is the percent of the volume of the structure

                                fdose = plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DoseValue tfdose = plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);
                                 gfdose = plan.GetDoseAtVolume(S, (Convert.ToDouble(morty.goal) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                Thread.Sleep(5000);

                            }
                            else if(morty.limunit == "cc")
                            {

                                fdose = plan.GetDoseAtVolume(S, Convert.ToDouble(morty.limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                                gfdose = plan.GetDoseAtVolume(S, Convert.ToDouble(morty.goal), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                            }


                            if (morty.strict == "[record]")
                            {

                                fstatus = "PASS";

                            }
                            else if (morty.strict == "<")
                            {

                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((fdose.Dose < Vgy) && (gfdose.Dose < Vgy))
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (fdose.Dose < Vgy)
                                    {

                                        fstatus = "WARNING";

                                    }
                                    else
                                    {
                                        fstatus = "FAIL";

                                    }


                                }
                                else
                                {

                                    if (fdose.Dose < Vgy)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "FAIL";

                                    }

                                }


                            }
                            else if (morty.strict == "<=")
                            {


                                if (morty.goal != "NA")            // meaning there is a goal set
                                {

                                    if ((fdose.Dose <= Vgy) && (gfdose.Dose <= Vgy))
                                    {
                                        fstatus = "PASS";
                                    }
                                    else if (fdose.Dose <= Vgy)
                                    {

                                        fstatus = "WARNING";

                                    }
                                    else
                                    {
                                        fstatus = "FAIL";

                                    }


                                }
                                else
                                {

                                    if (fdose.Dose < Vgy)
                                    {
                                        fstatus = "PASS";
                                    }
                                    else
                                    {
                                        fstatus = "FAIL";

                                    }

                                }


                            }

                             // add thing to deal with Esophagus circumferential

                            Console.WriteLine("\nDOSE UNIT: {0}", fdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", fdose.Dose);
                            Thread.Sleep(5000);

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = morty.ROIName, limdose = Vgy  , actdose = fdose.Dose, status = fstatus, structvol = structvol});

                        }
                        else if (morty.limit.StartsWith("D"))            // D5%  - 5% is 5% of the volume of the structure that must be under a specific dose limit
                        {

                            string qstatus = null;
                            DoseValue qdose = new DoseValue();

                            Console.WriteLine("\nTRIGGER D ");
                            Console.WriteLine("\nD Dose Limit: {0}  {1}", morty.limval, morty.limunit);
                            Thread.Sleep(1000);

                                                                                             // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                                                                                             // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = morty.limit.Substring(1);                     // "V gray" 

                            Console.WriteLine("\nqstring after start cut: {0}", qstring);

                            if (morty.limit.EndsWith("cc"))
                            {
                                char[] endl = new char[2] { 'c', 'c' };

                                qstring.TrimEnd(endl);

                                Console.WriteLine("\n qstring is: {0}", qstring);

                                qdose = plan.GetDoseAtVolume(S, Convert.ToDouble(qstring), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                Console.WriteLine("\n ABS DOSE: {0}", qdose.Dose);
                                Thread.Sleep(4000);

                            }
                            else if (morty.limit.EndsWith("%"))
                            {
                                char[] endv = new char[1] { '%' };

                                qstring.TrimEnd(endv);

                                Console.WriteLine("\n qstring is: {0}", qstring);

                                double qvol = (structvol * ((Convert.ToDouble(qstring)) / 100.0));           // specific volume that the ROI is concerned with. Here, limval is the percent of the volume of the structure

                                qdose = plan.GetDoseAtVolume(S, qvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DoseValue tqdose = plan.GetDoseAtVolume(S, (Convert.ToDouble(qstring) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", qdose.Dose, tqdose.Dose);
                                Thread.Sleep(5000);
                             
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


                            Console.WriteLine("\nDOSE UNIT: {0}", qdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", qdose.Dose);
                            Thread.Sleep(5000);

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = morty.ROIName, limdose = Convert.ToDouble(morty.limval), actdose = qdose.Dose, status = qstatus, structvol = structvol });


                        }  // ends the D loop


                    }   // ends the if structure match loop



                }   // ends structure iterating through current ROIE loop




                /*

                                var DVH = plan.DVHEstimates;         // plan.DVHEstimates returns a list of all the "EstimatedDVH" objects in the plan. EstimatedDVH is a custom class of the VMS API

                    foreach (EstimatedDVH dvh in DVH)
                    {

                        if (dvh.StructureId == morty.Rstruct)
                        {

                            foreach (DVHPoint dpoint in dvh.CurveData)            // "CurveData" returns an array of "DVHPoints", another custom class
                            {
                                dose[dptcount] = dpoint.DoseValue.Dose;

                                dptcount++;
                            }

                            maxptdose = dose.Max();

                        }

                    }

                    if (maxptdose <= morty.limval)
                    {
                        morty.status = "PASS";
                    }
                    else
                    {
                        morty.status = "FAIL";

                    }

                    */






            }    // Ends ROIE iterator loop



            // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI comparison.

            Console.WriteLine("\n{0} unique dose objectives matched with structures in the current plan.", ROIA.Count);


            Console.WriteLine("\nDose objective check complete.");

            Thread.Sleep(2000);

            Console.WriteLine("\n\n\t\tDOSE OBJECTIVE REPORT FOR {0}'S PLAN {1}", patient.Name, plan.Name);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Name                                                   |  Hard Dose Limit (cGy) | Goal Dose Limit (cGy) | Eclipse Estimated Dose (cGy) |  Status  | Structure Volume  ");
                         
            foreach(Auto_Report_Script.ROI aroi in ROIA)
            {

                if(aroi.strict == "[record]")
                {

                    Console.WriteLine("\n\n");
                    Console.WriteLine("{0}      NA           {1}         {3}         {2}    {3} ", aroi.ROIName, aroi.goal, aroi.actdose, aroi.status, aroi.structvol);

                }
                else
                {

                    Console.WriteLine("\n\n");
                    Console.WriteLine("{0}      {1}         {2}          {3}        {4}   {5} ", aroi.ROIName, aroi.limdose, aroi.goal, aroi.actdose, aroi.status, aroi.structvol);

                }

            }

            Console.WriteLine("\n\n");

            Console.WriteLine("Would you like to generate a PDF of this dose objective report (Y/N)?  ");
            input = Console.ReadLine();
            if (input == "n" | input == "N")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("This program will now close.");
                Thread.Sleep(3000);

                return k;
            }
            else if (input == "y" | input == "Y")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("Your report will now open in your default PDF viewer. \n \n");
                Console.WriteLine("This Program will now close. \n");
                Thread.Sleep(3000);

                k = 1;
                return k;
            }

            return k;



        }  // end of IOConsole





        public void Execute(ScriptContext context )     // PROGRAM START - sending a return to Execute will end the program
        {
            //regular variables

            int output = 0;

            //ESAPI variables 

            Patient patient = context.Patient;   // creates an object of the patient class called patient equal to the active patient open in Eclipse
            Course course = context.Course;
            PlanSetup plan = context.PlanSetup;
            Image image3D = context.Image;
            StructureSet structureSet = context.StructureSet;
            User user = context.CurrentUser;



            if (context.Patient == null)
            {
                MessageBox.Show("Please load a patient with a treatment plan before running this script!");
                return;
            }

            output = IOConsole(user, patient, plan, course, structureSet);  // calls the I/O interface and assigns the int it returns


            if (output == 0 )  
            {
                return;
            }
            else if (output == 1)
            {

               // MessageBox.Show("Trigger Fire main from Auto");
                PdfReport.PDFGenerator.Program.Main(patient, course, plan, image3D, structureSet, user);





            }

            // code to put images in pdf goes here





















            //script code here

            return;

        }  //ends Execute  END OF PROGRAM






            



          

























                

 
    }   //ends Script class
}  //ends namespace

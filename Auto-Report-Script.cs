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
    Lahey RadOnc Automatic Patient Plan Report Generator and ROI criteria checker
    Copyright (c) 2019 Beth Isreal Lahey Health

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's APIs to run properly.
    This program also requires .NET Framework 4.5.0 to run properly.

    This is the source code for a .NET Framework assembly file, however this functions as an executable file in Eclipse.
    In addition to Varian's APIs and .NET Framework, this program uses the following dependent libraries:
    MigraDoc
    PdfSharp
    Custom Pdf export libraries using MigraDoc and PdfSharp
    OXY Plot

    Release 1.0
    Automatically prints preformatted pictures from the patient's plan as a PDF document, including DRRs, 3 view +3d plan images, DVH curves

*/

namespace VMS.TPS
{
    public class Script  // creates a class called Script within the VMS.TPS Namesapce
    {

        public Script() { }  // instantiates a Script class

        // Assembly Commands

        [MethodImpl(MethodImplOptions.NoInlining)]   // prevents compiler optimization from messing with the program's methods. Protection for eventual Eclipse 15.5 upgrade

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]    // This stuff manually imports the Windows Kernel and then sets up a function that instantiates a cmd line
        static extern bool AllocConsole();

        // Declaration Space for Functions and Variables

        public string Ttype = null;
        public string Tsite = null;
        public static List<Auto_Report_Script.ROI> ROIE = new List<Auto_Report_Script.ROI>();     // Expected ROI list
        public static List<Auto_Report_Script.ROI> ROIA = new List<Auto_Report_Script.ROI>();     // Actual ROI list from Eclipse 


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
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypofx 60 Gy", Name = "ProstateHypofx60Gy", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypofx 70 Gy", Name = "ProstateHypofx70Gy", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax", Name = "Thorax", Id = 15 });

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


        static int IOConsole(string Ttype, string Tsite, User user, Patient patient, PlanSetup plan, Course course, StructureSet structureSet)   // This "IOConsole" function is the entire I/O interface for the program, and as such it is very long. It runs once and collects the requisite info from the user. 
                                                                                                                    // This is neccessary so we can use the Command Line in what is technically a .DLL assembly file.                        
        {
            int k = 0;
            string input = null;
            int t = 0;
            List<TreatSite> sitelist = null;



            AllocConsole();
            Console.Title = "Lahey RadOnc Automatic Patient Plan Report Generator and ROI Criteria Checker  V 1.0";

            Console.SetWindowSize(200, 70);                                 //these specific values are here for a reason, don't change them
            Console.SetBufferSize(200, 70);

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
            t = System.Convert.ToInt32(Console.ReadLine());

            foreach (TreatSite aTreatSite in sitelist)
            {
                
                if (aTreatSite.Id == t)
                {
                    Console.WriteLine("\nYou have chosen the {0} treatment site.", aTreatSite.DisplayName);
                    Tsite = aTreatSite.Name;
                     Thread.Sleep(2000);
                    break;
                }
                
            }

            Console.WriteLine("\n\nStarting ROI criteria check ... \n");
                                                                                   // ROI is its own custom class
            ROIE = Auto_Report_Script.LISTMAKER.Listmaker(Ttype, Tsite);          // separate class with LISTMAKER function which generates a list of ROIs for the given treeatment type and site

         //   if(ROIE.Count > 10)
          //  {
                Console.WriteLine("\n{0} ROI list created successfully.", Tsite);
                Console.WriteLine("\nThe {0} ROI list contains {1} ROIs.", Tsite, ROIE.Count);
                Thread.Sleep(2000);
            //  }




            // This part of code below gets DVH data from Eclipse. The way it works is different for different limit types, like MaxPtDose, V80, D1cc. etc.

            double[] dose = new double[4000000000];              // variables used with obtaining MaxPtDose form DVH curve
            uint dptcount = 0;
            double maxptdose = 0;

            double pdose = plan.TotalPrescribedDose.Dose;       // prescribed dose of the plan


            Console.WriteLine("\nPRESCRIBED DOSE UNIT: {0}", plan.TotalPrescribedDose.Unit.ToString());
            Console.WriteLine("\nPRESCRIBED DOSE VALUE: {0}", pdose);

            Thread.Sleep(5000);

            Console.WriteLine("\nObtaining DVH data of these ROIs from Eclipse ... ");

            for (int i = 0; i <= ROIE.Count; i++)
            {

                foreach (Structure S in structureSet.Structures)        // iterates thriugh all the structures in the structureset of the current plan
                {

                    if (S.Id == ROIE[i].Rstruct)
                    {

                        double structvol = S.Volume;
                        Console.WriteLine("\n\n{0} - STRUCTURE VOLUME: {0}", S.Id, S.Volume);
                        Thread.Sleep(5000);


                        if (ROIE[i].limit == "MaxPtDose")        // MaxPtDose
                        {
                            string kstatus = null;
 
                            Console.WriteLine("\nTRIGGER MAX PT Dose");
                            Console.WriteLine("\nMAX PT Dose Limit: {0}  {1}", ROIE[i].limval, ROIE[i].limunit);
                            Thread.Sleep(4000);

                            DVHData kDVH = plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue maxdose = kDVH.MaxDose;
                            
                            Console.WriteLine("\nDOSE UNIT: {0}", maxdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", maxdose.Dose);
                            Thread.Sleep(5000);


                            if(ROIE[i].strict == "[record]")
                            {

                                kstatus = "PASS";

                            }
                            else if(ROIE[i].strict == "<")
                            {

                               if(maxdose.Dose < ROIE[i].limval)
                               {
                                    kstatus = "PASS";
                               }
                               else
                               {
                                    kstatus = "FAIL";

                               }

                            }
                            else if(ROIE[i].strict == "<=")
                            {

                                if (maxdose.Dose <= ROIE[i].limval)
                                {
                                    kstatus = "PASS";
                                }
                                else
                                {
                                    kstatus = "FAIL";
 
                                }

                            }

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = ROIE[i].ROIName, limdose = ROIE[i].limval , actdose = maxdose.Dose, status = kstatus, structvol = structvol });


                        }
                        else if (ROIE[i].limit == "MeanDose")        // Mean dose
                        {
                            string jstatus = null;

                            Console.WriteLine("\nTRIGGER Mean");
                            Console.WriteLine("\nMean Dose Limit: {0}  {1}", ROIE[i].limval, ROIE[i].limunit);
                            Thread.Sleep(4000);

                            DVHData jDVH = plan.GetDVHCumulativeData(S, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, 0.1);

                            DoseValue meandose = jDVH.MaxDose;

                            Console.WriteLine("\nDOSE UNIT: {0}", meandose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", meandose.Dose);
                            Thread.Sleep(5000);


                            if (ROIE[i].strict == "[record]")
                            {

                                jstatus = "PASS";

                            }
                            else if (ROIE[i].strict == "<")
                            {

                                if (meandose.Dose < ROIE[i].limval)
                                {
                                    jstatus = "PASS";
                                }
                                else
                                {
                                    jstatus = "FAIL";
 
                                }

                            }
                            else if (ROIE[i].strict == "<=")
                            {

                                if (meandose.Dose <= ROIE[i].limval)
                                {
                                    jstatus = "PASS";
                                }
                                else
                                {
                                    jstatus = "FAIL";


                                }

                            }


                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = ROIE[i].ROIName, limdose = ROIE[i].limval, actdose = meandose.Dose, status = jstatus, structvol = structvol});


                        }
                        else if (ROIE[i].limit.StartsWith("V"))         // specifies a dose that a specific percentage of the volume of the structure must be less than
                        {
                            string fstatus = null;
                            DoseValue fdose = new DoseValue();

                            Console.WriteLine("\nTRIGGER V ");
                            Console.WriteLine("\nV Dose Limit: {0}  {1}", ROIE[i].limval, ROIE[i].limunit);
                            Thread.Sleep(4000);

                                                                                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                                                                                            // This allows the "V" in the limit string to be omitted so we just get the number    
                            double Vgy = Convert.ToDouble(ROIE[i].limit.Substring(1));      // "V gray" 


                            if(ROIE[i].limunit == "%")
                            {

                                double fvol = (structvol * ((Convert.ToDouble(ROIE[i].limval)) / 100.0));         // specific volume that the ROI is concerned with. Here, limval is the percent of the volume of the structure

                                fdose = plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DoseValue tfdose = plan.GetDoseAtVolume(S, (Convert.ToDouble(ROIE[i].limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                Thread.Sleep(4000);

                            }
                            else if(ROIE[i].limunit == "cc")
                            {

                                fdose = plan.GetDoseAtVolume(S, Convert.ToDouble(ROIE[i].limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                            }


                            if(ROIE[i].strict == "<=")
                            {

                                if(fdose.Dose <= Vgy)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "FAIL";
                                }

                            }
                            else if(ROIE[i].strict == "<")
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
                            else // thing to deal with Esophagus circumferential

                            Console.WriteLine("\nDOSE UNIT: {0}", fdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", fdose.Dose);
                            Thread.Sleep(5000);

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = ROIE[i].ROIName, limdose = Vgy  , actdose = fdose.Dose, status = fstatus, structvol = structvol});


                        }
                        else if (ROIE[i].limit.StartsWith("D"))
                        {

                            string qstatus = null;
                            DoseValue qdose = new DoseValue();

                            Console.WriteLine("\nTRIGGER D ");
                            Console.WriteLine("\nD Dose Limit: {0}  {1}", ROIE[i].limval, ROIE[i].limunit);
                            Thread.Sleep(4000);

                                                                                            // "Substring" is an extremely useful string method that creates a new string starting at a specific character position.
                                                                                             // This allows the "V" in the limit string to be omitted so we just get the number    
                            string qstring = ROIE[i].limit.Substring(1);      // "V gray" 

                            if(ROIE[i].limit.EndsWith("cc"))
                            {
                               
                              //  ROIE[i].limit.Remove()

                                    // STOPPED HERE



                            }
                            else if(ROIE[i].limit.EndsWith("%"))
                            {


                                ROIE[i].limit.TrimStart()








                            }















                            if (ROIE[i].limunit == "%")
                            {

                                double fvol = (structvol * ((Convert.ToDouble(ROIE[i].limval)) / 100.0));         // specific volume that the ROI is concerned with. Here, limval is the percent of the volume of the structure

                                fdose = plan.GetDoseAtVolume(S, fvol, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                                DoseValue tfdose = plan.GetDoseAtVolume(S, (Convert.ToDouble(ROIE[i].limval) / 100.0), VolumePresentation.Relative, DoseValuePresentation.Absolute);

                                Console.WriteLine("\n\n PERCENT DOSE TEST: {0}  {1}", fdose.Dose, tfdose.Dose);
                                Thread.Sleep(4000);

                            }
                            else if (ROIE[i].limunit == "cc")
                            {

                                fdose = plan.GetDoseAtVolume(S, Convert.ToDouble(ROIE[i].limval), VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                            }


                            if (ROIE[i].strict == "<=")
                            {

                                if (fdose.Dose <= Vgy)
                                {
                                    fstatus = "PASS";
                                }
                                else
                                {
                                    fstatus = "FAIL";
                                }

                            }
                            else if (ROIE[i].strict == "<")
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
                            else // thing to deal with Esophagus circumferential

                                Console.WriteLine("\nDOSE UNIT: {0}", fdose.Unit.ToString());
                            Console.WriteLine("\nDOSE Vale: {0}", fdose.Dose);
                            Thread.Sleep(5000);

                            ROIA.Add(new Auto_Report_Script.ROI { ROIName = ROIE[i].ROIName, limdose = Vgy, actdose = fdose.Dose, status = fstatus, structvol = structvol });























                        }
























                    }



                }









                foreach (Structure S in structureSet.Structures)
                {

                    if (S.Id == ROIE[i].Rstruct)
                    {



                        plan.GetDoseAtVolume(S,)


                        ROIA.Add(new Auto_Report_Script.ROI { ROIName = ROIE[i].ROIName, limdose = , actdose =  , status = });




                    }



                }

                /*

                                var DVH = plan.DVHEstimates;         // plan.DVHEstimates returns a list of all the "EstimatedDVH" objects in the plan. EstimatedDVH is a custom class of the VMS API

                    foreach (EstimatedDVH dvh in DVH)
                    {

                        if (dvh.StructureId == ROIE[i].Rstruct)
                        {

                            foreach (DVHPoint dpoint in dvh.CurveData)            // "CurveData" returns an array of "DVHPoints", another custom class
                            {
                                dose[dptcount] = dpoint.DoseValue.Dose;

                                dptcount++;
                            }

                            maxptdose = dose.Max();

                        }

                    }

                    if (maxptdose <= ROIE[i].limval)
                    {
                        ROIE[i].status = "PASS";
                    }
                    else
                    {
                        ROIE[i].status = "FAIL";

                    }

                    */






            }



            // Code which gets data from Eclipse ends here. Below this is the ouput for the ROI comparison.

            Console.WriteLine("\nROI Check complete.");

            Thread.Sleep(1000);

            Console.WriteLine("\n\n\t\tROI COMPARISON REPORT FOR {0}'S PLAN {1}", patient.Name, plan.Name);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Name                                                   |  Calculated Dose Limit (cGy)  |  Eclipse Estimated Dose (cGy)  |  Status    |  Structure Volume  ");
                         
            foreach(Auto_Report_Script.ROI aroi in ROIA)
            {
                Console.WriteLine("\n\n"); 
                Console.WriteLine("{0}   {1}      {2}      {3}   {4} ", aroi.ROIName, aroi.limdose, aroi.actdose, aroi.status, aroi.structvol);
            }

            Console.WriteLine("\n\n");

            Console.WriteLine("Would you like to generate a PDF of this ROI comparison report (Y/N)?  ");
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


            /*
            int template = 0;

            Console.WriteLine("This is the current list of report templates... \n \n");

            //list  
            //figure out how to make indexed arrays in C#, make one here

            Console.WriteLine(" \n \n");
            Console.WriteLine("Please enter the number next to the report template that matches this plan: ");
            template = System.Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(" \n");

            Console.WriteLine("You chose {0} \n", template);


            // System.Diagnostics.Process.Start("templateprint.vbs")   // calls a yet to be created .vbs file that literally inputs the keystrokes to print a report from Eclipse

            */

                //  Console.WriteLine("Your report will now print! \n");
                //  Thread.Sleep(4000);
                //  Console.WriteLine("This Command Line Window will now close. \n");
                //  Thread.Sleep(5000);



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

            output = IOConsole(Ttype, Tsite, user, patient, plan, course, structureSet);  // calls the I/O interface and assigns the int it returns





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

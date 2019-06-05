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

        public class TreatSite : IEquatable<TreatSite>        //makes a treatment site class used to make a list of treatment sites 
        {                                                     //the treatment site list is used when the user is prompted to choose the treatment site of the selected plan
            public string TreatSiteName { get; set; }

            public int TreatSiteId { get; set; }

            public override string ToString()
            {
                return "ID: " + TreatSiteId + "   Name: " + TreatSiteName;
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
                return TreatSiteId;
            }
            public bool Equals(TreatSite other)
            {
                if (other == null) return false;
                return (this.TreatSiteId.Equals(other.TreatSiteId));
            }
            // Should also override == and != operators.

        }

        static List<TreatSite> MakelistConv()           //treatment site list for Conventional plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { TreatSiteName = "Abdomen", TreatSiteId = 1 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Brain ", TreatSiteId = 2 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Brain Hypofx", TreatSiteId = 3 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Breast", TreatSiteId = 4 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Esophagus", TreatSiteId = 5 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Gynecological", TreatSiteId = 6 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Head & Neck", TreatSiteId = 7 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Lung", TreatSiteId = 8 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Pelvis (Other)", TreatSiteId = 9 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Pelvis EBRT + HDR", TreatSiteId = 10 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Prostate", TreatSiteId = 11 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Prostate Bed", TreatSiteId = 12 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Prostate Hypofx 60 Gy", TreatSiteId = 13 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Prostate Hypofx 70 Gy", TreatSiteId = 14 });
            treatsite.Add(new TreatSite() { TreatSiteName = "Thorax", TreatSiteId = 15 });

            return treatsite;
        }

        static List<TreatSite> MakelistSRS()           //treatment site list for SRS plans
        {
            List<TreatSite> treatsite = new List<TreatSite>();

                treatsite.Add(new TreatSite() { TreatSiteName = " Single fraction", TreatSiteId = 1 });
                treatsite.Add(new TreatSite() { TreatSiteName = "3 fraction", TreatSiteId = 2 });
                treatsite.Add(new TreatSite() { TreatSiteName = "4 fraction", TreatSiteId = 3 });
                treatsite.Add(new TreatSite() { TreatSiteName = "5 fraction", TreatSiteId = 4 });
                treatsite.Add(new TreatSite() { TreatSiteName = "6 fraction", TreatSiteId = 5 });
                treatsite.Add(new TreatSite() { TreatSiteName = "8 fraction", TreatSiteId = 6 });
                treatsite.Add(new TreatSite() { TreatSiteName = "10 fraction", TreatSiteId = 7 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Liver", TreatSiteId = 8 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Lung 4 fraction", TreatSiteId = 9 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Lung 5 fraction", TreatSiteId = 10 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Lung 8 fraction", TreatSiteId = 11 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Oligomets 1 fraction", TreatSiteId = 12 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Oligomets 3 fractions", TreatSiteId = 13 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Oligomets 5 fractions", TreatSiteId = 14 });
                treatsite.Add(new TreatSite() { TreatSiteName = "Pancreas", TreatSiteId = 15 });
                treatsite.Add(new TreatSite() { TreatSiteName = "SRS Cranial 1 fraction", TreatSiteId = 16 });
                treatsite.Add(new TreatSite() { TreatSiteName = "SRS Cranial 3 fraction", TreatSiteId = 17 });
                treatsite.Add(new TreatSite() { TreatSiteName = "SRS Cranial 5 fraction", TreatSiteId = 18 });
                treatsite.Add(new TreatSite() { TreatSiteName = "SRS Cranial AVM", TreatSiteId = 19 });
                treatsite.Add(new TreatSite() { TreatSiteName = "SRS Cranial Trigeminal Neuralgia", TreatSiteId = 20 });

            return treatsite;
        }


        static int IOConsole(string Ttype, string Tsite, String user, String patient, String plan, String course)   // This "IOConsole" function is the entire I/O interface for the program, and as such it is very long. It runs once and collects the requisite info from the user. 
                                  // This is neccessary so we can use the Command Line in what is technically a .DLL assembly file.                        
        {
            int k = 0;
            string i = null;
            int t = 0;
            List<TreatSite> sitelist = null;

            AllocConsole();
            Console.Title = "Lahey RadOnc Automatic Patient Plan Report Generator and ROI Criteria Checker  V 1.0";

            Console.SetWindowSize(200, 70);                                 //these specific values are here for a reason, don't change them
            Console.SetBufferSize(200, 70);

          //  if (course == null)
          //  {
          //      course = plan;
          //  }

            Console.WriteLine(" Hi {0}, Welcome to the Lahey RadOnc Automatic Patient Plan Report Generator and ROI Criteria Checker  V 1.0 \n \n", user);
                                                                                                                                                                                                    //this is the size limit for characters on one line of cmd prompt
            Thread.Sleep(2000);

            Console.WriteLine("You have loaded {0}'s course {1}. The currently selected plan of course {1} is {2}.  \n", patient, course, plan);
            Console.WriteLine("This program will now check the selected plan {0} to see if the DVH values estimated by the Eclipse DVH Estimation Algorithim are within the ROI criteria established \n by the Lahey Radiation Oncology Department for the specific anatomic treatment site that it is designed for. \n", plan);
            Console.WriteLine("In order to do this, you must specify the treatment type and treatment site. Is this a conventionally fractionated plan or an SRS/SBRT plan? \n");
            Console.WriteLine("(Enter C for conventional or S for SRS/SBRT): ");
            i = Console.ReadLine();
            Console.WriteLine("\n\n ");
          
            while (i != "c" & i != "C" & i != "S" & i != "s")
            {
                Console.WriteLine("Let's try that again. You must enter either C for Conventional or S for SRS/SBRT.");
                i = Console.ReadLine();
            }

            if (i == "c" | i =="C")
            {
                Console.WriteLine("You have chosen a Conventional plan.");
                Ttype = "Conventional";
                sitelist = MakelistConv();
            }

            else if (i == "s" | i == "S")
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
                
                if (aTreatSite.TreatSiteId == t)
                {
                    Console.WriteLine("\nYou have chosen the {0} treatment site.", aTreatSite.TreatSiteName);
                    Tsite = aTreatSite.TreatSiteName;
                    break;
                }
                break;

            }

            Console.WriteLine("\n\nEXECUTING ROI CRITERIA CHECK.... ");

            List<Auto_Report_Script.ROI> roi =  Auto_Report_Script.ROIcriteriacheck.Main(Ttype, Tsite);  //backend ROI check


            /* the ROIcriteriacheck function will return a custom made list class of the ROI criteria for the given treatment site. This list class will contain a pass/fail parameter that will be assigned
             * a value when the function checks it. This list will then be passed to Auto-Report-Script, which will loop through the list to display the ouput, including pass/fail parameter.
             * this output function will do something special when it comes across a "fail"
             */

            //code for ROI ouput here

            

 

            Console.WriteLine("Would you like to generate a report of this ROI comparison (Y/N)?  ");
            i = Console.ReadLine();
            if (i == "n" | i == "N")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("This program will now close.");
                Thread.Sleep(3000);

                return k;
            }
            else if (i == "y" | i == "Y")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("Your report will now open in your default PDF viewer. \n \n", plan);
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

            output = IOConsole(Ttype, Tsite, user.Name, patient.Name, plan.Id, course.Id);  // calls the I/O interface and assigns the int it returns

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

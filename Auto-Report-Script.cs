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
    Lahey RadOnc Automatic Patient Plan Report Generator 
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

        static int IOConsole(String user, String patient, String plan, String course)   // This "IOConsole" function is the entire I/O interface for the program, and as such it is very long. It runs once and collects the requisite info from the user. 
                                  // This is neccessary so we can use the Command Line in what is technically a .DLL assembly file.                        
        {
            int k = 0;
            String input = null;

            AllocConsole();

            Console.SetWindowSize(80, 20);
            Console.SetBufferSize(80, 20);

          //  if (course == null)
          //  {
          //      course = plan;
          //  }

            Console.WriteLine(" Hi {0}, Welcome to the Lahey RadOnc Automatic Patient Plan Report Generator   V 1.0 \n \n", user);

            Thread.Sleep(3000);

            Console.WriteLine("You have loaded {0}'s course {1}. The currently selected plan of course {1} is {2}.  \n", patient, course, plan);
            Console.WriteLine("This program will print all required images for the current plan {0}. If you want to do this, press Y. \n", plan);
            Console.WriteLine("If you want to print the images for a different course, plan, or patient, or exit the program for any reason, press N. \n");
            Console.WriteLine("(Y/N)? ");
            input = Console.ReadLine();

            if (input == "n" | input == "N")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("The program will now close.");
                Thread.Sleep(3000);

                return k;
            }
            else if (input == "y" | input == "Y")
            {
                Console.WriteLine("\n \n");
                Console.WriteLine("All images for plan {0} will now print. \n \n", plan);
                Console.WriteLine("This Command Line Window will now close. \n");
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

            output = IOConsole(user.Name, patient.Name, plan.Id, course.Id);  // calls the I/O interface and assigns the int it returns

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

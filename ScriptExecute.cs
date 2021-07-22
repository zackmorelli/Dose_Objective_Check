using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media.Media3D;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


/*
    Dose Objective Check - (Script Execute)
    
    Description:
    This is the Script Execute start-up file for the Dose Objective Check program.
    The code in this file uses ESAPI to create a "Script" class and then uses the ESAPI Execute method to start an Eclipse script and pull the scriptcontext object to get info about the current open patient in Eclipse.

    Dose Objective Check uses space-delimited text files to run a dose objective check (using the department's unique criteria) on an external beam Plan (or Plansum) in Eclipse using some user-provided input. 
    This program also generates a report of the dose objective check as a PDF, which includes a standard header used to identify the patient/Plan that the report was made for.
    The program is also capable of analyzing and displaying target dose coverage. Clinical goals can also be used by the program.
    The code used to generate the PDF is based off a program developed by Carlos J Anderson and obtained from him via his website.

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


namespace VMS.TPS
{
    public class Script  // creates a class called Script within the VMS.TPS Namespace
    {
        [MethodImpl(MethodImplOptions.NoInlining)]       // prevents compiler optimization from messing with the program's methods.
        public Script() { }  // instantiates a Script object

        //The script Execute method is at the end of this file.
        //The Execute method gets the plan info from the ScriptContext, and then passes everything to a WinForms GUI, stored in a separate .cs file
        //The GUI allows the user to select which plan, from among the plans they have open in Eclipse, that they want to run the program on,
        //the treatment site, and if they want PTV dose coverage statistics. It will prompt them for a plan laterality for certain treament site with
        //laterality specific dose objectives.
        //The GUI will eventually call the dose objective analysis methods that reside here, the meat of the program.
        //Their are two dose objective analysis methods for performing the dose objective analysis for Plans and Plansums.
        //These two methods are basically identical; they should do the same thing.
        //This is an anachronism from when the program was first written. Back then ESAPI could not call DVH info for Plansums.
        //The dose objective analysis methods return a list of ROI class objects to the GUI.
        //The GUI then calls the PDF generation methods (again, there are two, one for plans and one plansums) in the PdfReport.PDFGenerator.cs file.


        //Immediately below is the TreatSite class and the methods used to call the treatsite lists for Conventional and SRS/SBRT plans.
        //After that are the dose objective analysis methods.
        //The dose objective analysis methods call LISTMAKER, which is a class stored in a separate file.
        //LISTMAKER dynamically creates a list of ROI class objects (ROI is also stored in it's own file) that represent the dose objectives that we want to evaluate the plan against.
        //It does this by pulling in all the dose objectives from the master lists (text documents) that have the treatment site tag selected by the user in the GUI
        //LISTMAKER also checks to see if their are any custom dose objective lists created by the macro in the Doctor's CTPN ARIA document
        //If this happens, a popup will appear.
        //After LISTMAKER is called and it returns the List of Dose Objectives, there is code to deal with if the user wants to have the program analyze dose objectives
        //they added using the clinical goals feature in Eclipse.


        public void Execute(ScriptContext context)     // PROGRAM START - sending a return to Execute will end the program
        {
            //regular variables

            List<DoseObjectiveCheck.DoseObjective> output = new List<DoseObjectiveCheck.DoseObjective>();

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

            //IEnumerable<Diagnosis> diagenum = course.Diagnoses;
            //string diag = "Course Diagnoses";

            //foreach(Diagnosis D in diagenum)
            //{
            //    diag = diag + "\n\n" + D.ClinicalDescription + "\n";
            //}

            // MessageBox.Show("Trig1");

            // GUI STARTS HERE
            // settings for windows forms GUI
            System.Windows.Forms.Application.EnableVisualStyles();

            //Starts GUI for Dose objective check.
            //It's important to understand what this does. Application is a class of the Windows Forms class that provides ways to manage Windows Forms.
            //The Run method starts a message loop, which is a loop of all the input from the user that gets routed through the Windows Operating System
            //and then sent to the GUI. The GUI then decides what to do with these messages. This is what Event Handlers are.
            //Run starts the message loop of the new Windows Form that is being instantiated inside the parentheses on the CURRENT thread.
            //This is NOT multithreaded. So, this GUI will be occupying the thread once it starts. This program doesn't have a need for multithreading
            //To learn more about Windows Forms, look it up online.
            System.Windows.Forms.Application.Run(new DoseObjectiveCheck.GUI(patient, course, image3D, user, Plansums, Plans));

            return;

        }  //ends Execute  END OF PROGRAM

    }   //ends Script class
}  //ends namespace

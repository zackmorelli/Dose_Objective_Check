using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using g3;


/*
    Dose Objective Check - GUI

    Description:
    This is the main GUI of the Dose Objective Check program. The GUI is called by the the Execute method of the Script class, in the main .cs file called "ScriptExecute".
    The GUI is a Windows Form that gathers information needed from the user to run the program. The user indicates the plan they want to run the program on (from among the plans they
    have open in Eclipse at the moment they run the script), the type of plan (Conventional or SRS, or both for a plansum), the specific treatment site of the plan, and the laterality for certain treatment sites
    that have laterality-dependent objectives. Conventional plans and SBRT plans have separate lists of standard treatment sites which are used in planning. These are hard-coded into the TreatSite class
    and used to populate a list box in the GUI that the user selects the treatment site of their plan from. Each treatment site has a standard list of dose objectives that accompanies it. 
    The dose objective lists for each treatment site are not hard-coded. They are dynamically generated each time the program runs by reading in a text file. This makes editing the standard dose objective lists easier.
    The user can also use the GUI to indicate that they want the program to add Clinical Goals they have made in Eclipse to the dose objective list created by the program, and if they want the report
    to include target dose coverage information. The GUI simply collects the users's input through event handlers and passes of all of this information to the dose objective analysis methods which are called
    when the usr clicks the "Execute" button. The dose objective analysis methods return a list of the dose objectives (containing all of the information determined in the analysis) to GUI.
    It then calls the PDF preparation methods, which it passes the analyzed dose objective list too. The program continues on from there without being called back to the GUI.

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
    //This declares a Windows Form class called GUI
    public partial class GUI : Form
    {
        //variables used by the GUI
        public int dt = 0;
        public double dd = 0.0;
        public string ptype = null;
        public string TS = null;
        public string laterality = null;
        public ListBox.SelectedObjectCollection TSA;
        public string pl = null;
        public int c = 0;
        public int k = 0;
        public int c1 = 0;
        public int k1 = 0;
        public string gyntype = null;
        public string[] SRScoverageRequirements = new string[2];
        public string[] ConvCoverageRequirements = new string[8];
        public string[] BothCoverageRequirements = new string[10];
        public bool DoseStat = false;
        List<string> plannames = new List<string>();
        List<string> sumnames = new List<string>();
        List<DoseObjective> output = new List<DoseObjective>();
        
        //This method, which has the same name as the Windows Form class, is the method that is actually called when this GUI class is instantiated.
        //So, in the Execute method in DoseObjectiveCheck, this method is called when the GUI object is created inside the Application.Run method
        //The Run method interacts with the Windows operating system for us and makes the GUI appear onscreen and makes a message loop so it can actually work.
        // So, this is where you put code that needs to be executed when the GUI starts
        public GUI(VMS.TPS.Common.Model.API.Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {
            //Initialize Component is very important. This calls all of the code in the GUI.Designer class, which Visual Studio makes for you
            //when you create a new Windows Form class. Of course, you can make your own Form from scratch, which I think can be useful.
            //But here I used Visual Studio to make a Form.
            //If you are not familiar with Windows Forms, I reccomend opening a clean solution in Visual Studio and playing around with making your own Form.
            InitializeComponent();

            OuputBox.Text = "GUI Initialized";

            // MessageBox.Show("Trig 6");
            //These loops populate the Plan_List listbox with all the plans and plansums the user had open.
            foreach (PlanSum aplansum in Plansums)
            {
                Plan_List.Items.Add(aplansum.Id);
                sumnames.Add(aplansum.Id);
                // MessageBox.Show("Trig 7");
            }

            foreach (PlanSetup aplan in Plans)
            {
                Plan_List.Items.Add(aplan.Id);
                plannames.Add(aplan.Id);
                // MessageBox.Show("Trig 8");
            }
            OuputBox.Text = "Plans loaded";


            //This line below requires some explanation. The EXECUTE method, which does all the work that this GUI needs to do once the user has filled everything
            //out and clicked the button, requires a number of variables, bascially all of the plan information required by the rest of the program.

            //So, we have a bit of a problem here. When we make the GUI, everything we pass to it gets passed to this special executable GUI method that we are
            //sitting in right now. But, all of the other methods in this GUI class, which are all used by the GUI's event handlers, or called by one of the other methods,
            //don't know anything about these variables. Because of this special executable method which recieves the variables we passed to the GUI,
            //they are not global variables available to the entire GUI class, which you might expect when you make the GUI object and pass stuff to it.

            //Now, normally this would not be a problem, we would just pass the variables that the other methods need when we call them.
            //But, in a Windows Form class like this, all the other methods are event driven. They are reacting to user input, that is what a GUI does.
            //So, the other methods are not being called from the executable GUI method. They are called through the Event Handlers and the message pump.
            //So, how are we supposed to pass these variables to the other methods that need them?
            //The workaround is putting a custom event handler in the GUI method, which is the line below. This creates an Event handler which continues running after
            //this method is done executing. The line below creates a Click event handler for the GUI's button, called button1. Now, there is a separate event handler
            //method below which is called by the click event handler in the designer code, but as you can see it doesn't do anything. I'm not sure if it is neccesary
            //to have this empty method, but it is there. 

            //The Click Event handler below uses a lambda expression (the =>). If you don't know, this is a more recent syntax of C#, and also Java, that is not in C++.
            //It provides an easy way of using what are called delegates in C#. Delegates don't exist in C++ either. They are like pointers in C++, except they
            //are actually fully object-oriented and can pass parameters to the method they point to, which is what we are doing here.
            //So, the lambda expression points to a method called buttonNext_Click and passes all the variables we need to it. buttonNext_Click then calls EXECUTE and
            //it all the variables it needs.
             button1.Click += (sender, EventArgs) => { buttonNext_Click(sender, EventArgs, patient, course, image3D, user, Plansums, Plans); };
        }


        private async void EXECUTE(string laterality, string TS, ListBox.SelectedObjectCollection TSA, VMS.TPS.Common.Model.API.Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {
            //This is where everything happens after the execute button is clicked. Most of the code is a series of if - else statements that goes through the list of plans,
            //or plansums, to find the plan that mwatches with the string pl. This is very clunky since I wrote it a long time ago, but it works fine, so I
            //would just leave it alone.

            // MessageBox.Show("Trig EXE - 1");

            bool UseGoals = false;
            if(UseGoalsCheck.Checked == true)
            {
                UseGoals = true;
            }

            //This launches a WinForm that will prompt the user for coverage requirment values. This isn't a custom WinForm like the others, it's a normal one made using Visual Studio
            //It works beacuse it is actually multi-threaded. The SRSGUICaller method is called, and that method then starts an awaitable Task,
            //which is a way of having Windows figure out the multithreading for you. This Task, running on a separate thread, then starts another WinForm.
            //While that is running, the GUI here simply sits in a while loop, until the user closes it.
            if (DoseStatisticsBox.Checked == true)
            {
                //MessageBox.Show("Dosestat true");
                DoseStat = true;

                if (ptype == "SRS/SBRT")
                {
                    await GUICalls.SRSGUICaller(SRScoverageRequirements);
                    //MessageBox.Show("SRS Coverage Requirements: " + SRScoverageRequirements[0] + "    " + SRScoverageRequirements[1]);
                }
                else if (ptype == "Conventional")
                {
                    await GUICalls.ConvGUICaller(ConvCoverageRequirements);
                    //MessageBox.Show("Conv. Coverage Requirements: " + ConvCoverageRequirements[0] + "    " + ConvCoverageRequirements[1]);
                }
                else if (ptype == "Both")
                {
                    DoseStat = false;
                    MessageBox.Show("The Dose Objective Script is not capable of generating dose coverage statistics for plansums made up of Conventional and SRS/SBRT plans.\nPlease evaluate the plans individually.");
                }
            }

            //these are used for control flow, depending on whether the program is evaluating a plan or plansum
            bool LOCKSUM = false;
            bool LOCKPLAN = false;

            OuputBox.AppendText(Environment.NewLine);
            OuputBox.AppendText("Execution Initiated");

          //  MessageBox.Show("pl is: " + pl);
            // plans
            IEnumerator ER = Plans.GetEnumerator();
            ER.MoveNext();
            PlanSetup Plan = (PlanSetup)ER.Current;
           // MessageBox.Show("K is: " + k1.ToString());

            int CNT = Plans.Count();

            //  MessageBox.Show("number of plans: " + CNT);

            if (Plan.Id == pl)
            {
               //   MessageBox.Show("Trig EXE - 2");
                Plan = (PlanSetup)ER.Current;
                // MessageBox.Show("Plan Id is: " + Plan.Id);
                LOCKSUM = true;
            }
            else if (CNT > 1)
            {
                ER.MoveNext();
                Plan = (PlanSetup)ER.Current;
                if (Plan.Id == pl)
                {
                    // MessageBox.Show("Plan Id is: " + Plan.Id);
                    Plan = (PlanSetup)ER.Current;
                    LOCKSUM = true;
                }
                else if (CNT > 2)
                {
                    ER.MoveNext();
                    Plan = (PlanSetup)ER.Current;
                    if (Plan.Id == pl)
                    {
                        Plan = (PlanSetup)ER.Current;
                        LOCKSUM = true;
                    }
                    else if (CNT > 3)
                    {
                        ER.MoveNext();
                        Plan = (PlanSetup)ER.Current;
                        if (Plan.Id == pl)
                        {
                            Plan = (PlanSetup)ER.Current;
                            LOCKSUM = true;
                        }
                        else if (CNT > 4)
                        {
                            ER.MoveNext();
                            Plan = (PlanSetup)ER.Current;
                            if (Plan.Id == pl)
                            {
                                Plan = (PlanSetup)ER.Current;
                                LOCKSUM = true;
                            }
                            else if (CNT > 5)
                            {
                                ER.MoveNext();
                                Plan = (PlanSetup)ER.Current;
                                if (Plan.Id == pl)
                                {
                                    Plan = (PlanSetup)ER.Current;
                                    LOCKSUM = true;
                                }
                                else if (CNT > 6)
                                {
                                    ER.MoveNext();
                                    Plan = (PlanSetup)ER.Current;
                                    if (Plan.Id == pl)
                                    {
                                        Plan = (PlanSetup)ER.Current;
                                        LOCKSUM = true;
                                    }
                                    else if (CNT > 7)
                                    {
                                        ER.MoveNext();
                                        Plan = (PlanSetup)ER.Current;
                                        if (Plan.Id == pl)
                                        {
                                            Plan = (PlanSetup)ER.Current;
                                            LOCKSUM = true;
                                        }
                                        else if (CNT > 8)
                                        {
                                            ER.MoveNext();
                                            Plan = (PlanSetup)ER.Current;
                                            if (Plan.Id == pl)
                                            {
                                                Plan = (PlanSetup)ER.Current;
                                                LOCKSUM = true;
                                            }
                                            else if (CNT > 9)
                                            {
                                                ER.MoveNext();
                                                Plan = (PlanSetup)ER.Current;
                                                if (Plan.Id == pl)
                                                {
                                                    Plan = (PlanSetup)ER.Current;
                                                    LOCKSUM = true;
                                                }
                                                else if (CNT > 10)
                                                {
                                                    ER.MoveNext();
                                                    Plan = (PlanSetup)ER.Current;
                                                    if (Plan.Id == pl)
                                                    {
                                                        Plan = (PlanSetup)ER.Current;
                                                        LOCKSUM = true;
                                                    }
                                                    else if (CNT > 11)
                                                    {
                                                        ER.MoveNext();
                                                        Plan = (PlanSetup)ER.Current;
                                                        if (Plan.Id == pl)
                                                        {
                                                            Plan = (PlanSetup)ER.Current;
                                                            LOCKSUM = true;
                                                        }
                                                        else if (CNT > 12)
                                                        {
                                                            ER.MoveNext();
                                                            Plan = (PlanSetup)ER.Current;
                                                            if (Plan.Id == pl)
                                                            {
                                                                Plan = (PlanSetup)ER.Current;
                                                                LOCKSUM = true;
                                                            }
                                                            else if (CNT > 13)
                                                            {
                                                                ER.MoveNext();
                                                                Plan = (PlanSetup)ER.Current;
                                                                if (Plan.Id == pl)
                                                                {
                                                                    Plan = (PlanSetup)ER.Current;
                                                                    LOCKSUM = true;
                                                                }
                                                                else if (CNT > 14)
                                                                {
                                                                    ER.MoveNext();
                                                                    Plan = (PlanSetup)ER.Current;
                                                                    if (Plan.Id == pl)
                                                                    {
                                                                        Plan = (PlanSetup)ER.Current;
                                                                        LOCKSUM = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        MessageBox.Show("Could not find the selected plan!");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

          //  MessageBox.Show("Trig EXE - 3");

            if (c1 > 0 && LOCKSUM == false)    // plansum
            {
              //  MessageBox.Show("Trig EXE - 4");
                IEnumerator TR = Plansums.GetEnumerator();
                TR.MoveNext();
                PlanSum Plansum = (PlanSum)TR.Current;

                int CNTS = Plansums.Count();

              //  MessageBox.Show("Trig EXE - 5");

                if (Plansum.Id == pl)
                {
                  //  MessageBox.Show("Trig EXE - 6");
                    Plansum = (PlanSum)TR.Current;
                    LOCKPLAN = true;
                }
                else if( CNTS > 1 )
                {
                    TR.MoveNext();
                    Plansum = (PlanSum)TR.Current;
                    if (Plansum.Id == pl)
                    {
                        Plansum = (PlanSum)TR.Current;
                        LOCKPLAN = true;
                    }
                    else if (CNTS > 2)
                    {
                        TR.MoveNext();
                        Plansum = (PlanSum)TR.Current;
                        if (Plansum.Id == pl)
                        {
                            Plansum = (PlanSum)TR.Current;
                            LOCKPLAN = true;
                        }
                        else if (CNTS > 3)
                        {
                            TR.MoveNext();
                            Plansum = (PlanSum)TR.Current;
                            if (Plansum.Id == pl)
                            {
                                Plansum = (PlanSum)TR.Current;
                                LOCKPLAN = true;
                            }
                            else if (CNTS > 4)
                            {
                                TR.MoveNext();
                                Plansum = (PlanSum)TR.Current;
                                if (Plansum.Id == pl)
                                {
                                    Plansum = (PlanSum)TR.Current;
                                    LOCKPLAN = true;
                                }
                                else if (CNTS > 5)
                                {
                                    TR.MoveNext();
                                    Plansum = (PlanSum)TR.Current;
                                    if (Plansum.Id == pl)
                                    {
                                        Plansum = (PlanSum)TR.Current;
                                        LOCKPLAN = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Could not find the selected plansum!");
                                    }
                                }
                            }
                        }
                    }
                }

                if (LOCKSUM == false)   // plansum
                {
                   //MessageBox.Show("Trig EXE - 7");
                    //EXECUTE PLANSUM DOSE OBJECTIVE ANALYSIS

                    //Si is an array used to store potentially multiple treatment sites for plansums
                    string[] Si = new string[60];

                    // this is here to make sure the list of selected treatment sites exists, because if the user selects a plansum after running a plan without clicking on the treatment site again, this can cause a problem
                    if( TSA == null)
                    {
                        TSA = TSiteList.SelectedItems;
                    }

                    IEnumerator GG = TSA.GetEnumerator();
                    GG.MoveNext();

                   // MessageBox.Show("TSA Count is : " + TSA.Count);
                    for (int i = 0; i < TSA.Count; i++)
                    {
                       //  MessageBox.Show("Trig EXE - 7.5");
                        Si[i] = Convert.ToString(GG.Current);
                        GG.MoveNext();
                    }

                    //This is a safety thing to make sure there is a structure set. Otherwise, it'll throw an error later on.
                    try
                    {
                        string strctid = Plansum.StructureSet.Id;
                    }
                    catch (NullReferenceException e)
                    {
                        MessageBox.Show("This plansum does not have a structure set. The script can only perform an analysis on plansums with structure sets. The script will now end.");
                        return;
                    }

                    OuputBox.AppendText(Environment.NewLine);
                    OuputBox.AppendText("Plansum Analysis Initiated, please be patient");

                    //Calls plansum dose objective analysis, returns a list of ROI objects. Unlike the list initially made by LISTMAKER, this will have the actual doses from the DVH
                    //estimation algorithim, and pass/fail status
                    output = DoseObjectiveAnalysis.PlansumAnalysis(laterality, Si, ptype, patient, course, Plansum.StructureSet, Plansum, dt, dd, OuputBox, gyntype, pBar, UseGoals);
                  //  MessageBox.Show("Trig EXE - 8");

                    //This is some code to alert the user right away if there is an objective that isn't passing
                    bool T = false;
                    foreach (DoseObjective DO in output)
                    {
                        if (DO.status == "REVIEW")
                        {
                            T = true;
                        }
                    }

                    if (T == true)
                    {
                        MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
                    }

                    OuputBox.AppendText(Environment.NewLine);
                    OuputBox.AppendText("PDF Generation Initiated");
                    //Calls the PDF Generation
                    PDFPreparation.PlansumMain(patient, course, Si, ptype, Plansum, image3D, Plansum.StructureSet, user, DoseStat, SRScoverageRequirements, ConvCoverageRequirements, output, dt, dd);
                }
            }

            if (LOCKPLAN == false)  // plans
            {
                //This is a safety thing to make sure there is a structure set. Otherwise, it'll throw an error later on.
                try
                {
                    string strctid = Plan.StructureSet.Id;
                }
                catch(NullReferenceException e)
                {
                    MessageBox.Show("This plan does not have a structure set. The script can only perform an analysis on plans with structure sets. The script will now end.");
                    return;
                }

                OuputBox.AppendText(Environment.NewLine);
                OuputBox.AppendText("Plan Analysis Initiated, please be patient");

                //Calls plan dose objective analysis, returns a list of ROI objects. Unlike the list initially made by LISTMAKER, this will have the actual doses from the DVH
                //estimation algorithim, and pass/fail status
                output = DoseObjectiveAnalysis.PlanAnalysis(laterality, TS, ptype, user, patient, course, Plan.StructureSet, Plan, OuputBox, gyntype, UseGoals);
                //  MessageBox.Show("Trig EXE - 9");

                //This is some code to alert the user right away if there is an objective that isn't passing
                bool T = false;
                foreach (DoseObjective DO in output)
                {
                    if (DO.status == "REVIEW")
                    {
                        T = true;
                    }
                }

                if (T == true)
                {
                    MessageBox.Show("THIS PLAN HAS NOT MET ONE OR MORE DOSE OBJECTIVES AND REQUIRES REVIEW.");
                }

                OuputBox.AppendText(Environment.NewLine);
                //Calls the PDF Generation
                OuputBox.AppendText("PDF Generation Initiated");
                PDFPreparation.PlanMain(patient, course, TS, ptype, Plan, image3D, Plan.StructureSet, user, DoseStat, SRScoverageRequirements, ConvCoverageRequirements, output);

            }
        }

        //This method fires when the user selects a plan. A string representing the plan is assigned to a global variable called pl.
        //If the selected plan is a plansum, it will call a method containing a custom Windows Form which will prompt the user
        //to select how they want the program to handle the total dose of the plansum. The dt variable is then carried throughout
        //the rest of the program as a way of knowing which situation was selected. It also states the selection mode of the treatment
        //site listbox, since if it is a plansum the user can choose more than one treatment site.
        void PlanList_SelectedIndexChanged(object sender, EventArgs e)
        {
            pl = Plan_List.SelectedItem.ToString();
            bool LOCK = false;

            //  MessageBox.Show("Trig 10");
            foreach (string str in sumnames)
            {
                c++;
                if (pl == str)
                {
                    LOCK = true;

                    string sid = null;

                    sid = PlansumTotalDoseDialog();

                    if (sid == "Normal Plansum")
                    {
                        // add Rx doses of the two plans in the plansum
                        dt = 1;
                    }
                    else if (sid == "\"fake plansum\" - Both plans have the same Rx dose and you want to evaluate using that dose, NOT the sum.")
                    {
                        //use the Rx dose of one of the plans
                        dt = 2;
                    }
                    else if (sid == "For other situations, enter your own dose to use in evaluation. You'll be prompted.")
                    {
                        //Enter your own dose. There is an additional custom Form, called below where the user enters the dose they want to use
                        dt = 3;
                        dd = Convert.ToDouble(Dialog());
                    }
                  //  else if (sid == "The Rx dose of one plan (assuming they have different Rx)")
                 //   {
                 //       dd = PlansumTotalDoseChoice4Dialog(sumnames);
                 //       dt = 4;
                 //   }

                    OuputBox.AppendText(Environment.NewLine);
                    OuputBox.AppendText("Plansum Dose Type Selected: " + sid);

                    TSiteList.SelectionMode = SelectionMode.MultiSimple;
                }
            }
            foreach (string str in plannames)
            {
                k++;
                if (pl == str)
                {
                    TSiteList.SelectionMode = SelectionMode.One;
                }
            }
            // MessageBox.Show("Trig 11");
            c1 = c;
            k1 = k;
            c = 0;
            k = 0;
            OuputBox.Text = "Plan Selected: " + pl;
        }

        void lateralitybox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Object latobj = lateralitybox.SelectedItem;
            // MessageBox.Show("Site List object string is: " + obj.ToString());
            laterality = latobj.ToString();
        }

        //This fires when a treatment site is selected. There are two logic branches because of the case of multiple treatment sites
        //selected for a plansum, but they do the same thing. If the treatment site is Gynecological, a custom WinForm is called that
        //asks the user if it is a Dose painted plan or a sequential boosts. This is necessary becuase there is no way for Eclipse to know about this.
        //This information is needed for calculating the dose limits of three special dynamic limits in Gynecological plans, but the limits
        //depend on the planning technique, dose painted vs. sequential.
        //The other thing is laterality for breast plans. This is actually a listbox on the GUI which is kept hidden until triggered below.
        //This information is used to decide which breast structure (right or left) is ipsilateral or contralateral for the given plan,
        //since their are different dose objectives for ipsilateral and contralateral breast.

        void TSiteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("OrganList fire");
            if (TSiteList.SelectionMode == SelectionMode.One)
            {
                Object obj = TSiteList.SelectedItem;
                // MessageBox.Show("Site List object string is: " + obj.ToString());
                TS = obj.ToString();

                OuputBox.AppendText(Environment.NewLine);
                OuputBox.AppendText("Treatment Site Selected: " + TS);

                if (TS == "Pelvis - GYN")
                {
                    gyntype = GynecologicalEdits();
                }

                if (TS == "Breast 23+fx" || TS == "Breast Hypofx" || TS == "Breast + regional_LN 23 + fx" || TS == "Breast+regional_LN Hypofx")
                {
                    lateralitybox.Visible = true;
                    MessageBox.Show("Please select the laterality of the breast plan (above the execute button). ");
                }

                //  Type TRF = TSiteList.SelectedItem.GetType();
                //  MessageBox.Show("Site List object type is: " + TRF.ToString());
            }
            else if (TSiteList.SelectionMode == SelectionMode.MultiSimple)
            {
                TSA = TSiteList.SelectedItems;
                string ject = null;

                foreach (Object obj in TSA)
                {
                    ject = ject + obj.ToString();

                    if (obj.ToString() == "Pelvis - GYN")
                    {
                        gyntype = GynecologicalEdits();
                    }

                    if (obj.ToString() == "Breast 23+fx" || obj.ToString() == "Breast Hypofx" || obj.ToString() == "Breast+regional_LN 23+fx" || obj.ToString() == "Breast+regional_LN Hypofx")
                    {
                        lateralitybox.Visible = true;
                        MessageBox.Show("Please select the laterality of the breast plan (above the execute button). ");
                    }

                    OuputBox.AppendText(Environment.NewLine);
                    OuputBox.AppendText("Treatment Site(s) Selected: " + ject);
                }
            }
        }

        public string GynecologicalEdits()
        {
            Form gynDialog = new Form()
            {
                Width = 600,
                Height = 600,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Gynecological Plan Information",
                StartPosition = FormStartPosition.CenterScreen,
                Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
            };
            Label gtxtlab = new Label() { Left = 10, Top = 15, Width = 320, Height = 140, Text = "Is this Gynecological plan a Dose Painted plan or a plan with Sequential Courses?" };
            Button gconfirm = new Button() { Text = "Ok", Left = 120, Width = 100, Top = 500, DialogResult = DialogResult.OK };
            ListBox gopt = new ListBox() { Left = 30, Top = 90, Width = 450 };
            gopt.Items.AddRange(new object[] {
                     "Dose Painted",
                     "Sequential Courses"});
            gconfirm.Click += (sender, e) => { gynDialog.Close(); };
            gynDialog.Controls.Add(gopt);
            gynDialog.Controls.Add(gconfirm);
            gynDialog.Controls.Add(gtxtlab);
            gynDialog.AcceptButton = gconfirm;

            return gynDialog.ShowDialog() == DialogResult.OK ? (string)gopt.SelectedItem : "";
        }

        //This fires when the plan type is selected, SRS or Conventional
        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("checkedlistboxfire");

            SelectionMode sel = TSiteList.SelectionMode;   
            TSiteList.SelectionMode = SelectionMode.None;   // the selection mode is set to none before the datasource is set or it causes the first element to be automatically selected.

            if (listBox1.SelectedItem.ToString() == "Conventional")
            {
                //conventional
                ptype = "Conventional";
                TSiteList.DataSource = TreatSite.MakelistConv();               
            }
            else if (listBox1.SelectedItem.ToString() == "SRS/SBRT")
            {
                //SRS
                ptype = "SRS/SBRT";
                TSiteList.DataSource = TreatSite.MakelistSRS(); 
            }
            else if (listBox1.SelectedItem.ToString() == "Both (Plansums Only)")
            {
                //Both
                ptype = "Both";
                TSiteList.DataSource = TreatSite.MakelistBoth();               
            }

            OuputBox.AppendText(Environment.NewLine);
            OuputBox.AppendText("Treatment Type Selected: " + ptype);

            TSiteList.SelectionMode = sel;   // after the datasource is set, the selection mode is set back to what was when the plan is selected.
                                             // this is an issue because we choose the selection mode when the plan is selected, before the list has even been made
        }

        private void button1_Click(object sender, EventArgs args)
        {
            //  MessageBox.Show("Organ: " + org.ToString());
            //  MessageBox.Show("Trig 12 - First Click");
        }

        void buttonNext_Click(object sender, EventArgs e, VMS.TPS.Common.Model.API.Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {
            // MessageBox.Show("Trig MORTY");
            EXECUTE(laterality, TS, TSA, patient, course, image3D, user, Plansums, Plans);
        }

        public static string PlansumTotalDoseDialog()
        {
            Form Dialog = new Form()
            {
                Width = 960,
                Height = 290,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Plansum Dose",
                StartPosition = FormStartPosition.CenterScreen,
                Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
            };
            Label txtlab = new Label() { Left = 10, Top = 15, Width = 650, Height = 140, Text = "How do you want the Total Dose of this Plansum to be calculated?" };
            Button confirm = new Button() { Text = "Ok", Left = 120, Width = 75, Top = 210, DialogResult = DialogResult.OK };
            ListBox opt = new ListBox() { Left = 30, Top = 90, Width = 900};
            opt.Items.AddRange(new object[] {
                     "Normal Plansum",
                     "\"fake plansum\" - Both plans have the same Rx dose and you want to evaluate using that dose, NOT the sum.",
                     "For other situations, enter your own dose to use in evaluation. You'll be prompted."});
            confirm.Click += (sender, e) => { Dialog.Close(); };
            Dialog.Controls.Add(opt);
            Dialog.Controls.Add(confirm);
            Dialog.Controls.Add(txtlab);
            Dialog.AcceptButton = confirm;

            return Dialog.ShowDialog() == DialogResult.OK ? (string)opt.SelectedItem : "";
        }

        public static string Dialog()
        {
            //MessageBox.Show("Start plansum custom dialog");
            Form Dialog = new Form()
            {
                Width = 350,
                Height = 225,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Custom Plansum Dose",
                StartPosition = FormStartPosition.CenterScreen,
                Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
            };
            Label txtlab = new Label() { Left = 10, Top = 15, Width = 320, Height = 140, Text = "Please enter the dose (in cGy) that you would like to use as the total dose of the PlanSum." };
            TextBox txtb = new TextBox() { Left = 20, Top = 90, Width = 200 };
            Button confirm = new Button() { Text = "Ok", Left = 120, Width = 100, Top = 120, DialogResult = DialogResult.OK };
            confirm.Click += (sender, e) => { Dialog.Close(); };
            Dialog.Controls.Add(txtb);
            Dialog.Controls.Add(confirm);
            Dialog.Controls.Add(txtlab);
            Dialog.AcceptButton = confirm;

            //MessageBox.Show("Before return plansum custom dialog");
            return Dialog.ShowDialog() == DialogResult.OK ? txtb.Text : "";
        }

    } // end of class GUI



    public class GUICalls
    {
        public static async Task SRSGUICaller(string[] SRSCoverageRequirements)
        {
            await Task.Run(() => System.Windows.Forms.Application.Run(new SRSstatsGUI(SRSCoverageRequirements)));
            return;
        }

        public static async Task ConvGUICaller(string[] ConvCoverageRequirements)
        {
            await Task.Run(() => System.Windows.Forms.Application.Run(new ConvStatsGUI(ConvCoverageRequirements)));
            return;
        }
    }



} // end namespace


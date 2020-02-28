using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS;

/*
    Lahey RadOnc Dose Objective Checker - GUI
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
    This is the GUI of the Dose Objective Check program.

*/



namespace DoseObjectiveCheck
{
    public partial class GUI : Form
    {
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
        List<string> plannames = new List<string>();
        List<string> sumnames = new List<string>();
        List<ROI.ROI> output = new List<ROI.ROI>();

        public GUI(Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {
            InitializeComponent();

            OuputBox.Text = "GUI Initialized";

            // MessageBox.Show("Trig 6");

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

            button1.Click += (sender, EventArgs) => { buttonNext_Click(sender, EventArgs, patient, course, image3D, user, Plansums, Plans); };
        }


        private void EXECUTE(string laterality, string TS, ListBox.SelectedObjectCollection TSA, Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {

            // MessageBox.Show("Trig EXE - 1");
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
                   //  MessageBox.Show("Trig EXE - 7");

                    //EXECUTE PLANSUM DOSE OBJECTIVE ANALYSIS

                    string[] Si = new string[50];

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
                    output = Script.PlansumAnalysis(laterality, Si, ptype, patient, course, Plansum.StructureSet, Plansum, dt, dd, OuputBox, gyntype);
                  //  MessageBox.Show("Trig EXE - 8");
                    bool T = false;
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

                    OuputBox.AppendText(Environment.NewLine);
                    OuputBox.AppendText("PDF Generation Initiated");
                    PdfReport.PDFGenerator.Program.PlansumMain(patient, course, Plansum, image3D, Plansum.StructureSet, user, output, dt, dd);
                }
            }

            if (LOCKPLAN == false)  // plans
            {
               
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
                output = Script.PlanAnalysis(laterality, TS, ptype, user, patient, course, Plan.StructureSet, Plan, OuputBox, gyntype);
              //  MessageBox.Show("Trig EXE - 9");
                bool T = false;

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

                OuputBox.AppendText(Environment.NewLine);
                OuputBox.AppendText("PDF Generation Initiated");
                PdfReport.PDFGenerator.Program.PlanMain(patient, course, Plan, image3D, Plan.StructureSet, user, output);
            }
        }


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

                    sid = Dialog1();

                    if (sid == "Sum of Rx dose of the constituent plans")
                    {
                        // add Rx doses of the two plans in the plansum
                        dt = 1;
                    }
                    else if (sid == "Rx dose of one of the constituent plans")
                    {
                        //use the Rx dose of one of the plans
                        dt = 2;
                    }
                    else if (sid == "Enter your own dose")
                    {
                        //Enter your own dose
                        dt = 3;
                        dd = Convert.ToDouble(Dialog());
                    }

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

                if (TS == "Gynecological")
                {
                    gyntype = GynecologicalEdits();
                }

                if (TS == "Breast 23+fx" || TS == "Breast Hypofx")
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

                    if (obj.ToString() == "Gynecological")
                    {
                        gyntype = GynecologicalEdits();
                    }

                    if (obj.ToString() == "Breast 23+fx" || obj.ToString() == "Breast Hypofx")
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

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("checkedlistboxfire");

            SelectionMode sel = TSiteList.SelectionMode;   
            TSiteList.SelectionMode = SelectionMode.None;   // the selection mode is set to none before the datasource is set or it causes the first element to be automatically selected.

            if (listBox1.SelectedItem.ToString() == "Conventional")
            {
                //conventional
                ptype = "Conventional";
                TSiteList.DataSource = Script.MakelistConv();               
            }
            else if (listBox1.SelectedItem.ToString() == "SRS/SBRT")
            {
                //SRS
                ptype = "SRS/SBRT";
                TSiteList.DataSource = Script.MakelistSRS();               
            }
            else if (listBox1.SelectedItem.ToString() == "Both (Plansums Only)")
            {
                //Both
                ptype = "Both";
                TSiteList.DataSource = Script.MakelistBoth();               
            }

            OuputBox.AppendText(Environment.NewLine);
            OuputBox.AppendText("Treatment Type Selected: " + ptype);

            TSiteList.SelectionMode = sel;   // after the datasource is set, the selection mode is set back to what was when the plan is selected.
                                                // this is an issue because we choose the selection mode when the plan is selected, before the list has even been made
        }

        /*
        void checkedListBox1_ItemCheck(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(0))
            {
                //Conventional
                ptype = "Conventional";
                TSiteList.DataSource = Script.MakelistConv();               
            }
            else if (checkedListBox1.GetItemChecked(1))
            {
                //SRS
                ptype = "SRS/SBRT";
                TSiteList.DataSource = Script.MakelistSRS();              
            }
            else if (checkedListBox1.GetItemChecked(2))
            {
                //Both
                ptype = "Both";
                TSiteList.DataSource = Script.MakelistBoth();               
            }
            OuputBox.Text = "Plan Type Selected";
        }
        */


        /*
        void checkedListBox2_ItemCheck(object sender, EventArgs e)
        {
            if (listBox2.GetItemChecked(0))
            {
                // add Rx doses of the two plans in the plansum
                dt = 1;
            }
            else if (listBox2.GetItemChecked(1))
            {
                //use the Rx dose of one of the plans
                dt = 2;
            }
            else if (listBox2.GetItemChecked(2))
            {
                //Enter your own dose
                dt = 3;
                dd = Convert.ToDouble(Dialog());
            }
        }
        */

        private void button1_Click(object sender, EventArgs args)
        {
            //  MessageBox.Show("Organ: " + org.ToString());
            //  MessageBox.Show("Trig 12 - First Click");
        }

        void buttonNext_Click(object sender, EventArgs e, Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans)
        {
            // MessageBox.Show("Trig MORTY");
            EXECUTE(laterality, TS, TSA, patient, course, image3D, user, Plansums, Plans);
        }

        public static string Dialog1()
        {
            Form Dialog = new Form()
            {
                Width = 600,
                Height = 600,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Plansum Dose",
                StartPosition = FormStartPosition.CenterScreen,
                Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
            };
            Label txtlab = new Label() { Left = 10, Top = 15, Width = 320, Height = 140, Text = "How do you want the Total Dose of this Plansum to be calculated?" };
            Button confirm = new Button() { Text = "Ok", Left = 120, Width = 100, Top = 500, DialogResult = DialogResult.OK };
            ListBox opt = new ListBox() { Left = 30, Top = 90, Width = 450 };
            opt.Items.AddRange(new object[] {
                     "Sum of Rx dose of the constituent plans",
                     "Rx dose of one of the constituent plans",
                     "Enter your own dose"});
            confirm.Click += (sender, e) => { Dialog.Close(); };
            Dialog.Controls.Add(opt);
            Dialog.Controls.Add(confirm);
            Dialog.Controls.Add(txtlab);
            Dialog.AcceptButton = confirm;

            return Dialog.ShowDialog() == DialogResult.OK ? (string)opt.SelectedItem : "";
        }

        public static string Dialog()
        {
            Form Dialog = new Form()
            {
                Width = 350,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Custom Plansum Dose",
                StartPosition = FormStartPosition.CenterScreen,
                Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
            };
            Label txtlab = new Label() { Left = 10, Top = 15, Width = 320, Height = 140, Text = "Please enter the dose (in cGy) that you would like to use as the total dose of the PlanSum" };
            TextBox txtb = new TextBox() { Left = 20, Top = 80, Width = 200 };
            Button confirm = new Button() { Text = "Ok", Left = 120, Width = 100, Top = 110, DialogResult = DialogResult.OK };
            confirm.Click += (sender, e) => { Dialog.Close(); };
            Dialog.Controls.Add(txtb);
            Dialog.Controls.Add(confirm);
            Dialog.Controls.Add(txtlab);
            Dialog.AcceptButton = confirm;

            return Dialog.ShowDialog() == DialogResult.OK ? txtb.Text : "";
        }
    }
}


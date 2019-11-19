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









namespace Auto_Report_Script
{
    public partial class GUI : Form
    {
        public int dt = 0;
        public double dd = 0.0;
        public string ptype = null;
        public string TS = null;
        public ListBox.SelectedObjectCollection TSA;
        public string pl = null;
        public int c = 0;
        public int k = 0;
        public int c1 = 0;
        public int k1 = 0;
        List<string> plannames = new List<string>();
        List<string> sumnames = new List<string>();
        List<ROI.ROI> output = new List<ROI.ROI>();

        public GUI(Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, StructureSet structureSet)
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

            button1.Click += (sender, EventArgs) => { buttonNext_Click(sender, EventArgs, patient, course, image3D, user, Plansums, Plans, structureSet); };
        }


        private void EXECUTE(string TS, ListBox.SelectedObjectCollection TSA, Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, StructureSet structureSet)
        {
            // need to make plan/plansum pick more robust using PL
           // MessageBox.Show("Trig EXE - 1");
            bool LOCKSUM = false;
            bool LOCKPLAN = false;

            OuputBox.Text = "Execution Initiated";

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
              //  MessageBox.Show("Trig EXE - 2");
                Plan = (PlanSetup)ER.Current;
               // MessageBox.Show("Plan Id is: " + Plan.Id);
                LOCKSUM = true;
            }
                else if( CNT > 1) 
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
                                else
                                {
                                    MessageBox.Show("Could not find the selected plan!");
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
                        else
                        {
                            MessageBox.Show("Could not find the selected plansum!");
                        }
                    }
                }

                if (LOCKSUM == false)   // plansum
                {
                   //  MessageBox.Show("Trig EXE - 7");

                    //EXECUTE PLANSUM DOSE OBJECTIVE ANALYSIS

                    string[] Si = new string[36];

                    // this is here to make sure the list of selected treatmant sites exists, because if the user selects a plansum after running a plan without clicking on the treatment site again, this can cause a problem
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

                    OuputBox.Text = "Plansum Analysis Initiated";
                    output = Script.PlansumAnalysis(Si, ptype, patient, course, structureSet, Plansum, dt, dd);
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

                    OuputBox.Text = "PDF Generation Initiated";
                    PdfReport.PDFGenerator.Program.PlansumMain(patient, course, Plansum, image3D, structureSet, user, output, dt, dd);
                }
            }

            if (LOCKPLAN == false)  // plans
            {
              //  MessageBox.Show("Plan Id is: " + Plan.Id);
                OuputBox.Text = "Plan Analysis Initiated";
                output = Script.PlanAnalysis(TS, ptype, user, patient, course, structureSet, Plan);
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

                OuputBox.Text = "PDF Generation Initiated";
                PdfReport.PDFGenerator.Program.PlanMain(patient, course, Plan, image3D, structureSet, user, output);
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

                    this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
                    this.label1 = new System.Windows.Forms.Label();

                    this.label1.AutoSize = true;
                    this.label1.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    this.label1.Location = new System.Drawing.Point(35, 370);
                    this.label1.Name = "label1";
                    this.label1.Size = new System.Drawing.Size(56, 23);
                    this.label1.TabIndex = 11;
                    this.label1.Text = "How do you want the Total Dose of this Plansum to be calculated?";

                    this.checkedListBox2.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    this.checkedListBox2.FormattingEnabled = true;
                    this.checkedListBox2.Items.AddRange(new object[] {
                     "Sum of Rx dose of the constituent plans",
                     "Rx dose of one of the constituent plans",
                     "Enter your own dose"});
                    this.checkedListBox2.Location = new System.Drawing.Point(196, 406);
                    this.checkedListBox2.Name = "checkedListBox2";
                    this.checkedListBox2.Size = new System.Drawing.Size(337, 79);
                    this.checkedListBox2.TabIndex = 10;
                    this.checkedListBox2.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox2_ItemCheck);
                    this.checkedListBox2.SelectedIndexChanged += new System.EventHandler(this.checkedListBox2_SelectedIndexChanged);

                    this.checkedListBox2.Visible = true;
                    this.label1.Visible = true;


                    this.Controls.Add(this.label1);
                    this.Controls.Add(this.checkedListBox2);

                    if (c == 1)
                    {
                        // MessageBox.Show("Trig L1");
                        TSiteList.SelectionMode = SelectionMode.MultiSimple;
                        break;
                    }
                    else if (c == 2)
                    {
                        // MessageBox.Show("Trig L2");
                        TSiteList.SelectionMode = SelectionMode.MultiSimple;
                        break;
                    }
                    else if (c == 3)
                    {
                        // MessageBox.Show("Trig L3");
                        TSiteList.SelectionMode = SelectionMode.MultiSimple;
                        break;
                    }
                }
            }
            foreach (string str in plannames)
            {
                k++;
                if (pl == str)
                {
                    if (LOCK == true)
                    {
                        this.checkedListBox2.Visible = false;
                        this.label1.Visible = false;
                    }

                    if (k == 1)
                    {
                        //  MessageBox.Show("Trig L4");
                        TSiteList.SelectionMode = SelectionMode.One;
                        break;
                    }
                    else if (k == 2)
                    {
                        // MessageBox.Show("Trig L5");
                        TSiteList.SelectionMode = SelectionMode.One;
                        break;
                    }
                    else if (k == 3)
                    {
                        // MessageBox.Show("Trig L6");
                        TSiteList.SelectionMode = SelectionMode.One;
                        break;
                    }
                    else if (k == 4)
                    {
                        // MessageBox.Show("Trig L7");
                        TSiteList.SelectionMode = SelectionMode.One;
                        break;
                    }
                    else if (k == 5)
                    {
                        // MessageBox.Show("Trig L8");
                        TSiteList.SelectionMode = SelectionMode.One;
                        break;
                    }
                }
            }
            // MessageBox.Show("Trig 11");
            c1 = c;
            k1 = k;
            c = 0;
            k = 0;
            OuputBox.Text = "Plan Selected";
        }

        void TSiteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("OrganList fire");
            if(TSiteList.SelectionMode == SelectionMode.One)
            {
                Object obj = TSiteList.SelectedItem;
               // MessageBox.Show("Site List object string is: " + obj.ToString());
                TS = obj.ToString();


              //  Type TRF = TSiteList.SelectedItem.GetType();
              //  MessageBox.Show("Site List object type is: " + TRF.ToString());
            }
            else if (TSiteList.SelectionMode == SelectionMode.MultiSimple)
            {
                TSA = TSiteList.SelectedItems;
            }

            OuputBox.Text = "Treatment Site Selected: " + TS;
        }

        void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("checkedlistboxfire");
            if (checkedListBox1.GetItemChecked(0))
            {
                //conventional
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

            OuputBox.Text = "Treatment Type Selected: " + ptype;
        }

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

        void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("checkedlistboxfire");
            if (checkedListBox2.GetItemChecked(0))
            {
                // add Rx doses of the two plans in the plansum
                dt = 1;
            }
            else if (checkedListBox2.GetItemChecked(1))
            {
                //use the Rx dose of one of the plans
                dt = 2;
            }
            else if (checkedListBox2.GetItemChecked(2))
            {
                //Enter your own dose
                dt = 3;
                dd = Convert.ToDouble(Dialog());
            }
        }

        void checkedListBox2_ItemCheck(object sender, EventArgs e)
        {
            if (checkedListBox2.GetItemChecked(0))
            {
                // add Rx doses of the two plans in the plansum
                dt = 1;
            }
            else if (checkedListBox2.GetItemChecked(1))
            {
                //use the Rx dose of one of the plans
                dt = 2;
            }
            else if (checkedListBox2.GetItemChecked(2))
            {
                //Enter your own dose
                dt = 3;
                dd = Convert.ToDouble(Dialog());
            }
        }

        private void button1_Click(object sender, EventArgs args)
        {
            //  MessageBox.Show("Organ: " + org.ToString());
            //  MessageBox.Show("Trig 12 - First Click");
        }

        void buttonNext_Click(object sender, EventArgs e, Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, StructureSet structureSet)
        {
            // MessageBox.Show("Trig MORTY");
            EXECUTE(TS, TSA, patient, course, image3D, user, Plansums, Plans, structureSet);
        }

        public static string Dialog()
        {
            Form Dialog = new Form()
            {
                Width = 350,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Plansum Dose Dialog Box",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label txtlab = new Label() { Left = 10, Top = 15, Width = 320, Height = 140, Text = "Please enter the dose that you would like to use as the total dose of the PlanSum" };
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


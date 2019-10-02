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

namespace Auto_Report_Script
{
    public partial class GUI : Form
    {
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

            button1.Click += (sender, EventArgs) => { buttonNext_Click(sender, EventArgs, patient, course, image3D, user, Plansums, Plans, structureSet); };


        }






        private void EXECUTE(string TS, ListBox.SelectedObjectCollection TSA, Patient patient, Course course, VMS.TPS.Common.Model.API.Image image3D, User user, IEnumerable<PlanSum> Plansums, IEnumerable<PlanSetup> Plans, StructureSet structureSet)
        {

            if (c1 > 0)
            {
                // MessageBox.Show("Trig EXE - 2-1");
                IEnumerator TR = Plansums.GetEnumerator();
                TR.MoveNext();
                PlanSum Plansum = (PlanSum)TR.Current;

                // MessageBox.Show("Trig EXE - 3");
                if (c1 == 1)
                {
                    Plansum = (PlanSum)TR.Current;
                }
                else if (c1 == 2)
                {
                    TR.MoveNext();
                    Plansum = (PlanSum)TR.Current;
                }
                else if (c1 == 3)
                {
                    TR.MoveNext();
                    TR.MoveNext();
                    Plansum = (PlanSum)TR.Current;
                }
                // MessageBox.Show("Trig EXE - 6");

                //EXECUTE PLANSUM DOSE OBJECTIVE ANALYSIS

                string[] Si = new string[35];

                IEnumerator GG = TSA.GetEnumerator();
                GG.MoveNext();

                for(int i = 0; i < TSA.Count; i++ )
                {
                    Si[i] = Convert.ToString(GG.Current);
                    GG.MoveNext();
                }

                output = Script.PlansumAnalysis(Si, ptype, patient, course, structureSet, Plansum);

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

                PdfReport.PDFGenerator.Program.PlansumMain(patient, course, Plansum, image3D, structureSet, user, output);

            }
            else if (k1 > 0)
            {
                IEnumerator ER = Plans.GetEnumerator();
                ER.MoveNext();
                PlanSetup Plan = (PlanSetup)ER.Current;

                //  MessageBox.Show("K is: " + k1.ToString());
                if (k1 == 1)
                {
                    // MessageBox.Show("Trig EXE - 7");
                    Plan = (PlanSetup)ER.Current;
                }
                else if (k1 == 2)
                {
                    ER.MoveNext();
                    Plan = (PlanSetup)ER.Current;
                }
                else if (k1 == 3)
                {
                    ER.MoveNext();
                    ER.MoveNext();
                    Plan = (PlanSetup)ER.Current;
                }
                else if (k1 == 4)
                {
                    ER.MoveNext();
                    ER.MoveNext();
                    ER.MoveNext();
                    Plan = (PlanSetup)ER.Current;
                }
                else if (k1 == 5)
                {
                    ER.MoveNext();
                    ER.MoveNext();
                    ER.MoveNext();
                    ER.MoveNext();
                    Plan = (PlanSetup)ER.Current;
                }

                //  MessageBox.Show("Trig EXE - 8");

                //EXECUTE PLAN DOSE OBJECTIVE ANALYSIS

                output = Script.PlanAnalysis(TS, ptype, user, patient, course, structureSet, Plan);

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

                PdfReport.PDFGenerator.Program.PlanMain(patient, course, Plan, image3D, structureSet, user, output);

            }
        }


        void PlanList_SelectedIndexChanged(object sender, EventArgs e)
        {
            pl = Plan_List.SelectedItem.ToString();

            //  MessageBox.Show("Trig 10");
            foreach (string str in sumnames)
            {
                c++;
                if (pl == str)
                {
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
        }

        void TSiteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // MessageBox.Show("OrganList fire");
            if(TSiteList.SelectionMode == SelectionMode.One)
            {
                TS = TSiteList.SelectedItem.ToString();
            }
            else if (TSiteList.SelectionMode == SelectionMode.MultiSimple)
            {
                TSA = TSiteList.SelectedItems;
            }
            
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

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class SRSDoseCoverageTargetSelectorGUI : Form
    {
        public SRScoveragestats srscoveragestats = new SRScoveragestats();
        
        public SRSDoseCoverageTargetSelectorGUI(List<string> SRSTargetList, List<string> targetdoses, List<string> targetids, string plandose)
        {
            InitializeComponent();
            //MessageBox.Show("Start of SRS Dose Coverage Target Selector");
            foreach (string str in SRSTargetList)
            {
                t1list.Items.Add(str);
                t2list.Items.Add(str);
                t3list.Items.Add(str);
                t4list.Items.Add(str);
                t5list.Items.Add(str);
            }

            t1list.SelectedItem = "NA";
            t2list.SelectedItem = "NA";
            t3list.SelectedItem = "NA";
            t4list.SelectedItem = "NA";
            t5list.SelectedItem = "NA";

            if (plandose != null)
            {
                //this applies to single target SRS plans and SRS plansums
                DBox1.Text = plandose;
                DBox2.Text = plandose;
                DBox3.Text = plandose;
                DBox4.Text = plandose;
                DBox5.Text = plandose;

                if (SRSTargetList.Any(tar => tar.Equals("_PTV")))
                {
                    t1list.SelectedItem = "_PTV";
                    t2list.SelectedItem = "NA";
                    t3list.SelectedItem = "NA";
                    t4list.SelectedItem = "NA";
                    t5list.SelectedItem = "NA";
                }
            }
            else
            {
                //multi-target SRS
                if(targetids.Count == 2)
                {
                    t1list.SelectedItem = targetids[0];
                    t2list.SelectedItem = targetids[1];
                    t3list.SelectedItem = "NA";
                    t4list.SelectedItem = "NA";
                    t5list.SelectedItem = "NA";
                    DBox1.Text = targetdoses[0];
                    DBox2.Text = targetdoses[1];
                    DBox3.Text = "NA";
                    DBox4.Text = "NA";
                    DBox5.Text = "NA";
                }
                else if (targetids.Count == 3)
                {
                    t1list.SelectedItem = targetids[0];
                    t2list.SelectedItem = targetids[1];
                    t3list.SelectedItem = targetids[2];
                    t4list.SelectedItem = "NA";
                    t5list.SelectedItem = "NA";
                    DBox1.Text = targetdoses[0];
                    DBox2.Text = targetdoses[1];
                    DBox3.Text = targetdoses[2];
                    DBox4.Text = "NA";
                    DBox5.Text = "NA";
                }
                else if (targetids.Count == 4)
                {
                    t1list.SelectedItem = targetids[0];
                    t2list.SelectedItem = targetids[1];
                    t3list.SelectedItem = targetids[2];
                    t4list.SelectedItem = targetids[3];
                    t5list.SelectedItem = "NA";
                    DBox1.Text = targetdoses[0];
                    DBox2.Text = targetdoses[1];
                    DBox3.Text = targetdoses[2];
                    DBox4.Text = targetdoses[3];
                    DBox5.Text = "NA";
                }
            }

            Okay_But.Click += (sender, EventArgs) => { LAMBDALINK(sender, EventArgs); };
        }

        void LAMBDALINK(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("Start LAMBDALINK");
                SRSTargetstats temptarget = new SRSTargetstats();
                
                if (t1list.SelectedItem.ToString() != "NA")
                {
                    //MessageBox.Show("t1 grab");
                    temptarget.TargetNAME = t1list.SelectedItem.ToString();
                    temptarget.Dose = Convert.ToDouble(DBox1.Text);

                    //MessageBox.Show("before Target list add");

                    srscoveragestats.Targets.Add(temptarget);

                        //Add(new Reporting.SRSTargetstats { TargetNAME = t1list.SelectedItem.ToString(), Dose = Convert.ToDouble(DBox1.Text) });
                }

                if (t2list.SelectedItem.ToString() != "NA")
                {
                   // MessageBox.Show("t2 grab");
                    srscoveragestats.Targets.Add(new SRSTargetstats { TargetNAME = t2list.SelectedItem.ToString(), Dose = Convert.ToDouble(DBox2.Text) });
                }

                if (t3list.SelectedItem.ToString() != "NA")
                {
                    srscoveragestats.Targets.Add(new SRSTargetstats { TargetNAME = t3list.SelectedItem.ToString(), Dose = Convert.ToDouble(DBox3.Text) });
                }

                if (t4list.SelectedItem.ToString() != "NA")
                {
                    srscoveragestats.Targets.Add(new SRSTargetstats { TargetNAME = t4list.SelectedItem.ToString(), Dose = Convert.ToDouble(DBox4.Text) });
                }

                if (t5list.SelectedItem.ToString() != "NA")
                {
                    srscoveragestats.Targets.Add(new SRSTargetstats { TargetNAME = t5list.SelectedItem.ToString(), Dose = Convert.ToDouble(DBox5.Text) });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString() + "\n\n" + ex.Message + "\n\n" + ex.Source + "\n\n" + ex.StackTrace);
            }

            //MessageBox.Show("End of SRS Dose Coverage Target Selector");
            this.Close();
        }

    }
}

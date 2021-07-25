using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfReport.PDFGenerator
{
    public partial class ConventionalDoseCoverageTargetSelector : Form
    {
        public Reporting.ConventionalCoverageStats conventionalCoverageStats = new Reporting.ConventionalCoverageStats();
        List<DoseCoverageUtilities.conventionalplaninfo> planlist1 = new List<DoseCoverageUtilities.conventionalplaninfo>();
        public ConventionalDoseCoverageTargetSelector(List<string> TargetList, List<DoseCoverageUtilities.conventionalplaninfo> PlanList, string planid, bool SequentialStatus)
        {
            InitializeComponent();
            planlist1 = PlanList;
            conventionalCoverageStats.Sequential = SequentialStatus;

            if (planid.Contains("PlanSum") || planid.Contains("Plan Sum") || planid.Contains("plan sum") || planid.Contains("plansum"))
            {
                Dbox1.Text = PlanList.Last().dose;
                Dbox2.Text = PlanList.Last().dose;
                Dbox3.Text = PlanList.Last().dose;
                Dbox4.Text = PlanList.Last().dose;
                Dbox5.Text = PlanList.Last().dose;
                Dbox6.Text = PlanList.Last().dose;
                Dbox7.Text = PlanList.Last().dose;
                Dbox8.Text = PlanList.Last().dose;
                Dbox9.Text = PlanList.Last().dose;
                Dbox10.Text = PlanList.Last().dose;
            }
            else
            {
                string pdose = PlanList.Where(pl => pl.id.Equals(planid)).First().dose;

                Dbox1.Text = pdose;
                Dbox2.Text = pdose;
                Dbox3.Text = pdose;
                Dbox4.Text = pdose;
                Dbox5.Text = pdose;
                Dbox6.Text = pdose;
                Dbox7.Text = pdose;
                Dbox8.Text = pdose;
                Dbox9.Text = pdose;
                Dbox10.Text = pdose;
            }

            foreach (string str in TargetList)
            {
                t1list.Items.Add(str);
                t2list.Items.Add(str);
                t3list.Items.Add(str);
                t4list.Items.Add(str);
                t5list.Items.Add(str);
                t6list.Items.Add(str);
                t7list.Items.Add(str);
                t8list.Items.Add(str);
                t9list.Items.Add(str);
                t10list.Items.Add(str);
            }

            t1list.SelectedItem = "NA";
            t2list.SelectedItem = "NA";
            t3list.SelectedItem = "NA";
            t4list.SelectedItem = "NA";
            t5list.SelectedItem = "NA";
            t6list.SelectedItem = "NA";
            t7list.SelectedItem = "NA";
            t8list.SelectedItem = "NA";
            t9list.SelectedItem = "NA";
            t10list.SelectedItem = "NA";

            if (TargetList.Any(tar => tar.Equals("_PTV")))
            {
                t1list.SelectedItem = "_PTV";
            }

            if(TargetList.Any(tar => tar.Equals("_CTV")))
            {
                t2list.SelectedItem = "_CTV";
            }

            foreach (DoseCoverageUtilities.conventionalplaninfo pi in PlanList)
            {
                p1list.Items.Add(pi.id);
                p2list.Items.Add(pi.id);
                p3list.Items.Add(pi.id);
                p4list.Items.Add(pi.id);
                p5list.Items.Add(pi.id);
                p6list.Items.Add(pi.id);
                p7list.Items.Add(pi.id);
                p8list.Items.Add(pi.id);
                p9list.Items.Add(pi.id);
                p10list.Items.Add(pi.id);
            }

            p1list.SelectedItem = planid;
            p2list.SelectedItem = planid;
            p3list.SelectedItem = planid;
            p4list.SelectedItem = planid;
            p5list.SelectedItem = planid;
            p6list.SelectedItem = planid;
            p7list.SelectedItem = planid;
            p8list.SelectedItem = planid;
            p9list.SelectedItem = planid;
            p10list.SelectedItem = planid;


            if (SequentialStatus == true)
            {
                p1list.Visible = true;
                p2list.Visible = true;
                p3list.Visible = true;
                p4list.Visible = true;
                p5list.Visible = true;
                p6list.Visible = true;
                p7list.Visible = true;
                p8list.Visible = true;
                p9list.Visible = true;
                p10list.Visible = true;

                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                label15.Visible = true;
                label26.Visible = true;
                label27.Visible = true;
                label28.Visible = true;
                label29.Visible = true;
                label30.Visible = true;
            }
            else if (SequentialStatus == false)
            {
                p1list.Visible = false;
                p2list.Visible = false;
                p3list.Visible = false;
                p4list.Visible = false;
                p5list.Visible = false;
                p6list.Visible = false;
                p7list.Visible = false;
                p8list.Visible = false;
                p9list.Visible = false;
                p10list.Visible = false;

                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                label14.Visible = false;
                label15.Visible = false;
                label26.Visible = false;
                label27.Visible = false;
                label28.Visible = false;
                label29.Visible = false;
                label30.Visible = false;
            }

            Okay_But.Click += (sender, EventArgs) => { LAMBDALINK(sender, EventArgs, conventionalCoverageStats); };
        }

        void LAMBDALINK(object sender, EventArgs e, PdfReport.Reporting.ConventionalCoverageStats conventionalCoverageStats)
        {
            if (conventionalCoverageStats.Sequential == false)
            {
                if (t1list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t1list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox1.Text), plan = "NA" });
                }

                if (t2list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t2list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox2.Text), plan = "NA" });
                }

                if (t3list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t3list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox3.Text), plan = "NA" });
                }

                if (t4list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t4list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox4.Text), plan = "NA" });
                }

                if (t5list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t5list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox5.Text), plan = "NA" });
                }

                if (t6list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t6list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox6.Text), plan = "NA" });
                }

                if (t7list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t7list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox7.Text), plan = "NA" });
                }

                if (t8list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t8list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox8.Text), plan = "NA" });
                }

                if (t9list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t9list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox9.Text), plan = "NA" });
                }

                if (t10list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t10list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox10.Text), plan = "NA" });
                }
            }
            else if(conventionalCoverageStats.Sequential == true)
            {
                if (t1list.SelectedItem.ToString() != "NA")
                {
                     conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t1list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox1.Text), plan = p1list.SelectedItem.ToString() });
                }

                if (t2list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t2list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox2.Text), plan = p2list.SelectedItem.ToString() });
                }

                if (t3list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t3list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox3.Text), plan = p3list.SelectedItem.ToString() });
                }

                if (t4list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t4list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox4.Text), plan = p4list.SelectedItem.ToString() });
                }

                if (t5list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t5list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox5.Text), plan = p5list.SelectedItem.ToString() });
                }

                if (t6list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t6list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox6.Text), plan = p6list.SelectedItem.ToString() });
                }

                if (t7list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t7list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox7.Text), plan = p7list.SelectedItem.ToString() });
                }

                if (t8list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t8list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox8.Text), plan = p8list.SelectedItem.ToString() });
                }

                if (t9list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t9list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox9.Text), plan = p9list.SelectedItem.ToString() });
                }

                if (t10list.SelectedItem.ToString() != "NA")
                {
                    conventionalCoverageStats.TargetStructures.Add(new Reporting.TargetStructure { StructureNAME = t10list.SelectedItem.ToString(), Dose = Convert.ToDouble(Dbox10.Text), plan = p10list.SelectedItem.ToString() });
                }

            }
            this.Close();
        }

        void p1sel(object sender, EventArgs e)
        {
            Dbox1.Text = planlist1.Where(pl => pl.id.Equals(p1list.SelectedItem)).First().dose;
        }

        void p2sel(object sender, EventArgs e)
        {
            Dbox2.Text = planlist1.Where(pl => pl.id.Equals(p2list.SelectedItem)).First().dose;
        }

        void p3sel(object sender, EventArgs e)
        {
            Dbox3.Text = planlist1.Where(pl => pl.id.Equals(p3list.SelectedItem)).First().dose;
        }

        void p4sel(object sender, EventArgs e)
        {
            Dbox4.Text = planlist1.Where(pl => pl.id.Equals(p4list.SelectedItem)).First().dose;
        }

        void p5sel(object sender, EventArgs e)
        {
            Dbox5.Text = planlist1.Where(pl => pl.id.Equals(p5list.SelectedItem)).First().dose;
        }

        void p6sel(object sender, EventArgs e)
        {
            Dbox6.Text = planlist1.Where(pl => pl.id.Equals(p6list.SelectedItem)).First().dose;
        }

        void p7sel(object sender, EventArgs e)
        {
            Dbox7.Text = planlist1.Where(pl => pl.id.Equals(p7list.SelectedItem)).First().dose;
        }

        void p8sel(object sender, EventArgs e)
        {
            Dbox8.Text = planlist1.Where(pl => pl.id.Equals(p8list.SelectedItem)).First().dose;
        }

        void p9sel(object sender, EventArgs e)
        {
            Dbox9.Text = planlist1.Where(pl => pl.id.Equals(p9list.SelectedItem)).First().dose;
        }

        void p10sel(object sender, EventArgs e)
        {
            Dbox10.Text = planlist1.Where(pl => pl.id.Equals(p10list.SelectedItem)).First().dose;
        }

    }
}

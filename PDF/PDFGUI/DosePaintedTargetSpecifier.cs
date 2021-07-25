using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class DosePaintedTargetSpecifier : Form
    {
        public ConventionalCoverageStats ConventionalCoverageStats = new ConventionalCoverageStats();
        public DosePaintedTargetSpecifier(List<string> ConvTargList)
        {
            InitializeComponent();
            
            foreach(string str in ConvTargList)
            {
                structlist1.Items.Add(str);
                structlist2.Items.Add(str);
                structlist3.Items.Add(str);
                structlist4.Items.Add(str);
            }

            structlist1.SelectedItem = "NA";
            structlist2.SelectedItem = "NA";
            structlist3.SelectedItem = "NA";
            structlist4.SelectedItem = "NA";
        }

        void DoneClick(object sender, EventArgs e)
        {

            object naval = null;
            foreach (object obj in structlist1.SelectedItems)
            {
                //deselect the NA value if the user actually selects something
                if (obj.ToString().Equals("NA"))
                {
                    naval = obj;
                    //MessageBox.Show("NA skip trigger");
                    continue;
                }
                ConventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = obj.ToString(), DosePaintedTargetNumber = 1, Dose = Convert.ToDouble(Dbox1.Text) });
            }
            structlist1.SelectedItems.Remove(naval);


            foreach (object obj in structlist2.SelectedItems)
            {
                //deselect the NA value if the user actually selects something
                if (obj.ToString().Equals("NA"))
                {
                    naval = obj;
                    continue;
                }
                ConventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = obj.ToString(), DosePaintedTargetNumber = 2, Dose = Convert.ToDouble(Dbox2.Text) });
            }
            structlist2.SelectedItems.Remove(naval);


            foreach (object obj in structlist3.SelectedItems)
            {
                //deselect the NA value if the user actually selects something
                if (obj.ToString().Equals("NA"))
                {
                    naval = obj;
                    continue;
                }
                ConventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = obj.ToString(), DosePaintedTargetNumber = 3, Dose = Convert.ToDouble(Dbox3.Text) });
            }
            structlist3.SelectedItems.Remove(naval);


            foreach (object obj in structlist4.SelectedItems)
            {
                //deselect the NA value if the user actually selects something
                if (obj.ToString().Equals("NA"))
                {
                    naval = obj;
                    continue;
                }
                ConventionalCoverageStats.TargetStructures.Add(new TargetStructure { StructureNAME = obj.ToString(), DosePaintedTargetNumber = 4, Dose = Convert.ToDouble(Dbox4.Text) });
            }
            structlist4.SelectedItems.Remove(naval);




            this.Close();
        }


    }
}

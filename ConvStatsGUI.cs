using System;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class ConvStatsGUI : Form
    {
        public ConvStatsGUI(string[] CoverageRequirements)
        {
            InitializeComponent();
            OkayButton.Click += (sender, EventArgs) => { LAMBDALINK(sender, EventArgs, CoverageRequirements); };
        }

        private void OkayBut_Click(object sender, EventArgs args)
        {
            //  MessageBox.Show("Organ: " + org.ToString());
        }

        void LAMBDALINK(object sender, EventArgs e, string[] ConvCoverageRequirements)
        {
            ConvCoverageRequirements[0] = CTV_Rx.Text;
            ConvCoverageRequirements[1] = CTV_Vol.Text;
            ConvCoverageRequirements[2] = PTV1_Rx.Text;
            ConvCoverageRequirements[3] = PTV1_Vol.Text;
            ConvCoverageRequirements[4] = PTV2_Rx.Text;
            ConvCoverageRequirements[5] = PTV2_Vol.Text;
            ConvCoverageRequirements[6] = PTV3_Rx.Text;
            ConvCoverageRequirements[7] = PTV3_Vol.Text;
            this.Close();
        }
    }
}

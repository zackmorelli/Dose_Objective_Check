using System;

using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class SRSstatsGUI : Form
    {
        public SRSstatsGUI(string[] SRScoverageRequirements)
        {
            InitializeComponent();

            OkayBut.Click += (sender, EventArgs) => { LAMBDALINK(sender, EventArgs, SRScoverageRequirements); };
        }

        private void OkayBut_Click(object sender, EventArgs args)
        {
            //  MessageBox.Show("Organ: " + org.ToString());
        }

        void LAMBDALINK(object sender, EventArgs e, string[] SRScoverageRequirements)
        {
            SRScoverageRequirements[0] = PTVRX.Text;
            SRScoverageRequirements[1] = PerVol.Text;
            this.Close();
        }




    }
}

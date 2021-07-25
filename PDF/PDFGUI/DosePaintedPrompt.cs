using System;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class DosePaintedPrompt : Form
    {
        public bool DosePainted = false;
        public DosePaintedPrompt()
        {
            InitializeComponent();
        }

        void YesClick(object sender, EventArgs e)
        {
            DosePainted = true;
            this.Close();
        }

        void NoClick(object sender, EventArgs e)
        {
            DosePainted = false;
            this.Close();
        }


    }
}

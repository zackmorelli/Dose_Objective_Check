using System;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class PlansumSequentialPrompt : Form
    {
        public bool sequentialprompt = false;
        public PlansumSequentialPrompt()
        {
            InitializeComponent();
        }

        void yesclick(object sender, EventArgs e)
        {
            sequentialprompt = true;
            this.Close();
        }

        void noclick(object sender, EventArgs e)
        {
            sequentialprompt = false;
            this.Close();
        }


    }
}

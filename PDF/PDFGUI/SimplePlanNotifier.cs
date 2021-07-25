using System;
using System.Windows.Forms;

namespace DoseObjectiveCheck
{
    public partial class SimplePlanNotifier : Form
    {
        public bool manualselection = false;
        public SimplePlanNotifier()
        {
            InitializeComponent();
        }

        void continuebuttonclick(object sender, EventArgs e)
        {
            manualselection = false;
            this.Close();
        }

        void manualbuttonclick(object sender, EventArgs e)
        {
            manualselection = true;
            this.Close();
        }
    }
}

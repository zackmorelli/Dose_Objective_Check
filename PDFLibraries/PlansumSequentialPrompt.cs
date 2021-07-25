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

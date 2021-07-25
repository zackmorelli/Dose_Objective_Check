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

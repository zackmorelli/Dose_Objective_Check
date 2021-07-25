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

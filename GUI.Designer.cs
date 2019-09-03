namespace Auto_Report_Script
{
    partial class GUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Plan_List = new System.Windows.Forms.ListBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.DirectionText = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.OuputBox = new System.Windows.Forms.TextBox();
            this.TypeList = new System.Windows.Forms.ListBox();
            this.SiteList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Plan_List
            // 
            this.Plan_List.FormattingEnabled = true;
            this.Plan_List.Location = new System.Drawing.Point(860, 99);
            this.Plan_List.Name = "Plan_List";
            this.Plan_List.Size = new System.Drawing.Size(416, 56);
            this.Plan_List.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(956, 634);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(330, 20);
            this.progressBar1.TabIndex = 1;
            // 
            // DirectionText
            // 
            this.DirectionText.Location = new System.Drawing.Point(860, 26);
            this.DirectionText.Multiline = true;
            this.DirectionText.Name = "DirectionText";
            this.DirectionText.ReadOnly = true;
            this.DirectionText.Size = new System.Drawing.Size(416, 48);
            this.DirectionText.TabIndex = 2;
            this.DirectionText.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // OuputBox
            // 
            this.OuputBox.Location = new System.Drawing.Point(12, 12);
            this.OuputBox.Multiline = true;
            this.OuputBox.Name = "OuputBox";
            this.OuputBox.ReadOnly = true;
            this.OuputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OuputBox.Size = new System.Drawing.Size(815, 250);
            this.OuputBox.TabIndex = 5;
            // 
            // TypeList
            // 
            this.TypeList.FormattingEnabled = true;
            this.TypeList.Location = new System.Drawing.Point(867, 178);
            this.TypeList.Name = "TypeList";
            this.TypeList.Size = new System.Drawing.Size(408, 30);
            this.TypeList.TabIndex = 6;
            // 
            // SiteList
            // 
            this.SiteList.FormattingEnabled = true;
            this.SiteList.Location = new System.Drawing.Point(864, 232);
            this.SiteList.Name = "SiteList";
            this.SiteList.Size = new System.Drawing.Size(410, 30);
            this.SiteList.TabIndex = 7;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 655);
            this.Controls.Add(this.SiteList);
            this.Controls.Add(this.TypeList);
            this.Controls.Add(this.OuputBox);
            this.Controls.Add(this.DirectionText);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Plan_List);
            this.Name = "GUI";
            this.Text = "Lahey Radiation Oncology Plan Checker";
            this.Load += new System.EventHandler(this.GUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Plan_List;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox DirectionText;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox OuputBox;
        private System.Windows.Forms.ListBox TypeList;
        private System.Windows.Forms.ListBox SiteList;
    }
}
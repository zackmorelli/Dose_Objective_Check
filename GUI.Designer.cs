namespace DoseObjectiveCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.Plan_List = new System.Windows.Forms.ListBox();
            this.DirectionText = new System.Windows.Forms.TextBox();
            this.OuputBox = new System.Windows.Forms.TextBox();
            this.TSiteList = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lateralitybox = new System.Windows.Forms.ListBox();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.DoseStatisticsBox = new System.Windows.Forms.CheckBox();
            this.UseGoalsCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Plan_List
            // 
            this.Plan_List.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Plan_List.FormattingEnabled = true;
            this.Plan_List.ItemHeight = 24;
            this.Plan_List.Location = new System.Drawing.Point(12, 364);
            this.Plan_List.Name = "Plan_List";
            this.Plan_List.Size = new System.Drawing.Size(318, 148);
            this.Plan_List.TabIndex = 0;
            this.Plan_List.SelectedIndexChanged += new System.EventHandler(this.PlanList_SelectedIndexChanged);
            // 
            // DirectionText
            // 
            this.DirectionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DirectionText.Location = new System.Drawing.Point(12, 12);
            this.DirectionText.Multiline = true;
            this.DirectionText.Name = "DirectionText";
            this.DirectionText.ReadOnly = true;
            this.DirectionText.Size = new System.Drawing.Size(983, 346);
            this.DirectionText.TabIndex = 2;
            this.DirectionText.Text = resources.GetString("DirectionText.Text");
            // 
            // OuputBox
            // 
            this.OuputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OuputBox.Location = new System.Drawing.Point(12, 533);
            this.OuputBox.Multiline = true;
            this.OuputBox.Name = "OuputBox";
            this.OuputBox.ReadOnly = true;
            this.OuputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OuputBox.Size = new System.Drawing.Size(420, 269);
            this.OuputBox.TabIndex = 5;
            // 
            // TSiteList
            // 
            this.TSiteList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TSiteList.FormattingEnabled = true;
            this.TSiteList.ItemHeight = 24;
            this.TSiteList.Location = new System.Drawing.Point(607, 361);
            this.TSiteList.Name = "TSiteList";
            this.TSiteList.Size = new System.Drawing.Size(388, 412);
            this.TSiteList.TabIndex = 7;
            this.TSiteList.SelectedIndexChanged += new System.EventHandler(this.TSiteList_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(459, 677);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 70);
            this.button1.TabIndex = 9;
            this.button1.Text = "Execute";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Items.AddRange(new object[] {
            "Conventional",
            "SRS/SBRT",
            "Both (Plansums Only)"});
            this.listBox1.Location = new System.Drawing.Point(368, 364);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(206, 76);
            this.listBox1.TabIndex = 8;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // lateralitybox
            // 
            this.lateralitybox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lateralitybox.FormattingEnabled = true;
            this.lateralitybox.ItemHeight = 24;
            this.lateralitybox.Items.AddRange(new object[] {
            "Right",
            "Left",
            "Bilateral"});
            this.lateralitybox.Location = new System.Drawing.Point(477, 548);
            this.lateralitybox.Name = "lateralitybox";
            this.lateralitybox.Size = new System.Drawing.Size(86, 76);
            this.lateralitybox.TabIndex = 10;
            this.lateralitybox.Visible = false;
            this.lateralitybox.SelectedIndexChanged += new System.EventHandler(this.lateralitybox_SelectedIndexChanged);
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(438, 779);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(557, 23);
            this.pBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pBar.TabIndex = 11;
            this.pBar.Visible = false;
            // 
            // DoseStatisticsBox
            // 
            this.DoseStatisticsBox.AutoSize = true;
            this.DoseStatisticsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoseStatisticsBox.Location = new System.Drawing.Point(368, 446);
            this.DoseStatisticsBox.Name = "DoseStatisticsBox";
            this.DoseStatisticsBox.Size = new System.Drawing.Size(186, 52);
            this.DoseStatisticsBox.TabIndex = 12;
            this.DoseStatisticsBox.Text = "Include Target\r\nCoverage Statistics";
            this.DoseStatisticsBox.UseVisualStyleBackColor = true;
            // 
            // UseGoalsCheck
            // 
            this.UseGoalsCheck.AutoSize = true;
            this.UseGoalsCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseGoalsCheck.Location = new System.Drawing.Point(368, 499);
            this.UseGoalsCheck.Name = "UseGoalsCheck";
            this.UseGoalsCheck.Size = new System.Drawing.Size(180, 28);
            this.UseGoalsCheck.TabIndex = 13;
            this.UseGoalsCheck.Text = "Use Clinical Goals";
            this.UseGoalsCheck.UseVisualStyleBackColor = true;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 814);
            this.Controls.Add(this.UseGoalsCheck);
            this.Controls.Add(this.DoseStatisticsBox);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.lateralitybox);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TSiteList);
            this.Controls.Add(this.OuputBox);
            this.Controls.Add(this.DirectionText);
            this.Controls.Add(this.Plan_List);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GUI";
            this.Text = " Dose Objective Check";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Plan_List;
        private System.Windows.Forms.TextBox DirectionText;
        public System.Windows.Forms.TextBox OuputBox;
        private System.Windows.Forms.ListBox TSiteList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox lateralitybox;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.CheckBox DoseStatisticsBox;
        private System.Windows.Forms.CheckBox UseGoalsCheck;
    }
}
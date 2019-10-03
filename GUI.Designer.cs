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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.Plan_List = new System.Windows.Forms.ListBox();
            this.DirectionText = new System.Windows.Forms.TextBox();
            this.OuputBox = new System.Windows.Forms.TextBox();
            this.TSiteList = new System.Windows.Forms.ListBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Plan_List
            // 
            this.Plan_List.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Plan_List.FormattingEnabled = true;
            this.Plan_List.ItemHeight = 23;
            this.Plan_List.Location = new System.Drawing.Point(12, 165);
            this.Plan_List.Name = "Plan_List";
            this.Plan_List.Size = new System.Drawing.Size(293, 96);
            this.Plan_List.TabIndex = 0;
            this.Plan_List.SelectedIndexChanged += new System.EventHandler(this.PlanList_SelectedIndexChanged);
            // 
            // DirectionText
            // 
            this.DirectionText.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DirectionText.Location = new System.Drawing.Point(12, 12);
            this.DirectionText.Multiline = true;
            this.DirectionText.Name = "DirectionText";
            this.DirectionText.ReadOnly = true;
            this.DirectionText.Size = new System.Drawing.Size(938, 132);
            this.DirectionText.TabIndex = 2;
            this.DirectionText.Text = resources.GetString("DirectionText.Text");
            // 
            // OuputBox
            // 
            this.OuputBox.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OuputBox.Location = new System.Drawing.Point(31, 282);
            this.OuputBox.Multiline = true;
            this.OuputBox.Name = "OuputBox";
            this.OuputBox.ReadOnly = true;
            this.OuputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OuputBox.Size = new System.Drawing.Size(393, 86);
            this.OuputBox.TabIndex = 5;
            // 
            // TSiteList
            // 
            this.TSiteList.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TSiteList.FormattingEnabled = true;
            this.TSiteList.ItemHeight = 23;
            this.TSiteList.Location = new System.Drawing.Point(562, 165);
            this.TSiteList.Name = "TSiteList";
            this.TSiteList.Size = new System.Drawing.Size(388, 303);
            this.TSiteList.TabIndex = 7;
            this.TSiteList.SelectedIndexChanged += new System.EventHandler(this.TSiteList_SelectedIndexChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Conventional",
            "SRS",
            "Both (Plansums Only)"});
            this.checkedListBox1.Location = new System.Drawing.Point(327, 165);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(206, 79);
            this.checkedListBox1.TabIndex = 8;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(140, 393);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 75);
            this.button1.TabIndex = 9;
            this.button1.Text = "Execute Dose Objective Check";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 497);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.TSiteList);
            this.Controls.Add(this.OuputBox);
            this.Controls.Add(this.DirectionText);
            this.Controls.Add(this.Plan_List);
            this.Name = "GUI";
            this.Text = "Lahey Radiation Oncology Plan Checker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Plan_List;
        private System.Windows.Forms.TextBox DirectionText;
        private System.Windows.Forms.TextBox OuputBox;
        private System.Windows.Forms.ListBox TSiteList;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button1;
    }
}
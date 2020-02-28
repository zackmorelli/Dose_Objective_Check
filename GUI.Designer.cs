﻿namespace DoseObjectiveCheck
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
            this.SuspendLayout();
            // 
            // Plan_List
            // 
            this.Plan_List.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Plan_List.FormattingEnabled = true;
            this.Plan_List.ItemHeight = 23;
            this.Plan_List.Location = new System.Drawing.Point(12, 142);
            this.Plan_List.Name = "Plan_List";
            this.Plan_List.Size = new System.Drawing.Size(293, 142);
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
            this.DirectionText.Size = new System.Drawing.Size(990, 124);
            this.DirectionText.TabIndex = 2;
            this.DirectionText.Text = resources.GetString("DirectionText.Text");
            // 
            // OuputBox
            // 
            this.OuputBox.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OuputBox.Location = new System.Drawing.Point(12, 300);
            this.OuputBox.Multiline = true;
            this.OuputBox.Name = "OuputBox";
            this.OuputBox.ReadOnly = true;
            this.OuputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OuputBox.Size = new System.Drawing.Size(420, 235);
            this.OuputBox.TabIndex = 5;
            // 
            // TSiteList
            // 
            this.TSiteList.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TSiteList.FormattingEnabled = true;
            this.TSiteList.ItemHeight = 23;
            this.TSiteList.Location = new System.Drawing.Point(614, 142);
            this.TSiteList.Name = "TSiteList";
            this.TSiteList.Size = new System.Drawing.Size(388, 372);
            this.TSiteList.TabIndex = 7;
            this.TSiteList.SelectedIndexChanged += new System.EventHandler(this.TSiteList_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(448, 325);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 77);
            this.button1.TabIndex = 9;
            this.button1.Text = "Execute Dose Objective Check";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 23;
            this.listBox1.Items.AddRange(new object[] {
            "Conventional",
            "SRS/SBRT",
            "Both (Plansums Only)"});
            this.listBox1.Location = new System.Drawing.Point(348, 142);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(206, 73);
            this.listBox1.TabIndex = 8;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // lateralitybox
            // 
            this.lateralitybox.Font = new System.Drawing.Font("Goudy Old Style", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lateralitybox.FormattingEnabled = true;
            this.lateralitybox.ItemHeight = 23;
            this.lateralitybox.Items.AddRange(new object[] {
            "Right",
            "Left",
            "Bilateral"});
            this.lateralitybox.Location = new System.Drawing.Point(468, 230);
            this.lateralitybox.Name = "lateralitybox";
            this.lateralitybox.Size = new System.Drawing.Size(86, 73);
            this.lateralitybox.TabIndex = 10;
            this.lateralitybox.Visible = false;
            this.lateralitybox.SelectedIndexChanged += new System.EventHandler(this.lateralitybox_SelectedIndexChanged);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 547);
            this.Controls.Add(this.lateralitybox);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
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
        public System.Windows.Forms.TextBox OuputBox;
        private System.Windows.Forms.ListBox TSiteList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox lateralitybox;
    }
}
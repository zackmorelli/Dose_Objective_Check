namespace DoseObjectiveCheck
{ 
    partial class PlansumSequentialPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlansumSequentialPrompt));
            this.label1 = new System.Windows.Forms.Label();
            this.YesBut = new System.Windows.Forms.Button();
            this.NoBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(781, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "You are attempting to run the program on a plansum. Is this for a sequential cour" +
    "se of treatment for which you\r\n would like to generate target dose coverage stat" +
    "istics for each plan in the course?";
            // 
            // YesBut
            // 
            this.YesBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YesBut.Location = new System.Drawing.Point(156, 63);
            this.YesBut.Name = "YesBut";
            this.YesBut.Size = new System.Drawing.Size(80, 45);
            this.YesBut.TabIndex = 1;
            this.YesBut.Text = "Yes";
            this.YesBut.UseVisualStyleBackColor = true;
            this.YesBut.Click += new System.EventHandler(this.yesclick);
            // 
            // NoBut
            // 
            this.NoBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoBut.Location = new System.Drawing.Point(508, 63);
            this.NoBut.Name = "NoBut";
            this.NoBut.Size = new System.Drawing.Size(80, 45);
            this.NoBut.TabIndex = 2;
            this.NoBut.Text = "No";
            this.NoBut.UseVisualStyleBackColor = true;
            this.NoBut.Click += new System.EventHandler(this.noclick);
            // 
            // PlansumSequentialPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 115);
            this.Controls.Add(this.NoBut);
            this.Controls.Add(this.YesBut);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlansumSequentialPrompt";
            this.Text = "Plansum Sequential Prompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button YesBut;
        private System.Windows.Forms.Button NoBut;
    }
}
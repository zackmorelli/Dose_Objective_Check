namespace DoseObjectiveCheck
{
    partial class SRSstatsGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SRSstatsGUI));
            this.PTVLabel = new System.Windows.Forms.Label();
            this.labelRX = new System.Windows.Forms.Label();
            this.labelVol = new System.Windows.Forms.Label();
            this.PTVRX = new System.Windows.Forms.TextBox();
            this.PerVol = new System.Windows.Forms.TextBox();
            this.OkayBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PTVLabel
            // 
            this.PTVLabel.AutoSize = true;
            this.PTVLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PTVLabel.Location = new System.Drawing.Point(12, 21);
            this.PTVLabel.Name = "PTVLabel";
            this.PTVLabel.Size = new System.Drawing.Size(39, 20);
            this.PTVLabel.TabIndex = 1;
            this.PTVLabel.Text = "PTV";
            // 
            // labelRX
            // 
            this.labelRX.AutoSize = true;
            this.labelRX.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRX.Location = new System.Drawing.Point(119, 27);
            this.labelRX.Name = "labelRX";
            this.labelRX.Size = new System.Drawing.Size(96, 20);
            this.labelRX.TabIndex = 2;
            this.labelRX.Text = "% Rx covers";
            // 
            // labelVol
            // 
            this.labelVol.AutoSize = true;
            this.labelVol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVol.Location = new System.Drawing.Point(283, 27);
            this.labelVol.Name = "labelVol";
            this.labelVol.Size = new System.Drawing.Size(81, 20);
            this.labelVol.TabIndex = 3;
            this.labelVol.Text = "% Volume";
            // 
            // PTVRX
            // 
            this.PTVRX.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PTVRX.Location = new System.Drawing.Point(57, 21);
            this.PTVRX.Name = "PTVRX";
            this.PTVRX.Size = new System.Drawing.Size(56, 26);
            this.PTVRX.TabIndex = 4;
            this.PTVRX.Text = "100";
            // 
            // PerVol
            // 
            this.PerVol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PerVol.Location = new System.Drawing.Point(221, 21);
            this.PerVol.Name = "PerVol";
            this.PerVol.Size = new System.Drawing.Size(56, 26);
            this.PerVol.TabIndex = 5;
            this.PerVol.Text = "95";
            // 
            // OkayBut
            // 
            this.OkayBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkayBut.Location = new System.Drawing.Point(123, 63);
            this.OkayBut.Name = "OkayBut";
            this.OkayBut.Size = new System.Drawing.Size(69, 28);
            this.OkayBut.TabIndex = 6;
            this.OkayBut.Text = "Okay";
            this.OkayBut.UseVisualStyleBackColor = true;
            this.OkayBut.Click += new System.EventHandler(this.OkayBut_Click);
            // 
            // SRSstatsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 103);
            this.Controls.Add(this.OkayBut);
            this.Controls.Add(this.PerVol);
            this.Controls.Add(this.PTVRX);
            this.Controls.Add(this.labelVol);
            this.Controls.Add(this.labelRX);
            this.Controls.Add(this.PTVLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SRSstatsGUI";
            this.Text = "SRS/SBRT Coverage Requirement";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label PTVLabel;
        private System.Windows.Forms.Label labelRX;
        private System.Windows.Forms.Label labelVol;
        private System.Windows.Forms.TextBox PTVRX;
        private System.Windows.Forms.TextBox PerVol;
        private System.Windows.Forms.Button OkayBut;
    }
}
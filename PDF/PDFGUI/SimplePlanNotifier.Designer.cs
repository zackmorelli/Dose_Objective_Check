namespace DoseObjectiveCheck
{
    partial class SimplePlanNotifier
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimplePlanNotifier));
            this.label1 = new System.Windows.Forms.Label();
            this.contbut = new System.Windows.Forms.Button();
            this.manualbut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(553, 60);
            this.label1.TabIndex = 0;
            this.label1.Text = "This seems to be a simple conventional plan with one PTV and one CTV/GTV.\r\nThe pr" +
    "ogram can attempt to process this automatically, unless you want to\r\nmanually se" +
    "lect structures for target evaluation.";
            // 
            // contbut
            // 
            this.contbut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contbut.Location = new System.Drawing.Point(125, 79);
            this.contbut.Name = "contbut";
            this.contbut.Size = new System.Drawing.Size(81, 35);
            this.contbut.TabIndex = 1;
            this.contbut.Text = "Continue";
            this.contbut.UseVisualStyleBackColor = true;
            this.contbut.Click += new System.EventHandler(this.continuebuttonclick);
            // 
            // manualbut
            // 
            this.manualbut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.manualbut.Location = new System.Drawing.Point(362, 72);
            this.manualbut.Name = "manualbut";
            this.manualbut.Size = new System.Drawing.Size(133, 48);
            this.manualbut.TabIndex = 2;
            this.manualbut.Text = "Manually select targets";
            this.manualbut.UseVisualStyleBackColor = true;
            this.manualbut.Click += new System.EventHandler(this.manualbuttonclick);
            // 
            // SimplePlanNotifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 125);
            this.Controls.Add(this.manualbut);
            this.Controls.Add(this.contbut);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimplePlanNotifier";
            this.Text = "Simple Plan Notifier";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button contbut;
        private System.Windows.Forms.Button manualbut;
    }
}
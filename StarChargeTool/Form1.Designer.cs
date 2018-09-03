namespace StarChargeTool
{
    partial class MainForm
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
            this.tbSignature = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.tbPatchedStations = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbSignature
            // 
            this.tbSignature.Location = new System.Drawing.Point(27, 12);
            this.tbSignature.Name = "tbSignature";
            this.tbSignature.Size = new System.Drawing.Size(1322, 31);
            this.tbSignature.TabIndex = 0;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(1366, 1);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(152, 52);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tbPatchedStations
            // 
            this.tbPatchedStations.Location = new System.Drawing.Point(27, 71);
            this.tbPatchedStations.Multiline = true;
            this.tbPatchedStations.Name = "tbPatchedStations";
            this.tbPatchedStations.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPatchedStations.Size = new System.Drawing.Size(1491, 961);
            this.tbPatchedStations.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1536, 1064);
            this.Controls.Add(this.tbPatchedStations);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.tbSignature);
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSignature;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox tbPatchedStations;
    }
}


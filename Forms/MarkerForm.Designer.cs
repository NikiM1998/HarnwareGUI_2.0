namespace HarnwareGUI.Forms
{
    partial class MarkerForm
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
            kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            kryptonPanel2 = new Krypton.Toolkit.KryptonPanel();
            dgvMarkerType = new Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).BeginInit();
            kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).BeginInit();
            kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMarkerType).BeginInit();
            SuspendLayout();
            // 
            // kryptonPanel1
            // 
            kryptonPanel1.Controls.Add(kryptonLabel1);
            kryptonPanel1.Location = new Point(8, 8);
            kryptonPanel1.Name = "kryptonPanel1";
            kryptonPanel1.Size = new Size(1054, 96);
            kryptonPanel1.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Location = new Point(4, 4);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(98, 20);
            kryptonLabel1.TabIndex = 0;
            kryptonLabel1.Values.Text = "Define a Marker";
            // 
            // kryptonPanel2
            // 
            kryptonPanel2.Controls.Add(dgvMarkerType);
            kryptonPanel2.Dock = DockStyle.Bottom;
            kryptonPanel2.Location = new Point(0, 114);
            kryptonPanel2.Name = "kryptonPanel2";
            kryptonPanel2.Size = new Size(1072, 250);
            kryptonPanel2.TabIndex = 1;
            // 
            // dgvMarkerType
            // 
            dgvMarkerType.AllowUserToAddRows = false;
            dgvMarkerType.BorderStyle = BorderStyle.None;
            dgvMarkerType.Location = new Point(11, 9);
            dgvMarkerType.Name = "dgvMarkerType";
            dgvMarkerType.Size = new Size(1051, 235);
            dgvMarkerType.TabIndex = 0;
            // 
            // MarkerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1072, 364);
            Controls.Add(kryptonPanel2);
            Controls.Add(kryptonPanel1);
            Name = "MarkerForm";
            Text = "MarkerForm";
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).EndInit();
            kryptonPanel1.ResumeLayout(false);
            kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).EndInit();
            kryptonPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMarkerType).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonDataGridView dgvMarkerType;
    }
}
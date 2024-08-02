namespace HarnwareGUI.Forms
{
    partial class WireType
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
            btnWireTypeAssign = new Krypton.Toolkit.KryptonButton();
            btnWireTypeDefine = new Krypton.Toolkit.KryptonButton();
            pnlWireType = new Krypton.Toolkit.KryptonPanel();
            dgvWireType = new Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)pnlWireType).BeginInit();
            pnlWireType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvWireType).BeginInit();
            SuspendLayout();
            // 
            // btnWireTypeAssign
            // 
            btnWireTypeAssign.Location = new Point(12, 12);
            btnWireTypeAssign.Name = "btnWireTypeAssign";
            btnWireTypeAssign.Size = new Size(120, 48);
            btnWireTypeAssign.TabIndex = 1;
            btnWireTypeAssign.Values.Text = "Assign";
            btnWireTypeAssign.Click += btnWireTypeAssign_Click;
            // 
            // btnWireTypeDefine
            // 
            btnWireTypeDefine.Location = new Point(153, 12);
            btnWireTypeDefine.Name = "btnWireTypeDefine";
            btnWireTypeDefine.Size = new Size(120, 48);
            btnWireTypeDefine.TabIndex = 2;
            btnWireTypeDefine.Values.Text = "Define";
            btnWireTypeDefine.Click += btnWireTypeDefine_Click;
            // 
            // pnlWireType
            // 
            pnlWireType.Controls.Add(dgvWireType);
            pnlWireType.Dock = DockStyle.Bottom;
            pnlWireType.Location = new Point(0, 97);
            pnlWireType.Name = "pnlWireType";
            pnlWireType.Size = new Size(800, 353);
            pnlWireType.TabIndex = 3;
            // 
            // dgvWireType
            // 
            dgvWireType.AllowUserToAddRows = false;
            dgvWireType.BorderStyle = BorderStyle.None;
            dgvWireType.Location = new Point(3, 12);
            dgvWireType.Name = "dgvWireType";
            dgvWireType.Size = new Size(792, 337);
            dgvWireType.TabIndex = 0;
            // 
            // WireType
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlWireType);
            Controls.Add(btnWireTypeDefine);
            Controls.Add(btnWireTypeAssign);
            Name = "WireType";
            Text = "WireType";
            ((System.ComponentModel.ISupportInitialize)pnlWireType).EndInit();
            pnlWireType.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvWireType).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonButton btnWireTypeAssign;
        private Krypton.Toolkit.KryptonButton btnWireTypeDefine;
        private Krypton.Toolkit.KryptonPanel pnlWireType;
        private Krypton.Toolkit.KryptonDataGridView dgvWireType;
    }
}
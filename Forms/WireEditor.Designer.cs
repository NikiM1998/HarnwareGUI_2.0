namespace HarnwareGUI.Forms
{
    partial class WireEditor
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
            btnWireEditorAssign = new Krypton.Toolkit.KryptonButton();
            btnWireEditorDefine = new Krypton.Toolkit.KryptonButton();
            pnlWireEditor = new Krypton.Toolkit.KryptonPanel();
            dgvWireEditor = new Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)pnlWireEditor).BeginInit();
            pnlWireEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvWireEditor).BeginInit();
            SuspendLayout();
            // 
            // btnWireEditorAssign
            // 
            btnWireEditorAssign.Location = new Point(10, 13);
            btnWireEditorAssign.Name = "btnWireEditorAssign";
            btnWireEditorAssign.Size = new Size(120, 48);
            btnWireEditorAssign.TabIndex = 0;
            btnWireEditorAssign.Values.Text = "Assign";
            btnWireEditorAssign.Click += btnWireEditorAssign_Click;
            // 
            // btnWireEditorDefine
            // 
            btnWireEditorDefine.Location = new Point(154, 13);
            btnWireEditorDefine.Name = "btnWireEditorDefine";
            btnWireEditorDefine.Size = new Size(120, 48);
            btnWireEditorDefine.TabIndex = 1;
            btnWireEditorDefine.Values.Text = "Define";
            btnWireEditorDefine.Click += btnWireEditorDefine_Click;
            // 
            // pnlWireEditor
            // 
            pnlWireEditor.Controls.Add(dgvWireEditor);
            pnlWireEditor.Dock = DockStyle.Bottom;
            pnlWireEditor.Location = new Point(0, 103);
            pnlWireEditor.Name = "pnlWireEditor";
            pnlWireEditor.Size = new Size(800, 347);
            pnlWireEditor.TabIndex = 2;
            // 
            // dgvWireEditor
            // 
            dgvWireEditor.AllowUserToAddRows = false;
            dgvWireEditor.BorderStyle = BorderStyle.None;
            dgvWireEditor.Location = new Point(6, 9);
            dgvWireEditor.Name = "dgvWireEditor";
            dgvWireEditor.Size = new Size(791, 333);
            dgvWireEditor.TabIndex = 0;
            // 
            // WireEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlWireEditor);
            Controls.Add(btnWireEditorDefine);
            Controls.Add(btnWireEditorAssign);
            Name = "WireEditor";
            Text = "WireEditor";
            ((System.ComponentModel.ISupportInitialize)pnlWireEditor).EndInit();
            pnlWireEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvWireEditor).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonButton btnWireEditorAssign;
        private Krypton.Toolkit.KryptonButton btnWireEditorDefine;
        private Krypton.Toolkit.KryptonPanel pnlWireEditor;
        private Krypton.Toolkit.KryptonDataGridView dgvWireEditor;
    }
}
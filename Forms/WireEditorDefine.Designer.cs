namespace HarnwareGUI.Forms
{
    partial class WireEditorDefine
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
            pnlWireEditorDefineHead = new Krypton.Toolkit.KryptonPanel();
            pnlWireEditorDefineData = new Krypton.Toolkit.KryptonPanel();
            lblWireEditorDefineHead = new Krypton.Toolkit.KryptonLabel();
            btnSaveWireEditor = new Krypton.Toolkit.KryptonButton();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel4 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel5 = new Krypton.Toolkit.KryptonLabel();
            tbSupplierPartNum = new Krypton.Toolkit.KryptonTextBox();
            tbWireType = new Krypton.Toolkit.KryptonTextBox();
            tbCores = new Krypton.Toolkit.KryptonTextBox();
            tbCoreDiameter = new Krypton.Toolkit.KryptonTextBox();
            tbWireDiameter = new Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)pnlWireEditorDefineHead).BeginInit();
            pnlWireEditorDefineHead.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlWireEditorDefineData).BeginInit();
            pnlWireEditorDefineData.SuspendLayout();
            SuspendLayout();
            // 
            // pnlWireEditorDefineHead
            // 
            pnlWireEditorDefineHead.Controls.Add(btnSaveWireEditor);
            pnlWireEditorDefineHead.Controls.Add(lblWireEditorDefineHead);
            pnlWireEditorDefineHead.Location = new Point(7, 7);
            pnlWireEditorDefineHead.Name = "pnlWireEditorDefineHead";
            pnlWireEditorDefineHead.Size = new Size(786, 122);
            pnlWireEditorDefineHead.TabIndex = 0;
            // 
            // pnlWireEditorDefineData
            // 
            pnlWireEditorDefineData.Controls.Add(tbWireDiameter);
            pnlWireEditorDefineData.Controls.Add(tbCoreDiameter);
            pnlWireEditorDefineData.Controls.Add(tbCores);
            pnlWireEditorDefineData.Controls.Add(tbWireType);
            pnlWireEditorDefineData.Controls.Add(tbSupplierPartNum);
            pnlWireEditorDefineData.Controls.Add(kryptonLabel5);
            pnlWireEditorDefineData.Controls.Add(kryptonLabel4);
            pnlWireEditorDefineData.Controls.Add(kryptonLabel3);
            pnlWireEditorDefineData.Controls.Add(kryptonLabel2);
            pnlWireEditorDefineData.Controls.Add(kryptonLabel1);
            pnlWireEditorDefineData.Dock = DockStyle.Bottom;
            pnlWireEditorDefineData.Location = new Point(0, 156);
            pnlWireEditorDefineData.Name = "pnlWireEditorDefineData";
            pnlWireEditorDefineData.Size = new Size(800, 294);
            pnlWireEditorDefineData.TabIndex = 1;
            // 
            // lblWireEditorDefineHead
            // 
            lblWireEditorDefineHead.Location = new Point(12, 10);
            lblWireEditorDefineHead.Name = "lblWireEditorDefineHead";
            lblWireEditorDefineHead.Size = new Size(99, 20);
            lblWireEditorDefineHead.TabIndex = 0;
            lblWireEditorDefineHead.Values.Text = "New Wire Editor";
            // 
            // btnSaveWireEditor
            // 
            btnSaveWireEditor.Location = new Point(5, 56);
            btnSaveWireEditor.Name = "btnSaveWireEditor";
            btnSaveWireEditor.Size = new Size(122, 51);
            btnSaveWireEditor.TabIndex = 1;
            btnSaveWireEditor.Values.Text = "Save";
            btnSaveWireEditor.Click += btnSaveWireEditor_Click;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Location = new Point(35, 34);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(131, 20);
            kryptonLabel1.TabIndex = 1;
            kryptonLabel1.Values.Text = "Supplier Part Number:";
            // 
            // kryptonLabel2
            // 
            kryptonLabel2.Location = new Point(35, 78);
            kryptonLabel2.Name = "kryptonLabel2";
            kryptonLabel2.Size = new Size(68, 20);
            kryptonLabel2.TabIndex = 2;
            kryptonLabel2.Values.Text = "Wire Type:";
            // 
            // kryptonLabel3
            // 
            kryptonLabel3.Location = new Point(35, 120);
            kryptonLabel3.Name = "kryptonLabel3";
            kryptonLabel3.Size = new Size(44, 20);
            kryptonLabel3.TabIndex = 3;
            kryptonLabel3.Values.Text = "Cores:";
            // 
            // kryptonLabel4
            // 
            kryptonLabel4.Location = new Point(35, 169);
            kryptonLabel4.Name = "kryptonLabel4";
            kryptonLabel4.Size = new Size(124, 20);
            kryptonLabel4.TabIndex = 4;
            kryptonLabel4.Values.Text = "Core Diameter (mm):";
            // 
            // kryptonLabel5
            // 
            kryptonLabel5.Location = new Point(35, 220);
            kryptonLabel5.Name = "kryptonLabel5";
            kryptonLabel5.Size = new Size(124, 20);
            kryptonLabel5.TabIndex = 5;
            kryptonLabel5.Values.Text = "Wire Diameter (mm):";
            // 
            // tbSupplierPartNum
            // 
            tbSupplierPartNum.Location = new Point(183, 31);
            tbSupplierPartNum.Name = "tbSupplierPartNum";
            tbSupplierPartNum.Size = new Size(534, 23);
            tbSupplierPartNum.TabIndex = 6;
            // 
            // tbWireType
            // 
            tbWireType.Location = new Point(183, 75);
            tbWireType.Name = "tbWireType";
            tbWireType.Size = new Size(534, 23);
            tbWireType.TabIndex = 7;
            // 
            // tbCores
            // 
            tbCores.Location = new Point(183, 117);
            tbCores.Name = "tbCores";
            tbCores.Size = new Size(534, 23);
            tbCores.TabIndex = 8;
            // 
            // tbCoreDiameter
            // 
            tbCoreDiameter.Location = new Point(183, 166);
            tbCoreDiameter.Name = "tbCoreDiameter";
            tbCoreDiameter.Size = new Size(534, 23);
            tbCoreDiameter.TabIndex = 9;
            // 
            // tbWireDiameter
            // 
            tbWireDiameter.Location = new Point(183, 217);
            tbWireDiameter.Name = "tbWireDiameter";
            tbWireDiameter.Size = new Size(534, 23);
            tbWireDiameter.TabIndex = 10;
            // 
            // WireEditorDefine
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlWireEditorDefineData);
            Controls.Add(pnlWireEditorDefineHead);
            Name = "WireEditorDefine";
            Text = "WireEditorDefine";
            ((System.ComponentModel.ISupportInitialize)pnlWireEditorDefineHead).EndInit();
            pnlWireEditorDefineHead.ResumeLayout(false);
            pnlWireEditorDefineHead.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pnlWireEditorDefineData).EndInit();
            pnlWireEditorDefineData.ResumeLayout(false);
            pnlWireEditorDefineData.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel pnlWireEditorDefineHead;
        private Krypton.Toolkit.KryptonLabel lblWireEditorDefineHead;
        private Krypton.Toolkit.KryptonPanel pnlWireEditorDefineData;
        private Krypton.Toolkit.KryptonButton btnSaveWireEditor;
        private Krypton.Toolkit.KryptonTextBox tbWireDiameter;
        private Krypton.Toolkit.KryptonTextBox tbCoreDiameter;
        private Krypton.Toolkit.KryptonTextBox tbCores;
        private Krypton.Toolkit.KryptonTextBox tbWireType;
        private Krypton.Toolkit.KryptonTextBox tbSupplierPartNum;
        private Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}
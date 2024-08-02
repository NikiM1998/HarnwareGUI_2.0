namespace HarnwareGUI.Forms
{
    partial class WireTypeDefine
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
            kryptonPanel2 = new Krypton.Toolkit.KryptonPanel();
            lblNewWireType = new Krypton.Toolkit.KryptonLabel();
            btnWireTypeDefSave = new Krypton.Toolkit.KryptonButton();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
            kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
            tbSupplier = new Krypton.Toolkit.KryptonTextBox();
            tbSupplierWireTypeCode = new Krypton.Toolkit.KryptonTextBox();
            tbWireTypeDescription = new Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).BeginInit();
            kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).BeginInit();
            kryptonPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // kryptonPanel1
            // 
            kryptonPanel1.Controls.Add(btnWireTypeDefSave);
            kryptonPanel1.Controls.Add(lblNewWireType);
            kryptonPanel1.Location = new Point(12, 12);
            kryptonPanel1.Name = "kryptonPanel1";
            kryptonPanel1.Size = new Size(776, 119);
            kryptonPanel1.TabIndex = 0;
            // 
            // kryptonPanel2
            // 
            kryptonPanel2.Controls.Add(tbWireTypeDescription);
            kryptonPanel2.Controls.Add(tbSupplierWireTypeCode);
            kryptonPanel2.Controls.Add(tbSupplier);
            kryptonPanel2.Controls.Add(kryptonLabel3);
            kryptonPanel2.Controls.Add(kryptonLabel2);
            kryptonPanel2.Controls.Add(kryptonLabel1);
            kryptonPanel2.Dock = DockStyle.Bottom;
            kryptonPanel2.Location = new Point(0, 145);
            kryptonPanel2.Name = "kryptonPanel2";
            kryptonPanel2.Size = new Size(800, 305);
            kryptonPanel2.TabIndex = 1;
            // 
            // lblNewWireType
            // 
            lblNewWireType.Location = new Point(3, 29);
            lblNewWireType.Name = "lblNewWireType";
            lblNewWireType.Size = new Size(93, 20);
            lblNewWireType.TabIndex = 0;
            lblNewWireType.Values.Text = "New Wire Type";
            // 
            // btnWireTypeDefSave
            // 
            btnWireTypeDefSave.Location = new Point(3, 68);
            btnWireTypeDefSave.Name = "btnWireTypeDefSave";
            btnWireTypeDefSave.Size = new Size(120, 48);
            btnWireTypeDefSave.TabIndex = 2;
            btnWireTypeDefSave.Values.Text = "Assign";
            btnWireTypeDefSave.Click += btnWireTypeDefSave_Click;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Location = new Point(42, 53);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(58, 20);
            kryptonLabel1.TabIndex = 1;
            kryptonLabel1.Values.Text = "Supplier:";
            // 
            // kryptonLabel2
            // 
            kryptonLabel2.Location = new Point(42, 123);
            kryptonLabel2.Name = "kryptonLabel2";
            kryptonLabel2.Size = new Size(148, 20);
            kryptonLabel2.TabIndex = 2;
            kryptonLabel2.Values.Text = "Supplier Wire Type Code:";
            // 
            // kryptonLabel3
            // 
            kryptonLabel3.Location = new Point(42, 194);
            kryptonLabel3.Name = "kryptonLabel3";
            kryptonLabel3.Size = new Size(133, 20);
            kryptonLabel3.TabIndex = 3;
            kryptonLabel3.Values.Text = "Wire Type Description:";
            // 
            // tbSupplier
            // 
            tbSupplier.Location = new Point(218, 50);
            tbSupplier.Name = "tbSupplier";
            tbSupplier.Size = new Size(544, 23);
            tbSupplier.TabIndex = 4;
            // 
            // tbSupplierWireTypeCode
            // 
            tbSupplierWireTypeCode.Location = new Point(218, 120);
            tbSupplierWireTypeCode.Name = "tbSupplierWireTypeCode";
            tbSupplierWireTypeCode.Size = new Size(544, 23);
            tbSupplierWireTypeCode.TabIndex = 5;
            // 
            // tbWireTypeDescription
            // 
            tbWireTypeDescription.Location = new Point(218, 191);
            tbWireTypeDescription.Name = "tbWireTypeDescription";
            tbWireTypeDescription.Size = new Size(544, 23);
            tbWireTypeDescription.TabIndex = 6;
            // 
            // WireTypeDefine
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(kryptonPanel2);
            Controls.Add(kryptonPanel1);
            Name = "WireTypeDefine";
            Text = "WireTypeDefine";
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).EndInit();
            kryptonPanel1.ResumeLayout(false);
            kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).EndInit();
            kryptonPanel2.ResumeLayout(false);
            kryptonPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonLabel lblNewWireType;
        private Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private Krypton.Toolkit.KryptonButton btnWireTypeDefSave;
        private Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonTextBox tbWireTypeDescription;
        private Krypton.Toolkit.KryptonTextBox tbSupplierWireTypeCode;
        private Krypton.Toolkit.KryptonTextBox tbSupplier;
        private Krypton.Toolkit.KryptonLabel kryptonLabel3;
    }
}
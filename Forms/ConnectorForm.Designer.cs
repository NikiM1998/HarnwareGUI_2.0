namespace HarnwareGUI.Forms
{
    partial class ConnectorForm
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
            pnlConFormControls = new Krypton.Toolkit.KryptonPanel();
            btnRemovePin = new Krypton.Toolkit.KryptonButton();
            btnAddPin = new Krypton.Toolkit.KryptonButton();
            lblConnectors = new Krypton.Toolkit.KryptonLabel();
            cbConnectors = new Krypton.Toolkit.KryptonComboBox();
            pnlConFormView = new Krypton.Toolkit.KryptonPanel();
            dgvConnectorsView = new Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)pnlConFormControls).BeginInit();
            pnlConFormControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cbConnectors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlConFormView).BeginInit();
            pnlConFormView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvConnectorsView).BeginInit();
            SuspendLayout();
            // 
            // pnlConFormControls
            // 
            pnlConFormControls.Controls.Add(btnRemovePin);
            pnlConFormControls.Controls.Add(btnAddPin);
            pnlConFormControls.Controls.Add(lblConnectors);
            pnlConFormControls.Controls.Add(cbConnectors);
            pnlConFormControls.Location = new Point(7, 6);
            pnlConFormControls.Name = "pnlConFormControls";
            pnlConFormControls.Size = new Size(786, 203);
            pnlConFormControls.TabIndex = 0;
            // 
            // btnRemovePin
            // 
            btnRemovePin.Location = new Point(664, 161);
            btnRemovePin.Name = "btnRemovePin";
            btnRemovePin.Size = new Size(117, 39);
            btnRemovePin.TabIndex = 5;
            btnRemovePin.Values.Text = "Remove Pin";
            btnRemovePin.Click += btnRemovePin_Click;
            // 
            // btnAddPin
            // 
            btnAddPin.Location = new Point(541, 161);
            btnAddPin.Name = "btnAddPin";
            btnAddPin.Size = new Size(117, 39);
            btnAddPin.TabIndex = 4;
            btnAddPin.Values.Text = "Add Pin";
            btnAddPin.Click += btnAddPin_Click;
            // 
            // lblConnectors
            // 
            lblConnectors.Location = new Point(5, 37);
            lblConnectors.Name = "lblConnectors";
            lblConnectors.Size = new Size(113, 20);
            lblConnectors.TabIndex = 3;
            lblConnectors.Values.Text = "Select a Connector";
            // 
            // cbConnectors
            // 
            cbConnectors.DropDownWidth = 769;
            cbConnectors.IntegralHeight = false;
            cbConnectors.Location = new Point(5, 63);
            cbConnectors.Name = "cbConnectors";
            cbConnectors.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365White;
            cbConnectors.Size = new Size(769, 21);
            cbConnectors.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cbConnectors.TabIndex = 2;
            cbConnectors.Text = "kryptonComboBox1";
            cbConnectors.SelectedIndexChanged += cbConnectors_SelectedIndexChanged;
            // 
            // pnlConFormView
            // 
            pnlConFormView.Controls.Add(dgvConnectorsView);
            pnlConFormView.Location = new Point(8, 222);
            pnlConFormView.Name = "pnlConFormView";
            pnlConFormView.Size = new Size(785, 224);
            pnlConFormView.TabIndex = 1;
            // 
            // dgvConnectorsView
            // 
            dgvConnectorsView.AllowUserToAddRows = false;
            dgvConnectorsView.BorderStyle = BorderStyle.None;
            dgvConnectorsView.Location = new Point(6, 6);
            dgvConnectorsView.Name = "dgvConnectorsView";
            dgvConnectorsView.Size = new Size(774, 214);
            dgvConnectorsView.TabIndex = 0;
            // 
            // ConnectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlConFormView);
            Controls.Add(pnlConFormControls);
            Name = "ConnectorForm";
            Text = "ConnectorForm";
            ((System.ComponentModel.ISupportInitialize)pnlConFormControls).EndInit();
            pnlConFormControls.ResumeLayout(false);
            pnlConFormControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cbConnectors).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlConFormView).EndInit();
            pnlConFormView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvConnectorsView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel pnlConFormControls;
        private Krypton.Toolkit.KryptonPanel pnlConFormView;
        private Krypton.Toolkit.KryptonButton btnAddPin;
        private Krypton.Toolkit.KryptonLabel lblConnectors;
        private Krypton.Toolkit.KryptonComboBox cbConnectors;
        private Krypton.Toolkit.KryptonDataGridView dgvConnectorsView;
        private Krypton.Toolkit.KryptonButton btnRemovePin;
    }
}
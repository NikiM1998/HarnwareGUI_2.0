namespace HarnwareGUI.Forms
{
    partial class PinForm
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
            pnlPinFormControls = new Krypton.Toolkit.KryptonPanel();
            btnDefinePin = new Krypton.Toolkit.KryptonButton();
            lblPins = new Krypton.Toolkit.KryptonLabel();
            cbPins = new Krypton.Toolkit.KryptonComboBox();
            dgvPinsView = new Krypton.Toolkit.KryptonDataGridView();
            pnlPinFormView = new Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)pnlPinFormControls).BeginInit();
            pnlPinFormControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cbPins).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPinsView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlPinFormView).BeginInit();
            pnlPinFormView.SuspendLayout();
            SuspendLayout();
            // 
            // pnlPinFormControls
            // 
            pnlPinFormControls.Controls.Add(btnDefinePin);
            pnlPinFormControls.Controls.Add(lblPins);
            pnlPinFormControls.Controls.Add(cbPins);
            pnlPinFormControls.Location = new Point(7, 5);
            pnlPinFormControls.Name = "pnlPinFormControls";
            pnlPinFormControls.Size = new Size(786, 203);
            pnlPinFormControls.TabIndex = 2;
            // 
            // btnDefinePin
            // 
            btnDefinePin.Location = new Point(666, 161);
            btnDefinePin.Name = "btnDefinePin";
            btnDefinePin.Size = new Size(117, 39);
            btnDefinePin.TabIndex = 6;
            btnDefinePin.Values.Text = "Define Pin";
            btnDefinePin.Click += btnDefinePin_Click;
            // 
            // lblPins
            // 
            lblPins.Location = new Point(5, 37);
            lblPins.Name = "lblPins";
            lblPins.Size = new Size(73, 20);
            lblPins.TabIndex = 3;
            lblPins.Values.Text = "Select a Pin";
            // 
            // cbPins
            // 
            cbPins.DropDownWidth = 769;
            cbPins.IntegralHeight = false;
            cbPins.Location = new Point(5, 63);
            cbPins.Name = "cbPins";
            cbPins.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365White;
            cbPins.Size = new Size(769, 21);
            cbPins.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cbPins.TabIndex = 2;
            cbPins.SelectedIndexChanged += cbPins_SelectedIndexChanged;
            // 
            // dgvPinsView
            // 
            dgvPinsView.AllowUserToAddRows = false;
            dgvPinsView.BorderStyle = BorderStyle.None;
            dgvPinsView.Location = new Point(6, 6);
            dgvPinsView.Name = "dgvPinsView";
            dgvPinsView.Size = new Size(774, 214);
            dgvPinsView.TabIndex = 0;
            // 
            // pnlPinFormView
            // 
            pnlPinFormView.Controls.Add(dgvPinsView);
            pnlPinFormView.Location = new Point(8, 221);
            pnlPinFormView.Name = "pnlPinFormView";
            pnlPinFormView.Size = new Size(785, 224);
            pnlPinFormView.TabIndex = 3;
            // 
            // PinForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlPinFormControls);
            Controls.Add(pnlPinFormView);
            Name = "PinForm";
            Text = "PinForm";
            ((System.ComponentModel.ISupportInitialize)pnlPinFormControls).EndInit();
            pnlPinFormControls.ResumeLayout(false);
            pnlPinFormControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cbPins).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPinsView).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlPinFormView).EndInit();
            pnlPinFormView.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel pnlPinFormControls;
        private Krypton.Toolkit.KryptonLabel lblPins;
        private Krypton.Toolkit.KryptonComboBox cbPins;
        private Krypton.Toolkit.KryptonDataGridView dgvPinsView;
        private Krypton.Toolkit.KryptonPanel pnlPinFormView;
        private Krypton.Toolkit.KryptonButton btnDefinePin;
    }
}
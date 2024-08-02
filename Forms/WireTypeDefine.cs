using Krypton.Toolkit;
using Npgsql;

namespace HarnwareGUI.Forms
{
    public partial class WireTypeDefine : KryptonForm
    {
        public WireTypeDefine()
        {
            InitializeComponent();
        }

        private void btnWireTypeDefSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSupplier.Text)
                || string.IsNullOrWhiteSpace(tbSupplierWireTypeCode.Text)
                || string.IsNullOrWhiteSpace(tbWireTypeDescription.Text)) // checking that the textboxes are not empty
            {
                MessageBox.Show("Please fill in all the input fields before saving", "ERROR");
                return;
            }

            //save to database
            try
            {
                using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO gui_wire_type (supplier, supplier_wire_type_code, wire_type_description) VALUES (@supplier, @wire_type_code, @wire_type_desc)";
                        cmd.Parameters.AddWithValue("supplier", tbSupplier.Text);
                        cmd.Parameters.AddWithValue("wire_type_code", tbSupplierWireTypeCode.Text);
                        cmd.Parameters.AddWithValue("wire_type_desc", tbWireTypeDescription.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Data Saved Successfully", "Info");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
            }
        }
    }
}

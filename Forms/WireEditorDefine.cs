using Krypton.Toolkit;
using Npgsql;

namespace HarnwareGUI.Forms
{
    public partial class WireEditorDefine : KryptonForm
    {
        public WireEditorDefine()
        {
            InitializeComponent();
        }

        private void btnSaveWireEditor_Click(object sender, EventArgs e)
        {
            decimal coreDiameter, wireDiameter;
            int cores;

            if (string.IsNullOrWhiteSpace(tbSupplierPartNum.Text)
                || string.IsNullOrWhiteSpace(tbWireType.Text)
                || string.IsNullOrWhiteSpace(tbCores.Text)
                || string.IsNullOrWhiteSpace(tbCoreDiameter.Text)
                || string.IsNullOrWhiteSpace(tbWireDiameter.Text)) // checking that the textboxes are not empty
            {
                MessageBox.Show("Please fill in all the input fields before saving", "ERROR");
                return;
            }

            if (!decimal.TryParse(tbCoreDiameter.Text, out coreDiameter))
            {
                MessageBox.Show("Invalid value for core diameter. Please enter a numeric value.", "ERROR");
                return;
            }

            if (!decimal.TryParse(tbWireDiameter.Text, out wireDiameter))
            {
                MessageBox.Show("Invalid value for wire diameter. Please enter a numeric value.", "ERROR");
                return;
            }

            if (!int.TryParse(tbWireDiameter.Text, out cores))
            {
                MessageBox.Show("Invalid value for cores. Please enter a numeric value.", "ERROR");
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
                        cmd.CommandText = "INSERT INTO gui_wire_editor (supplier_part_number, wire_type, cores, core_diameter_mm, wire_diameter_mm) VALUES (@supplier_part_num, @wire_type, @cores, @core_diameter, @wire_diameter)";
                        cmd.Parameters.AddWithValue("supplier_part_num", tbSupplierPartNum.Text);
                        cmd.Parameters.AddWithValue("wire_type", tbWireType.Text);
                        cmd.Parameters.AddWithValue("cores", cores);
                        cmd.Parameters.AddWithValue("core_diameter", coreDiameter);
                        cmd.Parameters.AddWithValue("wire_diameter", wireDiameter);
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

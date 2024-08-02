using Krypton.Toolkit;
using Npgsql;
using System.Data;

namespace HarnwareGUI.Forms
{
    public partial class WireType : KryptonForm
    {
        private int _id; // Field to store the ID

        public WireType(int id)
        {
            InitializeComponent();
            PopulateDgv();
            _id = id; // Store the ID in the field
        }

        private void PopulateDgv()
        {
            dgvWireType.DataSource = null;
            dgvWireType.Columns.Clear();

            try
            {
                using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM gui_wire_type order by id desc";

                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvWireType.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
            }
        }

        private void btnWireTypeAssign_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvWireType.SelectedRows.Count > 0)
            {
                // Get the ID of the selected row
                int selectedId = Convert.ToInt32(dgvWireType.SelectedRows[0].Cells["id"].Value);

                try
                {
                    using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                    {
                        conn.Open();

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            // Update the wire_type column in the gui_wires table for the ID that was passed to the form
                            cmd.CommandText = "UPDATE gui_wires SET wire_type_id = @wire_type_id WHERE id = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@wire_type_id", selectedId);
                            cmd.Parameters.AddWithValue("@id", _id); // Assuming 'id' is the ID that was passed to the form

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
                }
                MessageBox.Show("Successfully assigned wire type.", "Info");
                this.Close();
            }
        }

        private void btnWireTypeDefine_Click(object sender, EventArgs e)
        {
            WireTypeDefine wireTypeDefineForm = new WireTypeDefine();
            wireTypeDefineForm.ShowDialog();
            PopulateDgv();
        }
    }
}

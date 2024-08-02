using HarnwareGUI.Helpers;
using HarnwareGUI.Services;
using Krypton.Toolkit;
using Npgsql;
using System.Data;

namespace HarnwareGUI.Forms
{
    public partial class PinForm : KryptonForm
    {
        private OdooDatabaseService _odooDatabaseService;
        DataTable parts = null;

        public PinForm()
        {
            InitializeComponent();
            this.Load += PinForm_Load;
        }

        private void PinForm_Load(object sender, EventArgs e)
        {
            if (RoleManager.Role != "Viewer") // enable grid editing
            {
                dgvPinsView.ReadOnly = false;
            }
            else
            {
                dgvPinsView.ReadOnly = true;
                MessageBox.Show("You do not have permission to edit this form.");
            }

            _odooDatabaseService = new OdooDatabaseService(Globals.ConnectionString);

            string query = "SELECT harn_item, MIN(id) AS id, odoo_item FROM gui_part_link WHERE part_type = 'Pin' GROUP BY harn_item, odoo_item ORDER BY id DESC;";
            parts = _odooDatabaseService.ExecuteQuery(query, new Dictionary<string, object>());

            List<string> harnItemsForComboBox = new List<string>();

            foreach (DataRow part in parts.Rows)
            {
                int partId = Convert.ToInt32(part["id"]);
                string checkQuery = "SELECT COUNT(*) FROM gui_pins WHERE gui_part_link_id = @PartId";
                var queryParams = new Dictionary<string, object> { { "@PartId", partId } };
                object result = _odooDatabaseService.ExecuteScalar(checkQuery, queryParams);
                int count = Convert.ToInt32(result);

                if (count == 0) // If the id is not found in the gui_connectors table, this means the part is not yet added to the connectors table
                {
                    harnItemsForComboBox.Add(part["harn_item"].ToString());
                }
            }

            // Assuming comboBoxHarnItems is your ComboBox
            cbPins.DataSource = harnItemsForComboBox;

            string columnQuery = "SELECT * FROM gui_pins WHERE 1=0"; // A query that returns no rows, but includes the column names
            using (var connection = new NpgsqlConnection(Globals.ConnectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(columnQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        var schemaTable = reader.GetSchemaTable();
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string columnName = row["ColumnName"].ToString();
                            if (columnName != "id") // Skip the id column
                            {
                                dgvPinsView.Columns.Add(columnName, columnName);
                            }
                        }
                    }
                }
            }
        }

        public DataTable GetPinData()
        {
            DataTable dataTable = new DataTable();

            foreach (DataGridViewColumn column in dgvPinsView.Columns)
            {
                if (column.Name != "is_approved")
                {
                    dataTable.Columns.Add(column.Name, typeof(string));
                }
                else
                {
                    dataTable.Columns.Add(column.Name, typeof(bool));
                }
            }

            // Iterate through DataGridView rows and add them to the DataTable
            foreach (DataGridViewRow row in dgvPinsView.Rows)
            {
                DataRow dataRow = dataTable.NewRow();

                // Iterate through DataGridView columns to fill the row
                for (int i = 0; i < dgvPinsView.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value ?? DBNull.Value; // Use DBNull.Value for null values
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private void btnDefinePin_Click(object sender, EventArgs e)
        {
            if (dgvPinsView != null && dgvPinsView.Rows.Count > 0)
            {
                return;
            }

            int index = dgvPinsView.Rows.Add(); // Adds a new row and returns the index of the new row
            DataGridViewRow newRow = dgvPinsView.Rows[index];

            // Find the DataRow in parts that matches the selected item in cbConnectors
            string selectedItem = cbPins.SelectedItem.ToString();
            DataRow selectedPart = parts.AsEnumerable()
                                        .FirstOrDefault(row => row.Field<string>("harn_item") == selectedItem);

            if (selectedPart != null)
            {
                // Extract the id from that DataRow
                int partId = selectedPart.Field<int>("id");

                if (dgvPinsView.Columns.Contains("gui_part_link_id"))
                {
                    // Set the value of the gui_part_link_id column in the new row to this id
                    newRow.Cells["gui_part_link_id"].Value = partId;
                }
                else
                {
                    MessageBox.Show("Column 'gui_part_link_id' does not exist in dgvPinsView.");
                }
            }
            else
            {
                MessageBox.Show("Selected item does not match any part.");
            }
        }

        private void cbPins_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = dgvPinsView.Rows.Count - 1; i >= 0; i--)
            {
                dgvPinsView.Rows.RemoveAt(i);
            }
        }
    }
}

using HarnwareGUI.Helpers;
using HarnwareGUI.Services;
using Krypton.Toolkit;
using Npgsql;
using System.Data;

namespace HarnwareGUI.Forms
{
    public partial class ConnectorForm : KryptonForm
    {
        private OdooDatabaseService _odooDatabaseService;
        DataTable parts = null;

        public ConnectorForm()
        {
            InitializeComponent();
            this.Load += ConnectorForm_Load;
        }

        private void ConnectorForm_Load(object sender, EventArgs e)
        {
            if (RoleManager.Role != "Viewer") // enable grid editing
            {
                dgvConnectorsView.ReadOnly = false;
            }
            else
            {
                dgvConnectorsView.ReadOnly = true;
                MessageBox.Show("You do not have permission to edit this form.");
            }

            _odooDatabaseService = new OdooDatabaseService(Globals.ConnectionString);

            string query = "SELECT harn_item, MIN(id) AS id, odoo_item FROM gui_part_link WHERE part_type = 'Con' GROUP BY harn_item, odoo_item ORDER BY id DESC;";
            parts = _odooDatabaseService.ExecuteQuery(query, new Dictionary<string, object>());

            List<string> harnItemsForComboBox = new List<string>();

            foreach (DataRow part in parts.Rows)
            {
                int partId = Convert.ToInt32(part["id"]);
                string checkQuery = "SELECT COUNT(*) FROM gui_connectors WHERE gui_part_link_id = @PartId";
                var queryParams = new Dictionary<string, object> { { "@PartId", partId } };
                object result = _odooDatabaseService.ExecuteScalar(checkQuery, queryParams);
                int count = Convert.ToInt32(result);

                if (count == 0) // If the id is not found in the gui_connectors table, this means the part is not yet added to the connectors table
                {
                    harnItemsForComboBox.Add(part["harn_item"].ToString());
                }
            }

            // Assuming comboBoxHarnItems is your ComboBox
            cbConnectors.DataSource = harnItemsForComboBox;

            string columnQuery = "SELECT * FROM gui_connectors WHERE 1=0"; // A query that returns no rows, but includes the column names
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
                            // Skip the id column and odoo_pin_name column
                            if (columnName != "id" && columnName != "odoo_pin_name")
                            {
                                dgvConnectorsView.Columns.Add(columnName, columnName);
                            }
                        }
                    }
                }
            }

            // Manually add the odoo_pin_name column as a DataGridViewComboBoxColumn
            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "odoo_pin_name",
                HeaderText = "Odoo Pin Name",
                // You can set the DataSource, DisplayMember, and ValueMember properties here if you have the data available
                // Otherwise, you can set them when adding rows or based on some other events
            };
            dgvConnectorsView.Columns.Add(comboBoxColumn);
        }

        public DataTable GetConnectorData()
        {
            DataTable dataTable = new DataTable();

            // Add columns to the DataTable. Assuming dgvConnectorsView columns are not dynamically changing.
            foreach (DataGridViewColumn column in dgvConnectorsView.Columns)
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
            foreach (DataGridViewRow row in dgvConnectorsView.Rows)
            {
                DataRow dataRow = dataTable.NewRow();

                // Iterate through DataGridView columns to fill the row
                for (int i = 0; i < dgvConnectorsView.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value ?? DBNull.Value; // Use DBNull.Value for null values
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private void btnAddPin_Click(object sender, EventArgs e)
        {
            int index = dgvConnectorsView.Rows.Add(); // Adds a new row and returns the index of the new row
            DataGridViewRow newRow = dgvConnectorsView.Rows[index];

            // Find the DataRow in parts that matches the selected item in cbConnectors
            string selectedItem = cbConnectors.SelectedItem.ToString();
            DataRow selectedPart = parts.AsEnumerable()
                                        .FirstOrDefault(row => row.Field<string>("harn_item") == selectedItem);

            if (selectedPart != null)
            {
                // Extract the id from that DataRow
                int partId = selectedPart.Field<int>("id");

                if (dgvConnectorsView.Columns.Contains("gui_part_link_id"))
                {
                    // Set the value of the gui_part_link_id column in the new row to this id
                    newRow.Cells["gui_part_link_id"].Value = partId;
                }
                else
                {
                    MessageBox.Show("Column 'gui_part_link_id' does not exist in dgvConnectorsView.");
                    return;
                }

                if (dgvConnectorsView.Columns["odoo_pin_name"] is DataGridViewComboBoxColumn)
                {
                    // Execute the provided query to fetch the items for the ComboBox
                    string query = @"SELECT gui_pins.gui_part_link_id, gui_part_link.harn_item 
                             FROM gui_pins 
                             JOIN gui_part_link ON gui_pins.gui_part_link_id = gui_part_link.id ";
                    DataTable comboBoxItems = _odooDatabaseService.ExecuteQuery(query, new Dictionary<string, object>());

                    List<string> items = new List<string>();
                    foreach (DataRow row in comboBoxItems.Rows)
                    {
                        items.Add(row["harn_item"].ToString()); // Assuming you want to display harn_item in the ComboBox
                    }

                    // Set the cell's Value to the first item if you want a default value or handle it as needed
                    DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)newRow.Cells["odoo_pin_name"];
                    comboBoxCell.DataSource = items;
                    if (items.Count > 0)
                    {
                        comboBoxCell.Value = items[0];
                    }
                }
            }
            else
            {
                MessageBox.Show("Selected item does not match any part.");
            }
        }

        private void btnRemovePin_Click(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dgvConnectorsView.SelectedRows.Count > 0)
            {
                // Assuming multi-select is disabled, so only one row can be selected at a time
                DataGridViewRow selectedRow = dgvConnectorsView.SelectedRows[0];
                dgvConnectorsView.Rows.Remove(selectedRow);
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void cbConnectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = dgvConnectorsView.Rows.Count - 1; i >= 0; i--)
            {
                dgvConnectorsView.Rows.RemoveAt(i);
            }
        }
    }
}

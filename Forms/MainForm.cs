using HarnwareGUI.Helpers;
using HarnwareGUI.Services;
using Krypton.Toolkit;
using Npgsql;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace HarnwareGUI.Forms
{
    public partial class MainForm : KryptonForm
    {
        private OdooDatabaseService _odooDatabaseService;
        private DataTable partLinkTable = null;
        private DataTable originalPartLinkTable = null;
        private DataTable connectorsTable = null;
        private DataTable pinsTable = null;

        private List<(string labelName, string controlName)> drawingInfoOrder;
        private List<(string labelName, string controlName)> stateOrder;
        private List<(string labelName, string controlName)> revisionOrder;
        private List<(string labelName, string controlName)> approveOrder;
        private List<(string labelName, string controlName)> importsOrder;

        private Dictionary<string, List<string>> OdooItems = new Dictionary<string, List<string>>();
        private List<string> OdooWires = new List<string>();
        private List<string> OdooConnectors = new List<string>();
        private List<string> OdooPins = new List<string>();
        List<int> wireIds = new List<int>();

        private bool SearchClicked = false, updateClicked = false, saveClicked = false;
        private bool updatePartsClicked = false;
        string currentUserRole = "", currentUser = "";
        private string fileNameOriginal;
        private string fileNameVisio;
        private string fileNameHarnware;

        int insertedHarnessId = -1;
        int markerHarnessId = -1;

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;

            // if dynamic resizing of form elements is required, uncomment the following line
            SetupLayout();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtbxOdooPart.Enabled = false;
            btnSave.Enabled = false;

            _odooDatabaseService = new OdooDatabaseService(Globals.ConnectionString);

            GetOdooItems();
            OdooWires = OdooItems.Where(kvp => kvp.Value.Any(value => value.IndexOf("wire", StringComparison.OrdinalIgnoreCase) >= 0))
                                        .Select(kvp => kvp.Key)
                                        .ToList();
            PopulateMarkersDrawings();

            currentUser = RoleManager.User;
            currentUserRole = RoleManager.Role;

            // check the user's UserRole and enable editing if necessary
            if (currentUserRole != null && currentUserRole != "user")
            {
                // needs to be implemented still.
            }

            if (cbUserToApprove.Items.Count <= 0)
            {
                foreach (string username in Globals.AllUsers)
                {
                    cbUserToApprove.Items.Add(username);
                }
            }

            lblUserLoggedIn.Text = "Logged in as: " + currentUser;

            SetFontSize(this.Controls, 8); // Change to your desired font size

            // Iterate over all controls in the form
            foreach (Control control in this.Controls)
            {
                // Check if the control is a DataGridView
                if (control is KryptonDataGridView dgv)
                {
                    // Prevent the automatic addition of a new row
                    dgv.AllowUserToAddRows = false;
                }

                if (control is KryptonComboBox cb)
                {
                    cb.Enabled = false;
                }
            }

            dgvWires.CellFormatting += dgvWires_CellFormatting;
            dgvWires.CellContentClick += dgvWires_CellContentClick;
        }

        private DataTable GetMarkerTemplates()
        {
            var queryParameters = new Dictionary<string, object>();
            string query = "SELECT radel_part_number, min_diameter_mm, max_diameter_mm FROM gui_marker_templates;";
            return _odooDatabaseService.ExecuteQuery(query, queryParameters);
        }

        private List<string> FilterMarkersByDiameter(DataTable markerTemplates, double diameter)
        {
            List<string> filteredMarkers = new List<string>();

            foreach (DataRow row in markerTemplates.Rows)
            {
                if (double.TryParse(row["min_diameter_mm"].ToString(), out double minDiameter) &&
                    double.TryParse(row["max_diameter_mm"].ToString(), out double maxDiameter))
                {
                    if (diameter >= minDiameter && diameter <= maxDiameter)
                    {
                        filteredMarkers.Add(row["radel_part_number"].ToString());
                    }
                }
            }

            return filteredMarkers;
        }

        private void PopulateMarkersDrawings()
        {
            cbSelectDrawing.DataSource = null;

            var queryParameters = new Dictionary<string, object>();
            string query = "select distinct odoopart from harness";
            DataTable results = _odooDatabaseService.ExecuteQuery(query, new Dictionary<string, object>());

            if (results != null && results.Rows.Count > 0)
            {
                cbSelectDrawing.DataSource = results;
                cbSelectDrawing.DisplayMember = "odoopart";
                cbSelectDrawing.ValueMember = "odoopart";
            }
            else
            {
                cbSelectDrawing.DataSource = null;
            }
        }

        private List<string> GetPins()
        {
            List<string> supplierPartNumbers = new List<string>();

            var queryParameters = new Dictionary<string, object>();
            string query = "SELECT DISTINCT supplier_part_number FROM gui_pins;";

            DataTable results = _odooDatabaseService.ExecuteQuery(query, queryParameters);

            if (results != null && results.Rows.Count > 0)
            {
                foreach (DataRow row in results.Rows)
                {
                    supplierPartNumbers.Add(row["supplier_part_number"].ToString());
                }
            }

            return supplierPartNumbers;
        }

        private List<string> GetConnectors()
        {
            List<string> connectors = new List<string>();

            string query = "SELECT DISTINCT supplier_part_number FROM gui_connectors;";

            DataTable results = _odooDatabaseService.ExecuteQuery(query, null);

            if (results != null && results.Rows.Count > 0)
            {
                foreach (DataRow row in results.Rows)
                {
                    connectors.Add(row["supplier_part_number"].ToString());
                }
            }

            return connectors;
        }

        private void dgvWires_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if this is the cell you want to change.
            if (dgvWires.Columns[e.ColumnIndex].Name == "wire_type_id" || dgvWires.Columns[e.ColumnIndex].Name == "wire_editor_id")
            {
                if (e.Value != null && e.Value.ToString() == "-1")
                {
                    // Set the value of the cell in the correct column to "Link".
                    dgvWires["wire_type_link", e.RowIndex].Value = "Link";
                    dgvWires["wire_editor_link", e.RowIndex].Value = "Link";
                }
            }
        }

        private void dgvWires_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                // Get the ID value of the row
                int id = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells["id"].Value);

                // Check if the clicked cell is in the "wire_type_link" column
                if (senderGrid.Columns[e.ColumnIndex].Name == "wire_type_link")
                {
                    WireType wireTypeForm = new WireType(id);
                    wireTypeForm.ShowDialog();
                    //re-populate the datagridview
                    PopulateWiresDgv();
                }

                // Check if the clicked cell is in the "wire_editor_link" column
                if (senderGrid.Columns[e.ColumnIndex].Name == "wire_editor_link")
                {
                    WireEditor wireEditorForm = new WireEditor(id);
                    wireEditorForm.ShowDialog();
                    //re-populate the datagridview
                    PopulateWiresDgv();
                }
            }
        }

        private void SetFontSize(Control.ControlCollection controls, float fontSize)
        {
            foreach (Control control in controls)
            {
                control.Font = new Font(control.Font.FontFamily, fontSize, control.Font.Style);
                if (control.HasChildren)
                {
                    SetFontSize(control.Controls, fontSize);
                }
            }
        }

        private void GetOdooItems()
        {
            Dictionary<string, List<string>> Items = new Dictionary<string, List<string>>();

            try
            {
                using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();
                    string query = "select \"Item\" as item, \"Description\" as description from gui_odoo_items order by item asc";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string key = reader["item"] as string;
                                string value = reader["description"] as string;
                                if (key != null && value != null)
                                {
                                    if (!OdooItems.ContainsKey(key))
                                    {
                                        OdooItems[key] = new List<string>();
                                    }
                                    OdooItems[key].Add(value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Form1.GetOdooItems\n" + ex.Message, "ERROR");
            }
        }

        private void PopulateHarnessInfo()
        {
            // Assuming cbOdooPart is the ComboBox and its Text property contains the selected key.
            string selectedKey = txtbxOdooPart.Text;

            // Check if the selected key exists in the OdooItems dictionary.
            if (OdooItems.ContainsKey(selectedKey))
            {
                // Retrieve the list of descriptions associated with the selected key.
                List<string> descriptions = OdooItems[selectedKey];

                string pattern = @"^(.*?) \(.*?(\d+).*?REV \s*([a-zA-Z\d]+).*";

                foreach (string description in descriptions)
                {
                    Match match = Regex.Match(description, pattern, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        // Extracting the matched groups
                        txtbxOdooDescription.Text = match.Groups[1].Value.Trim();
                        txtbxClientDescription.Text = match.Groups[1].Value.Trim();
                        txtbxClientPartNumber.Text = match.Groups[2].Value.Trim();
                        // Extracting only the digit part from the revision (match.Groups[3])
                        string revisionText = match.Groups[3].Value.Trim();
                        txtbxClientRevision.Text = revisionText; // This will contain only the 
                    }
                    else
                    {
                        // Handle the case where the description format does not match the expected pattern.
                        MessageBox.Show($"The description format does not match the expected pattern: {description}", "Information");
                    }
                }
            }
            else
            {
                // Handle the case where the selected key is not found in the dictionary.
                MessageBox.Show($"No descriptions found for the selected item: {selectedKey}", "Information");
            }

            PopulateOdooRevision(selectedKey);
        }

        private void PopulateOdooRevision(string OdooItem)
        {
            var queryParameters = new Dictionary<string, object>
            {
                { "@odoopart", OdooItem }
            };

            try
            {
                string query = "SELECT odoorev FROM harness WHERE odoopart = @odoopart;";

                DataTable result = _odooDatabaseService.ExecuteQuery(query, queryParameters);

                if (result.Rows.Count > 0)
                {
                    txtbxOdooRevision.Text = result.Rows[0]["odoorev"].ToString();
                }
                else // there is no item in the database, must be the first Odoo Revision. We take the Client revision and append it with .0 for the first revision.
                {
                    txtbxOdooRevision.Text = txtbxClientRevision.Text + ".0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving Odoo revision: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateFormInputs()
        {
            bool allInputsValid = true;
            StringBuilder errorMessage = new StringBuilder("Please fill in the following fields:\n");

            // Assuming the Import tab is named "tabImport"
            foreach (Control control in tabImport.Controls)
            {
                if (control is KryptonPanel pnl)
                {
                    foreach (Control cntrl in pnl.Controls)
                    {
                        if (cntrl is KryptonTextBox textBox && string.IsNullOrWhiteSpace(textBox.Text)) //looping through all the textboxes in the panels on the Import tab
                        {
                            if (textBox.Name == "txtbxApprovedBy") // No one has approved a new harness yet, no need for input here.
                            {
                                continue;
                            }
                            else
                            {
                                allInputsValid = false;
                                errorMessage.AppendLine($"- {textBox.Name}");
                            }
                        }
                    }
                }
            }

            if (!allInputsValid)
            {
                MessageBox.Show(errorMessage.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return allInputsValid;
        }

        private void ParseVisioFileAndStoreInDb(string filePath)
        {
            using (var conn = new NpgsqlConnection(Globals.ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("Shape Text:"))
                            {
                                string[] words = new string[2];
                                Match match = Regex.Match(line, @"(^\D+)(.*)");
                                if (match.Success)
                                {
                                    words[0] = match.Groups[1].Value;
                                    words[1] = match.Groups[2].Value;
                                }

                                wireIds.Clear();

                                // Check if the line contains any of the exclude words. 
                                if (!String.IsNullOrEmpty(words[1]) && !line.Contains("RECORDS", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] valuesToInsert = words[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Skip(3).ToArray();

                                    // Now you can insert valuesToInsert into your database
                                    cmd.CommandText = "INSERT INTO raw_cutting_list_visio (conna, connb, wiretype, wirediameter, wirelength, drawing, code, radel_rev, client_rev) VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value8, @value9, @value10) RETURNING id";

                                    for (int i = 0; i < valuesToInsert.Length; i++)
                                    {
                                        cmd.Parameters.AddWithValue($"value{i + 1}", valuesToInsert[i]);
                                        if (i == 2)
                                        {
                                            string wireCrimp = valuesToInsert[i];

                                            // Create a new command for the query
                                            using (var cmd2 = new NpgsqlCommand("SELECT code FROM data_wire WHERE wire = @crimp", conn))
                                            {
                                                cmd2.Parameters.AddWithValue("@crimp", wireCrimp);

                                                // Execute the query and retrieve the result
                                                var result = cmd2.ExecuteScalar();
                                                cmd.Parameters.AddWithValue($"value{8}", result);
                                            }
                                        }
                                    }
                                    cmd.Parameters.AddWithValue($"value{6}", txtbxOdooPart.Text.ToString());
                                    if (double.TryParse(txtbxOdooRevision.Text, out double revisionValue))
                                    {
                                        cmd.Parameters.AddWithValue($"value{9}", revisionValue);
                                    }
                                    else
                                    {

                                        throw new InvalidOperationException("The text in txtbxOdooRevision is not a valid double.");
                                    }
                                    if (int.TryParse(txtbxClientRevision.Text, out int clientRevisionValue))
                                    {
                                        cmd.Parameters.AddWithValue($"value{10}", clientRevisionValue);
                                    }
                                    else
                                    {
                                        // Handle the case where the text is not a valid int
                                        // For example, you might want to log an error or throw an exception
                                        throw new InvalidOperationException("The text in txtbxClientRevision is not a valid int.");
                                    }

                                    // Execute the command and retrieve the inserted row's ID
                                    var insertedId = cmd.ExecuteScalar();
                                    // Assuming ids is a List<int> to store the inserted IDs
                                    wireIds.Add(Convert.ToInt32(insertedId));

                                    cmd.Parameters.Clear();
                                }
                            }
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    // Copy data from raw_cutting_list_visio to gui_wires and return the IDs
                    cmd.CommandText = @"
                                        WITH inserted AS (
                                            INSERT INTO gui_wires (con_a, con_b, wire_length, wire_type_id, wire_editor_id, harness_id) 
                                            SELECT conna, connb, wirelength, -1, -1, @insertedId FROM raw_cutting_list_visio
                                            RETURNING id
                                        )
                                        SELECT id FROM inserted";

                    // Add the parameter to the command
                    cmd.Parameters.AddWithValue("@insertedId", insertedHarnessId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            wireIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        private void ParseHarnwareFileAndStoreInDb(string filePath)
        {
            using (var conn = new NpgsqlConnection(Globals.ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        string[] values = line.Split(',');
                        //if (values.Count() != 19)
                        //{
                        //    Console.WriteLine("Invalid line: " + line + ". Import failed.");
                        //    return;
                        //}
                        string laser = values[2] + "-" + values[4] + "<" + values[6] + "-" + values[8] + ">" + values[9] + "-" + values[11];
                        values[1] = Regex.Replace(values[1], "(?i)awg", "").Trim();

                        cmd.CommandText = "INSERT INTO raw_cutting_list_harnware (colour, awg, conna, conna_part, pina, typeconna, bundlea, twista, signala, connb, connb_part, pinb, typeconnb, bundleb, twistb, signalb) VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8, @value9, @value10, @value11, @value12, @value13, @value14, @value15, @value16) returning id";

                        for (int i = 0; i < values.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"value{i + 1}", values[i]);
                        }

                        int newId = (int)cmd.ExecuteScalar();

                        cmd.Parameters.Clear();

                        string awg = values[1];
                        //mapping awg reading from file to a col name in the data_connectors table
                        string awgCol = "";
                        if (awg == "2") { awgCol = "awg2"; }
                        if (awg == "4") { awgCol = "awg4"; }
                        if (awg == "6") { awgCol = "awg6"; }
                        if (awg == "8") { awgCol = "awg8"; }
                        if (awg == "10") { awgCol = "awg10"; }
                        if (awg == "12") { awgCol = "awg12"; }
                        if (awg == "14") { awgCol = "awg14"; }
                        if (awg == "16") { awgCol = "awg16"; }
                        if (awg == "18") { awgCol = "awg18"; }
                        if (awg == "20") { awgCol = "awg20"; }
                        if (awg == "22") { awgCol = "awg22"; }
                        if (awg == "24") { awgCol = "awg24"; }
                        if (awg == "26") { awgCol = "awg26"; }
                        if (awg == "28") { awgCol = "awg28"; }
                        //end of mapping
                        string conna_part = values[3];
                        string connb_part = values[10];

                        // Query to fetch the values for pina_part
                        cmd.CommandText = $"SELECT {awgCol} FROM data_connectors WHERE connector = @conna_part";
                        cmd.Parameters.AddWithValue("@conna_part", conna_part);

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        string pina_part = null;
                        string pinb_part = null;

                        if (reader.Read())
                        {
                            pina_part = reader.GetString(0);
                        }

                        reader.Close();
                        cmd.Parameters.Clear();

                        // Query to fetch the values for pinb_part
                        cmd.CommandText = $"SELECT {awgCol} FROM data_connectors WHERE connector = @connb_part";
                        cmd.Parameters.AddWithValue("@connb_part", connb_part);

                        NpgsqlDataReader readerB = cmd.ExecuteReader();

                        if (readerB.Read())
                        {
                            pinb_part = readerB.GetString(0);
                        }

                        readerB.Close();
                        cmd.Parameters.Clear();

                        //create new query to update pina_part and pinb_part
                        if (pina_part != null && pinb_part != null)
                        {
                            cmd.CommandText = "UPDATE raw_cutting_list_harnware SET pina_part = @value1, pinb_part = @value2, drawing = @value4, laser = @value5, radel_rev = @value6, client_rev = @value7 WHERE id = @id";
                            cmd.Parameters.AddWithValue("@value1", pina_part);
                            cmd.Parameters.AddWithValue("@value2", pinb_part);
                            cmd.Parameters.AddWithValue("@value4", txtbxOdooPart.Text.ToString());
                            cmd.Parameters.AddWithValue("@value5", laser);

                            if (double.TryParse(txtbxOdooRevision.Text, out double revisionValue))
                            {
                                cmd.Parameters.AddWithValue("@value6", revisionValue);
                            }
                            else
                            {
                                // Handle the case where the text is not a valid double
                                throw new InvalidOperationException("The text in txtbxOdooRevision is not a valid double.");
                            }

                            if (int.TryParse(txtbxClientRevision.Text, out int clientRevisionValue))
                            {
                                cmd.Parameters.AddWithValue("@value7", clientRevisionValue);
                            }
                            else
                            {
                                // Handle the case where the text is not a valid int
                                throw new InvalidOperationException("The text in txtbxClientRevision is not a valid int.");
                            }

                            cmd.Parameters.AddWithValue("@id", newId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }
                }

                // Use the returned IDs to update the corresponding rows in gui_wires
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    foreach (var id in wireIds)
                    {
                        // Update the row with the given ID
                        cmd.CommandText = @"
                             UPDATE gui_wires 
                             SET pin_a = (SELECT pina FROM raw_cutting_list_harnware WHERE id = @id),
                                 pin_b = (SELECT pinb FROM raw_cutting_list_harnware WHERE id = @id),
                                 cona_type = (SELECT typeconna FROM raw_cutting_list_harnware WHERE id = @id),
                                 conb_type = (SELECT typeconnb FROM raw_cutting_list_harnware WHERE id = @id),
                                 signal_name = (SELECT signala FROM raw_cutting_list_harnware WHERE id = @id),
                                 bundle = (SELECT bundlea FROM raw_cutting_list_harnware WHERE id = @id),
                                 twist_a = (SELECT twista FROM raw_cutting_list_harnware WHERE id = @id),
                                 twist_b = (SELECT twistb FROM raw_cutting_list_harnware WHERE id = @id)
                             WHERE id = @id";

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }

                try
                {
                    // Use the returned IDs to update the corresponding rows in gui_markers
                    using (var cmd2 = new NpgsqlCommand())
                    {
                        cmd2.Connection = conn;

                        //get the unique bundle names for the inserted harness
                        cmd2.CommandText = @"SELECT DISTINCT bundlea, harn_id 
                                        FROM raw_cutting_list_harnware 
                                        WHERE harn_id = @harnId";

                        cmd2.Parameters.AddWithValue("@harnId", insertedHarnessId);

                        var bundles = new List<Tuple<string, string>>();
                        using (NpgsqlDataReader reader2 = cmd2.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                string bundle = reader2.GetString(0);
                                string harn_id = reader2.GetString(1);
                                bundles.Add(Tuple.Create(bundle, harn_id));
                            }
                        }

                        cmd2.Parameters.Clear();

                        foreach (var bundle in bundles)
                        {
                            using (var cmdInsert = new NpgsqlCommand())
                            {
                                cmdInsert.Connection = conn;
                                cmdInsert.CommandText = @"INSERT INTO gui_markers (bundle_name, harness_id, marker_type_id, marker_table_id) VALUES (@bundle, @harn_id, -1, -1)";
                                cmdInsert.Parameters.AddWithValue("@bundle", bundle.Item1);
                                cmdInsert.Parameters.AddWithValue("@harn_id", int.Parse(bundle.Item2));
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private DataTable GetUpdatedTableFromDgv(string dgvName)
        {
            DataTable table = new DataTable();
            DataGridView targetDgv = null;

            // Use a switch statement or similar logic to map the name to the actual DataGridView
            switch (dgvName)
            {
                case "dgvPins":
                    targetDgv = this.dgvPins;
                    break;
                case "dgvWires":
                    targetDgv = this.dgvWires;
                    break;
                case "dgvConnectors":
                    targetDgv = this.dgvConnectors;
                    break;
                case "dgvMarkers":
                    targetDgv = this.dgvMarkers;
                    break;
                default:
                    throw new ArgumentException("Invalid DataGridView name", nameof(dgvName));
            }

            if (targetDgv == null)
                return table;

            // Add columns to table based on target DataGridView columns
            foreach (DataGridViewColumn column in targetDgv.Columns)
            {
                table.Columns.Add(column.Name, column.ValueType);
            }

            // Add rows to table based on target DataGridView rows
            foreach (DataGridViewRow dgvRow in targetDgv.Rows)
            {
                if (!dgvRow.IsNewRow) // Skip the new row template
                {
                    DataRow tableRow = table.NewRow();

                    // Iterate through the cells of the DataGridViewRow
                    for (int i = 0; i < dgvRow.Cells.Count; i++)
                    {
                        tableRow[i] = dgvRow.Cells[i].Value ?? DBNull.Value;
                    }

                    table.Rows.Add(tableRow);
                }
            }

            return table;
        }

        private bool DataTablesAreEqual(DataTable dt1, DataTable dt2)
        {
            if (dt1.Rows.Count != dt2.Rows.Count || dt1.Columns.Count != dt2.Columns.Count)
            {
                return false;
            }

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    if (!dt1.Rows[i][j].Equals(dt2.Rows[i][j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SaveUpdatedDataToDatabase(string query, Dictionary<string, object> queryParams)
        {
            try
            {
                _odooDatabaseService.ExecuteNonQuery(query, queryParams);
                MessageBox.Show("Changes saved successfully.", "Information");
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                MessageBox.Show("Error saving to database: " + ex.Message, "Error");
            }
        }

        private void PopulateWiresDgv()
        {
            dgvWires.DataSource = null;
            dgvWires.Columns.Clear();

            try
            {
                using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM gui_wires order by id desc";

                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.AsEnumerable().Any(row => row.Field<int>("wire_editor_id") == -1) || !dgvWires.Columns.Contains("wire_editor_link"))
                            {
                                // Create a new column of type DataGridViewButtonColumn.
                                var buttonColumn = new DataGridViewButtonColumn
                                {
                                    Name = "wire_editor_link",
                                    HeaderText = "wire_editor",
                                    Text = "Link",
                                    UseColumnTextForButtonValue = true
                                };

                                // Add the new column to the DataGridView.
                                dgvWires.Columns.Add(buttonColumn);
                            }

                            // Check if the DataTable contains a "-1" in the "wire_type_id" or "wire_editor_id" columns.
                            if (dt.AsEnumerable().Any(row => row.Field<int>("wire_type_id") == -1) || !dgvWires.Columns.Contains("wire_type_link"))
                            {
                                // Create a new column of type DataGridViewButtonColumn.
                                var buttonColumn = new DataGridViewButtonColumn
                                {
                                    Name = "wire_type_link",
                                    HeaderText = "wire_type",
                                    Text = "Link",
                                    UseColumnTextForButtonValue = true
                                };

                                // Add the new column to the DataGridView.
                                dgvWires.Columns.Add(buttonColumn);
                            }

                            dgvWires.DataSource = dt;

                            // Add a new column for the wire type text
                            if (!dgvWires.Columns.Contains("wire_type_description"))
                            {
                                dgvWires.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "wire_type_description",
                                    HeaderText = "Supplier Part Number"
                                });
                            }

                            if (!dgvWires.Columns.Contains("odoo_part_link"))
                            {
                                DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                                newColumn.Name = "odoo_part_link";
                                newColumn.HeaderText = "Odoo Part Link";
                                newColumn.DataSource = OdooWires;
                                dgvWires.Columns.Add(newColumn);
                                dgvWires.EditingControlShowing += dgvWires_EditingControlShowing;
                            }

                            // Add a new column for the wire type text
                            if (!dgvWires.Columns.Contains("wire_editor_description"))
                            {
                                dgvWires.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "wire_editor_description",
                                    HeaderText = "Supplier Part Number"
                                });
                            }

                            // Populate the new column with the results of your query
                            foreach (DataGridViewRow row in dgvWires.Rows)
                            {
                                int wireTypeId = (int)row.Cells["wire_type_id"].Value;
                                int wireEditorId = (int)row.Cells["wire_editor_id"].Value;
                                if (wireTypeId != -1)
                                {
                                    cmd.CommandText = "SELECT supplier_wire_type_code FROM gui_wire_type WHERE id = @wireTypeId";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@wireTypeId", wireTypeId);

                                    object result = cmd.ExecuteScalar();
                                    if (result != null)
                                    {
                                        row.Cells["wire_type_description"].Value = result.ToString();
                                    }
                                }
                                else
                                {
                                    row.Cells["wire_type_description"].Value = "";
                                }

                                if (wireEditorId != -1)
                                {
                                    cmd.CommandText = "SELECT supplier_part_number FROM gui_wire_editor WHERE id = @wireEditorId";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@wireEditorId", wireEditorId);

                                    object result = cmd.ExecuteScalar();
                                    if (result != null)
                                    {
                                        row.Cells["wire_editor_description"].Value = result.ToString();
                                    }
                                }
                                else
                                {
                                    row.Cells["wire_editor_description"].Value = "";
                                }
                            }

                            // Set the display order of the columns
                            dgvWires.Columns["id"].DisplayIndex = 0;
                            dgvWires.Columns["wire_type_id"].DisplayIndex = 1;
                            dgvWires.Columns["wire_type_description"].DisplayIndex = 2;
                            dgvWires.Columns["wire_type_link"].DisplayIndex = 3;
                            dgvWires.Columns["wire_editor_id"].DisplayIndex = 4;
                            dgvWires.Columns["wire_editor_description"].DisplayIndex = 5;
                            dgvWires.Columns["wire_editor_link"].DisplayIndex = 6;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
            }
        }

        private void dgvWires_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvWires.CurrentCell.ColumnIndex == dgvWires.Columns["odoo_part_link"].Index && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

                // Remove previously attached event handlers to prevent multiple subscriptions
                comboBox.TextChanged -= ComboBox_TextChanged;

                // Attach a TextChanged event handler to filter the items
                comboBox.TextChanged += ComboBox_TextChanged;

                // Detach previously attached event handlers to prevent multiple subscriptions.
                comboBox.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;

                // Attach the SelectionChangeCommitted event handler.
                comboBox.SelectionChangeCommitted += ComboBox_SelectionChangeCommitted;

            }
        }

        private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e) //for updating the database with the selected odoo item
        {
            if (sender is ComboBox comboBox && dgvWires.CurrentRow != null)
            {
                var selectedValue = comboBox.SelectedValue;
                var wireId = dgvWires.CurrentRow.Cells["id"].Value;
                int.TryParse(wireId.ToString(), out int wi);

                using (var conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = $"Update gui_wires set odoo_part = '{selectedValue.ToString()}' where id = {wi}";
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Odoo Part updated successfully.", "Info");

                PopulateWiresDgv();
            }
        }

        private void ComboBox_TextChanged(object sender, EventArgs e) //for filtering the datasource list.
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string userInput = comboBox.Text;

                // Filter OdooWires based on userInput. Adjust this logic based on your needs.
                var filteredItems = OdooWires.Where(item => item.IndexOf(userInput, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                // Temporarily detach the event handler to prevent recursion
                comboBox.TextChanged -= ComboBox_TextChanged;

                // Update the ComboBox items
                comboBox.DataSource = filteredItems;
                comboBox.Text = userInput; // Restore the user input
                comboBox.SelectionStart = userInput.Length; // Move the cursor to the end of the input

                // Reattach the event handler
                comboBox.TextChanged += ComboBox_TextChanged;
            }
        }

        #region Button Clicks
        private void btnSearchPart_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control is KryptonComboBox cb)
                {
                    cb.Enabled = true;
                }
            }

            SearchClicked = true;
            PopulateHarnessInfo();
            SearchClicked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveClicked = true;

            if (!ValidateFormInputs())
            {
                saveClicked = false;
                return;
            }

            foreach (Control control in this.Controls)
            {
                if (control is KryptonComboBox cb)
                {
                    cb.Enabled = false;
                }
            }

            if (DateTime.TryParse(txtbxCreateDate.Text, out DateTime createDateValue))
            {

            }
            else
            {
                // Handle the case where the text is not a valid DateTime
                throw new InvalidOperationException("The text in txtbxCreateDate is not a valid DateTime.");
            }

            if (DateTime.TryParse(txtbxUpdateDate.Text, out DateTime updateDateValue))
            {

            }
            else
            {
                // Handle the case where the text is not a valid DateTime
                throw new InvalidOperationException("The text in txtbxUpdateDate is not a valid DateTime.");
            }

            double odooRevisionDouble;
            bool isOdooRevisionDouble = double.TryParse(txtbxOdooRevision.Text, out odooRevisionDouble);
            double clientRevisionDouble;
            bool isClientRevisionDouble = double.TryParse(txtbxClientRevision.Text, out clientRevisionDouble);

            var queryParameters = new Dictionary<string, object>
            {
                { "@OdooPart", txtbxOdooPart.Text.ToString() },
                { "@OdooDescription", txtbxOdooDescription.Text.ToString() },
                { "@OdooRevision", isOdooRevisionDouble ? odooRevisionDouble : -1  },
                { "@ClientPart", txtbxClientPartNumber.Text.ToString() },
                { "@ClientDescription", txtbxClientDescription.Text.ToString() },
                { "@ClientRevision", isClientRevisionDouble ? clientRevisionDouble : -1 },
                { "@State", cbState.Text.ToString() },
                { "@CreateDate", createDateValue },
                { "@UpdateDate", updateDateValue },
                { "@CreatedBy", txtbxCreatedBy.Text.ToString() },
                { "@UserToApprove", cbUserToApprove.Text.ToString() },
                { "@ApprovedBy", txtbxApprovedBy.Text.ToString() }
            };

            try
            {
                string insertQuery = "INSERT INTO harness (odoopart, odoodescription, odoorev, clientpart, clientdescription, clientrev, state, createdate, updatedate, createdby, usertoapprove, approvedby) " +
                                     "VALUES (@OdooPart, @OdooDescription, @OdooRevision, @ClientPart, @ClientDescription, @ClientRevision, @State, @CreateDate, @UpdateDate, @CreatedBy, @UserToApprove, @ApprovedBy) " +
                                     "RETURNING id";

                int result = _odooDatabaseService.ExecuteNonQuery(insertQuery, queryParameters);
                if (result != null)
                {
                    insertedHarnessId = result;
                    MessageBox.Show("Data saved successfully.", "Information");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting into harness table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            PopulateMarkersDrawings();
            txtbxOdooPart.Enabled = false;
            btnSave.Enabled = false;
        }

        private void btnNewImport_Click(object sender, EventArgs e)
        {
            txtbxOdooPart.Enabled = true;
            btnSave.Enabled = true;

            cbState.SelectedIndex = 0;
            txtbxCreateDate.Text = DateTime.Now.ToString("G");
            txtbxUpdateDate.Text = DateTime.Now.ToString("G");
            txtbxCreatedBy.Text = currentUser;
        }

        private void btnImportOriginal_Click(object sender, EventArgs e)
        {
            string filePath = FileHelper.ImportOriginalFile(Globals.DrawingsLocation, out fileNameOriginal);
            if (!string.IsNullOrEmpty(filePath))
            {
                txtbxImportOriginalLocation.Text = filePath;
            }
            else
            {
                txtbxImportOriginalLocation.Text = "";
            }

        }

        private void btnImportVisio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                ParseVisioFileAndStoreInDb(filePath);

                fileNameVisio = Path.GetFileName(filePath);

                // Split the file name at the underscore character
                string[] parts = fileNameVisio.Split('_');
                string drawing = parts[0]; //drawing number
                string remainder = parts[1]; //remainder of the file name

                if (remainder.Contains("V")) //correct file selected.
                {
                    string destinationStoragePath = Path.Combine(Globals.DrawingsLocation, drawing, "V", fileNameVisio);
                    fileNameVisio = destinationStoragePath;

                    // Copy the file to the known location (executable location)
                    try
                    {
                        File.Copy(filePath, destinationStoragePath, false);
                        txtbxImportVLocation.Text = destinationStoragePath;
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show("File already exists in destination. To avoid errors, please manually delete the existing file before uploading one with the same name\n" + ioEx.Message, "Error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error copying file to known location: " + ex.Message, "Error");
                        return;
                    }

                    int maxIdVisio = 0;

                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(Globals.ConnectionString))
                        {
                            conn.Open();
                            string querymaxIdV = "SELECT MAX(Id) FROM public.raw_cutting_list_visio";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(querymaxIdV, conn))
                            {
                                object result = cmd.ExecuteScalar();
                                if (result != DBNull.Value)
                                {
                                    maxIdVisio = Convert.ToInt32(result);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
                    }
                }
                else
                {
                    MessageBox.Show("Wrong file selected for import. Please select the Visio file.", "ERROR");
                }

                saveClicked = false;
                MessageBox.Show("Please import the harnware file now to complete the import.", "Info");
            }
        }

        private void btnImportHarnware_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ParseHarnwareFileAndStoreInDb(filePath);
                fileNameHarnware = Path.GetFileName(filePath);

                // Split the file name at the underscore character
                string[] parts = fileNameHarnware.Split('_');
                string drawing = parts[0]; //drawing number
                string remainder = parts[1]; //remainder of the file name

                if (remainder.Contains("HW")) //correct file selected.
                {
                    string destinationStoragePath = Path.Combine(Globals.DrawingsLocation, drawing, "HW", fileNameHarnware);
                    fileNameHarnware = destinationStoragePath;
                    // Copy the file to the known location (executable location)
                    try
                    {
                        File.Copy(filePath, destinationStoragePath, false);
                        txtbxImportHWLocation.Text = filePath;
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show("File already exists in destination. To avoid errors, please manually delete the existing file before uploading one with the same name\n" + ioEx.Message, "Error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error copying file to known location: " + ex.Message, "Error");
                        return;
                    }

                    int maxIdHW = 0;

                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(Globals.ConnectionString))
                        {
                            conn.Open();
                            string querymaxIdV = "SELECT MAX(Id) FROM public.raw_cutting_list_visio";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(querymaxIdV, conn))
                            {
                                object result = cmd.ExecuteScalar();
                                if (result != DBNull.Value)
                                {
                                    maxIdHW = Convert.ToInt32(result);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not connect to database.\n" + ex.Message, "ERROR");
                    }
                }
                else
                {
                    MessageBox.Show("Wrong file selected for import. Please select the Harnware file.", "ERROR");
                }
            }
        }

        private void btnNewParts_Click(object sender, EventArgs e)
        {
            dgvParts.DataSource = null;

            // Remove the columns
            if (dgvParts.Columns.Contains("odoo_item"))
            {
                dgvParts.Columns.Remove("odoo_item");
            }

            if (dgvParts.Columns.Contains("description"))
            {
                dgvParts.Columns.Remove("description");
            }

            try
            {
                var queryParameters = new Dictionary<string, object>(); //blank because there are no parameters
                string query = "SELECT * from gui_new_part where \"Type\" != 'Wire'";

                DataTable result = _odooDatabaseService.ExecuteQuery(query, queryParameters);
                if (result != null && result.Rows.Count > 0)
                {
                    dgvParts.DataSource = result;
                    if (!dgvParts.Columns.Contains("odoo_item"))
                    {
                        DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                        newColumn.Name = "odoo_item";
                        newColumn.HeaderText = "Odoo Item";
                        newColumn.DataSource = new BindingSource(OdooItems, null);
                        newColumn.ValueMember = "Key";
                        newColumn.DisplayMember = "Key";
                        dgvParts.Columns.Add(newColumn);
                    }

                    if (!dgvParts.Columns.Contains("description"))
                    {
                        DataGridViewTextBoxColumn descColumn = new DataGridViewTextBoxColumn();
                        descColumn.Name = "description";
                        descColumn.HeaderText = "Description";
                        descColumn.Width = 500;
                        dgvParts.Columns.Add(descColumn);
                    }

                    // Handle the EditingControlShowing event to capture the ComboBox control
                    dgvParts.EditingControlShowing += (s, e) =>
                    {
                        if (dgvParts.CurrentCell.ColumnIndex == dgvParts.Columns["odoo_item"].Index && e.Control is ComboBox)
                        {
                            ComboBox comboBox = e.Control as ComboBox;
                            comboBox.DropDownStyle = ComboBoxStyle.DropDown; // Allow typing in the ComboBox
                            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest; // enable the autocomplete suggest dropdown

                            // Prevent the initial dropdown from showing
                            comboBox.DropDown += (s, e) =>
                            {
                                comboBox.DroppedDown = false;
                            };

                            // Update the description when an item is selected
                            comboBox.SelectedIndexChanged += (s, e) =>
                            {
                                string selectedKey = comboBox.Text; // Use Text property instead of SelectedValue
                                if (OdooItems.ContainsKey(selectedKey))
                                {
                                    List<string> descriptions = OdooItems[selectedKey]; // Get the list of descriptions

                                    // Apply the regular expression to each description in the list
                                    List<string> newDescriptions = new List<string>();
                                    foreach (var description in descriptions)
                                    {
                                        string trimmedDescription = description.Trim();
                                        string newDescription = Regex.Replace(trimmedDescription, @"\s*\([^()]*\/\/.*$", "").Trim();
                                        if (!newDescriptions.Contains(newDescription))
                                        {
                                            newDescriptions.Add(newDescription);
                                        }
                                    }

                                    // If there are multiple descriptions, replace the TextBoxCell with a ComboBoxCell
                                    if (newDescriptions.Count > 1)
                                    {
                                        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                                        comboBoxCell.DataSource = newDescriptions;
                                        dgvParts.CurrentRow.Cells["description"] = comboBoxCell;
                                    }
                                    else // If there's only one description, replace the ComboBoxCell with a TextBoxCell
                                    {
                                        DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
                                        string description = newDescriptions[0];
                                        textBoxCell.Value = description;
                                        dgvParts.CurrentRow.Cells["description"] = textBoxCell;
                                    }
                                }
                            };
                        }
                    };
                }
                else
                {
                    MessageBox.Show("No new parts.", "Information");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateParts_Click(object sender, EventArgs e)
        {
            updatePartsClicked = true;
            dgvParts.DataSource = null;

            // Remove the manually added columns
            if (dgvParts.Columns.Contains("odoo_item"))
            {
                dgvParts.Columns.Remove("odoo_item");
            }

            if (dgvParts.Columns.Contains("description"))
            {
                dgvParts.Columns.Remove("description");
            }

            var queryParameters = new Dictionary<string, object>(); //blank because there are no parameters
            string query = "SELECT * from gui_part_link order by id desc";

            DataTable result = _odooDatabaseService.ExecuteQuery(query, queryParameters);
            partLinkTable = result;
            dgvParts.DataSource = result;

            // Store a copy of the original DataTable
            originalPartLinkTable = partLinkTable.Copy();

            if (currentUserRole != "Viewer") // enable grid editing
            {
                dgvParts.ReadOnly = false;
            }
            else
            {
                MessageBox.Show("You do not have permission to edit the parts table.", "Information");
                return;
            }
        }

        private void btnSaveParts_Click(object sender, EventArgs e)
        {
            // still need to implement the approved by logic when approving a part in with the check box in the dgv (2024-07-16).

            if (dgvParts.DataSource == null)
            {
                MessageBox.Show("No data to save.", "Information");
                return;
            }

            if (updatePartsClicked == true) // update is being done
            {
                DataTable updatedTable = (DataTable)dgvParts.DataSource;

                if (!DataTablesAreEqual(originalPartLinkTable, updatedTable)) //changes detected
                {
                    // Get only the modified rows
                    DataTable modifiedRows = updatedTable.GetChanges(DataRowState.Modified);

                    if (modifiedRows != null && modifiedRows.Rows.Count > 0)
                    {
                        foreach (DataRow row in modifiedRows.Rows)
                        {
                            var queryParameters = new Dictionary<string, object>
                            {
                                { "harnItem", row["harn_item"] },
                                { "odooItem", row["odoo_item"] },
                                { "partType", row["part_type"] },
                                { "creationDate", row["creation_date"] },
                                { "updateDate", DateTime.Now },
                                { "userId", Globals.UserId },
                                { "description", row["description"] },
                                { "isApproved", row["is_approved"] },
                                { "odoo_alternative_1", row["odoo_alternative_1"] },
                                { "odoo_alternative_2", row["odoo_alternative_2"] },
                                { "odoo_alternative_3", row["odoo_alternative_3"] }
                            };

                            // Check if 'is_approved' was changed from false to true
                            bool originalIsApproved = row["is_approved", DataRowVersion.Original] == DBNull.Value ? false : (bool)row["is_approved", DataRowVersion.Original];
                            bool currentIsApproved = (bool)row["is_approved"];

                            if (!originalIsApproved && currentIsApproved)
                            {
                                // 'is_approved' changed from false to true, add 'approvedBy'
                                queryParameters.Add("approvedBy", Globals.User);
                            }

                            string query;
                            if (queryParameters.ContainsKey("approvedBy"))
                            {
                                query = "INSERT INTO gui_part_link (harn_item, odoo_item, part_type, creation_date, update_date, user_id, description, is_approved, approved_by, odoo_alternative_1, odoo_alternative_2, odoo_alternative_3) VALUES (@harnItem, @odooItem, @partType, @creationDate, @updateDate, @userId, @description, @isApproved, @approvedBy, @odoo_alternative_1, @odoo_alternative_2, @odoo_alternative_3)";
                            }
                            else
                            {
                                query = "INSERT INTO gui_part_link (harn_item, odoo_item, part_type, creation_date, update_date, user_id, description, is_approved, odoo_alternative_1, odoo_alternative_2, odoo_alternative_3) VALUES (@harnItem, @odooItem, @partType, @creationDate, @updateDate, @userId, @description, @isApproved, @odoo_alternative_1, @odoo_alternative_2, @odoo_alternative_3)";
                            }

                            SaveUpdatedDataToDatabase(query, queryParameters);
                        }
                    }
                    else
                    {
                        updateClicked = false;
                        MessageBox.Show("No changes detected.", "Information");
                    }
                    updateClicked = false;
                }
                else
                {
                    updateClicked = false;
                    MessageBox.Show("No changes detected.", "Information");
                }
            }
            else // new parts are being linked
            {
                DataTable newPartsTable = (DataTable)dgvParts.DataSource;

                foreach (DataGridViewRow dgvRow in dgvParts.Rows)
                {
                    if (dgvRow.IsNewRow) continue; // Skip the new row placeholder

                    foreach (DataGridViewColumn column in dgvParts.Columns)
                    {
                        Console.WriteLine(column.Name);
                    }

                    var queryParameters = new Dictionary<string, object>
                    {
                        { "harnItem", dgvRow.Cells["harn_item"].Value },
                        { "odooItem", dgvRow.Cells["odoo_item"].Value ?? (dgvRow.Cells["odoo_item"] as DataGridViewComboBoxCell)?.FormattedValue },
                        { "partType", dgvRow.Cells["Type"].Value },
                        { "creationDate", DateTime.Now },
                        { "updateDate", DateTime.Now },
                        { "userId", Globals.UserId },
                        { "isApproved", false },
                        { "approvedBy", "" }
                    };

                    // Check if the description column exists and add it to the parameters
                    if (dgvRow.Cells["description"] != null && dgvRow.Cells["description"].Value != null)
                    {
                        queryParameters.Add("description", dgvRow.Cells["description"].Value);
                    }

                    string query = "INSERT INTO gui_part_link (harn_item, odoo_item, part_type, creation_date, update_date, user_id, is_approved, approved_by, description) VALUES (@harnItem, @odooItem, @partType, @creationDate, @updateDate, @userId, @isApproved, @approvedBy, @description)";

                    SaveUpdatedDataToDatabase(query, queryParameters);
                }
            }
        }

        private void btnNewConnectors_Click(object sender, EventArgs e)
        {
            dgvConnectors.DataSource = null;

            ConnectorForm connectorForm = new ConnectorForm();

            // Subscribe to the FormClosing event
            connectorForm.FormClosing += (s, args) =>
            {
                // Retrieve the data from ConnectorForm
                DataTable connectorsData = connectorForm.GetConnectorData();

                dgvConnectors.DataSource = connectorsData;
            };

            connectorForm.Show();
        }

        private void btnUpdateConnectors_Click(object sender, EventArgs e)
        {
            dgvConnectors.DataSource = null;
            var queryParameters = new Dictionary<string, object>(); //blank because there are no parameters
            string query = "SELECT * from gui_connectors order by id desc";

            DataTable result = _odooDatabaseService.ExecuteQuery(query, queryParameters);
            connectorsTable = result; //before updates are made in the dgv.
            dgvConnectors.DataSource = result;
        }

        private void btnSaveConnectors_Click(object sender, EventArgs e)
        {
            string target = "dgvConnectors";
            DataTable updatedTable = GetUpdatedTableFromDgv(target);

            if (updatedTable == null && connectorsTable == null)
            {
                MessageBox.Show("No data to save.", "Information");
                return;
            }
            else if (updatedTable != null && (connectorsTable == null || updatedTable.Rows.Count != connectorsTable.Rows.Count)) //saving for the first time, no connectors table to compare to.
            {
                foreach (DataRow row in updatedTable.Rows)
                {
                    var queryParameters = new Dictionary<string, object>
                    {
                        { "gui_part_link_id", Convert.ToInt32(row["gui_part_link_id"]) },
                        { "connector_pin_name", row["connector_pin_name"] },
                        { "odoo_pin_name", row["odoo_pin_name"] },
                        { "is_approved", row["is_approved"] }
                    };

                    // Assuming 'is_approved' might need special handling similar to the update case
                    bool isApproved = row["is_approved"] == DBNull.Value ? false : (bool)row["is_approved"];
                    if (isApproved)
                    {
                        // If 'is_approved' is true, add 'approvedBy'
                        queryParameters.Add("approvedBy", Globals.User);
                    }

                    string query;
                    if (queryParameters.ContainsKey("approvedBy"))
                    {
                        query = "INSERT INTO gui_connectors (gui_part_link_id, connector_pin_name, odoo_pin_name, is_approved, approved_by) VALUES (@gui_part_link_id, @connector_pin_name, @odoo_pin_name, @is_approved, @approvedBy)";
                    }
                    else
                    {
                        query = "INSERT INTO gui_connectors (gui_part_link_id, connector_pin_name, odoo_pin_name, is_approved) VALUES (@gui_part_link_id, @connector_pin_name, @odoo_pin_name, @is_approved)";
                    }

                    SaveUpdatedDataToDatabase(query, queryParameters);
                }
            }
            else if (!DataTablesAreEqual(connectorsTable, updatedTable)) //changes detected
            {
                // Get only the modified rows
                DataTable modifiedRows = updatedTable.GetChanges(DataRowState.Modified);

                if (modifiedRows != null && modifiedRows.Rows.Count > 0)
                {
                    foreach (DataRow row in modifiedRows.Rows)
                    {
                        // Your existing logic for handling modified rows goes here
                    }
                }
                else
                {
                    MessageBox.Show("No changes detected.", "Information");
                }
            }
            else
            {
                MessageBox.Show("No changes detected.", "Information");
            }
        }

        private void btnNewPins_Click(object sender, EventArgs e)
        {
            dgvPins.DataSource = null;

            PinForm pinForm = new PinForm();

            // Subscribe to the FormClosing event
            pinForm.FormClosing += (s, args) =>
            {
                // Retrieve the data from ConnectorForm
                DataTable pinsData = pinForm.GetPinData();

                dgvPins.DataSource = pinsData;
            };

            pinForm.Show();
        }

        private void btnUpdatePins_Click(object sender, EventArgs e)
        {
            dgvPins.DataSource = null;
            var queryParameters = new Dictionary<string, object>(); //blank because there are no parameters
            string query = "SELECT * from gui_pins order by id desc";

            DataTable result = _odooDatabaseService.ExecuteQuery(query, queryParameters);
            pinsTable = result; //before updates are made in the dgv.
            dgvPins.DataSource = result;
        }

        private void btnSavePins_Click(object sender, EventArgs e)
        {
            string target = "dgvPins";
            DataTable updatedTable = GetUpdatedTableFromDgv(target);
            List<string> pins = GetPins();

            if (updatedTable == null && pinsTable == null)
            {
                MessageBox.Show("No data to save.", "Information");
                return;
            }
            else if (updatedTable.AsEnumerable().Any(row => !pins.Contains(row["supplier_part_number"].ToString())))//saving for the first time, no pins table to compare to.
            {
                foreach (DataRow row in updatedTable.Rows)
                {
                    var queryParameters = new Dictionary<string, object>
                    {
                        { "gui_part_link_id",  Convert.ToInt32(row["gui_part_link_id"]) },
                        { "supplier", row["supplier"] },
                        { "supplier_part_number", row["supplier_part_number"] },
                        { "wire_type", row["wire_type"] },
                        { "strip_length_mm", row["strip_length_mm"] },
                        { "largest_wire", row["largest_wire"] },
                        { "smallest_wire", row["smallest_wire"] },
                        { "crimp_tool_1", row["crimp_tool_1"] },
                        { "crimp_tool_2", row["crimp_tool_2"] },
                        { "crimp_press", row["crimp_press"] },
                        { "is_approved", row["is_approved"] }
                    };

                    // Assuming 'is_approved' might need special handling similar to the update case
                    bool isApproved = row["is_approved"] == DBNull.Value ? false : (bool)row["is_approved"];
                    if (isApproved)
                    {
                        // If 'is_approved' is true, add 'approvedBy'
                        queryParameters.Add("approvedBy", Globals.User);
                    }

                    string query;
                    if (queryParameters.ContainsKey("approvedBy"))
                    {
                        query = "INSERT INTO gui_pins (gui_part_link_id, supplier, supplier_part_number, wire_type, strip_length_mm, largest_wire, smallest_wire, crimp_tool_1, crimp_tool_2, crimp_press, is_approved, approved_by) VALUES (@gui_part_link_id, @supplier, @supplier_part_number, @wire_type, @strip_length_mm, @largest_wire, @smallest_wire, @crimp_tool_1, @crimp_tool_2, @crimp_press, @is_approved, @approvedBy)";
                    }
                    else
                    {
                        query = "INSERT INTO gui_pins (gui_part_link_id, supplier, supplier_part_number, wire_type, strip_length_mm, largest_wire, smallest_wire, crimp_tool_1, crimp_tool_2, crimp_press, is_approved) VALUES (@gui_part_link_id, @supplier, @supplier_part_number, @wire_type, @strip_length_mm, @largest_wire, @smallest_wire, @crimp_tool_1, @crimp_tool_2, @crimp_press, @is_approved)";
                    }

                    SaveUpdatedDataToDatabase(query, queryParameters);
                }
            }
            else if (!DataTablesAreEqual(pinsTable, updatedTable)) //changes detected
            {
                // Get only the modified rows
                DataTable modifiedRows = updatedTable.GetChanges(DataRowState.Modified);

                if (modifiedRows != null && modifiedRows.Rows.Count > 0)
                {
                    foreach (DataRow row in modifiedRows.Rows)
                    {
                        var queryParameters = new Dictionary<string, object>
                        {
                            { "gui_part_link_id", row["gui_part_link_id"] },
                            { "supplier", row["supplier"] },
                            { "supplier_part_number", row["supplier_part_number"] },
                            { "wire_type", row["wire_type"] },
                            { "strip_length_mm", row["strip_length_mm"] },
                            { "largest_wire", row["largest_wire"] },
                            { "smallest_wire", row["smallest_wire"] },
                            { "crimp_tool_1", row["crimp_tool_1"] },
                            { "crimp_tool_2", row["crimp_tool_2"] },
                            { "crimp_press", row["crimp_press"] },
                            { "is_approved", row["is_approved"] }
                        };

                        // Check if 'is_approved' was changed from false to true
                        bool originalIsApproved = row["is_approved", DataRowVersion.Original] == DBNull.Value ? false : (bool)row["is_approved", DataRowVersion.Original];
                        bool currentIsApproved = (bool)row["is_approved"];

                        if (!originalIsApproved && currentIsApproved)
                        {
                            // 'is_approved' changed from false to true, add 'approvedBy'
                            queryParameters.Add("approvedBy", Globals.User);
                        }

                        string query;
                        if (queryParameters.ContainsKey("approvedBy"))
                        {
                            query = "INSERT INTO gui_pins (gui_part_link_id, supplier, supplier_part_number, wire_type, strip_length_mm, largest_wire, smallest_wire, crimp_tool_1, crimp_tool_2, crimp_press, is_approved, approved_by) VALUES (@partLinkId, @supplier, @supplierPartNo, @wireType, @stripLengthMM, @largestWire, @smallestWire, @crimp1, @crimp2, @crimpPress, @isApproved, @approvedBy)";
                        }
                        else
                        {
                            query = "INSERT INTO gui_pins (gui_part_link_id, supplier, supplier_part_number, wire_type, strip_length_mm, largest_wire, smallest_wire, crimp_tool_1, crimp_tool_2, crimp_press, is_approved) VALUES (@partLinkId, @supplier, @supplierPartNo, @wireType, @stripLengthMM, @largestWire, @smallestWire, @crimp1, @crimp2, @crimpPress, @isApproved)";
                        }

                        SaveUpdatedDataToDatabase(query, queryParameters);
                    }
                }
                else
                {
                    MessageBox.Show("No changes detected.", "Information");
                }
            }
            else
            {
                MessageBox.Show("No changes detected.", "Information");
            }
        }

        private void btnViewWires_Click(object sender, EventArgs e)
        {
            PopulateWiresDgv();
        }

        private void btnNewMarkers_Click(object sender, EventArgs e)
        {
            dgvMarkers.DataSource = null;
            MarkerForm markerForm = new MarkerForm();
            markerForm.Show();
        }

        private void btnUpdateMarkers_Click(object sender, EventArgs e)
        {
            if (dgvMarkers.Columns.Count > 0)
            {
                dgvMarkers.Columns.Clear();
            }
            if (dgvMarkers.Rows.Count > 0)
            {
                dgvMarkers.Rows.Clear();
            }

            if (cbSelectDrawing.Text == "")
            {
                MessageBox.Show("Please select a drawing number.", "Information");
                return;
            }

            var queryParameters = new Dictionary<string, object>
            {
                { "@SelectedHarnessId", Convert.ToDouble(markerHarnessId) }
            };
            string query = "SELECT * from gui_markers where harness_id = @SelectedHarnessId;";

            DataTable results = _odooDatabaseService.ExecuteQuery(query, queryParameters);

            if (results != null && results.Rows.Count > 0)
            {
                dgvMarkers.DataSource = results;

                if (dgvMarkers.Columns.Contains("id"))
                {
                    dgvMarkers.Columns.Remove("id");
                }
                return;
            }
            else
            {
                MessageBox.Show("No markers found for the selected harness. Retrieving data from raw import of the harness.", "Information");
            }

            string drawingName = cbSelectDrawing.Text;
            double radelRev = Convert.ToDouble(txtbxRRev.Text);
            double clientRev = Convert.ToDouble(txtbxCRev.Text);

            var qParams = new Dictionary<string, object>
            {
                { "@DrawingName", drawingName },
                { "@RadelRev", radelRev },
                { "@ClientRev", clientRev }
            };
            string q = "SELECT DISTINCT conna, connb FROM raw_cutting_list_harnware WHERE drawing = @DrawingName AND radel_rev = @RadelRev AND client_rev = @ClientRev;";

            DataTable rawResults = _odooDatabaseService.ExecuteQuery(q, qParams);

            HashSet<string> uniqueConnectors = new HashSet<string>();

            if (rawResults != null && rawResults.Rows.Count > 0)
            {
                foreach (DataRow row in rawResults.Rows)
                {
                    uniqueConnectors.Add(row["conna"].ToString());
                    uniqueConnectors.Add(row["connb"].ToString());
                }
            }
            else
            {
                MessageBox.Show("No data found in raw import for the specified criteria.", "Information");
            }

            List<string> connectorsList = uniqueConnectors.ToList();
            Dictionary<string, double> combinedDiameters = BundleDiameter.GetCombinedDiameter(drawingName, radelRev);

            // Create a BindingSource for OdooItems
            BindingSource odooItemsBindingSource = new BindingSource();
            odooItemsBindingSource.DataSource = OdooItems.Keys.ToList();

            // Add columns directly to the DataGridView
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "harness_id", HeaderText = "Harness ID" });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "connector", HeaderText = "Connector" });
            dgvMarkers.Columns.Add(new DataGridViewComboBoxColumn { Name = "odoo_part", HeaderText = "Odoo Part", DataSource = odooItemsBindingSource });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "description", HeaderText = "Description" });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "bundle_diameter", HeaderText = "Bundle Diameter" });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "font", HeaderText = "Font" });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "font_size", HeaderText = "Font Size" });
            dgvMarkers.Columns.Add(new DataGridViewTextBoxColumn { Name = "number_of_lines", HeaderText = "Number of Lines" });

            DataTable markerTemplates = GetMarkerTemplates();

            // Populate the DataGridView with the connectors list
            foreach (string connector in connectorsList)
            {
                int rowIndex = dgvMarkers.Rows.Add();
                DataGridViewRow row = dgvMarkers.Rows[rowIndex];
                row.Cells["harness_id"].Value = markerHarnessId;
                row.Cells["connector"].Value = connector;

                if (combinedDiameters.TryGetValue(connector, out double diameter))
                {
                    row.Cells["bundle_diameter"].Value = diameter;

                    // Filter marker templates based on the diameter
                    var filteredMarkers = FilterMarkersByDiameter(markerTemplates, diameter);
                    var comboBoxCell = (DataGridViewComboBoxCell)row.Cells["odoo_part"];
                    comboBoxCell.DataSource = filteredMarkers;
                }
            }

            dgvMarkers.EditingControlShowing += (s, e) =>
            {
                if (dgvMarkers.Columns.Contains("odoo_part"))
                {
                    if (dgvMarkers.CurrentCell != null) 
                    {
                        if (dgvMarkers.CurrentCell.ColumnIndex == dgvMarkers.Columns["odoo_part"].Index)
                        {
                            if (e.Control is ComboBox comboBox)
                            {
                                comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged; 
                                comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged; 
                            }
                        }
                    }
                }
            };
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    string selectedKey = comboBox.SelectedItem.ToString();
                    if (OdooItems.ContainsKey(selectedKey))
                    {
                        string description = OdooItems[selectedKey].FirstOrDefault();
                        int rowIndex = dgvMarkers.CurrentCell.RowIndex;
                        dgvMarkers.Rows[rowIndex].Cells["description"].Value = description;
                    }
                }
            }
        }

        private void btnSaveMarkers_Click(object sender, EventArgs e)
        {
            // Create a DataTable and define its columns
            DataTable markersTable = new DataTable();
            markersTable.Columns.Add("harness_id", typeof(int));
            markersTable.Columns.Add("connector", typeof(string));
            markersTable.Columns.Add("odoo_part", typeof(string));
            markersTable.Columns.Add("description", typeof(string));
            markersTable.Columns.Add("bundle_diameter", typeof(string));
            markersTable.Columns.Add("font", typeof(string));
            markersTable.Columns.Add("font_size", typeof(int));
            markersTable.Columns.Add("number_of_lines", typeof(int));

            // Loop through the rows of dgvMarkers and add them to the DataTable
            foreach (DataGridViewRow row in dgvMarkers.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dataRow = markersTable.NewRow();
                    dataRow["harness_id"] = row.Cells["harness_id"].Value ?? DBNull.Value;
                    dataRow["connector"] = row.Cells["connector"].Value ?? DBNull.Value;
                    dataRow["odoo_part"] = row.Cells["odoo_part"].Value ?? DBNull.Value;
                    dataRow["description"] = row.Cells["description"].Value ?? DBNull.Value;
                    dataRow["bundle_diameter"] = row.Cells["bundle_diameter"].Value ?? DBNull.Value;
                    dataRow["font"] = row.Cells["font"].Value ?? DBNull.Value;
                    dataRow["font_size"] = row.Cells["font_size"].Value ?? DBNull.Value;
                    dataRow["number_of_lines"] = row.Cells["number_of_lines"].Value ?? DBNull.Value;
                    markersTable.Rows.Add(dataRow);
                }
            }

            // Execute the insert query using the populated DataTable
            string insertQuery = "INSERT INTO gui_markers (harness_id, connector, odoo_part, description, bundle_diameter, font, font_size, number_of_lines) VALUES (@harness_id, @connector, @odoo_part, @description, @bundle_diameter, @font, @font_size, @number_of_lines)";
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            foreach (DataRow dataRow in markersTable.Rows)
            {
                queryParams.Clear();
                queryParams.Add("@harness_id", Convert.ToInt32(dataRow["harness_id"]));
                queryParams.Add("@connector", dataRow["connector"].ToString());
                queryParams.Add("@odoo_part", dataRow["odoo_part"].ToString());
                queryParams.Add("@description", dataRow["description"].ToString());
                queryParams.Add("@bundle_diameter", dataRow["bundle_diameter"].ToString());
                queryParams.Add("@font", dataRow["font"].ToString());
                queryParams.Add("@font_size", Convert.ToInt32(dataRow["font_size"]));
                queryParams.Add("@number_of_lines", Convert.ToInt32(dataRow["number_of_lines"]));

                SaveUpdatedDataToDatabase(insertQuery, queryParams);
            }
        }

        private void btnAssignMarker_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Dynamic Resizing of form elements
        private void SetupLayout()
        {
            this.Resize += (sender, e) => AdjustMainFormLayout();
            tbcntrlTabsOnMainForm.Resize += (sender, e) => AdjustTabControlLayout();

            // Define the order of labels and controls for each panel
            drawingInfoOrder = new List<(string labelName, string controlName)>
            {
                ("lblOdooPart", "txtbxOdooPart"),
                ("lblOdooDescription", "txtbxOdooDescription"),
                ("lblClientPartNumber", "txtbxClientPartNumber"),
                ("lblClientDescription", "txtbxClientDescription")
            };

            stateOrder = new List<(string labelName, string controlName)>
            {
                ("lblState", "cbState"),
                ("lblCreateDate", "txtbxCreateDate"),
                ("lblUpdateDate", "txtbxUpdateDate"),
                ("lblCreatedBy", "txtbxCreatedBy")
            };

            revisionOrder = new List<(string labelName, string controlName)>
            {
                ("lblOdooRev", "txtbxOdooRevision"),
                ("lblClientRev", "txtbxClientRevision")
            };

            approveOrder = new List<(string labelName, string controlName)>
            {
                ("lblUserToApprove", "cbUserToApprove"),
                ("lblApprovedBy", "txtbxApprovedBy")
            };

            importsOrder = new List<(string labelName, string controlName)>
            {
                ("btnImportOriginal", "txtbxImportOriginalLocation"),
                ("btnImportVisio", "txtbxImportVLocation"),
                ("btnImportHarnware", "txtbxImportHWLocation")
            };

            // Adjust panels within each tab
            pnlDrawingInfo.Resize += (sender, e) => AdjustPanelLayout(pnlDrawingInfo, drawingInfoOrder, "btnSearchPart", "lblOdooPart");
            pnlState.Resize += (sender, e) => AdjustPanelLayout(pnlState, stateOrder);
            pnlRevision.Resize += (sender, e) => AdjustPanelLayout(pnlRevision, revisionOrder);
            pnlApprove.Resize += (sender, e) => AdjustPanelLayout(pnlApprove, approveOrder);
            pnlImports.Resize += (sender, e) => AdjustPanelLayout(pnlImports, importsOrder);

            // Register resize events for new panels
            pnlParts.Resize += (sender, e) => AdjustDataGridLayout(pnlParts, dgvParts);
            pnlConnectors.Resize += (sender, e) => AdjustDataGridLayout(pnlConnectors, dgvConnectors);
            pnlPins.Resize += (sender, e) => AdjustDataGridLayout(pnlPins, dgvPins);
            pnlWires.Resize += (sender, e) => AdjustDataGridLayout(pnlWires, dgvWires);
            pnlMarkers.Resize += (sender, e) => AdjustDataGridLayout(pnlMarkers, dgvMarkers);

            AdjustMainFormLayout();
        }

        private void AdjustMainFormLayout()
        {
            int padding = 20;
            int spaceBetweenPanels = 10;
            int spaceFromTop = 90; // Increased space from top
            int spaceFromBottom = 20;
            int spaceBetweenColumns = 20;

            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // Resize the tab control based on the form size
            tbcntrlTabsOnMainForm.Size = new Size(formWidth - 2 * padding, formHeight - 2 * padding);
            tbcntrlTabsOnMainForm.Location = new Point(padding, padding);

            // Get the size of the tab control's client area
            int tabWidth = tbcntrlTabsOnMainForm.ClientSize.Width;
            int tabHeight = tbcntrlTabsOnMainForm.ClientSize.Height;

            // Calculate widths
            int panelWidth = (tabWidth - 3 * padding - spaceBetweenColumns) / 4;
            int importPanelWidth = tabWidth - 2 * padding - 2 * panelWidth - spaceBetweenColumns;

            // Calculate heights
            int panelHeight = (tabHeight - 2 * padding - spaceBetweenPanels - spaceFromTop - spaceFromBottom) / 2;

            // Set sizes and positions for DrawingInfo and RevisionInfo panels
            pnlDrawingInfo.Size = new Size(panelWidth, panelHeight);
            pnlDrawingInfo.Location = new Point(padding, spaceFromTop);

            pnlRevision.Size = new Size(panelWidth, panelHeight);
            pnlRevision.Location = new Point(padding, pnlDrawingInfo.Bottom + spaceBetweenPanels);

            // Set sizes and positions for StatusInfo and ApprovalInfo panels
            pnlState.Size = new Size(panelWidth, panelHeight);
            pnlState.Location = new Point(pnlDrawingInfo.Right + spaceBetweenColumns, spaceFromTop);

            pnlApprove.Size = new Size(panelWidth, panelHeight);
            pnlApprove.Location = new Point(pnlState.Left, pnlState.Bottom + spaceBetweenPanels);

            // Set sizes and positions for Imports panel
            pnlImports.Size = new Size(importPanelWidth, tabHeight - 2 * padding);
            pnlImports.Location = new Point(pnlState.Right + spaceBetweenColumns, padding);

            // Adjust child controls within each panel
            AdjustPanelLayout(pnlDrawingInfo, drawingInfoOrder, "btnSearchPart", "lblOdooPart");
            AdjustPanelLayout(pnlRevision, revisionOrder);
            AdjustPanelLayout(pnlState, stateOrder);
            AdjustPanelLayout(pnlApprove, approveOrder);
            AdjustPanelLayout(pnlImports, importsOrder);

            // Adjust new panels
            AdjustDataGridLayout(pnlParts, dgvParts);
            AdjustDataGridLayout(pnlConnectors, dgvConnectors);
            AdjustDataGridLayout(pnlPins, dgvPins);
            AdjustDataGridLayout(pnlWires, dgvWires);
            AdjustDataGridLayout(pnlMarkers, dgvMarkers);
        }

        private void AdjustPanelLayout(KryptonPanel panel, List<(string labelName, string controlName)> controlOrder, string specialButtonName = null, string specialButtonLabelName = null)
        {
            int padding = 10;
            int controlHeight = 30;
            int spaceBetweenControls = 5;
            int currentY = padding;

            foreach (var (labelName, controlName) in controlOrder)
            {
                Control label = panel.Controls.Find(labelName, true).FirstOrDefault();
                Control control = panel.Controls.Find(controlName, true).FirstOrDefault();

                if (label != null && control != null)
                {
                    label.Location = new Point(padding, currentY);
                    label.Size = new Size(panel.Width - 2 * padding, controlHeight);
                    currentY += controlHeight + spaceBetweenControls;

                    control.Location = new Point(padding, currentY);
                    control.Size = new Size(panel.Width - 2 * padding, controlHeight);
                    currentY += controlHeight + spaceBetweenControls;
                }
            }

            // Special handling for pnlImports
            if (panel == pnlImports)
            {
                int customControlHeight = 80; // Custom height for buttons and textboxes
                int customSpaceBetweenControls = 60; // Custom spacing between controls
                int rightPadding = 20; // Additional padding to the right

                Control btnImportOriginal = panel.Controls.Find("btnImportOriginal", true).FirstOrDefault();
                Control txtbxImportOriginalLocation = panel.Controls.Find("txtbxImportOriginalLocation", true).FirstOrDefault();
                Control btnImportVisio = panel.Controls.Find("btnImportVisio", true).FirstOrDefault();
                Control txtbxImportVLocation = panel.Controls.Find("txtbxImportVLocation", true).FirstOrDefault();
                Control btnImportHarnware = panel.Controls.Find("btnImportHarnware", true).FirstOrDefault();
                Control txtbxImportHWLocation = panel.Controls.Find("txtbxImportHWLocation", true).FirstOrDefault();

                if (btnImportOriginal != null && txtbxImportOriginalLocation != null)
                {
                    btnImportOriginal.Location = new Point(padding, padding);
                    btnImportOriginal.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                    txtbxImportOriginalLocation.Location = new Point(padding, btnImportOriginal.Bottom + customSpaceBetweenControls);
                    txtbxImportOriginalLocation.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                }

                if (btnImportVisio != null && txtbxImportVLocation != null)
                {
                    btnImportVisio.Location = new Point(padding, txtbxImportOriginalLocation.Bottom + customSpaceBetweenControls);
                    btnImportVisio.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                    txtbxImportVLocation.Location = new Point(padding, btnImportVisio.Bottom + customSpaceBetweenControls);
                    txtbxImportVLocation.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                }

                if (btnImportHarnware != null && txtbxImportHWLocation != null)
                {
                    btnImportHarnware.Location = new Point(padding, txtbxImportVLocation.Bottom + customSpaceBetweenControls);
                    btnImportHarnware.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                    txtbxImportHWLocation.Location = new Point(padding, btnImportHarnware.Bottom + customSpaceBetweenControls);
                    txtbxImportHWLocation.Size = new Size(panel.Width - 2 * padding - rightPadding, customControlHeight);
                }
            }

            // Special handling for btnSearchPart in pnlDrawingInfo
            if (panel == pnlDrawingInfo)
            {
                int rightPadding = 20; // Padding from the right edge
                Control btnSearchPart = panel.Controls.Find("btnSearchPart", true).FirstOrDefault();

                if (btnSearchPart != null)
                {
                    btnSearchPart.Location = new Point(panel.Width - btnSearchPart.Width - rightPadding, btnSearchPart.Location.Y);
                }
            }
        }

        private void AdjustTabControlLayout()
        {
            foreach (TabPage tab in tbcntrlTabsOnMainForm.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is KryptonPanel panel)
                    {
                        panel.Size = new Size(tab.ClientSize.Width - 40, tab.ClientSize.Height - 40);
                    }
                }
            }
        }

        private void AdjustDataGridLayout(KryptonPanel panel, DataGridView dataGridView)
        {
            int padding = 20;
            int panelWidth = panel.ClientSize.Width;
            int panelHeight = panel.ClientSize.Height;

            dataGridView.Location = new Point(padding, padding);
            dataGridView.Size = new Size(panelWidth - 2 * padding, panelHeight - 2 * padding);
        }

        #endregion

        private void cbSelectDrawing_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is a DataRowView
            if (cbSelectDrawing.SelectedItem is DataRowView selectedRow)
            {
                // Get the text from the specific column
                var selectedItem = selectedRow["odoopart"].ToString();

                var queryParameters = new Dictionary<string, object>
                {
                    { "@SelectedItem", selectedItem }
                };
                string query = "SELECT h.id, h.odoorev, h.clientrev FROM harness h INNER JOIN (SELECT MAX(id) as max_id FROM harness WHERE odoopart = @SelectedItem GROUP BY odoopart) mh ON h.id = mh.max_id WHERE h.odoopart = @SelectedItem;";

                DataTable results = _odooDatabaseService.ExecuteQuery(query, queryParameters);

                if (results != null && results.Rows.Count > 0)
                {
                    DataRow row = results.Rows[0];

                    markerHarnessId = Convert.ToInt32(row["id"]);
                    txtbxCRev.Text = row["clientrev"].ToString();
                    txtbxRRev.Text = row["odoorev"].ToString();
                }
                else
                {
                    txtbxCRev.Text = "";
                    txtbxRRev.Text = "";
                }
            }
        }
    }
}

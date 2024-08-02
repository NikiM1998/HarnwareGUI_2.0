using HarnwareGUI.Services;
using Krypton.Toolkit;
using Npgsql;
using System.Data;

namespace HarnwareGUI.Forms
{
    public partial class MarkerForm : KryptonForm
    {
        private OdooDatabaseService _odooDatabaseService;

        public MarkerForm()
        {
            InitializeComponent();
            this.Load += MarkerForm_Load;
            this.FormClosing += MarkerForm_FormClosing;
        }

        private void MarkerForm_Load(object sender, EventArgs e)
        {
            _odooDatabaseService = new OdooDatabaseService(Globals.ConnectionString);
            PopulateDGV();
        }

        private void PopulateDGV()
        {
            var queryParameters = new Dictionary<string, object>();
            string query = "select * from gui_marker_templates where 1=0"; //get column names only
            DataTable results = _odooDatabaseService.ExecuteQuery(query, queryParameters);

            if (results != null)
            {
                if (results.Columns.Contains("id"))
                {
                    results.Columns.Remove("id");
                }

                DataRow newRow = results.NewRow();
                results.Rows.Add(newRow);

                dgvMarkerType.DataSource = results;
            }
        }

        private void MarkerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDataToDatabase(e);
        }

        private void SaveDataToDatabase(FormClosingEventArgs e)
        {
            DataTable markersData = GetMarkerData();

            if (markersData == null || markersData.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (DataRow row in markersData.Rows)
            {
                bool isRowValid = true;

                foreach (var item in row.ItemArray)
                {
                    if (item == null || string.IsNullOrWhiteSpace(item.ToString()))
                    {
                        isRowValid = false;
                        break;
                    }
                }

                if (!isRowValid)
                {
                    MessageBox.Show("One or more cells in the row are empty. Not saving Data to database.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string sqlCommandText = "insert into gui_marker_templates(supplier, supplier_part_number, radel_part_number, description, min_diameter_mm, max_diameter_mm, alternative_1, alternative_2, alternative_3) values(@Supplier,@SupplierPartNumber,@RadelPartNumber,@Description,@MinDiameter,@MaxDiameter,@Alternative1,@Alternative2,@Alternative3)";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Supplier", row["supplier"] },
                    { "@SupplierPartNumber", row["supplier_part_number"] },
                    { "@RadelPartNumber", row["radel_part_number"] },
                    { "@Description", row["description"] },
                    { "@MinDiameter", Convert.ToDouble(row["min_diameter_mm"]) },
                    { "@MaxDiameter", Convert.ToDouble(row["max_diameter_mm"]) },
                    { "@Alternative1", row["alternative_1"] },
                    { "@Alternative2", row["alternative_2"] },
                    { "@Alternative3", row["alternative_3"] }
                };

                _odooDatabaseService.ExecuteNonQuery(sqlCommandText, parameters);
            }
        }

        public DataTable GetMarkerData()
        {
            return (DataTable)dgvMarkerType.DataSource;
        }
    }
}

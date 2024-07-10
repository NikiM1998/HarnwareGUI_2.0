using HarnwareGUI.Forms;
using HarnwareGUI.Helpers;
using Krypton.Toolkit;
using Npgsql;

namespace HarnwareGUI
{
    public partial class LoginForm : KryptonForm
    {
        string? role;

        public LoginForm()
        {
            InitializeComponent();
            SetupLayout();
        }

        private bool IsUserValid(string connectionString)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                if (Globals.AllUsers == null || Globals.AllUsers.Count == 0)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.users;", conn))
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        Globals.AllUsers ??= new List<string>();

                        while (reader.Read())
                        {
                            Globals.AllUsers.Add(reader.GetString(0));
                        }
                    }
                }

                string username = txtbxUserName.Text;
                string password = txtbxPassword.Text;
                string? storedMaskedPassword = GetPassword(connectionString);
                if (storedMaskedPassword == null)
                {
                    return false;
                }
                else
                {
                    string storedUnmaskedPassword = new DataMasking().UnmaskData(storedMaskedPassword);

                    if (String.Compare(storedUnmaskedPassword, password) == 0)
                    {
                        Globals.User = username;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private string GetPassword(string connectionString)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string username = txtbxUserName.Text;

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT password FROM public.users WHERE username = @username;", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0); // get the string value of the first column (password)
                        }
                    }
                }
            }

            return null; // return null if no matching user was found
        }

        private void SetupLayout()
        {
            this.Resize += (sender, e) =>
            {
                int padding = 20; // Set the desired padding
                int buttonHeight = 50; // Set the desired button height
                int spaceBetweenButtons = 35; // Set the desired space between the buttons
                int spaceFromPanel = 20; // Set the desired space from the panel
                int spaceFromBottom = 10; // Set the desired space from the bottom of the form

                // Calculate the positions
                int formWidth = this.ClientSize.Width;
                int formHeight = this.ClientSize.Height;
                int availableWidth = formWidth - (2 * padding) - spaceBetweenButtons;

                // Calculate the button width
                int buttonWidth = availableWidth / 2;

                btnLogIn.Size = new Size(buttonWidth, buttonHeight);
                btnCreateUser.Size = new Size(buttonWidth, buttonHeight);

                // Calculate the positions based on the panel's height, desired padding, and space from the bottom
                int panelBottom = pnlLogIn.Bottom;
                int buttonYPositionFromPanel = panelBottom + spaceFromPanel;
                int buttonYPositionFromBottom = formHeight - buttonHeight - spaceFromBottom;

                // Ensure the buttons maintain the set distance from both the panel and the bottom of the form
                int buttonYPosition = Math.Min(buttonYPositionFromPanel, buttonYPositionFromBottom);

                btnLogIn.Location = new Point(padding, buttonYPosition);
                btnCreateUser.Location = new Point(formWidth - buttonWidth - padding, buttonYPosition);
            };

            pnlLogIn.Resize += (sender, e) =>
            {
                int padding = 20; // Set the desired padding within the panel

                // Calculate the positions and sizes of the elements within the panel
                int panelWidth = pnlLogIn.ClientSize.Width;
                int panelHeight = pnlLogIn.ClientSize.Height;

                lblLogInInfo.Location = new Point(padding, padding);
                lblLogInInfo.Size = new Size(panelWidth - 2 * padding, lblLogInInfo.Height);

                lblUserName.Location = new Point((panelWidth - lblUserName.Width) / 2, lblLogInInfo.Bottom + padding);

                txtbxUserName.Location = new Point(padding, lblUserName.Bottom + padding);
                txtbxUserName.Size = new Size(panelWidth - 2 * padding, txtbxUserName.Height);

                lblPassword.Location = new Point((panelWidth - lblPassword.Width) / 2, txtbxUserName.Bottom + padding);

                txtbxPassword.Location = new Point(padding, lblPassword.Bottom + padding);
                txtbxPassword.Size = new Size(panelWidth - 2 * padding, txtbxPassword.Height);
            };
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (IsUserValid(Globals.ConnectionString))
            {
                // user is valid, perform query to get user role
                using (NpgsqlConnection conn = new NpgsqlConnection(Globals.ConnectionString))
                {
                    conn.Open();

                    string username = txtbxUserName.Text;
                    RoleManager.User = username;

                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT role FROM public.users WHERE username = @username;", conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = reader.GetString(0); // get the string value of the first column
                                RoleManager.Role = role;
                                this.Hide(); // hide the login form

                                // Open the main form
                                MainForm mainForm = new MainForm();
                                mainForm.Show();
                            }
                        }
                    }
                }
            }
            else
            {
                txtbxUserName.Text = "";
                txtbxPassword.Text = "";
                // user is invalid, display error message
                MessageBox.Show("Invalid login details, please try again.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

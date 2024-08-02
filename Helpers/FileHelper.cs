namespace HarnwareGUI.Helpers
{
    public static class FileHelper
    {
        public static string ImportOriginalFile(string drawingsLocation, out string fileNameOriginal)
        {
            fileNameOriginal = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                fileNameOriginal = Path.GetFileName(filePath);

                string[] parts = fileNameOriginal.Split('_');
                if (parts.Length < 6)
                {
                    MessageBox.Show("Invalid file name format.", "ERROR");
                    return null;
                }

                string drawing = parts[0]; //drawing number
                string remainder = parts[5]; //remainder of the file name

                if (remainder.Contains("vsdx")) //correct file selected.
                {
                    // Check if the directory exists
                    string directoryPath = Path.Combine(drawingsLocation, drawing);
                    if (!Directory.Exists(directoryPath))
                    {
                        // If the directory doesn't exist, create it
                        Directory.CreateDirectory(directoryPath);
                        Directory.CreateDirectory(Path.Combine(directoryPath, "HW"));
                        Directory.CreateDirectory(Path.Combine(directoryPath, "V"));
                        Directory.CreateDirectory(Path.Combine(directoryPath, "Original"));
                    }

                    directoryPath = Path.Combine(directoryPath, "Original");
                    // Now the directory definitely exists, so we can construct the destination path
                    string destinationPath = Path.Combine(directoryPath, fileNameOriginal);

                    // Copy the file to the known location (executable location)
                    try
                    {
                        File.Copy(filePath, destinationPath, false);
                        return destinationPath;
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show("File already exists in destination. To avoid errors, please manually delete the existing file before uploading one with the same name\n" + ioEx.Message, "Error");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error copying file to known location: " + ex.Message, "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Wrong file selected for import. Please select the Original file.", "ERROR");
                }
            }

            return null;
        }
    }
}

using System.IO.Compression;
using System.Text;

namespace HarnwareGUI.Helpers
{
    class DataMasking
    {
        public string MaskData(string plaintext)
        {
            string result;

            //StringBuilder result = new StringBuilder();
            byte[] inputStrBytes = Encoding.UTF8.GetBytes(plaintext);
            using (var stream = new MemoryStream())
            using (var gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                gzip.Write(inputStrBytes, 0, inputStrBytes.Length);
                gzip.Close();

                byte[] compressed = stream.ToArray();
                result = Convert.ToBase64String(compressed);
            }
            return result;
        }

        public string UnmaskData(string ciphertext)
        {
            string compressedString = ciphertext;
            while (char.IsNumber(compressedString[compressedString.Length - 1]))
            {
                compressedString = compressedString.Remove(compressedString.Length - 1);
                if (compressedString.Length == 0)
                {
                    return "";
                }
            }

            string decompressedString = string.Empty;
            byte[] compressedBytes = new byte[0];
            try
            {
                compressedBytes = Convert.FromBase64String(compressedString);
            }
            catch
            {
                MessageBox.Show("Could not convert string to base64.", "ERROR");
            }
            using (var compressedStream = new MemoryStream(compressedBytes))
            using (var decompressedStream = new MemoryStream())
            using (var gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                gzip.CopyTo(decompressedStream);
                byte[] decompressedBytes = decompressedStream.ToArray();
                decompressedString = Encoding.UTF8.GetString(decompressedBytes);
            }
            return decompressedString;
        }
    }
}

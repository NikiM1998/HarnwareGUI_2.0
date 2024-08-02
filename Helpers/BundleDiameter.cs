using HarnwareGUI.Services;

namespace HarnwareGUI.Helpers
{
    internal class BundleDiameter
    {
        private static OdooDatabaseService _odooDatabaseService = new OdooDatabaseService(Globals.ConnectionString);

        public static Dictionary<string, double> GetCombinedDiameter(string drawing, double radelRev)
        {
            string query = $@"
            SELECT connector, awg, SUM(count) as count
            FROM (
                SELECT conna as connector, awg, COUNT(*) as count
                FROM raw_cutting_list_harnware rclh 
                WHERE drawing = '{drawing}' AND radel_rev = '{radelRev}'
                GROUP BY conna, awg
                UNION ALL
                SELECT connb as connector, awg, COUNT(*) as count
                FROM raw_cutting_list_harnware rclh 
                WHERE drawing = '{drawing}' AND radel_rev = '{radelRev}'
                GROUP BY connb, awg
            ) combined
            GROUP BY connector, awg;";

            // Execute the query and store the results
            var data = _odooDatabaseService.ExecuteQuery(query);

            Dictionary<string, double> connectorDiameters = new Dictionary<string, double>();


            // Calculate combined AWG for each connector
            foreach (var entry in data)
            {
                string connector = entry.Key;
                var awgCounts = entry.Value;

                List<int> awgValues = new List<int>();
                List<int> counts = new List<int>();

                foreach (var awgCount in awgCounts)
                {
                    awgValues.Add(awgCount.Key);
                    counts.Add(awgCount.Value);
                }

                double combinedAWG = CalculateCombinedAWG(awgValues, counts);
                double combinedDiameterInMm = AWGToDiameterInMm(combinedAWG);

                connectorDiameters[connector] = combinedDiameterInMm;
            }

            return connectorDiameters;
        }

        public static double CalculateCombinedAWG(List<int> awgValues, List<int> counts)
        {
            double totalArea = 0;

            for (int i = 0; i < awgValues.Count; i++)
            {
                double diameterInInches = AWGToDiameter(awgValues[i]);
                double area = DiameterToArea(diameterInInches) * counts[i];
                totalArea += area;
            }

            double totalDiameterInInches = Math.Sqrt(totalArea) / 1000;
            double combinedAWG = DiameterToAWG(totalDiameterInInches);

            return combinedAWG;
        }

        public static double AWGToDiameter(int awg)
        {
            return 0.005 * Math.Pow(92, (36.0 - awg) / 39.0);
        }

        public static double DiameterToArea(double diameter)
        {
            return Math.Pow(diameter * 1000, 2);
        }

        public static double DiameterToAWG(double diameter)
        {
            return 36 - 39 * Math.Log(diameter / 0.005, 92);
        }

        public static double AWGToDiameterInMm(double awg)
        {
            double diameterInInches = AWGToDiameter((int)Math.Round(awg));
            return diameterInInches * 25.4; // Convert inches to millimeters
        }
    }
}

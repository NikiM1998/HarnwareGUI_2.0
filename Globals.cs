namespace HarnwareGUI
{
    public static class Globals
    {
        public static string? User { get; set; }

        public static int? UserId { get; set; }

        public static List<string>? AllUsers { get; set; }

        public static string ConnectionString { get; set; } = "Server=192.168.1.1;Port=5432;Database=harnware;User ID=harnware;Password=mQLwbkSDmXZZ;";

        public static string DrawingsLocation { get; set; } = @"C:\Nextcloud\HarnwareDev\Drawings";

    }
}

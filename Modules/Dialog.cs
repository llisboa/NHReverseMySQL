public static class Dialog
{
    public static string? Host { get; set; }
    public static string? Database { get; set; }
    public static string? User { get; set; }
    public static string? Password { get; set; }
    public static string? Directory { get; set; }

    public static bool GetEntries()
    {
        Console.WriteLine("MYSQL HOST(*mandatory):");
        Host = Console.ReadLine();
        if (Host == "") { return false; }

        Console.WriteLine("DATABASE NAME(*mandatory):");
        Database = Console.ReadLine();
        if (Database == "") { return false; }

        Console.WriteLine("USER(*mandatory):");
        User = Console.ReadLine();
        if (User == "") { return false; }

        Console.WriteLine("PASSWORD:");
        Password = Console.ReadLine();

        Console.WriteLine("DIRECTORY(ex: c:\\project\\classes, *mandatory):");
        Directory = Console.ReadLine();
        if (Directory  == "") { return false; }

        return true;
    }
}


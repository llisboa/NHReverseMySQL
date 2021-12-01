/*
    Program NHReverse
    Get structure from database and creating classes for use in C# projects
*/
using Newtonsoft.Json;
using ExtensionMethods;

Console.WriteLine("\n----------");
Console.WriteLine("NHReverseMySQL - NHibernate Reverse MySQL - class created from database - V01.00 - 2021");
Console.WriteLine("\nPlease:\n");


// user entries
if (!Dialog.GetEntries())
{
    return 1; // error code
}


// nhibernate instance
var db = DB.OpenSession($"Server={Dialog.Host};User Id={Dialog.User};Database={Dialog.Database};Password={Dialog.Password}");
string version = db.CreateSQLQuery("SELECT version() v").UniqueResult<string>();
Console.WriteLine($"\nVersão MySQL: {version}");

// loading structure 
List<Table> defs = Reverse.GetStructure(Dialog.Database);

// generating files
Reverse.WriteClasses(Dialog.Directory);

// end
return 0; // all ok



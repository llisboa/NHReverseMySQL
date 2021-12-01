using ExtensionMethods;
using System.IO;

public static class Reverse
{
    public static List<Table>? Defs { get; set; }

    public static List<Table> GetStructure(string? database)
    {
        var db = DB.currentSession;

        var listTabs = db?.CreateSQLQuery($"SELECT table_name FROM information_schema.tables WHERE table_schema = '{database}';").List<string>();

        Defs = new List<Table>();

        foreach (string tabDef in listTabs)
        {
            Table tab = new();
            tab.Name = tabDef;

            tab.Fields = new List<Field>();
            foreach (Object[] fldDef in db?.CreateSQLQuery($"SELECT column_name, is_nullable, data_type, character_maximum_length, numeric_precision, numeric_scale FROM information_schema.columns WHERE  table_schema = '{database}' and table_name = '{tabDef}';").List())
            {
                Field fld = new();
                fld.Name = fldDef[0].ToString();
                fld.IsNullable = (fldDef[1].ToString() == "NO") ? false : true;
                fld.DataType = fldDef[2].ToString();
                fld.CharacterMaximumLength = Convert.ToInt64(fldDef[3]?.ToString());
                fld.NumericPrecision = Convert.ToInt32(fldDef[4]?.ToString());
                fld.NumericScale = Convert.ToInt32(fldDef[5]?.ToString());
                tab.Fields.Add(fld);
            }
            Defs.Add(tab);
        };

        return Defs;
    }

    public static async void WriteClasses(string? Directory)
    {
        foreach (Table tab in Defs)
        {
            System.Text.StringBuilder tabCod = new();
            System.Text.StringBuilder tabMap = new();
            string tabSentense = tab.Name.ToSentenseCase();

            tabCod.AppendLine($"public class {tabSentense}");
            tabCod.AppendLine("{");
            tabMap.AppendLine("using NHibernate;");
            tabMap.AppendLine("using NHibernate.Mapping.ByCode;");
            tabMap.AppendLine("using NHibernate.Mapping.ByCode.Conformist;");
            tabMap.AppendLine($"\n    public class {tabSentense}Map : ClassMapping<{tabSentense}>");
            tabMap.AppendLine("    {");
            tabMap.AppendLine("    public AccountMap()");
            tabMap.AppendLine("    {");

            foreach (Field fld in tab.Fields)
            {
                string fldSentense = fld.Name.ToSentenseCase();
                string fldType = ReverseTypeForClass(fld);
                tabCod.AppendLine($"    public virtual {fldType} {fldSentense} {{ get; set; }}");

                if (fldSentense == "Id")
                {
                    tabMap.AppendLine(@"
        Id(x => x.Id, x =>
        {
            x.Generator(Generators.Increment);
            x.Type(NHibernateUtil.Int32);
            x.Column(""id"");
        });");
                }
                else
                {
                    tabMap.AppendLine(@"
        Property(b => b." + fldSentense + @", x =>
        {
            " + ReverseMapSpecifications(fld) + @"
        });");
                }

            }
            tabCod.AppendLine("}");
            tabMap.AppendLine("");
            tabMap.AppendLine($"        Table(\"{tab.Name}\");");
            tabMap.AppendLine("    }");
            tabMap.AppendLine("}");

            // write model class
            var fileCod = Directory + @"\" + tabSentense + ".cs";
            if (File.Exists(fileCod))
            {
                File.Delete(fileCod);
            }
            File.WriteAllText(fileCod.Replace(@"\\", @"\"), tabCod.ToString());

            // write map class
            var fileMap = Directory + @"\" + tabSentense + "Map.cs";
            if (File.Exists(fileMap))
            {
                File.Delete(fileMap);
            }
            File.WriteAllText(fileMap.Replace(@"\\", @"\"), tabMap.ToString());

        }

        static string ReverseTypeForClass(Field fld)
        {
            return fld.DataType switch
            {
                ("bigint") => "long",
                ("date") => "Date",
                ("datetime") => "DateTime",
                ("decimal") => "decimal",
                ("int") => "int",
                ("longtext") => "string",
                ("text") => "string",
                ("timestamp") => "DateTime",
                ("varchar") => "string",
                _ => "xxxxxx",
            } + (fld.IsNullable ? "?" : "");
        }

        static string ReverseTypeForMap(Field fld)
        {
            return fld.DataType switch
            {
                ("bigint") => "Int64",
                ("date") => "String",
                ("datetime") => "String",
                ("decimal") => "String",
                ("int") => "Int32",
                ("longtext") => "String",
                ("text") => "String",
                ("timestamp") => "String",
                ("varchar") => "String",
                _ => "xxxxxx",
            };
        }

        static string ReverseMapSpecifications(Field fld)
        {
            System.Text.StringBuilder ret = new();
            ret.AppendLine("x.Type(NHibernateUtil." + ReverseTypeForMap(fld) + ");");
            if (fld.DataType == "varchar" || fld.DataType == "text")
            {
                ret.AppendLine("            x.Length(" + fld.CharacterMaximumLength + ");");
            }
            ret.AppendLine("            x.NotNullable(" + (fld.IsNullable ? "false" : "true") + ");");
            ret.Append("            x.Column(\"" + fld.Name + "\");");
            return ret.ToString();
        }
    }
}
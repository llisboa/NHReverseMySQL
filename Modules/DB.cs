using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;

public static class DB
{

    public static ISession? currentSession {get; set;}
    public static ISession OpenSession(string connectionString)
    {
        var mapper = new ModelMapper();

        /*
        mapper.AddMappings(new List<Type>
                {
                    typeof (AccountMap),
                }
        );
        */

        HbmMapping entityMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
        var configuration = new Configuration();
        configuration.DataBaseIntegration(c =>
        {
            c.Dialect<MySQL57Dialect>();
            c.ConnectionString = connectionString;
            c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            c.SchemaAction = SchemaAutoAction.Update;
            c.LogFormattedSql = true;
            c.LogSqlInConsole = true;
        });
        configuration.AddMapping(entityMapping);
        ISessionFactory sessionFactory = configuration.BuildSessionFactory();
        currentSession = sessionFactory.OpenSession();
        return currentSession;
    }
}
using NextLevel.DBContext;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.PostgreSQL;

namespace Web.Helper
{
    internal static class ConfigurationHelper
    {
        internal static void ConfigureSerilog(IHostApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            // Initialize LoggerConfiguration
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            // Read DbProvider and ConnectionString from configuration
            var dbProvider = configuration.GetValue<DbProviderType>("DbProvider");
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configure the appropriate sink based on DbProvider
            if (dbProvider == DbProviderType.MSSQL)
            {
                loggerConfiguration.WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true });
            }
            else if (dbProvider == DbProviderType.PostgreSQL)
            {
                loggerConfiguration.WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: "Logs",
                    needAutoCreateTable: true,
                    columnOptions: new Dictionary<string, ColumnWriterBase>
                    {
                        { "message", new RenderedMessageColumnWriter() },
                        { "message_template", new MessageTemplateColumnWriter() },
                        { "level", new  LevelColumnWriter() },
                        { "timestamp", new TimestampColumnWriter() },
                        { "exception", new ExceptionColumnWriter() },
                        { "properties", new PropertiesColumnWriter() }
                    });
            }

            // Create logger
            var logger = loggerConfiguration.CreateLogger();

            // Clear existing logging providers and use Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
        }
    }
}

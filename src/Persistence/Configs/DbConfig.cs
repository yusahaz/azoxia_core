namespace Azoxia.Core.Persistence.Configs
{
    using Azoxia.Core.Configuration;
    using Azoxia.Core.Extensions;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Supported relational database providers for built-in connection string composition.
    /// </summary>
    public enum DbProvider
    {
        /// <summary>
        /// Microsoft SQL Server.
        /// </summary>
        MsSql = 10,

        /// <summary>
        /// MySQL-compatible server.
        /// </summary>
        MySql = 20,

        /// <summary>
        /// PostgreSQL.
        /// </summary>
        PostgreSQL = 30,
    }

    /// <summary>
    /// Database connection settings bound from configuration (host, credentials, provider).
    /// </summary>
    public record DbConfig :
        IConfig
    {
        #region Utils

        private List<string> BuildAdditionalParametersString()
        {
            if (Parameters == null || Parameters.Count == 0)
            {
                return [];
            }

            List<string> parameters = Parameters
                .Where(p => !p.Key.IsNullOrEmpty() && p.Value is not null)
                .Select(p => $"{p.Key}={(p.Value is bool boolValue ? boolValue.ToString().ToLowerInvariant() : p.Value.ToString())}")
                .ToList();

            return parameters;
        }

        private string GetConnectionString()
        {
            string MsSqlConnectionFactory()
            {
                if (Host.IsNullOrEmpty() || Database.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                List<string> parameters = [];

                parameters.Add($"Server={Host}{(Port > 0 ? $",{Port}" : string.Empty)}");
                parameters.Add($"Initial Catalog={Database}");

                if (!Username.IsNullOrEmpty() && !Password.IsNullOrEmpty())
                {
                    parameters.Add($"User ID={Username}");
                    parameters.Add($"Password={Password}");
                }

                parameters.AddRange(BuildAdditionalParametersString());

                return parameters.Join(';');
            }

            string MysqlConnectionFactory()
            {
                if (Host.IsNullOrEmpty() || Database.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                List<string> parameters = [];

                parameters.Add($"Server={Host}");
                parameters.Add($"Port={Port}");
                parameters.Add($"Database={Database}");

                if (!Username.IsNullOrEmpty() && !Password.IsNullOrEmpty())
                {
                    parameters.Add($"Uid={Username}");
                    parameters.Add($"Pwd={Password}");
                }

                parameters.AddRange(BuildAdditionalParametersString());

                return parameters.Join(';');
            }

            string PostgreSQLConnectionFactory()
            {
                if (Host.IsNullOrEmpty() || Database.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                List<string> parameters = [];

                parameters.Add($"Host={Host}");
                parameters.Add($"Port={Port}");
                parameters.Add($"Database={Database}");

                if (!Username.IsNullOrEmpty() && !Password.IsNullOrEmpty())
                {
                    parameters.Add($"Username={Username}");
                    parameters.Add($"Password={Password}");
                }

                parameters.AddRange(BuildAdditionalParametersString());

                return parameters.Join(';');
            }

            return Provider switch
            {
                DbProvider.MsSql => MsSqlConnectionFactory(),
                DbProvider.MySql => MysqlConnectionFactory(),
                DbProvider.PostgreSQL => PostgreSQLConnectionFactory(),
                _ => throw new NotSupportedException(),
            };
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Gets the composed ADO.NET-style connection string for <see cref="Provider"/>.
        /// </summary>
        public string ConnectionString => GetConnectionString();

        /// <summary>
        /// Gets or sets the database catalog name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the database server host name or address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets optional provider-specific key/value parameters appended to the connection string.
        /// </summary>
        public List<ConnectionParameter> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the database password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the TCP port (0 uses provider defaults where applicable).
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the database provider.
        /// </summary>
        public DbProvider Provider { get; set; } = DbProvider.MsSql;

        /// <summary>
        /// Gets or sets the database user name.
        /// </summary>
        public string Username { get; set; }

        #endregion Properties

        #region Nested

        /// <summary>
        /// Additional key/value pair for the connection string builder.
        /// </summary>
        public class ConnectionParameter
        {
            #region Properties

            /// <summary>
            /// Gets or sets the parameter key (left-hand side before '=').
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the parameter value (strings, booleans, or other serializable values).
            /// </summary>
            public object Value { get; set; }

            #endregion Properties
        }

        #endregion Nested
    }
}

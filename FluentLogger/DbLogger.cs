﻿using FluentLogger.Helpers;
using System;
using System.Data;

namespace FluentLogger
{

    public abstract class DbLogger : BaseLogger
    {
        protected readonly IDbConnection connection;
        protected string TableName { get; private set; } = "errors";

        protected DbLogger(IDbConnection connection, LogLevel minLevel=LogLevel.Info) : base(minLevel)
        {
            this.connection = connection;
        }
        protected DbLogger(IDbConnection connection, LogLevel minLevel = LogLevel.Info, string tableName="errors"): this(connection, minLevel)
        {
            TableName = tableName;
        }
        public void Init()
        {
            if (!TableExists()) CreateTable();
        }

        public abstract bool TableExists();

        public abstract void CreateTable();
        

    public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"insert into {TableName} values(@timestamp, @level, @message, @exception)";
            cmd.AddParameter("id", Guid.NewGuid().ToString());
            cmd.AddParameter("level", level.ToString());
            cmd.AddParameter("message", message);
            cmd.AddParameter("exception", ex);
            var objects = Serialize(objectsToSerialize);
            cmd.AddParameter("objects", objects);
            cmd.AddParameter("timestamp", DateTime.UtcNow);

            cmd.ExecuteNonQuery();
        }

    }
}

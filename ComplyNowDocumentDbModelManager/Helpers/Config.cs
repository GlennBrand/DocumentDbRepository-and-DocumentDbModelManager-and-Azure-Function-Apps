using System;
using System.Configuration;

namespace ComplyNowDocumentDbModelManager.Helpers
{
    public static class DocumentDbConfig
    {
        public static string DocDbEndpoint
        {
            get
            {
                var endPoint = GetEnvironmentVariable("docdb.endpoint");
                return endPoint;
            }
        }

        public static string DocDbAuth
        {
            get
            {
                var authorizationKey = GetEnvironmentVariable("docdb.auth");
                return authorizationKey;
            }
        }

        public static string DocDbDatabase
        {
            get
            {
                var doumentDbDatabase = GetEnvironmentVariable("docdb.database");
                return doumentDbDatabase;
            }
        }

        public static string DeviceCollection
        {
            get
            {
                var deviceCollection = GetEnvironmentVariable("DeviceCollection");
                return deviceCollection;
            }
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}

namespace DistributedCache.Models
{
    public class TenantProfileModel
    {

        #region Tenant information

        public string Id { get; set; }
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientUri { get; set; }

        #endregion Tenant information

        #region Sql Server

        public string SqlServer { get; set; }
        public string SqlDatabase { get; set; }
        public string SqlUserName { get; set; }
        public string SqlPassword { get; set; }

        public string GetSqlConnectionString(string templete)
        {
            return string.Format(templete, SqlServer.Replace("@", "\\"), SqlDatabase, SqlUserName, SqlPassword);
        }

        #endregion Sql Server

        #region MongoDB

        public string MongoDbServer { get; set; }
        public string MongoDbDatabase { get; set; }
        public string MongoDbUserName { get; set; }
        public string MongoDbPassword { get; set; }

        public string GetMongoDBConnectionString()
        {
            if (string.IsNullOrEmpty(MongoDbUserName))
            {
                return $"mongodb://{MongoDbServer}/{MongoDbDatabase}";
            }

            return $"mongodb://{MongoDbUserName}:{MongoDbPassword}@{MongoDbServer}/{MongoDbDatabase}";
        }
        #endregion
    }
}

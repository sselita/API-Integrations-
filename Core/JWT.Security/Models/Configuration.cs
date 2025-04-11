using System.Collections.Generic;
using System.Xml.Serialization;
namespace JWT.Security.Models
{
    public class Configuration
    {
        public Configuration()
        {
            TokenModel = new TokenModel();
            SQLConnection = new SQLConnection();
            FolderConfigModel = new FolderConfigModel();
            Users = new List<User>();
            ErpUser = new ErpUser();
        }
        public string DefaultInventoryCode { get; set; }
        public string CompanyName { get; set; }
        public string NewServiceName { get; set; }
        public string OldServiceName { get; set; }
        public SQLConnection SQLConnection { get; set; }
        public TokenModel TokenModel { get; set; }
        public FolderConfigModel FolderConfigModel { get; set; }
        public List<User> Users { get; set; }
        public ErpUser ErpUser { get; set; }


    }

    public partial class SQLConnection
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }

    public partial class TableGUIDS
    {
        [XmlAttribute()]
        public int ID { get; set; }
        [XmlElement()]
        public string TableName { get; set; }
        [XmlElement()]
        public string GUID { get; set; }
    }
    public partial class TokenModel
    {
        public string SigningKey { get; set; }
        public string TokenIssuer { get; set; }
        public string TokenAudience { get; set; }
        public string TokenLifetimeInMinutes { get; set; }
        public string ClientSettingsProviderServiceUri { get; set; }
    }

    public partial class FolderConfigModel
    {
        //Log paths (Used for Error responses.)
        public string LogFolderPath { get; set; }
        public string FolderName { get; set; }
    }
    public partial class User
    {
        public long UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }


    }
    public partial class ErpUser
    {
        public string Username { get; set; }
        public string Domain { get; set; }
        public string ErpPassword { get; set; }
    }

}
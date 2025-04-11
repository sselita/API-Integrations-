using JWT.Security.Models;
using System;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Generic;
using RestWebApi.Models;

namespace RestWebApi.Services.Helpers
{
    public class ManageResponseHelperService : IManageResponseHelperService
    {
        //Global variable for Configuration
        private Configuration conf = CommonHelperService.GetJsonFileData();
        private static SqlConnection sqlConnection;
        private static List<TableGUIDS> GUIDS = new List<TableGUIDS>();
        private readonly ISQLConnectionHelperService _SQLConnectionHelperService;

        public ManageResponseHelperService(ISQLConnectionHelperService SQLConnectionHelperService)
        {
            _SQLConnectionHelperService = SQLConnectionHelperService;
        }

        /// <summary>
        /// Initializes connection with SQL.
        /// </summary>
        public void InitializeConnection()
        {
            sqlConnection = new SqlConnection(_SQLConnectionHelperService.GenerateConnectionString());
        }

        /// <summary>
        /// Method used for Creating folder if does not exists
        /// </summary>
        /// <param name="filePath"></param>
        private void CreateFolder(out string filePath)
        {
            string subPath = conf.FolderConfigModel.LogFolderPath + "\\" + conf.FolderConfigModel.FolderName;


            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);

            }

            filePath = subPath + "\\";
        }

        /// <summary>
        /// Write structuret Log in a text File.
        /// </summary>
        /// <param name="strLog">Log string value</param>
        /// <param name="methodname">Gets method name as parameter</param>
        /// <param name="innermsg">Inner Exception</param>
        public void WriteLog(string strLog, string methodname, string innermsg = "")
        {
            StreamWriter log;
            FileStream fileStream;
            DirectoryInfo logDirInfo;
            FileInfo logFileInfo;

            string filePath = "";

            CreateFolder(out filePath);

            string logFilePath = filePath;
            logFilePath = logFilePath + "ErrorLogs-" + methodname + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) 
                logDirInfo.Create();
            
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine("------------------------------------------------------------------------------------");
            log.WriteLine(strLog + " | " + innermsg + " | " + Convert.ToString(DateTime.Now));
            log.WriteLine("------------------------------------------------------------------------------------");
            log.Close();
        }

        /// <summary>
        /// Method for writing logs in BC.
        /// </summary>
        /// <param name="strLog"></param>
        /// <param name="methodname"></param>
        /// <param name="innermsg"></param>
        public void WriteBCLogs(ErpResponse baseResponse,string CompanyName)
        {
            InitializeConnection();

            _SQLConnectionHelperService.ExecuteApiNonQuery(sqlConnection,BCExtension.PostLogsProcedure, BCExtension.LogTable, CompanyName, baseResponse);
        }
    }
}
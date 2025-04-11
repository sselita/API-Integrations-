namespace RestWebApi.Services.Helpers
{
    public interface IManageResponseHelperService
    {
        void InitializeConnection();
        void WriteLog(string strLog, string methodname, string innermsg = "");
        void WriteBCLogs(Models.ErpResponse baseResponse,string CompanyName);
    }
}
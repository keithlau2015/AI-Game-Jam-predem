using System.Threading.Tasks;
namespace BugReportSystem
{
    internal interface IReporter
    {
        Task<bool> SendReport(BugReport bugReport);
    }
}
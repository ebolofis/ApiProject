using Symposium.Models.Models;
using Symposium.Models.Models.DahuaRecorder;


namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DahuaRecorder
{
    public interface IPosRecorderTasks
    {
        bool MakeConnection();
        bool SendMessage(DBInfoModel dbInfo, PosRecorderModel Model);
    }
}

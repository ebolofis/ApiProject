using Symposium.Models.Models;
using Symposium.Models.Models.DahuaRecorder;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DahuaRecorder
{
    public interface IPosRecorderFlows
    {
        bool MakeConnection();
        bool SendMessage(DBInfoModel dbInfo, PosRecorderModel Model);
    }
}

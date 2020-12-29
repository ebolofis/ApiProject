using Symposium.Models.Models;
using Symposium.Models.Models.DahuaRecorder;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DahuaRecorder;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DahuaRecorder;


namespace Symposium.WebApi.MainLogic.Flows.DahuaRecorder
{

    public class PosRecorderFlows : IPosRecorderFlows
    {

        IPosRecorderTasks PosRecordertasks;

        public PosRecorderFlows(IPosRecorderTasks posRecordertasks)
        {
            this.PosRecordertasks = posRecordertasks;
        }

        public bool MakeConnection()
        {
            return PosRecordertasks.MakeConnection();
        }

        public bool SendMessage(DBInfoModel dbInfo, PosRecorderModel Model)
        {
            return PosRecordertasks.SendMessage(dbInfo, Model);
        }
    }
}

using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.DahuaRecorder;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DahuaRecorder;
using System;

namespace Symposium.WebApi.MainLogic.Tasks.DahuaRecorder
{

    public class PosRecorderTasks : IPosRecorderTasks
    {
        PosRecorderHelper recorderHelper = new PosRecorderHelper();
        string XvrIp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "XvrIpAddress");
        string XvrPort = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "XvrPosPort");


        public PosRecorderTasks()
        {
        }

        public bool MakeConnection()
        {
            bool res = true;
            recorderHelper.InitializeConnection(XvrIp, XvrPort);
            return res;
        }

        public bool SendMessage(DBInfoModel dbInfo, PosRecorderModel Model)
        {
            bool res = false;
            PosRecorderModel credentials = new PosRecorderModel();
            try
            {
                if (Model != null)
                {
                    recorderHelper.SendMessageToServer(Model);
                    res = true;
                }
                else
                {
                    res = false;
                }
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message, ex);
            }
            return res;
        }
    }
}


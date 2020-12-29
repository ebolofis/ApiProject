using Symposium.Helpers;
using Symposium.Models.Models.Configuration;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Configuration
{
    [RoutePrefix("api/v3/Configuration")]
    public class ConfigurationController : BasicV3Controller
    {
        IConfigurationFlows configurationFlows;

        public ConfigurationController(IConfigurationFlows _configurationFlows)
        {
            this.configurationFlows = _configurationFlows;
        }

        /// <summary>
        /// Refresh configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("RefreshConfiguration")]
        [Authorize]
        public HttpResponseMessage RefreshConfiguration()
        {
            try
            {
                configurationFlows.RefreshConfiguration();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Get configuration based on key from dictionary
        /// </summary>
        /// <param name="key">Key of configuration in dictionary</param>
        /// <returns>Configuration as dictionary</returns>
        [HttpGet, Route("GetConfiguration/Key/{key}")]
        [Authorize]
        public HttpResponseMessage GetConfiguration(string key)
        {
            Dictionary<string, dynamic> configuration;
            try
            {
                configuration = configurationFlows.GetConfiguration(key);
            }
            catch (BusinessException ex)
            {
                logger.Warn(ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, configuration);
        }

        /// <summary>
        /// Get All configuration (MainConfigurationDictionary)
        /// </summary>
        /// <returns>MainConfigurationDictionary</returns>
        [HttpGet, Route("GetAllConfiguration")]
        [Authorize]
        public HttpResponseMessage GetAllConfiguration()
        {
            Dictionary<string, Dictionary<string, dynamic>> MainConfigurationDictionary;
            try
            {
                MainConfigurationDictionary = configurationFlows.GetAllConfiguration();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, MainConfigurationDictionary);
        }

        /// <summary>
        /// Get All Descriptors configuration
        /// </summary>
        /// <returns>All Descriptors Configuration</returns>
        [HttpGet, Route("GetAllDescriptors")]
        [Authorize]
        public HttpResponseMessage GetAllDescriptors()
        {
            Dictionary<string, Dictionary<string, List<DescriptorsModel>>> MainDescriptorsDictionary = new Dictionary<string, Dictionary<string, List<DescriptorsModel>>>();
            try
            {
                MainDescriptorsDictionary = configurationFlows.GetAllDescriptors();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, MainDescriptorsDictionary);
        }

        /// <summary>
        /// Get All Configuration From BO
        /// </summary>
        /// <param name="NewMainConfigurationDictionary">New Configuration Model</param>
        /// <returns>true for successful update or false for fail</returns>
        [HttpPost, Route("SaveAllChanges")]
        [Authorize]
        public HttpResponseMessage SaveAllChanges(Dictionary<string, Dictionary<string, dynamic>> NewMainConfigurationDictionary)
        {
            bool res = false;
            try
            {
                res = configurationFlows.SaveAllChanges(NewMainConfigurationDictionary);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Save Specific Pos Configuration
        /// </summary>
        /// <param name="PosJsonConfigurationDictionary"></param>
        /// <returns>true for successful update or false for fail</returns>
        [HttpPost, Route("SaveSpecificPos")]
        [Authorize]
        public HttpResponseMessage SaveSpecificPos(Dictionary<string, Dictionary<string, dynamic>> PosJsonConfigurationDictionary)
        {
            bool res = false;
            try
            {
                res = configurationFlows.SaveSpecificPos(PosJsonConfigurationDictionary);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        [HttpGet, Route("GetAllPosInfoConfig")]
        [Authorize]
        public HttpResponseMessage GetAllPosInfoConfig()
        {
            List<string> configurationList = new List<string>();
            try
            {
                configurationList = configurationFlows.GetAllPosInfoConfig(DBInfo);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, configurationList);
        }

        /// <summary>
        /// Add Pos Configiguration File
        /// </summary>
        /// <param name="posJsonFile"></param>
        /// <returns>true for success or false for fail</returns>
        [HttpGet, Route("AddPosConfig/posJsonFile/{posJsonFile}")]
        [Authorize]
        public HttpResponseMessage AddPosConfig(string posJsonFile)
        {
            bool res = false;
            try
            {
                res = configurationFlows.AddPosConfig(posJsonFile);
            }
            catch (Exception ex)
            {
                res = false;
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Selected Pos Configiguration File
        /// </summary>
        /// <param name="PosConfigFile"></param>
        /// <returns>true for success or false for fail</returns>
        [HttpGet, Route("DeletePosConfig/PosConfigFile/{PosConfigFile}")]
        [Authorize]
        public HttpResponseMessage DeletePosConfig(string PosConfigFile)
        {
            bool res = false;
            try
            {
                res = configurationFlows.DeletePosConfig(DBInfo, PosConfigFile);
            }
            catch (Exception ex)
            {
                res = false;
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get pos configuration file names
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllPosFiles")]
        [Authorize]
        public HttpResponseMessage GetAllPosFiles()
        {
            List<string> PosFiles = new List<string>();
            try
            {
                PosFiles = configurationFlows.GetAllPosFiles();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, PosFiles);
        }

        /// <summary>
        /// Get store id and first pos id
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetStorePos")]
        [Authorize]
        public HttpResponseMessage GetStorePos()
        {
            DA_GetStorePosModel res = configurationFlows.GetStorePos(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get cancel statuses
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetCancelStatus")]
        [Authorize]
        public HttpResponseMessage GetCancelStatus()
        {
            List<string> res = configurationFlows.GetCancelStatus();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}

using apiHelpDesk.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiHelpDesk.Controllers
{
    public class CheckBdController : ApiController
    {
        [HttpGet]
        [Route("check/checkbd/mysqlconnectioncheckbd")]
        public clsApiStatus mysqlConnectionCheckBd()
        {
            // -------------------------------
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();

            // -------------------------------
            clsCheckBd objCheckBd = new clsCheckBd();
            objCheckBd.checkBd();

            // Validar resultado de la ejecucino
            if (objCheckBd.flag == 1)
                objRespuesta.statusExec = true;
            else
                objRespuesta.statusExec = false;
            objRespuesta.flag = objCheckBd.flag;
            objRespuesta.msg = objCheckBd.statusMsg;
            jsonResp.Add("msgData", objCheckBd.statusMsg);
            objRespuesta.datos = jsonResp;
            return objRespuesta;
            //<


        }

    }
}

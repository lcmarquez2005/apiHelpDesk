using apiHelpDesk.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Web.Http;

namespace apiHelpDesk.Controllers
{
    [RoutePrefix("api/acceso")]
    public class AccesoController : ApiController
    {
        // ============================================
        // POST api/acceso/login
        // ============================================

        [HttpPost]
        [Route("login")]
        public clsApiStatus Login([FromBody] clsUsuario modelo)
        {
            clsApiStatus objRespuesta = new clsApiStatus();

            try
            {
                clsUsuario objUsuario =
                    new clsUsuario(modelo.usuario, modelo.password);

                DataSet ds = objUsuario.Login();

                // Usuario encontrado y password correcto
                if (ds.Tables[0].Rows.Count > 0)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "Login correcto";
                    objRespuesta.flag = 1;

                    objRespuesta.datos = JObject.FromObject(ds);
                }
                else
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.msg = "Usuario o contraseña incorrectos";
                    objRespuesta.flag = 0;
                }
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.flag = -1;
            }

            return objRespuesta;
        }
    }
}
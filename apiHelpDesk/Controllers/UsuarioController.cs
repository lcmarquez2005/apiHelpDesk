using apiHelpDesk.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiHelpDesk.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpPost]
        [Route("full/usuario/spinsusuario")]
        public clsApiStatus spInsUsuario([FromBody] clsUsuario modelo)
        {
            // -----------------------
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            // -----------------------
            try
            {
                // Creación del objeto, en base al Modelo
                clsUsuario objUsuario = new clsUsuario(modelo.nombre_completo,
                                                       modelo.correo,
                                                       modelo.usuario,
                                                       modelo.password,
                                                       modelo.rol,
                                                       modelo.activo );
                DataSet ds = new DataSet();
                // Ejecución del Método del Modelo (y recepción de datos)
                ds = objUsuario.spInsUsuario();

                // Validación de que la BD devolvió resultados
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "Usuario registrado exitosamente !";
                    objRespuesta.flag = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    jsonResp.Add("msgData", "Usuario registrado exitosamente !");
                }
                else
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.msg = "El procedimiento no devolvió resultados.";
                    objRespuesta.flag = 0;
                    jsonResp.Add("msgData", "DataSet vacío o sin tablas.");
                }
                
                objRespuesta.datos = jsonResp;
            }
            catch (Exception e)
            {
                // Configuración del objeto de salida
                objRespuesta.statusExec = false;
                objRespuesta.msg = "Usuario NO registrado ...";
                objRespuesta.flag = -1;
                jsonResp.Add("msgData", e.Message.ToString());
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }
        // Endpoint para consulta de usuarios por tipo vwRptUsuario
        [HttpGet]
        [Route("full/usuario")]
        public clsApiStatus vwTipoUsuario()
        {
            // -----------------------
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            // -----------------------
            DataSet ds = new DataSet();
            try
            {
                clsUsuario objUsuario = new clsUsuario();
                ds = objUsuario.vwUsuario();

                // Configuración del objeto de salida
                objRespuesta.statusExec = true;
                objRespuesta.flag = ds.Tables[0].Rows.Count;
                objRespuesta.msg = "Consulta de usuarios " + "realizada exitosamente";

                // Migración del ds(DataSet) al objeto Json
                string jsonString = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                jsonResp = JObject.Parse($"{{\"{ds.Tables[0].TableName}\": {jsonString}}}");

                // DataSet migrado, se envía clsApiStatus
                objRespuesta.datos = jsonResp;
            }
            catch (Exception ex)
            {
                // Configuración del objeto de salida
                objRespuesta.statusExec = false;
                objRespuesta.msg =
                    "Fallo en consulta de reporte - Usuarios ...";
                objRespuesta.flag = -1;
                jsonResp.Add("msgData", ex.Message.ToString());
                objRespuesta.datos = jsonResp;
            }
            // Salida  del objeto configurado
            return objRespuesta;
        }



    }
}

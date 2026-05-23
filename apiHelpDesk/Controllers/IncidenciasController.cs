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
    public class IncidenciasController : ApiController
    {
        [HttpPost]
        [Route("full/incidencias")]
        public clsApiStatus spRegistrarIncidencia([FromBody] clsIncidencias modelo)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();

            try
            {
                // Creación del objeto basado en el modelo recibido
                // El constructor asegura que se respeten los tipos (incluyendo enums)
                clsIncidencias objIncidencia = new clsIncidencias(
                    modelo.idUsuario,
                    modelo.titulo,
                    modelo.descripcion,
                    modelo.categoria,
                    modelo.prioridad,
                    modelo.status,
                    modelo.observaciones
                );

                // Ejecución del método del modelo
                DataSet ds = objIncidencia.spRegistrarIncidencia();

                // Validación de resultados de la BD
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "Incidencia registrada exitosamente!";
                    objRespuesta.flag = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    jsonResp.Add("msgData", "Incidencia registrada correctamente.");
                }
                else
                {
                    // Algunos SP de registro pueden no devolver una tabla si no se configura un SELECT
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "Incidencia registrada (sin confirmación de ID).";
                    objRespuesta.flag = 1;
                    jsonResp.Add("msgData", "El registro se completó pero la BD no devolvió un ID.");
                }
                objRespuesta.datos = jsonResp;
            }
            catch (Exception e)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = "Error al registrar la incidencia.";
                objRespuesta.flag = -1;
                jsonResp.Add("msgData", e.Message);
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }
    }
}

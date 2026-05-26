using apiHelpDesk.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Web.Http;

namespace apiHelpDesk.Controllers
{
    [RoutePrefix("api/incidencias")]
    public class IncidenciasController : ApiController
    {
        [HttpPost]
        [Route("")]
        public clsApiStatus RegistrarIncidencia(clsIncidencias model)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            try
            {
                DataSet ds = model.spRegistrarIncidencia();
                objRespuesta.statusExec = true;
                objRespuesta.msg = "Incidencia registrada correctamente";
                objRespuesta.flag = 1;
                objRespuesta.datos = JObject.FromObject(ds);
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.flag = 0;
            }
            return objRespuesta;
        }

        [HttpGet]
        [Route("")]
        public clsApiStatus ObtenerIncidencias()
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            try
            {
                clsIncidencias obj = new clsIncidencias();
                DataSet ds = obj.vwPanelIncidencias();

                objRespuesta.statusExec = true;
                objRespuesta.msg = "Consulta exitosa";
                objRespuesta.flag = 1;
                objRespuesta.datos = JObject.FromObject(ds);
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.flag = 0;
            }
            return objRespuesta;
        }

        [HttpGet]
        [Route("{id}")]
        public clsApiStatus ObtenerIncidencia(int id)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            try
            {
                clsIncidencias obj = new clsIncidencias();
                DataSet ds = obj.ObtenerIncidenciaPorId(id);

                objRespuesta.statusExec = true;
                objRespuesta.msg = "Consulta exitosa";
                objRespuesta.flag = 1;
                objRespuesta.datos = JObject.FromObject(ds);
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.flag = 0;
            }
            return objRespuesta;
        }

        [HttpPut]
        [Route("{id}/estado")]
        public clsApiStatus ActualizarEstado(int id, [FromBody] clsIncidencias model)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            try
            {
                model.id = id;
                DataSet ds = model.ActualizarEstadoIncidencia();

                // Validar el código de status devuelto por el procedimiento almacenado
                int statusCode = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    statusCode = Convert.ToInt32(ds.Tables[0].Rows[0]["status"]);
                }

                if (statusCode == 200)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "Estado actualizado correctamente";
                    objRespuesta.flag = 200;
                }
                else if (statusCode == 404)
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.msg = "La incidencia con ID " + id + " no existe.";
                    objRespuesta.flag = 404;
                }
                else if (statusCode == 400)
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.msg = "Estado no válido. Solo se permite: Abierta, En proceso, Cerrada.";
                    objRespuesta.flag = 400;
                }
                else
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.msg = "Error desconocido al actualizar el estado.";
                    objRespuesta.flag = statusCode;
                }

                objRespuesta.datos = JObject.FromObject(ds);
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.flag = 0;
            }
            return objRespuesta;
        }
    }
}
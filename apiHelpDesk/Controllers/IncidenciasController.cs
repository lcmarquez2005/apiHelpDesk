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

                objRespuesta.statusExec = true;
                objRespuesta.msg = "Estado actualizado correctamente";
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
    }
}
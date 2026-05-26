using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace apiHelpDesk.Models
{
    public enum CategoriaIncidencia { Acceso, Sistema, Equipo, Red, Otro }
    public enum PrioridadIncidencia { Baja, Media, Alta }
    public enum StatusIncidencia { Abierta, En_Proceso, Cerrada }

    public class clsIncidencias
    {
        private string cadConn = ConfigurationManager.ConnectionStrings["bdHelpDeskAWS"].ConnectionString;

        public int id { get; set; }
        public int idUsuario { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public CategoriaIncidencia categoria { get; set; }
        public PrioridadIncidencia prioridad { get; set; }
        public StatusIncidencia status { get; set; }
        public string observaciones { get; set; }

        public clsIncidencias() { }

        public clsIncidencias(int idUsuario, string titulo, string descripcion, CategoriaIncidencia categoria, PrioridadIncidencia prioridad, StatusIncidencia status, string observaciones)
        {
            this.idUsuario = idUsuario;
            this.titulo = titulo;
            this.descripcion = descripcion;
            this.categoria = categoria;
            this.prioridad = prioridad;
            this.status = status;
            this.observaciones = observaciones;
        }

        public DataSet spRegistrarIncidencia()
        {
            // 1. Validar que el usuario existe antes de intentar el registro
            clsUsuario objUsuario = new clsUsuario();
            if (!objUsuario.existeUsuario(this.idUsuario))
            {
                throw new Exception("El ID de usuario " + this.idUsuario + " no existe en la base de datos.");
            }

            // Ajuste para el valor 'En proceso' que contiene un espacio en la BD
            string estadoDB = this.status.ToString().Replace("_", " ");

            // Creación del comando SQL corregido utilizando las propiedades de la instancia   
            string cadSql = "CALL spRegistrarIncidencia("
                                               + this.idUsuario + ", '"
                                               + this.titulo + "', '"
                                               + this.descripcion + "', '"
                                               + this.categoria + "', '"
                                               + this.prioridad + "', '"
                                               + estadoDB + "', '"
                                               + this.observaciones + "');";

            try
            {
                // Configuración de los objetos de conexión a MySQL
                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cadSql, cnn);
                    DataSet ds = new DataSet();
                    // Ejecución del Adaptador de Datos
                    da.Fill(ds, "spRegistrarIncidencia");
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BD (Incidencias): " + ex.Message);
            }
        }

        public DataSet vwPanelIncidencias()
        {
            string cadSQL = "select * from vwPanelIncidencias";

            MySqlConnection cnn = new MySqlConnection(cadConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
            DataSet ds = new DataSet();
            da.Fill(ds, "vwPanelIncidencias");
            return ds;
        }

        public DataSet vwMetricasEstado()
        {
            string cadSQL = "select * from vwMetricasEstado";

            MySqlConnection cnn = new MySqlConnection(cadConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
            DataSet ds = new DataSet();
            da.Fill(ds, "vwMetricasEstado");
            return ds;
        }

        public DataSet ObtenerIncidenciaPorId(int idIncidencia)
        {
            // Corregido con parámetros seguros
            string cadSQL = "SELECT * FROM vwPanelIncidencias WHERE id_incidencia = ?idIncidencia";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            {
                cmd.Parameters.AddWithValue("?idIncidencia", idIncidencia);

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "incidencia");
                    return ds;
                }
            }
        }

        public DataSet ActualizarEstadoIncidencia()
        {
            string estadoDB = this.status.ToString().Replace("_", " ");

            // Uso del procedimiento almacenado spActualizarEstadoIncidencia
            string cadSQL = "CALL spActualizarEstadoIncidencia(" 
                            + this.id + ", '" 
                            + estadoDB + "', '" 
                            + this.observaciones + "');";

            try
            {
                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "updateEstado");
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BD (Actualizar Estado): " + ex.Message);
            }
        }
    }
}
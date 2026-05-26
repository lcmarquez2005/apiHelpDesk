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
        private string cadConn = ConfigurationManager.ConnectionStrings["bdHelpDesk"].ConnectionString;

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

            string estadoDB = this.status.ToString().Replace("_", " ");

            try
            {
                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    // Usamos parámetros (?nome) para evitar inyección SQL al llamar al procedimiento
                    string cadSql = "CALL spRegistrarIncidencia(?p_id_usuario, ?p_titulo, ?p_descripcion, ?p_categoria, ?p_prioridad, ?p_estado, ?p_observaciones);";

                    using (MySqlCommand cmd = new MySqlCommand(cadSql, cnn))
                    {
                        cmd.Parameters.AddWithValue("?p_id_usuario", this.idUsuario);
                        cmd.Parameters.AddWithValue("?p_titulo", this.titulo);
                        cmd.Parameters.AddWithValue("?p_descripcion", this.descripcion);
                        cmd.Parameters.AddWithValue("?p_categoria", this.categoria.ToString());
                        cmd.Parameters.AddWithValue("?p_prioridad", this.prioridad.ToString());
                        cmd.Parameters.AddWithValue("?p_estado", estadoDB);
                        cmd.Parameters.AddWithValue("?p_observaciones", this.observaciones);

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "spRegistrarIncidencia");
                            return ds;
                        }
                    }
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

            // Añadido 'using' para asegurar el cierre de la conexión
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn))
            {
                DataSet ds = new DataSet();
                da.Fill(ds, "vwPanelIncidencias");
                return ds;
            }
        }

        public DataSet vwMetricasEstado()
        {
            string cadSQL = "select * from vwMetricasEstado";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn))
            {
                DataSet ds = new DataSet();
                da.Fill(ds, "vwMetricasEstado");
                return ds;
            }
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

            // Corregido con parámetros seguros
            string cadSQL = @"UPDATE incidencias 
                              SET estado = ?estado, observaciones = ?observaciones 
                              WHERE id_incidencia = ?id;";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            {
                cmd.Parameters.AddWithValue("?estado", estadoDB);
                cmd.Parameters.AddWithValue("?observaciones", this.observaciones);
                cmd.Parameters.AddWithValue("?id", this.id);

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "updateEstado");
                    return ds;
                }
            }
        }
    }
}
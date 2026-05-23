using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace apiHelpDesk.Models
{
    // Definición de Enums para restringir los valores posibles según la base de datos
    public enum CategoriaIncidencia
    {
        Acceso, Sistema, Equipo, Red, Otro
    }

    public enum PrioridadIncidencia
    {
        Baja, Media, Alta
    }

    public enum StatusIncidencia
    {
        Abierta,
        En_Proceso, // Se convertirá a 'En proceso' para la base de datos
        Cerrada
    }

    public class clsIncidencias
    {
        // Definición de cadena de Conexión
        private string cadConn = ConfigurationManager.
                    ConnectionStrings["bdHelpDeskAWS"].
                    ConnectionString;

        public int id { get; set; } // es autoincremental en la BD
        public int idUsuario { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public CategoriaIncidencia categoria { get; set; } // Enum restringido
        public PrioridadIncidencia prioridad { get; set; } // Enum restringido
        public StatusIncidencia status { get; set; }       // Enum restringido
        public string observaciones { get; set; }

        // Constructor vacío (necesario para la serialización/deserialización de Web API)
        public clsIncidencias() { }

        // Constructor que obliga a respetar los enums mediante el tipado fuerte
        public clsIncidencias(
                                int idUsuario,
                                string titulo,
                                string descripcion,
                                CategoriaIncidencia categoria,
                                PrioridadIncidencia prioridad,
                                StatusIncidencia status,
                                string observaciones)
        {
            this.idUsuario = idUsuario;
            this.titulo = titulo;
            this.descripcion = descripcion;
            this.categoria = categoria;
            this.prioridad = prioridad;
            this.status = status;
            this.observaciones = observaciones;
        }


        /* Procedimiento para registrar una nueva incidencia 
    IN p_id_usuario INT,
    IN p_titulo VARCHAR(120),
    IN p_descripcion TEXT,
    IN p_categoria ENUM('Acceso', 'Sistema', 'Equipo', 'Red', 'Otro'),
    IN p_prioridad ENUM('Baja', 'Media', 'Alta'),
    IN p_estado ENUM('Abierta', 'En proceso', 'Cerrada'),
    IN p_observaciones TEXT         */
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


        /*
         * Devuelve el resultado de la consulta a la vista vwPanelIncidencias 
         * */
        public DataSet vwPanelIncidencias()
        {
            // Crear el comando SQL
            string cadSQL = "select * from vwPanelIncidencias";
            // Configuración de objetos de conexión
            MySqlConnection cnn = new MySqlConnection(cadConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
            DataSet ds = new DataSet();
            // Ejecución y salida
            da.Fill(ds, "vwPanelIncidencias");
            return ds;
        }

        /* Devuelve el resultado de la consulta a la vista vw_metricas_estado
         * */
        public DataSet vwMetricasEstado()
        {
            // Crear el comando SQL
            string cadSQL = "select * from vwMetricasEstado";
            // Configuración de objetos de conexión
            MySqlConnection cnn = new MySqlConnection(cadConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
            DataSet ds = new DataSet();
            // Ejecución y salida
            da.Fill(ds, "vwMetricasEstado");
            return ds;
        }
    }
}

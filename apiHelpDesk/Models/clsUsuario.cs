using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace apiHelpDesk.Models
{
    public class clsUsuario
    {
        // Definición de atributos
        public int id { get; set; } // Cambiado a int (autoincremental)
        public string nombre_completo { get; set; }
        public string correo { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public string password_hash { get; set; }
        public string rol { get; set; }
        public int activo { get; set; }

        /* id usuario
            nombre_completo
            correo_institucional
            usuario
            password_hash
            rol
            activo
            fecha_registro
        */

        // Definición de cadena de Conexión
        private string cadConn = ConfigurationManager.
                    ConnectionStrings["bdHelpDeskAWS"].
                    ConnectionString;

        // Definición de Constructores del Modelo
        public clsUsuario()
        {
            // Código de inicialización posterior ...        
        }
        public clsUsuario(string usuario,
                          string password)
        {
            this.usuario = usuario;
            this.password = password;
        }
        public clsUsuario(string nombre_completo,
                          string correo,
                          string usuario,
                          string password,
                          string rol,
                          int activo)
        {
            this.nombre_completo = nombre_completo;
            this.correo = correo; // Fix: Asignación faltante
            this.usuario = usuario;
            this.password = password;
            this.password_hash = HashPassword(password);
            this.rol = rol;
            this.activo = activo;
        }



        /* Procedimiento para registrar un nuevo usuario 
            IN p_nombre_completo VARCHAR(120),
                IN p_correo_institucional VARCHAR(120),
                IN p_usuario VARCHAR(50),
                IN p_password_hash VARCHAR(255),
                IN p_rol ENUM('Usuario', 'Soporte')
        */
        public DataSet spInsUsuario()
        {
            DataSet ds = new DataSet();
            try
            {
                // Creación del comando SQL
                string cadSql = "CALL spInsUsuario('" + this.nombre_completo + "', '"
                                                      + this.correo + "','"
                                                      + this.usuario + "','"
                                                      + this.password_hash + "', '"
                                                      + this.rol + "');";
                
                // Configuración de los objetos de conexión a MySQL
                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cadSql, cnn);
                    // Ejecución del Adaptador de Datos
                    da.Fill(ds, "spInsUsuario");
                }
            }
            catch (Exception ex)
            {
                // Si hay un error de MySQL, lo capturamos aquí para que suba al controlador con detalle
                throw new Exception("Error en BD: " + ex.Message);
            }
            return ds;
        }
        // Proceso de Reporte de usuarios (vwRptUsuario)
        public DataSet vwUsuario()
        {
            // Crear el comando SQL
            string cadSQL = "";
            cadSQL = "select * from vwUsuarios";
            // Configuración de objetos de conexión
            MySqlConnection cnn = new MySqlConnection(cadConn);
            MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
            DataSet ds = new DataSet();
            // Ejecución y salida
            da.Fill(ds, "vwUsuarios");
            return ds;
        }

        /// <summary>
        /// Verifica si un usuario existe en la base de datos por su ID
        /// </summary>
        public bool existeUsuario(int idUsuario)
        {
            try
            {
                // Intentamos con tbl_usuario, si falla el catch nos dirá por qué
                string cadSQL = "SELECT * from vwUsuarios WHERE id_usuario = " + idUsuario;
                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    MySqlCommand cmd = new MySqlCommand(cadSQL, cnn);
                    cnn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Lanzamos la excepción para que el controlador la capture y nos muestre el error real de MySQL
                throw new Exception("Error al validar usuario: " + ex.Message);
            }
        }

        /// <summary>
        /// Generates a hash for the given password using PBKDF2
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>A base64-encoded string containing the salt and hash</returns>
        public static string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[32];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    byte[] hashWithSalt = new byte[salt.Length + hash.Length];
                    Array.Copy(salt, 0, hashWithSalt, 0, salt.Length);
                    Array.Copy(hash, 0, hashWithSalt, salt.Length, hash.Length);
                    return Convert.ToBase64String(hashWithSalt);
                }
            }
        }

        /// <summary>
        /// Validates a plain text password against a stored hash
        /// </summary>
        /// <param name="password">The plain text password to validate</param>
        /// <param name="hash">The stored password hash</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public static bool ValidatePassword(string password, string hash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hash);
                byte[] salt = new byte[32];
                Array.Copy(hashBytes, 0, salt, 0, salt.Length);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash2 = pbkdf2.GetBytes(32);
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + salt.Length] != hash2[i])
                            return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DataSet Login()
        {
            DataSet ds = new DataSet();

            try
            {
                // Uso del procedimiento almacenado spLoginUsuario siguiendo la misma sintaxis
                string cadSQL = "CALL spLoginUsuario('" + this.usuario + "');";

                using (MySqlConnection cnn = new MySqlConnection(cadConn))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cadSQL, cnn);
                    da.Fill(ds, "usuario");
                }

                // Verificar si encontró usuario
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string hashGuardado =
                        ds.Tables[0].Rows[0]["password_hash"].ToString();

                    bool passwordCorrecto =
                        ValidatePassword(this.password, hashGuardado);

                    // Si password incorrecto limpiamos resultados
                    if (!passwordCorrecto)
                    {
                        ds.Tables[0].Rows.Clear();
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en login: " + ex.Message);
            }
        }


    }

}
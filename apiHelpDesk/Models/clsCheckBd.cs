using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace apiHelpDesk.Models
{
    public class clsCheckBd
    {
        private string cadConn = ConfigurationManager.ConnectionStrings["bdHelpDeskAWS"].ConnectionString;

        public string statusMsg;
        public int flag;


        // Definición del método de conexión a MySql
        public void checkBd()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(cadConn);
                conn.Open();
                conn.Close();
                // Conexión exitosa, enviar salida
                flag = 1;
                statusMsg = "Conexión exitosa a MySql!";
            }
            catch (Exception ex)
            {
                // Conexión fallida, enviar salida:
                flag = 0;
                statusMsg = ex.Message.ToString();
            }
        }

    }
}
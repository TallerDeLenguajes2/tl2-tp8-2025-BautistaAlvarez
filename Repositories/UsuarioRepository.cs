using Microsoft.Data.Sqlite;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Models;

namespace tl2_tp8_2025_BautistaAlvarez.Repositories
{
    public class UsuarioRepository : IUserRepository
    {
        //string cadenaConexion = "Data Source=DB/Tienda.db";//conexion para todo el repositorio

        //tp11
        private readonly string _ConnectionString;
        //creo constructor para la inyeccion
        public UsuarioRepository(string connectionString)//necesario para incorporar la nueva inyeccion de dependencia
        {
            _ConnectionString = connectionString;//luego tuve que cambiar cada cadena conexon por _ConnectionString
        }
        public Usuario GetUser(string username, string contrasena)
        {
            Usuario user = null;
            //Consulta SQL que busca por Usuario Y Contrasena
            const string sql = @"
            SELECT Id, Nombre, User, Pass, Rol
            FROM Usuarios
            WHERE User = @Usuario AND Pass = @Contrasena";
            try//este try y catch es por si al intentar abrir el archivo ocurriera un error, en tal caso manda un mensaje personalizado y la excepcion generada por el mismo sistema
            {
                using var conexion = new SqliteConnection(_ConnectionString);
                conexion.Open();
                using var comando = new SqliteCommand(sql, conexion);

                // Se usan parámetros para prevenir inyección SQL
                comando.Parameters.AddWithValue("@Usuario", username);
                comando.Parameters.AddWithValue("@Contrasena", contrasena);
                using var reader = comando.ExecuteReader();
                if (reader.Read())
                {
                    // Si el lector encuentra una fila, el usuario existe y las credenciales son correctas
                    user = new Usuario
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        User = reader.GetString(2),
                        Pass = reader.GetString(3),
                        Rol = reader.GetString(4)
                    };
                }
                return user;
            }
            catch(Exception ex)
            {
                throw new Exception("No se pudo establecer conexion con la base", ex);
            }//haciendo este tipo de excepcion agregando el ex, ademas de poner nuestro mensaje mandamos la excepcion que ocurre
        }
    }
}
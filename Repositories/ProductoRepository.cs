using Microsoft.Data.Sqlite;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Models;

public class ProductoRepository : IProductoRepository
{
    string cadenaConexion = "Data Source=DB/Tienda.db";//conexion para todo el repositorio

    public void CrearNuevoProducto(Productos producto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();//establezco y abro la conexion

        string sql = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";//codigo sql

        using var comando = new SqliteCommand(sql, conexion);//creo comandos

        //comando.Parameters.Add(new SqliteParameter("@idProducto", producto.IdProducto));
        comando.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));//cambio los parametrso con comando
        comando.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));

        comando.ExecuteNonQuery();//como no devuelve nada ejecuto de esta manera non
        //no es necesario agregar conexion close ya que using se encarga de cerrarlo cuando deja de usar conexion.Close(); //siempre cerrar la conexion
    }

    public void ModificarProductoExistente(int idProducto, Productos producto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @idProducto";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
        comando.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));

        comando.ExecuteNonQuery();
    }

    public List<Productos> ListarTodosLosProductos()
    {
        var listaProductos = new List<Productos>();//creo lista ya que devuelvo una lista

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "SELECT idProducto, Descripcion, Precio FROM Productos";

        using var comando = new SqliteCommand(sql, conexion);

        using var lector = comando.ExecuteReader();//armo un lector con execute reader

        while (lector.Read())//mientras el lector este ejecutando read
        {
            var p = new Productos//armo el constructor del producto mientras uso el read para leer los datos
            {
                IdProducto = Convert.ToInt32(lector["idProducto"]),
                Descripcion = lector["Descripcion"].ToString(),
                Precio = Convert.ToInt32(lector["Precio"])
            };
            listaProductos.Add(p);//agrego el producto creado a partir de los datos a la lista
        }
        return listaProductos;//devuelvo lista
    }
    public Productos ObtenerDetalleProductoPorId(int idProducto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @idProducto";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));//como solo buscamos un objeto es asi si fuera varios se usa el de arriba

        using var lector = comando.ExecuteReader();//lector que viene de comando

        if (lector.Read())//si encontro algun registro, notese que al ser un solo objeto es un if y no un while
        {
            var producto = new Productos//creo un objeto producto en base a los datos leidos por el lector
            {
                IdProducto = Convert.ToInt32(lector["idProducto"]),
                Descripcion = lector["Descripcion"].ToString(),
                Precio = Convert.ToInt32(lector["Precio"])
            };
            return producto; //devuelvo producto si es que leyo algo
        }

        return null; //si no leyo nada devuelvo null
    }
    public void EliminarProductoPorId(int idProducto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "DELETE FROM Productos WHERE idProducto = @idProducto";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));
        comando.ExecuteNonQuery();
    }
    public bool ExisteProducto(int idProducto)
    {
    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();
        //select COUNT(*) sirve para contar todas las filas de la talba presupuesto, y el WHERE lo uso para que solo cuente cuando haya coincidencia de ID. Lo cual siempre me devolvera 1 o 0
        string sql = "SELECT COUNT(*) FROM Productos WHERE idProducto = @idProducto";
        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));

        long count = (long)comando.ExecuteScalar();//ExecuteScalar devuelve un solo valor, o te interesa sólo el primer valor de la primera fila.

        return count > 0;//aqui retorno un bool, si es mayor a 0 es positivo y sino falso
    }

}
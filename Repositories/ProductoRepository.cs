using Microsoft.Data.Sqlite;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Models;

public class ProductoRepository : IProductoRepository
{
    //string cadenaConexion = "Data Source=DB/Tienda.db";//conexion para todo el repositorio
    //tp11
    private readonly string _ConnectionString;
    //creo constructor para la inyeccion
    public ProductoRepository(string connectionString)
    {
        _ConnectionString = connectionString;//luego tuve que cambiar cada cadena conexon por _ConnectionString
    }

    public void CrearNuevoProducto(Productos producto)
    {
        //tp11, agrego excepciones por si la llamada del repositorio no es exitosa
        if(string.IsNullOrWhiteSpace(producto.Descripcion))
            throw new Exception("La descripcion no puede estar vacia");
        if(producto.Precio <= 0)
            throw new Exception("El precio debe ser mayor a cero");
        //tp11
        using var conexion = new SqliteConnection(_ConnectionString);
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
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sql = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @idProducto";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
        comando.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));

        int filasModificada = comando.ExecuteNonQuery();//ejecuto el comando y a su vez guardo la cantidad de filas que se modifican, ya que devuelve un int
        //tp11
        if(filasModificada == 0)//si es 0 significa que no se modifico nada por tanto no se encontro nada
            throw new Exception($"No se encontro el producto de id {idProducto} a modificar");
    }

    public List<Productos> ListarTodosLosProductos()
    {
        var listaProductos = new List<Productos>();//creo lista ya que devuelvo una lista, no es null es una lista iniciada y vacia

        using var conexion = new SqliteConnection(_ConnectionString);
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
        //tp11
        if (listaProductos.Count == 0)//si la lista no tiene productos entonces lanzo excepcion de lista vacia
            throw new Exception("La lista de productos esta vacia");
        return listaProductos;//devuelvo lista
    }
    public Productos ObtenerDetalleProductoPorId(int idProducto)
    {
        using var conexion = new SqliteConnection(_ConnectionString);
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

        //tp11
        throw new Exception($"Producto con id {idProducto} inexistente");// en vez de devolver un null tiro una excepcion si no encuentra producto
        //return null; //si no leyo nada devuelvo null
    }
    public void EliminarProductoPorId(int idProducto)
    {
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sql = "DELETE FROM Productos WHERE idProducto = @idProducto";

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));

        //tp11
        int filaAfectada = comando.ExecuteNonQuery();//ejecuto y guardo el numeros de filas, ya que execute da el numero de filas, en este caso siempre seria 1
        if (filaAfectada == 0)//comando.ExecuteNonQuery() ejecuta y luego guardo el numero
        {
            throw new Exception($"Producto de id: {idProducto} inexistentes o ya eliminados");
        }
    }
    public bool ExisteProducto(int idProducto)
    {
    using var conexion = new SqliteConnection(_ConnectionString);
    conexion.Open();
        //select COUNT(*) sirve para contar todas las filas de la talba presupuesto, y el WHERE lo uso para que solo cuente cuando haya coincidencia de ID. Lo cual siempre me devolvera 1 o 0
        string sql = "SELECT COUNT(*) FROM Productos WHERE idProducto = @idProducto";
        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));

        long count = (long)comando.ExecuteScalar();//ExecuteScalar devuelve un solo valor, o te interesa sólo el primer valor de la primera fila.

        return count > 0;//aqui retorno un bool, si es mayor a 0 es positivo y sino falso
    }

}
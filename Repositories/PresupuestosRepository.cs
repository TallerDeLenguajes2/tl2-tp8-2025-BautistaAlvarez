
using Microsoft.Data.Sqlite;
using SQLitePCL;
using tl2_tp8_2025_BautistaAlvarez.Models;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
public class PresupuestosRepository : IPresupuestosRepository
{
    //string cadenaConexion = "Data Source=DB/Tienda.db"; //conexion para todo el repositorio
    //tp11
    private readonly string _ConnectionString;//cambio a esto para la inyeccion de dependencia
    //creo constructor para la inyeccion
    public PresupuestosRepository(string connectionString)
    {
        _ConnectionString = connectionString;//luego tuve que cambiar cada cadena conexon por _ConnectionString
    }
    public void CrearPresupuesto(Presupuestos presupuesto)
    {
        //tp11
        if (string.IsNullOrWhiteSpace(presupuesto.NombreDestinatario))
            throw new Exception("No puede estar vacio el nombre del destinatario");
        if(presupuesto.FechaCreacion == DateOnly.MinValue)//si el usuario no ingresa fecha, por defecto se ingresa la fecha minima entonces si eso pasa sale una excepcion
            throw new Exception("Debe ingresar una fecha valida");
        if(presupuesto.FechaCreacion == DateOnly.FromDateTime(DateTime.Now))//si el usuario ingresa una fecha futura
            throw new Exception("Debe ingresar una fecha que no sea futura");
        //tp11
        using var conexion = new SqliteConnection(_ConnectionString);//usar using
        conexion.Open();//abro la conexion usando using para que se abra y cierre cuando sea deje de usarse

        string sql = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";//codigo sql

        using var comando = new SqliteCommand(sql, conexion);

        //comando.Parameters.Add(new SqliteParameter("@idPresupuesto", presupuesto.IdPresupuesto));//quito esto porque ya solo lo asigne la base de dato
        comando.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));//cambio los valores por los valores que le doy por la funcion
        comando.Parameters.Add(new SqliteParameter("@FechaCreacion", presupuesto.FechaCreacion.ToString("yyyy-MM-dd")));//llega un date only y lo paso a string con tipo fecha

        //presupuesto.FechaCreacion.ToDateTime(TimeOnly.MinValue))) //otra opcion
        //como uso DateOnly debo pasar de dateOnly a DateTime con el comando.ToDateTime() y dentro del parentesis van los minutos que inicio por eso timeonly.minvalue

        int filasAfectadas = comando.ExecuteNonQuery(); //ejecuto sino tengo que mostrar nada y número de filas insertadas
        if (filasAfectadas == 0)//sino afecto ninguna fila, daria 0 y en ese caso no se pudo crear presupuesto
            throw new Exception("No se pudo crear el presupuesto, inténtelo de nuevo.");
    }

    public List<Presupuestos> ListarPresupuesto()
    {
        var listado = new List<Presupuestos>();//inicio lista

        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sql = "SELECT * FROM Presupuestos";//* es para elegir todo los campos, sin where para elegir toda la tabla
        using var comando = new SqliteCommand(sql, conexion);

        using var lector = comando.ExecuteReader();//inicio el reader

        while (lector.Read())//mientras el lector lea
        {
            var p = new Presupuestos
            {
                IdPresupuesto = Convert.ToInt32(lector["idPresupuesto"]),
                NombreDestinatario = lector["NombreDestinatario"].ToString(),
                FechaCreacion = DateOnly.Parse(lector["FechaCreacion"].ToString()),//paso el dato de la tabla a string y de string lo paso a dateonly para que lo lea mi clase

                // FechaCreacion = DateOnly.FromDateTime(Convert.ToDateTime(lector["FechaCreacion"])) //otra opcion
                //Cuando leés de SQLite, lector["FechaCreacion"] te devuelve un objeto que puede ser: un string ("2025-10-24") o un DateTime (2025-10-24 00:00:00)
                //Convert.ToDateTime(...) convierte lo que sea (texto o fecha) a un DateTime de C#. Me aseguro en transformarlo en date time para luego aplicar dateonly
                //DateOnly.FromDateTime(..) extrae solo la parte de la fecha, descartando la hora. 
                Detalle = new List<PresupuestosDetalle>()//inicio una lista vacia para luego ir llenando
            };
            listado.Add(p);//agrego el presupuesto
        }

        foreach (var presupuesto in listado)//Recorro la lista para agregar su listado de presupuesto
        {//saco la info desde presupuestoDetalle ya que une la info que necesito y hago un join a la tabla padre (producto)
            string sqlDetalle = @"
            SELECT d.idProducto, d.Cantidad, p.Descripcion, p.Precio
            FROM PresupuestosDetalle d
            JOIN Productos p ON d.idProducto = p.idProducto
            WHERE d.idPresupuesto = @idPresupuesto";

            using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion);
            comandoDetalle.Parameters.Add(new SqliteParameter("@idPresupuesto", presupuesto.IdPresupuesto));

            using var lectorDetalle = comandoDetalle.ExecuteReader();
            while (lectorDetalle.Read())//mientras el lector lea, como es un listado de presupuesto detalle, debo usar un while
            {
                var producto = new Productos//creo producto
                {
                    IdProducto = Convert.ToInt32(lectorDetalle["idProducto"]),
                    Descripcion = lectorDetalle["Descripcion"].ToString(),
                    Precio = Convert.ToInt32(lectorDetalle["Precio"])
                };
                var presupuestoDetalle = new PresupuestosDetalle//termino armando el presupuesto detalle
                {
                    Producto = producto,
                    Cantidad = Convert.ToInt32(lectorDetalle["Cantidad"])
                };
                presupuesto.Detalle.Add(presupuestoDetalle);//lo agrego a la lista hasta que el lector termine de leer
            }//una vez que termine de leer todo lo relacionado con este presupuesto, el foreach pasa al siguiente presupuesto repitiendo el ciclo
        }
        //tp11
        if(listado.Count == 0)
            throw new Exception("La lista de presupuesto esta vacia");

        return listado;//una vez terminado el foreach retorno el listado        
    }
    public Presupuestos PresupuestoPorId(int idPresupuesto)
    {
        var presupuesto = new Presupuestos();//creo una variable presupuesto para devolver

        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sqlPresupuesto = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";
        using var comandoPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);

        comandoPresupuesto.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));

        using var lectorPresupuesto = comandoPresupuesto.ExecuteReader(); //ejecuto el lector
        if (lectorPresupuesto.Read())//si encontro algo
        {
            presupuesto.IdPresupuesto = Convert.ToInt32(lectorPresupuesto["idPresupuesto"]); //convierto y agrego los datos encontrados
            presupuesto.NombreDestinatario = lectorPresupuesto["NombreDestinatario"].ToString();
            presupuesto.FechaCreacion = DateOnly.FromDateTime(Convert.ToDateTime(lectorPresupuesto["FechaCreacion"]));//converto a datetime por las dudas y luego uso date only.fromdate time para sacar solo la fecha
        }
        else//TP11
        {
            throw new Exception($"Presupuesto de id {idPresupuesto} inexistente");//si no encuentra el registro salta una excepcion
        }
        //lectorPresupuesto.Close(); aqui tendria que cerrar el lector pero como uso using ya lo hace solo

        //el arroba es para poder escribir de esta manera, el join sirve para unir 2 o mas tablas a partir de una coincidencia como por ejemplo el id de los productos
        // de FROM seria la tabla principal, de JOIN esta la tabla a la cual se relacion, d y p son las alias de las tablas una abreviatura para luego hacer ON d.algo = p.algo
        string sqlDetalle = @"
        SELECT d.idProducto, d.Cantidad, p.Descripcion, p.Precio
        FROM PresupuestosDetalle d
        JOIN Productos p ON d.idProducto = p.idProducto
        WHERE d.idPresupuesto = @idPresupuesto";
        //Este query seria si quiero combinar las 3 tablas pero no es recomendable ya que para los datos del presupuesto necesito pasar una sola vez en cambio para su listado si necesito el while
        /*
        string query = @"SELECT pr.id_presupuesto, pr.nombre_destinatario, pr.fecha_creacion,
                        p.id_producto, p.descripcion, p.precio, d.cantidad
                 FROM Presupuestos pr
                 JOIN PresupuestosDetalle d ON pr.id_presupuesto = d.id_presupuesto
                 JOIN Productos p ON d.id_producto = p.id_producto
                 WHERE pr.id_presupuesto = @idPresupuesto;";
        */
        using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion); //nuevo comando con otra orden sql
        comandoDetalle.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//remplazo el valor con el que le doy al metodo

        using var lectorDetalle = comandoDetalle.ExecuteReader();//ejecuto el lector

        var listaDetalle = new List<PresupuestosDetalle>();//inicio lista de presupuesto detalle
        while (lectorDetalle.Read())//mientras el lector lea, al ser un listado debo usar un while
        {
            var p = new Productos//creo el producto en base a la busqueda
            {
                IdProducto = Convert.ToInt32(lectorDetalle["idProducto"]),
                Descripcion = lectorDetalle["Descripcion"].ToString(),
                Precio = Convert.ToInt32(lectorDetalle["Precio"])
            };

            var pDetalle = new PresupuestosDetalle //creo el objeto PresupuestoDetalle
            {
                Producto = p, //agrego el producto formado anteriormente
                Cantidad = Convert.ToInt32(lectorDetalle["Cantidad"])
            };

            listaDetalle.Add(pDetalle);//agrego a la lista de presupuestoDetalle
        }
        //retomando el presupuesto
        presupuesto.Detalle = listaDetalle;//coloco la lista que forme
        return presupuesto;
    }
    public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        //tp11
        if (idPresupuesto <= 0)
            throw new Exception("El ID del presupuesto no es válido.");
        if (idProducto <= 0)
            throw new Exception("El ID del producto no es válido.");
        if (cantidad <= 0)
            throw new Exception("La cantidad debe ser mayor a cero.");
        //tp11
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sql = "INSERT INTO PresupuestosDetalle(idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//manera mas precisa de agregar
        comando.Parameters.AddWithValue("@idProducto", idProducto);//una forma de agregar valores de manera rapida
        comando.Parameters.Add(new SqliteParameter("@Cantidad", cantidad));

        int filasAfectadas = comando.ExecuteNonQuery();//como no hay nada que mostrar ejecuto, a su vez guardo el numero de filas afectadas

        if (filasAfectadas == 0)//si el numero de filas afectadas es 0 significa que no se efectuo por tanto lanza una excepcion
            throw new Exception("No se pudo agregar el producto al presupuesto.");
    }
    public void EliminarPresupuestoPorId(int idPresupuesto)
    {
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();
        //primero debo borrar el presupuesto de la tabla hijo (PresupuestosDetalles) ya que contiene el idPresupuesto y esta la clave foragnea, luego elimino la tabla origen o principal
        string sqlDetalle = "DELETE FROM PresupuestosDetalle where idPresupuesto = @idPresupuesto";//tabla hijo o la que esta conectada con la clave foranea

        using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion);
        comandoDetalle.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
        comandoDetalle.ExecuteNonQuery();//si el presupuesto o el detalle no existe no borra nada y no da error
        //tabla principal
        string sqlPrincipal = "DELETE FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";

        using var comando = new SqliteCommand(sqlPrincipal, conexion);

        comando.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//agrego el valor al sql
        
        //tp11
        int filaAfectada = comando.ExecuteNonQuery();//ejecuto y guardo el numeros de filas, ya que execute da el numero de filas, en este caso siempre seria 1
        if (filaAfectada == 0)//comando.ExecuteNonQuery() ejecuta y luego guardo el numero, si da 0 significa que no hizo nada entonces se hace la excepcion
        {
            throw new Exception($"Presupuesto de id: {idPresupuesto} inexistente o ya eliminado");
        }
    }
    public void ModificarPresupuesto(int idPresupuesto, Presupuestos presupuesto)
    {
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();

        string sqlPresupuesto = "UPDATE Presupuestos SET NombreDestinatario = @NombreDestinatario WHERE idPresupuesto = @idPresupuesto";
        using var comandoPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);

        comandoPresupuesto.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
        comandoPresupuesto.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));

        //tp11
        int filasModificada = comandoPresupuesto.ExecuteNonQuery();
        if(filasModificada == 0)
            throw new Exception($"Presupuesto con ID {idPresupuesto} no existe y no puede ser modificado.");

        //borrar detalle
        string sqlBorrarDetalle = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto";

        using var comandoBorrar = new SqliteCommand(sqlBorrarDetalle, conexion);
        comandoBorrar.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
        comandoBorrar.ExecuteNonQuery();
        //Insertar nuevo detalle
        string sqlDetalle = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";


        foreach (var detalle in presupuesto.Detalle)
        {
            using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion);
            comandoDetalle.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//va dentro ya que el comando se debe actualizar
            comandoDetalle.Parameters.Add(new SqliteParameter("@idProducto", detalle.Producto.IdProducto));
            comandoDetalle.Parameters.Add(new SqliteParameter("@Cantidad", detalle.Cantidad));
            //tp11
            int filas = comandoDetalle.ExecuteNonQuery();//ejecuto y guardo el numero filas
            if (filas == 0)//si diera 0, significa que no se hizo bien el cambio
                throw new Exception($"No se pudo agregar el producto de ID {detalle.Producto.IdProducto} al presupuesto de ID {idPresupuesto}.");
        }
    }

    public bool ExistePresupuesto(int idPresupuesto)
    {
        using var conexion = new SqliteConnection(_ConnectionString);
        conexion.Open();
        //select COUNT(*) sirve para contar todas las filas de la talba presupuesto, y el WHERE lo uso para que solo cuente cuando haya coincidencia de ID. Lo cual siempre me devolvera 1 o 0
        string sql = "SELECT COUNT(*) FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));

        long count = (long)comando.ExecuteScalar();//ExecuteScalar devuelve un solo valor, o te interesa sólo el primer valor de la primera fila.
        return count > 0;//aqui retorno un bool, si es mayor a 0 es positivo y sino falso
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
namespace tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Models;
public interface IProductoRepository
{
    void CrearNuevoProducto(Productos producto);
    void ModificarProductoExistente(int idProducto, Productos producto);
    List<Productos> ListarTodosLosProductos();
    Productos ObtenerDetalleProductoPorId(int idProducto);
    void EliminarProductoPorId(int idProducto);
    bool ExisteProducto(int idProducto);
}


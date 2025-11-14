namespace tl2_tp8_2025_BautistaAlvarez.Interfaces;

using tl2_tp8_2025_BautistaAlvarez.Models;
public interface IPresupuestosRepository
{
    void AgregarProducto(int idPresupuesto, int idProducto, int cantidad);
    void CrearPresupuesto(Presupuestos presupuesto);
    void EliminarPresupuestoPorId(int idPresupuesto);
    List<Presupuestos> ListarPresupuesto();
    void ModificarPresupuesto(int idPresupuesto, Presupuestos presupuesto);
    Presupuestos PresupuestoPorId(int idPresupuesto);
    bool ExistePresupuesto(int idPresupuesto);
    bool ExisteProducto(int idProducto);
}
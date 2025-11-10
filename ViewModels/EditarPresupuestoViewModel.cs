using System.ComponentModel.DataAnnotations;
using tl2_tp8_2025_BautistaAlvarez.Models;
namespace tl2_tp8_2025_BautistaAlvarez.ViewModels
{
    public class EditarDetallePresupuestoViewModel
    {
        public int IdProducto { get; set; }
        [Required(ErrorMessage = "Requiere una cantidad")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public int Cantidad { get; set; }
        public EditarDetallePresupuestoViewModel(PresupuestosDetalle detalle)
        {
            IdProducto = detalle.Producto.IdProducto;
            Cantidad = detalle.Cantidad;
        }
        public EditarDetallePresupuestoViewModel()//no olvidar poner un constructor vacio
        {
            
        }
        public PresupuestosDetalle ToModel()//accion para pasar de viewmodel a model
        {
            return new PresupuestosDetalle
            {
                Producto = new Productos { IdProducto = this.IdProducto },//solo ocupo el id  en el producto, resto va vacio
                Cantidad = this.Cantidad
            };
        }
    }
    public class EditarPresupuestoViewModel
    {
        public int IdPresupuesto { get; set; }
        [StringLength(50, ErrorMessage = "El nombre no puede superar a mas de 50.")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        public string NombreDestinatario { get; set; }
        public List<EditarDetallePresupuestoViewModel> Detalle { get; set; }

        public EditarPresupuestoViewModel(Presupuestos presupuesto)
        {
            IdPresupuesto = presupuesto.IdPresupuesto;
            NombreDestinatario = presupuesto.NombreDestinatario;
            Detalle = new List<EditarDetallePresupuestoViewModel>();
            foreach (var detalle in presupuesto.Detalle)
            {
                Detalle.Add(new EditarDetallePresupuestoViewModel(detalle));
            }
        }
        public EditarPresupuestoViewModel()//no olvidar de poner un constructor vacio
        {
            Detalle = new List<EditarDetallePresupuestoViewModel>();
        }
        public Presupuestos ToModel()//metodo para pasar de viewModel a model
        {
            var presupuesto = new Presupuestos
            {
                IdPresupuesto = this.IdPresupuesto,
                NombreDestinatario = this.NombreDestinatario,
                Detalle = new List<PresupuestosDetalle>()
            };

            foreach (var detalleVM in this.Detalle)
            {
                presupuesto.Detalle.Add(detalleVM.ToModel());//agrego a la lista de presupuesto, y hago uso de pasar de viewmodel a model
            }

            return presupuesto;
        }
    }
}
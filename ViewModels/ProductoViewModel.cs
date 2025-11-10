using System.ComponentModel.DataAnnotations;
using tl2_tp8_2025_BautistaAlvarez.Models;
namespace tl2_tp8_2025_BautistaAlvarez.ViewModels
{
    public class ProductoViewModel{
        //Se incluye id para la accion edicion
        public int IdProducto{get;set;}
        //Validacion: Maximo 250 caracteres. Por defecto si no tiene [Required]
        [Display(Name = "Descripcion del producto")]
        [StringLength(250, ErrorMessage = "La descripcion no puede superar los 250 caracteres.")]
        public string Descripcion{get;set;}
        // Validación: Requerido y debe ser positivo
        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        public ProductoViewModel(Productos producto)//constructores
        {
            IdProducto = producto.IdProducto;
            Descripcion = producto.Descripcion;
            Precio = producto.Precio;
        }
        public ProductoViewModel()
        {
            
        }
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList

namespace tl2_tp8_2025_BautistaAlvarez.ViewModels
{
    public class AgregarProductoViewModel
    {
        // ID del presupuesto al que se va a agregar (viene de la URL o campo oculto)
        public int IdPresupuesto { get; set; }
        // ID del producto seleccionado en el dropdown
        [Display(Name = "Producto a agregar")]
        public int IdProducto { get; set; }
        // Validación: Requerido y debe ser positivo
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }
        // Propiedad adicional para el Dropdown (no se valida, solo se usa en la Vista)
        [ValidateNever]//debo poner esto ya que cuando el formulario devuelve el viewmodel necesita rellenar los campos
        public SelectList ListaProductos { get; set; }//Un select para crear el formulario de la lista de productos
    }
}
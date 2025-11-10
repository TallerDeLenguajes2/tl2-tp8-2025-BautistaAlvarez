using System.ComponentModel.DataAnnotations;
using tl2_tp8_2025_BautistaAlvarez.Models;
namespace tl2_tp8_2025_BautistaAlvarez.ViewModels
{
    public class PresupuestoViewModel{
        public int IdPresupuesto { get; set; }
        // Validación: Requerido
        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        // Opcional: Se puede añadir la validación de formato de email si se opta por guardar el mail.
        // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string NombreDestinatario { get; set; }
        // Validación: Requerido y tipo de dato
        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateOnly FechaCreacion { get; set; }
        // La validación de que la fecha no es futura se hará en el Controlador (ver Etapa 3).
        //public List<PresupuestosDetalle> Detalle { get; set; }

        //constructores para pasar de viewmodel a model
        public PresupuestoViewModel()
        {

        }
        public PresupuestoViewModel(Presupuestos presupuesto)
        {
            IdPresupuesto = presupuesto.IdPresupuesto;
            NombreDestinatario = presupuesto.NombreDestinatario;
            FechaCreacion = presupuesto.FechaCreacion;
        }

    }
}
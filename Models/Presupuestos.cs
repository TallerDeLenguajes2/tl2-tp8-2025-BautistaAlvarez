using System.Reflection.Metadata;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

namespace tl2_tp8_2025_BautistaAlvarez.Models
{
    public class Presupuestos
    {
        private int idPresupuesto;
        private string nombreDestinatario;
        private DateOnly fechaCreacion;
        private List<PresupuestosDetalle> detalle;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
        public DateOnly FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public List<PresupuestosDetalle> Detalle { get => detalle; set => detalle = value; }

        public Presupuestos()//constructor
        {
            Detalle = new List<PresupuestosDetalle>();//importante inicializar la lista, evita errores al intentar recorrer listas null
        }

        public const float IVA = 0.21f;

        public float MontoPresupuesto()
        {
            float monto = 0;
            foreach (var presupuestoDetalle in detalle)
            {
                monto += presupuestoDetalle.Cantidad * (float)presupuestoDetalle.Producto.Precio;
            }

            return monto;
        }

        public float MontoPresupuestoConIva()
        {
            float montoConIva = 0;
            montoConIva = MontoPresupuesto() + MontoPresupuesto() * IVA;
            return montoConIva;
        }
        
        public int CantidadaProductos()
        {
            int cantidad = 0;
            foreach (var presupuestoDetalle in detalle)
            {
                cantidad += presupuestoDetalle.Cantidad;
            }
            return cantidad;
        }
    }
}
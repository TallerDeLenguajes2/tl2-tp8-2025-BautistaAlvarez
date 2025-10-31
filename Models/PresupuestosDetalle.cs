namespace tl2_tp8_2025_BautistaAlvarez.Models
{
    public class PresupuestosDetalle
    {
        private Productos producto;
        private int cantidad;

        public Productos Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }

        public PresupuestosDetalle()//constructor
            {
                
            }
    }

}
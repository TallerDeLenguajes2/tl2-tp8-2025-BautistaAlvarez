namespace tl2_tp8_2025_BautistaAlvarez.Models
{
    public class Productos
    {
        private int idProducto;

        private string descripcion;
        private decimal precio;
        public int IdProducto { get => idProducto; set => idProducto = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }

        public decimal Precio { get => precio; set => precio = value; }

        public Productos()
        {

        }
    }
    
}

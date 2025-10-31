using tl2_tp8_2025_BautistaAlvarez.Models;
using Microsoft.AspNetCore.Mvc;//agregar si o si para controller (a veces se agrega solo)
namespace tl2_tp8_2025_BautistaAlvarez.Controllers;

public class ProductosController : Controller
{
    private ProductoRepository productoRepository;
    public ProductosController(){//constructor
        productoRepository = new ProductoRepository();
    }
    //A partir de aqui van los actions

    [HttpGet]
    public IActionResult Index(){
        List<Productos> ListaProductos = productoRepository.ListarTodosLosProductos();
        return View(ListaProductos);
    }

}
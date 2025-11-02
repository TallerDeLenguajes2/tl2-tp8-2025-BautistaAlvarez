using tl2_tp8_2025_BautistaAlvarez.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;//agregar si o si para controller (a veces se agrega solo)
namespace tl2_tp8_2025_BautistaAlvarez.Controllers;

public class ProductosController : Controller
{
    private ProductoRepository productoRepository;
    public ProductosController(){//constructor
        productoRepository = new ProductoRepository();
    }
    //A partir de aqui van los actions

    [HttpGet]
    public IActionResult Index()
    {
        List<Productos> ListaProductos = productoRepository.ListarTodosLosProductos();
        return View(ListaProductos);
    }
    [HttpGet]
    public IActionResult Details(int idProducto)
    {
        Productos producto = productoRepository.ObtenerDetalleProductoPorId(idProducto);
        return View(producto);
    }
    [HttpGet]//un get para mostrar una pagina en blanco de formulario para que el usuario llene con datos
    public IActionResult Create()
    {
        var producto = new Productos();
        return View(producto);//creo un producto en blanco y muestro para rellenar en el formulario
    }
    [HttpPost]
    public IActionResult Create(Productos producto)//un post para mandar la info que ingrese el usuario en el formulario vacio mandado como get
    {
        productoRepository.CrearNuevoProducto(producto);//creo producot
        return RedirectToAction("Index");//me devuelve a la pagina inicio de producto
    }
    [HttpGet]
    public IActionResult Edit(int idProducto)//mando el id del producto a editar
    {
        var producto = productoRepository.ObtenerDetalleProductoPorId(idProducto);//retorno el producto buscado
        return View(producto);
    }
    [HttpPost]
    public IActionResult Edit(Productos producto)//ingreso un objeto del tipo producto
    {
        productoRepository.ModificarProductoExistente(producto.IdProducto, producto);//pongo el mismo id ya que es el mismo producto
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Delete(int idProducto)//introduzco el id del producto buscado
    {
        var producto = productoRepository.ObtenerDetalleProductoPorId(idProducto);
        return View(producto);//muestro el producto a eliminar
    }
    [HttpPost]
    public IActionResult Delete(Productos producto)//pongo un objeto producto porque en el get retorne un objeto producto, podria mandar solo un int pero requiere hacer mas cosas
    {
        productoRepository.EliminarProductoPorId(producto.IdProducto);
        return RedirectToAction("Index");
    }

}
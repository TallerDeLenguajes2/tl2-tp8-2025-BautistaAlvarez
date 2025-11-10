using tl2_tp8_2025_BautistaAlvarez.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;//agregar si o si para controller (a veces se agrega solo)
using tl2_tp8_2025_BautistaAlvarez.ViewModels;//nuevo using
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
        var listaProductoViewmodel = new List<ProductoViewModel>();//creo una lista de viewmodel
        foreach (var producto in ListaProductos)//paso los productos a la liste de viewmodel (de model a viewmodel)
        {
            var productoVM = new ProductoViewModel(producto);
            listaProductoViewmodel.Add(productoVM);
        }
        return View(listaProductoViewmodel);
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
        var productoVM = new ProductoViewModel();
        return View(productoVM);//creo un producto en blanco y muestro para rellenar en el formulario
    }
    [HttpPost]//un post para mandar la info que ingrese el usuario en el formulario vacio mandado como get
    public IActionResult Create(ProductoViewModel productoVM)//Aqui hago un cambio a viewModel
    {
        //1. Chequeo de seguridad del servidor
        if (!ModelState.IsValid)
        {
            // Si falla: Devolvemos el ViewModel con los datos y errores a la Vista
            return View(productoVM);
        }
        // 2. SI ES VÁLIDO: Mapeo Manual de VM a Modelo de Dominio
        var nuevoProducto = new Productos
        {
            //no coloco el id porque el repositorio no ocupa y se genera solo a la base de dato
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio//aqui cambie a tipo decimal el precio, originalmente estaba en int
        };
        // 3. Llamada al Repositorio
        productoRepository.CrearNuevoProducto(nuevoProducto);//creo producto
        return RedirectToAction("Index");//me devuelve a la pagina inicio de producto
    }
    [HttpGet]
    public IActionResult Edit(int idProducto)//mando el id del producto a editar
    {
        var producto = productoRepository.ObtenerDetalleProductoPorId(idProducto);//retorno el producto buscado
        var productoVM = new ProductoViewModel(producto);//paso de model a viewmodel
        return View(productoVM);
    }
    [HttpPost]//tener cuidado con el int idProducto, sus nombres deben coincidir y en el formulario de edit.cshtml
    public IActionResult Edit(int idProducto, ProductoViewModel productoVM)//ingreso un objeto del tipo producto
    {
        if (idProducto != productoVM.IdProducto) return NotFound();
        // 1. CHEQUEO DE SEGURIDAD DEL SERVIDOR
        if (!ModelState.IsValid)
        {
            return View(productoVM);
        }
        // 2. Mapeo Manual de VM a Modelo de Dominio
        var productoAEditar = new Productos
        {
            IdProducto = productoVM.IdProducto, // Necesario para el UPDATE
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio
        };
        // 3. Llamada al Repositorio
        productoRepository.ModificarProductoExistente(productoAEditar.IdProducto, productoAEditar);//pongo el mismo id ya que es el mismo producto
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
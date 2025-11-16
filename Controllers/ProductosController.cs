using tl2_tp8_2025_BautistaAlvarez.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;//agregar si o si para controller (a veces se agrega solo)
using tl2_tp8_2025_BautistaAlvarez.ViewModels;//nuevo using
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
namespace tl2_tp8_2025_BautistaAlvarez.Controllers;

public class ProductosController : Controller
{
    private readonly IAuthenticationService _authService;//para autentificar que los valores sean correctos
    //private ProductoRepository productoRepository; cambio a interfaz
    private IProductoRepository _repo;
    public ProductosController(IProductoRepository prodRepo, IAuthenticationService authService){//constructor
        //productoRepository = new ProductoRepository(); cambio
        _repo = prodRepo;
        _authService = authService;
    }
    //A partir de aqui van los actions

    [HttpGet]
    public IActionResult Index()
    {
        // Aplicamos el chequeo de seguridad, agregar en los get y post de las acciones
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad, el chequeo de seguridad podemos hacerlo mediante una accion como en este caso o directo en el codigo como en presupuesto

        List<Productos> ListaProductos = _repo.ListarTodosLosProductos();
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
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

        Productos producto = _repo.ObtenerDetalleProductoPorId(idProducto);
        return View(producto);
    }
    [HttpGet]//un get para mostrar una pagina en blanco de formulario para que el usuario llene con datos
    public IActionResult Create()
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

        var productoVM = new ProductoViewModel();
        return View(productoVM);//creo un producto en blanco y muestro para rellenar en el formulario
    }
    [HttpPost]//un post para mandar la info que ingrese el usuario en el formulario vacio mandado como get
    public IActionResult Create(ProductoViewModel productoVM)//Aqui hago un cambio a viewModel
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

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
        _repo.CrearNuevoProducto(nuevoProducto);//creo producto
        return RedirectToAction("Index");//me devuelve a la pagina inicio de producto
    }
    [HttpGet]
    public IActionResult Edit(int idProducto)//mando el id del producto a editar
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

        var producto = _repo.ObtenerDetalleProductoPorId(idProducto);//retorno el producto buscado
        var productoVM = new ProductoViewModel(producto);//paso de model a viewmodel
        return View(productoVM);
    }
    [HttpPost]//tener cuidado con el int idProducto, sus nombres deben coincidir y en el formulario de edit.cshtml
    public IActionResult Edit(int idProducto, ProductoViewModel productoVM)//ingreso un objeto del tipo producto
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

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
        _repo.ModificarProductoExistente(productoAEditar.IdProducto, productoAEditar);//pongo el mismo id ya que es el mismo producto
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Delete(int idProducto)//introduzco el id del producto buscado
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

        var producto = _repo.ObtenerDetalleProductoPorId(idProducto);
        return View(producto);//muestro el producto a eliminar
    }
    [HttpPost]
    public IActionResult Delete(Productos producto)//pongo un objeto producto porque en el get retorne un objeto producto, podria mandar solo un int pero requiere hacer mas cosas
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();//si algo anda mal devuelvo un valor, sino devuelve null
        if (securityCheck != null) return securityCheck;// si no esta vacio devuelvo la accion
        // Aplicamos el chequeo de seguridad

        _repo.EliminarProductoPorId(producto.IdProducto);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult AccesoDenegado(){//agrego, tp10
        // El usuario está logueado, pero no tiene el rol suficiente.
        return View();//solo pongo return view para mostrar la pagina AccesoDenegado.cshtml, al poner solamente view() el programa usa una pagina con el nombre de la accion
    }//el programa busca Views/<NombreDelControllerSinController>/<NombreDeLaAcción>.cshtml o sino en Views/Shared/<NombreDeLaAcción>.cshtml
    // resto del código con las correspondientes correcciones
    private IActionResult CheckAdminPermissions()//al ser private no lleva etiqueta [http], ya que solo es una funcion interna
    {
        // 1. No logueado? -> vuelve al login
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }
        // 2. No es Administrador? -> Da Error
        if (!_authService.HasAccessLevel("Administrador"))
        {
            // Llamamos a AccesoDenegado (llama a la vista correspondiente de Productos)
            return RedirectToAction(nameof(AccesoDenegado));
        }
        return null; // Permiso concedido, devuelvo null
        }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp8_2025_BautistaAlvarez.Models;
using tl2_tp8_2025_BautistaAlvarez.ViewModels;//agrego nuevo using

namespace tl2_tp8_2025_BautistaAlvarez.Controllers;

public class PresupuestosController : Controller
{
    private PresupuestosRepository presupuestosRepository;
    public PresupuestosController()//constructor
    {
        presupuestosRepository = new PresupuestosRepository();
    }
    //necesitamos el repositorio del producto para el dropdown del agregar producto. <select> (dropdown list)
    private ProductoRepository _productoRepository = new ProductoRepository();
    //Aqui van los Actions
    [HttpGet]
    public IActionResult Index()
    {//el nombre de los iactionresult seran el nombre de las paginas, en este caso pagina Index
        List<Presupuestos> listaPresupuestos = presupuestosRepository.ListarPresupuesto();//recupero la lista de presupuesto
        var listaPresupuestoVM = new List<PresupuestoViewModel>();//creo lista vacia del viewmodel
        foreach (var presupuesto in listaPresupuestos)//paso de model a viewmodel
        {
            var presupuestoVM = new PresupuestoViewModel(presupuesto);//creo viewmodel a partir del model
            listaPresupuestoVM.Add(presupuestoVM);//agrego a la lista
        }
        return View(listaPresupuestoVM);//retorno la lista viewmodel
    }
    [HttpGet]
    public IActionResult Details(int idPresupuesto)
    {
        Presupuestos presupuesto = presupuestosRepository.PresupuestoPorId(idPresupuesto);
        return View(presupuesto);
    }

    //CRUD
    [HttpGet]
    public IActionResult Create()
    {
        var presupuestoVM = new PresupuestoViewModel();//creo un viewmodel vacio
        return View(presupuestoVM);//creo un presupuesto vacio y lo mando a la pagina para que este en blanco y se le de info
    }
    [HttpPost]
    public IActionResult Create(PresupuestoViewModel presupuestoVM)
    {
        //sintaxis: ModelState.AddModelError(string key, string errorMessage);
        /*
        key → es el nombre de la propiedad del modelo donde querés asociar el error.
        Ejemplo: "FechaCreacion", "NombreDestinatario", "Cantidad", etc.
        errorMessage → es el texto que va a aparecer en la vista, junto al campo correspondiente.*/
        if (presupuestoVM.FechaCreacion > DateOnly.FromDateTime(DateTime.Now))//aqui paso de datetime a dateonly
        {
            ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser futura.");//seria la propiedad y el mensaje que quiera dar
        }
        //1. Chequeo de seguridad del servidor
        if (!ModelState.IsValid)
        {
            // Si falla: Devolvemos el ViewModel con los datos y errores a la Vista
            return View(presupuestoVM);
        }
        // 2. SI ES VÁLIDO: Mapeo Manual de VM a Modelo de Dominio
        var nuevoPresupuesto = new Presupuestos
        {
            NombreDestinatario = presupuestoVM.NombreDestinatario,//no se agrega el id ya que solo se necesita estos 2 parametros y ademas el id lo coloca el db
            FechaCreacion = presupuestoVM.FechaCreacion
        };
        // 3. Llamada al Repositorio
        presupuestosRepository.CrearPresupuesto(nuevoPresupuesto);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int idPresupuesto)//le ingreso un id de la tabla index para buscar el presupuesto
    {
        var presupuesto = presupuestosRepository.PresupuestoPorId(idPresupuesto);
        var presupuestoVM = new EditarPresupuestoViewModel(presupuesto);//paso de model a viewmodel
        return View(presupuestoVM);
    }
    [HttpPost]//cuidado que int idPresupuesto debe coincidir en ambos metodos sino hay error si tienen nombres diferentes
    public IActionResult Edit(int idPresupuesto, EditarPresupuestoViewModel presupuestoVM)//del formulario mando un objeto presupuesto
    {
        if (idPresupuesto != presupuestoVM.IdPresupuesto) return NotFound();
        // 1. CHEQUEO DE SEGURIDAD DEL SERVIDOR
        if (!ModelState.IsValid)
        {
            foreach (var kvp in ModelState)
            {
                var key = kvp.Key;
                foreach (var error in kvp.Value.Errors)
                {
                    Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }
            return View(presupuestoVM);
            //return View(presupuestoVM);
        }
        // 2. Mapeo con mi accion TOMODEL para pasar VM a Modelo de Dominio
        var presupuestoAEditar = presupuestoVM.ToModel();

        // 3. Llamada al Repositorio
        presupuestosRepository.ModificarPresupuesto(presupuestoAEditar.IdPresupuesto, presupuestoAEditar);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Delete(int idPresupuesto)
    {
        var presupuesto = presupuestosRepository.PresupuestoPorId(idPresupuesto);
        return View(presupuesto);
    }
    [HttpPost]
    public IActionResult Delete(Presupuestos presupuesto)
    {
        presupuestosRepository.EliminarPresupuestoPorId(presupuesto.IdPresupuesto);
        return RedirectToAction("Index");
    }
    //GET: Presupuesto/AgregarProducto
    [HttpGet]
    //cuidado con el nombre del parametro, debe coincidir con el formulario en asp-route-idPresupuesto="@presupuesto.IdPresupuesto"
    public IActionResult AgregarProducto(int idPresupuesto)//por eso tiene de nombre idPresupuesto y no simplemente id
    {
        // 1. Obtener los productos para el SelectList
        List<Productos> listaProductos = _productoRepository.ListarTodosLosProductos();//lista Productos
        // 2. Crear el ViewModel
        AgregarProductoViewModel model = new AgregarProductoViewModel
        {
            IdPresupuesto = idPresupuesto,// Pasamos el ID del presupuesto actual
            // 3. Crear el SelectList, sirve para mandarselo al formulario y nos muestre los productos para elegir
            ListaProductos = new SelectList(listaProductos, "IdProducto", "Descripcion")//new SelectList(lista, "Valor", "Texto")
        };
        //Explicacion de selectList, es para hacer un select en el formulario
        /*
        lista → es una lista de objetos (por ejemplo, List<Productos>).

        "Valor" → es el nombre de la propiedad del objeto que se usará como el value del <option>.

        "Texto" → es el nombre de la propiedad del objeto que se mostrará al usuario como el texto visible.
------------------------------------------------------------------------------------------------------------------------
        listaProductos → viene del repositorio (ListarTodosLosProductos()), o sea, una lista de objetos tipo Productos.

        "IdProducto" → será el valor del <option>, es decir, el número del producto.

        "Descripcion" → será el texto visible en el dropdown, o sea, lo que el usuario ve.
        //IdProducto y Descripcion pertenecen a la listaProductos o sea son campos de la lista que le mande
        */

        return View(model);
    }
    // ❗ El Método CLAVE para la validación de la cantidad
    // POST: Presupuestos/AgregarProducto
    [HttpPost]
    public IActionResult AgregarProducto(AgregarProductoViewModel model)
    {
        // 1. Chequeo de Seguridad para la Cantidad
        if (!ModelState.IsValid)
        {
            /*foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }*/
            // LÓGICA CRÍTICA DE RECARGA: Si falla la validación,
            // debemos recargar el SelectList porque se pierde en el POST.
            var ListaProductos = _productoRepository.ListarTodosLosProductos();
            model.ListaProductos = new SelectList(ListaProductos, "IdProducto", "Descripcion");
            // Devolvemos el modelo con los errores y el dropdown recargado
            return View(model);
        }
        // 2. Si es VÁLIDO: Llamamos al repositorio para guardar la relación
        presupuestosRepository.AgregarProducto(model.IdPresupuesto, model.IdProducto, model.Cantidad);//no es necesario pasar de viewmodel a modelo ya que la funcion ocupa valores primarios
        // 3. Redirigimos al detalle del presupuesto
        return RedirectToAction(nameof(Details), new { idPresupuesto = model.IdPresupuesto });//el nombre del parametro debe coincidir con el parametro de la accion detail, en este caso idPresupuesto
        //RedirectToAction("NombreDeAccion", "NombreDeControlador(Opcional)", new { nombreParametro = valor });

    }
}

/*
foreach (var kvp in ModelState)
{
    var campo = kvp.Key;
    foreach (var error in kvp.Value.Errors)
    {
        Console.WriteLine($"Error en '{campo}': {error.ErrorMessage}");
    }
}
*/
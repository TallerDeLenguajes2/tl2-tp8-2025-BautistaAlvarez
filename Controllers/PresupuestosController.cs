using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_BautistaAlvarez.Models;

namespace tl2_tp8_2025_BautistaAlvarez.Controllers;

public class PresupuestosController : Controller
{
    private PresupuestosRepository presupuestosRepository;
    public PresupuestosController(){//constructor
        presupuestosRepository = new PresupuestosRepository();
    }
    //Aqui van los Actions
    [HttpGet]
    public IActionResult Index(){//el nombre de los iactionresult seran el nombre de las paginas, en este caso pagina Index
        List<Presupuestos> listaPresupuestos = presupuestosRepository.ListarPresupuesto();
        return View(listaPresupuestos);
    }
    [HttpGet]
    public IActionResult Details(int idPresupuesto){
        Presupuestos presupuesto = presupuestosRepository.PresupuestoPorId(idPresupuesto);
        return View(presupuesto);
    }

    //CRUD
    [HttpGet]
    public IActionResult Create(){
        var presupuesto = new Presupuestos();
        return View(presupuesto);//creo un presupuesto vacio y lo mando a la pagina para que este en blanco y se le de info
    }
    [HttpPost]
    public IActionResult Create(Presupuestos presupuesto)
    {
        presupuestosRepository.CrearPresupuesto(presupuesto);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int idPresupuesto)//le ingreso un id de la tabla index para buscar el presupuesto
    {
        var presupuesto = presupuestosRepository.PresupuestoPorId(idPresupuesto);
        return View(presupuesto);
    }
    [HttpPost]
    public IActionResult Edit(Presupuestos presupuesto)//del formulario mando un objeto presupuesto
    {
        presupuestosRepository.ModificarPresupuesto(presupuesto.IdPresupuesto, presupuesto);
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
}
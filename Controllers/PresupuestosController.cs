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
    [HttpPost]
    public IActionResult Create(Presupuestos presupuesto){
        presupuestosRepository.CrearPresupuesto(presupuesto);
        return View(presupuesto);
    }
    [HttpPut]
    public IActionResult Edit(){
        return View();
    }

}
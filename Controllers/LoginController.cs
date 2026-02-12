// implementación del LoginController.cs
using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_BautistaAlvarez.ViewModels;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Models;

public class LoginController : Controller
{
    private readonly IAuthenticationService _authenthicationService;//lo uso para autentificar si la sesion es valida, ya que es una combinacion de http context y DB usuario

//tp11
    private readonly ILogger<LoginController> _logger;//inyecto el logger por inyeccion de dependencias, entre <> va el nombre del controlador en donde usamos
    //constructor
    public LoginController(IAuthenticationService authenticationService, ILogger<LoginController> logger)
    {
        _authenthicationService = authenticationService;
        _logger = logger; //cargo el logger tp11
    }

    [HttpGet]//muestra la vista de login
    public IActionResult Index()//debo crear index.cshtml solamente ya que las otras acciones solo vuelven al index
    {
        // ... (Crear LoginViewModel)
        return View(new LoginViewModel());
    }

    [HttpPost]//Procesa el login
    public IActionResult Login(LoginViewModel model)
    {
        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))//si mando vacio los datos
        {
            model.ErrorMessage = "Debe ingresar usuario y contraseña.";
            return View("Index", model);
        }

        try//hago un try en donde puede haber una excepcion, base de dato en este caso
        {
            if (_authenthicationService.Login(model.UserName, model.Password))//si es true, se cargaran los datos en el session del http context con los valores del usuario
            {
                _logger.LogInformation($"El usuario: {model.UserName} ingreso correctamente");//ocupo log para informar que se hizo exitosamente
                return RedirectToAction("Index", "Home");//redirijo al inicio
            }
            _logger.LogWarning($"Intento de acceso inválido + Usuario: {model.UserName} + Clave ingresada: {model.Password}");//log de warning por si falla
            model.ErrorMessage = "Credenciales invalidas.";
            return View("Index", model);//si introduce mal los datos retorno al index con el model
        }
        catch (Exception ex)
        {
            ErrorViewModel errorVM = new ErrorViewModel{RequestId = HttpContext.TraceIdentifier};//creo un viewmodel del error para mandarlo a la pagina de error
            _logger.LogError(ex.ToString());//capturo errores
            return View("Error",errorVM);//retorno vista de error en caso de uno y envio el viewmodel
        }


    }
    [HttpGet]//cierra sesion
    public IActionResult Logout()
    {
        _authenthicationService.Logout();
        return RedirectToAction("Index");
    }
}
// implementación del LoginController.cs
using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_BautistaAlvarez.ViewModels;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;

public class LoginController : Controller
{
    private readonly IAuthenticationService _authenthicationService;//lo uso para autentificar si la sesion es valida, ya que es una combinacion de http context y DB usuario
    //constructor
    public LoginController(IAuthenticationService authenticationService)
    {
        _authenthicationService = authenticationService;
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

        if (_authenthicationService.Login(model.UserName, model.Password))//si es true, se cargaran los datos al http context con los valores del usuario
        {
            return RedirectToAction("Index", "Home");//redirijo al inicio
        }

        model.ErrorMessage = "Credenciales invalidas.";
        return View("Index", model);//si introduce mal los datos retorno al index con el model
    }
    [HttpGet]//cierra sesion
    public IActionResult Logout()
    {
        _authenthicationService.Logout();
        return RedirectToAction("Index");
    }
}
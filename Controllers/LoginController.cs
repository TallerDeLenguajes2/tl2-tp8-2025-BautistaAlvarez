// implementación del LoginController.cs
using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_BautistaAlvarez.ViewModels;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;

public class LoginController : Controller
{
    private readonly IAuthenticationService _authenthicationService;
    //constructor
    public LoginController(IAuthenticationService authenticationService)
    {
        _authenthicationService = authenticationService;
    }

    [HttpGet]//muestra la vista de login
    public IActionResult Index()
    {
        // ... (Crear LoginViewModel)
        return View(new LoginViewModel());
    }

    [HttpPost]//Procesa el login
    public IActionResult Login(LoginViewModel model)
    {
        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
        {
            model.ErrorMessage = "Debe ingresar usuario y contraseña.";
        }

        if (_authenthicationService.Login(model.UserName, model.Password))
        {
            return RedirectToAction("Index", "Home");
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
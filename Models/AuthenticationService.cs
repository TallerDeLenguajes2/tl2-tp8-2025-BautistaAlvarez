using tl2_tp8_2025_BautistaAlvarez.Interfaces;
namespace tl2_tp8_2025_BautistaAlvarez.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;//esto es algo que tiene el sistema, no lo hice
    //private readonly HttpContext context;

    public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;//cargo los datos al hacer el constructor
        // context = _httpContextAccessor.HttpContext;
    }

    public bool Login(string username, string password)
    {
        var context = _httpContextAccessor.HttpContext;
        var user = _userRepository.GetUser(username, password);//obtengo usuario mediante sql
        if (user != null)
        {
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }
            context.Session.SetString("IsAuthenticated", "true");//seteo valores, pongo que si es autenficado
            context.Session.SetString("User", user.User);
            context.Session.SetString("UserNombre", user.Nombre);
            context.Session.SetString("Rol", user.Rol);//es el tipo de acceso/rol admin o cliente
            return true;
        }

        return false;
    }
    public void Logout()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        /* context.Session.Remove("IsAuthenticated");
        context.Session.Remove("User");
        context.Session.Remove("UserNombre");
        context.Session.Remove("Rol");
        */
        context.Session.Clear();
    }
    public bool IsAuthenticated()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("IsAuthenticated") == "true";//no entiendo bien que hace aca, creo que lo hace true y manda bool
    }
    public bool HasAccessLevel(string requiredAccessLevel)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("Rol") == requiredAccessLevel;//comparo ambos y si da verdadero retorna verdadero
    }

}
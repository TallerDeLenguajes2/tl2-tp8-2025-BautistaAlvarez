using tl2_tp8_2025_BautistaAlvarez.Interfaces;
namespace tl2_tp8_2025_BautistaAlvarez.Services;
//Esta clase en resumen sirve para el logueo y deslogueo y asi se cargan o limpian los datos de session. Ademas tiene funciones para verificar si esta autentificado o su rol cliente o admin
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;//para cargar los usuarios de la DB
    private readonly IHttpContextAccessor _httpContextAccessor;
    //esto es algo que tiene el sistema, sirve para tener acceso al contexto actual del http y para que carguemos datos a la sesion o sea context.sesion
    //private readonly HttpContext context;

    public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;//cargo los datos al hacer el constructor
        // context = _httpContextAccessor.HttpContext;
    }

    public bool Login(string username, string password)//sirve para loguear, cargo los datos de la base de dato usuario a la sesion que esta en el conxteto del http si se cumplen condiciones
    {
        var context = _httpContextAccessor.HttpContext;//obtengo el contexto
        var user = _userRepository.GetUser(username, password);//obtengo usuario mediante sql
        if (user != null)
        {
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }
            context.Session.SetString("IsAuthenticated", "true");//seteo valores, pongo que si es autenficado
            context.Session.SetString("User", user.User);//guardo valores en el Session para usarlas mas tarde
            context.Session.SetString("UserNombre", user.Nombre);
            context.Session.SetString("Rol", user.Rol);//es el tipo de acceso/rol admin o cliente
            return true;
        }

        return false;
    }
    public void Logout()//para cerrar sesion
    {
        var context = _httpContextAccessor.HttpContext;//accedo al contexto
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        /* context.Session.Remove("IsAuthenticated");
        context.Session.Remove("User");
        context.Session.Remove("UserNombre");
        context.Session.Remove("Rol");
        */
        context.Session.Clear();//limpias los datos que guardaste en sesion
    }
    public bool IsAuthenticated()//sirve para ver si esta autenficiado, compara el valor autheticated guardado en session si es true o no
    {
        var context = _httpContextAccessor.HttpContext;//cargo contexto actual
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("IsAuthenticated") == "true";//si IsAutheticated es true devuelvo true, obtenido de session
    }
    public bool HasAccessLevel(string requiredAccessLevel)//compara el Rol de la session actual del contexto http y con el string que le mandemos ("admin" o "Cliente")
    {
        var context = _httpContextAccessor.HttpContext;//cargo contexto
        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }
        return context.Session.GetString("Rol") == requiredAccessLevel;//comparo ambos y si da verdadero retorna verdadero
    }

}
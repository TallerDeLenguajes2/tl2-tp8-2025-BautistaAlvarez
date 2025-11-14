namespace tl2_tp8_2025_BautistaAlvarez.Interfaces;

using tl2_tp8_2025_BautistaAlvarez.Models;
public interface IUserRepository
{
    // Retorna el objeto Usuario si las credenciales son válidas, sino null.
    Usuario GetUser(string username, string password);
}

namespace pizzashop_service.Interface;

public interface IJwtService
{
      Task<string> GenerateJwtToken(string email, int roleId);
}

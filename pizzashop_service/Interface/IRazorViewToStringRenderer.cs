using Microsoft.AspNetCore.Mvc;

namespace pizzashop_service.Interface;

public interface IRazorViewToStringRenderer
{
    Task<string> RenderViewToStringAsync(string viewName, object model);
}

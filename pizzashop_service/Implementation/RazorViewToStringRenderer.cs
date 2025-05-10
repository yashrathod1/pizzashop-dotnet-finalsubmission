using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using pizzashop_service.Interface;



public class RazorViewToStringRenderer : IRazorViewToStringRenderer
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewToStringRenderer(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderViewToStringAsync(string viewName, object model)
    {
   
        DefaultHttpContext? httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

        ActionContext? actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
        {
            string? errorMessage = $"View '{viewName}' not found. Error: {viewResult.SearchedLocations}";
            Console.WriteLine(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
        var view = viewResult.View;

       
        using StringWriter? writer = new StringWriter();

        ViewContext? viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(httpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions()
        );

        await view.RenderAsync(viewContext);

      
        return writer.ToString();
    }
}


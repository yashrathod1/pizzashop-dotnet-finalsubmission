using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class OrderAppService : IOrderAppService
{
    private readonly IOrderAppRepository _orderAppRepository;

    public OrderAppService(IOrderAppRepository orderAppRepository)
    {
        _orderAppRepository = orderAppRepository;
    }

   

    
}

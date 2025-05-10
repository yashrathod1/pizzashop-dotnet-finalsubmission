using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;


namespace pizzashop_repository.Implementation;

public class OrderAppRepository : IOrderAppRepository
{
    private readonly PizzaShopDbContext _context;

    public OrderAppRepository(PizzaShopDbContext context)
    {
        _context = context;
    }


   

}

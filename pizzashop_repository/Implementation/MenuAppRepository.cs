using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;

namespace pizzashop_repository.Implementation;

public class MenuAppRepository : IMenuAppRepository
{
    private readonly PizzaShopDbContext _context;

    public MenuAppRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.Where(c => !c.Isdeleted).OrderBy(m => m.Id).ToListAsync();
    }

    public IQueryable<MenuItem> GetAllItemsQuery()
    {
        try
        {
            return _context.MenuItems.Where(m => !m.IsDeleted);
        }
        catch
        {
            throw;
        }
    }


    public async Task<MenuItem?> GetItemById(int Id)
    {
        try
        {
            return await _context.MenuItems.Where(m => m.Id == Id && !m.IsDeleted).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<Order?> GetOrderById(int Id)
    {
        try
        {
            return await _context.Orders.Include(o => o.Payment).Where(o => o.Id == Id).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateItemAsync(MenuItem item)
    {
        try
        {
            _context.MenuItems.Update(item);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the item in the database", ex);
        }
    }

    public async Task<List<MappingMenuItemWithModifier>> GetModifierInItemCardAsync(int id)
    {
        try
        {
            return await _context.MappingMenuItemWithModifiers.Include(mmim => mmim.ModifierGroup)
                                                        .ThenInclude(m => m.Modifiergroupmodifiers)
                                                        .ThenInclude(mgm => mgm.Modifier)
                                                        .Include(mmim => mmim.MenuItem)
                                                        .Where(mmim => mmim.MenuItemId == id).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error Fetching the modifer in itemcard", ex);
        }
    }

    public async Task<List<OrdersTableMapping>> GetTableDetailsByOrderIdAsync(int orderId)
    {
        try
        {
            return await _context.OrdersTableMappings.Include(ot => ot.Table).ThenInclude(t => t.Section).Where(o => o.Orderid == orderId).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<Modifier>> GetModifiersByIds(List<int> ids)
    {
        try
        {
            return await _context.Modifiers.Where(m => ids.Contains(m.Id)).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<Taxesandfee>> GetAllTaxesAsync()
    {
        return await _context.Taxesandfees
            .Where(t => !t.Isdeleted)
            .ToListAsync();
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {

        try
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }
        catch
        {

            return false;

        }

    }

    public async Task<bool> InsertOrderItemAsync(OrderItemsMapping item)
    {
        try
        {
            await _context.OrderItemsMappings.AddAsync(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error Occured", ex);
        }
    }

    public async Task<bool> InsertOrderModifierAsync(OrderItemModifier modifier)
    {
        try
        {
            await _context.OrderItemModifiers.AddAsync(modifier);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error Occured", ex);
        }
    }

    public async Task<bool> UpdateOrderTaxAsync(OrderTaxesMapping tax)
    {
        try
        {
            _context.OrderTaxesMappings.Update(tax);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Update Order Failed", ex);
        }
    }

    public async Task<bool> DecreaseItemQuantityAsync(int itemId, int quantity)
    {
        try
        {
            MenuItem? item = await _context.MenuItems.FirstOrDefaultAsync(x => x.Id == itemId);
            if (item != null)
            {
                item.Quantity -= quantity;
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DecreaseModifierQuantityAsync(string modifierName, int quantity)
    {
        try
        {
            Modifier? modifier = await _context.Modifiers.FirstOrDefaultAsync(x => x.Name == modifierName);
            if (modifier != null)
            {
                modifier.Quantity -= quantity;
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Table>> GetTablesByOrderIdAsync(int orderId)
    {
        try
        {
            return await _context.Tables
            .Include(t => t.OrdersTableMappings)
            .Where(t => t.OrdersTableMappings.Any(otm => otm.Orderid == orderId))
            .ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateTableAsync(Table table)
    {

        try
        {
            _context.Tables.Update(table);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {

            throw new Exception("Failed To Update Table", ex);

        }

    }

    public async Task<List<OrderItemsMapping>> GetOrderItemsAsync(int orderId)
    {
        try
        {
            return await _context.OrderItemsMappings.Include(oi => oi.Menuitem).Where(oi => oi.Orderid == orderId && !oi.Isdeleted).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<OrderItemModifier>> GetOrderModifiersAsync(int Orderid)
    {
        try
        {
            return await _context.OrderItemModifiers.Include(oim => oim.Orderitem)
                .Where(m => m.Orderitem.Orderid == Orderid)
                .ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<OrderTaxesMapping>> GetOrderTaxesAsync(int orderId)
    {
        try
        {
            return await _context.OrderTaxesMappings.Include(ot => ot.Tax)
                .Where(t => t.OrderId == orderId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
    {
        try
        {
            return await _context.Payments.Where(p => p.Orderid == orderId).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> InsertPaymentInfoAsync(Payment payment)
    {
        try
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("add payment failed", ex);
        }
    }

    public async Task<bool> UpdatePaymentInfoAsync(Payment payment)
    {

        try
        {
            _context.Payments.Update(payment);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {

            throw new Exception("Failed To Update OrderItem", ex);

        }

    }

    public async Task<OrderItemsMapping?> GetOrderItemByIdAndOrderIdAsync(int itemId, int orderId)
    {
        return await _context.OrderItemsMappings.Where(oi => oi.Id == itemId && oi.Orderid == orderId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateOrderItemAsync(OrderItemsMapping orderItems)
    {

        try
        {
            _context.OrderItemsMappings.Update(orderItems);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {

            throw new Exception("Failed To Update OrderItem", ex);

        }

    }

    public async Task<OrderItemsMapping?> GetOrderItemByItemIdAsync(int orderItemId)
    {
        try
        {
            return await _context.OrderItemsMappings.Where(oi => oi.Id == orderItemId && !oi.Isdeleted).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<Order?> GetCustomerDetailsFromOrderId(int orderId)
    {
        try
        {
            return await _context.Orders.Include(o => o.Customer).Include(c => c.OrdersTableMappings).Where(o => o.Id == orderId).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        try
        {
            return await _context.Customers
                                 .Include(c => c.WaitingTokens)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateCustomerAsync(Customer customer)
    {
        try
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync() > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<OrderItemsMapping>> GetOrderItemListByOrderIdAsync(int orderId)
    {
        try
        {
            return await _context.OrderItemsMappings.Where(oi => oi.Orderid == orderId && !oi.Isdeleted).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> AddFeedbackAsync(Feedback feedback)
    {
        try
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error Occured", ex);
        }
    }

    public async Task<bool> AddInvoiceAsync(Invoice invoice)
    {
        try
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating invoice", ex);
        }
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Payment)
            .Include(o => o.OrderItemsMappings)
                .ThenInclude(o => o.OrderItemModifiers)
            .Include(o => o.OrderTaxesMappings)
                .ThenInclude(o => o.Tax)
            .Include(o => o.OrdersTableMappings)
                .ThenInclude(o => o.Table)
                .ThenInclude(o => o.Section)
            .Include(o => o.Invoices)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

}








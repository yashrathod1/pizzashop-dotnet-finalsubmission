using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class KOTService : IKOTService
{
    private readonly IKOTRepository _kOTRepository;

    public KOTService(IKOTRepository kOTRepository)
    {
        _kOTRepository = kOTRepository;
    }

    public async Task<KOTViewModel> GetCategoryAsync()
    {
        List<Category>? category = await _kOTRepository.GetCategory();

        KOTViewModel? viewmodel = new KOTViewModel
        {
            KOTCategory = category.Select(c => new KOTCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList()
        };

        return viewmodel;
    }

    public async Task<KOTViewModel> GetKOTDataAsync(int? categoryId, string status)
    {
        List<Order>? orders = await _kOTRepository.GetOrdersWithItemsAsync(categoryId, status);

        List<KOTOrderCardViewModel>? orderCards = orders.Where(o => o.Status != "Completed" && o.Status != "Served").Select(o => new KOTOrderCardViewModel
        {
            OrderId = o.Id,
            CreatedAt = o.Createdat,
            OrderInstruction = o.Comment,
            ItemInstruction = o.OrderItemsMappings.Select(oi => oi.Instruction).FirstOrDefault(),
            Categoryid = o.OrderItemsMappings.Select(oi => oi.Menuitem.Categoryid).FirstOrDefault(),

            SectionTable = o.OrdersTableMappings.Select(map => new KOTOrderSectionTableViewModel
            {
                TableName = map.Table?.Name ?? "N/A",
                SectionName = map.Table?.Section?.Name ?? "N/A"
            }).ToList(),
            Items = o.OrderItemsMappings.Where(item => !item.Isdeleted &&
                       (categoryId == null || item.Menuitem.Categoryid == categoryId) &&
                       ((status == "Ready" && item.Preparedquantity > 0) ||
                       (status == "In Progress" && (item.Quantity - item.Preparedquantity) > 0))).Select(item => new KOTOrderItemViewModel
                       {
                           ItemName = item.ItemName ?? "Unknown",
                           Quantity = status == "Ready" ? item.Preparedquantity : (item.Quantity - item.Preparedquantity),
                           Modifiers = item.OrderItemModifiers
                           .Select(mod => mod.ModifierName ?? "")
                           .ToList()

                       }).ToList()
        }).ToList();

        return new KOTViewModel
        {
            OrderCard = orderCards
        };
    }

    public async Task<KOTOrderCardViewModel?> GetOrderCardByIdAsync(int orderId, string status)
    {
        Order? order = await _kOTRepository.GetOrderCardWithIdAsync(orderId);

        if (order == null) return null;

        List<KOTOrderItemViewModel>? items = new List<KOTOrderItemViewModel>();

        foreach (OrderItemsMapping? item in order.OrderItemsMappings.Where(oi => !oi.Isdeleted))
        {
            int? quantityToShow = 0;

            if (status == "In Progress")
            {
                quantityToShow = item.Quantity - item.Preparedquantity;
                if (quantityToShow <= 0) continue;
            }
            else if (status == "Ready")
            {
                quantityToShow = item.Preparedquantity;
                if (quantityToShow <= 0) continue;
            }

            items.Add(new KOTOrderItemViewModel
            {
                ItemId = item.Id,
                ItemName = item.ItemName,
                Quantity = quantityToShow,
                Modifiers = item.OrderItemModifiers.Select(m => m.ModifierName).ToList()
            });
        }

        return new KOTOrderCardViewModel
        {
            OrderId = order.Id,
            Items = items
        };
    }


    public async Task UpdatePreparedQuantitiesAsync(UpdatePreparedItemsViewModel model)
    {
        foreach (PreparedItemViewModel? item in model.Items)
        {
            OrderItemsMapping? orderItem = await _kOTRepository.GetOrderItemAsync(model.OrderId, item.ItemId);
            if (orderItem == null) continue;

            int totalQty = orderItem.Quantity;
            int currentQty = orderItem.Preparedquantity ?? 0;
            int newQty = currentQty;

            if (model.status == "In Progress")
            {
                newQty = Math.Min(currentQty + item.changeInQuantity, totalQty);
            }
            else if (model.status == "Ready")
            {
                newQty = Math.Max(currentQty - item.changeInQuantity, 0);
            }

            orderItem.Preparedquantity = (byte)newQty;

            await _kOTRepository.UpdateOrderItemAsync(orderItem);
        }

        List<OrderItemsMapping>? allItems = await _kOTRepository.GetOrderItemsByOrderIdAsync(model.OrderId);
        bool allPrepared = allItems.All(i => (i.Preparedquantity ?? 0) >= i.Quantity);
        if (allPrepared)
        {
            Order? order = await _kOTRepository.GetOrderByIdAsync(model.OrderId);
            if (order != null)
            {
                order.Status = "Served";
                order.Servingtime = DateTime.Now;
                await _kOTRepository.UpdateOrderAsync(order);
            }
        }
    }

}


using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;
using SelectPdf;

namespace pizzashop_service.Implementation;

public class MenuAppService : IMenuAppService
{
    private readonly IMenuAppRepository _menuAppRepository;

    private readonly IRazorViewToStringRenderer _razorRenderer;

    public MenuAppService(IMenuAppRepository menuAppRepository, IRazorViewToStringRenderer razorRenderer)
    {
        _menuAppRepository = menuAppRepository;
        _razorRenderer = razorRenderer;
    }

    public async Task<MenuAppViewModel> GetCategoriesAsync()
    {
        List<Category>? categories = await _menuAppRepository.GetCategoriesAsync();

        MenuAppViewModel? model = new()
        {
            CategoryList = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList(),
        };
        return model;
    }


    public async Task<List<MenuAppItemViewModel>> GetItemsAsync(int? categoryId = null, bool? isFavourite = null, string? searchTerm = null)
    {
        IQueryable<MenuItem> query = _menuAppRepository.GetAllItemsQuery();

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.Categoryid == categoryId.Value);
        }

        if (isFavourite.HasValue)
        {
            query = query.Where(m => m.Isfavourite == isFavourite.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string lowerSearch = searchTerm.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(lowerSearch));
        }

        List<MenuItem>? items = await query.ToListAsync();

        return items.Select(item => new MenuAppItemViewModel
        {
            Id = item.Id,
            Name = item.Name,
            ItemType = item.Type,
            IsFavourite = item.Isfavourite,
            Rate = item.Rate,
            ItemImagePath = item.ItemImage
        }).ToList();
    }

    public async Task<bool> ToggleIsFavourite(int id)
    {
        try
        {
            MenuItem? items = await _menuAppRepository.GetItemById(id);
            if (items == null) return false;

            items.Isfavourite = !items.Isfavourite;

            await _menuAppRepository.UpdateItemAsync(items);

            return items.Isfavourite;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in Fetching item by category", ex);
        }
    }

    public async Task<MenuAppModifierDetailViewModel> GetModifierInItemCardAsync(int id)
    {
        try
        {
            MenuItem? item = await _menuAppRepository.GetItemById(id) ?? throw new Exception("Item Not Found");

            List<MappingMenuItemWithModifier>? mapping = await _menuAppRepository.GetModifierInItemCardAsync(id);

            List<MenuAppItemModifierGroupViewModel>? modifierGroups = mapping.Select(mapping => new MenuAppItemModifierGroupViewModel
            {

                ModifierGroupName = mapping.ModifierGroup.Name,
                ModifierGroupId = mapping.ModifierGroup.Id,
                MinQuantity = mapping.MinModifierCount,
                MaxQuantity = mapping.MaxModifierCount,
                Modifiers = mapping.ModifierGroup.Modifiergroupmodifiers.Where(mgm => !mgm.Isdeleted && !mgm.Modifier.Isdeleted)
                                                                    .Select(mgm => new MenuAppItemModifiersViewModel
                                                                    {
                                                                        Id = mgm.Modifier.Id,
                                                                        Name = mgm.Modifier.Name,
                                                                        Amount = mgm.Modifier.Price,
                                                                        Quantity = mgm.Modifier.Quantity
                                                                    }).ToList()


            }).ToList();

            return new MenuAppModifierDetailViewModel
            {
                ItemQuantity = item.Quantity,
                ItemId = item.Id,
                ItemName = item.Name,
                ModifierGroups = modifierGroups
            };

        }
        catch (Exception ex)
        {
            throw new Exception("Error getting modifier in item card", ex);
        }
    }

    public async Task<MenuAppTableSectionViewModel> GetTableDetailsByOrderIdAsync(int orderId)
    {
        List<OrdersTableMapping>? mappings = await _menuAppRepository.GetTableDetailsByOrderIdAsync(orderId);

        if (mappings == null || mappings.Count == 0)
            throw new Exception("No table mappings found for this order.");

        string? sectionName = mappings.First().Table.Section.Name;

        List<string>? tableNames = mappings
            .Select(m => m.Table.Name)
            .Where(name => !string.IsNullOrEmpty(name))
            .Distinct()
            .ToList();

        return new MenuAppTableSectionViewModel
        {
            SectionName = sectionName,
            TableName = tableNames
        };
    }


    public async Task<MenuAppAddOrderItemViewModel> AddItemInOrder(int itemId, List<int> modifierIds)
    {
        MenuItem? item = await _menuAppRepository.GetItemById(itemId) ?? throw new ArgumentException("Invalid item ID");

        List<Modifier>? selectedModifiers = await _menuAppRepository.GetModifiersByIds(modifierIds);

        decimal totalModifierPrice = selectedModifiers.Sum(m => m.Price);

        MenuAppOrderItemViewModel? addItem = new MenuAppOrderItemViewModel
        {
            Id = item.Id,
            ItemName = item.Name,
            ItemAmount = item.Rate,
            ItemQuantity = item.Quantity,
            TotalModifierAmount = totalModifierPrice,
            SelectedModifiers = selectedModifiers.Select(m => new MenuAppModifierViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Amount = m.Price,
                Quantity = m.Quantity
            }).ToList(),
        };
        return new MenuAppAddOrderItemViewModel
        {
            Items = new List<MenuAppOrderItemViewModel> { addItem }
        };
    }

    public async Task<MenuAppOrderDetailsViewModel> GetOrderDetailsAsync(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderById(orderId) ?? throw new ArgumentException("Invalid Order ID");
        List<OrderItemsMapping>? items = await _menuAppRepository.GetOrderItemsAsync(orderId);
        List<OrderItemModifier>? modifiers = await _menuAppRepository.GetOrderModifiersAsync(orderId);
        List<OrderTaxesMapping>? taxes = await _menuAppRepository.GetOrderTaxesAsync(orderId);

        List<MenuAppOrderItemViewModel>? itemDetails = items.Select(i =>
        {

            List<OrderItemModifier>? itemModifiers = modifiers
                .Where(m => m.Orderitemid == i.Id)
                .ToList();

            return new MenuAppOrderItemViewModel
            {
                Id = i.Id,
                ItemName = i.ItemName,
                ItemQuantity = i.Quantity,
                ItemAmount = i.Price,
                SelectedModifiers = itemModifiers.Select(m => new MenuAppModifierViewModel
                {
                    Id = m.Id,
                    Name = m.ModifierName,
                    Amount = m.Price,
                    Quantity = m.Quantity
                }).ToList(),

                TotalModifierAmount = itemModifiers.Sum(m => m.Price) * i.Quantity
            };
        }).ToList();

        List<MenuAppOrderTaxSummaryViewModel>? taxDetails = taxes.Select(t => new MenuAppOrderTaxSummaryViewModel
        {
            Id = t.Id,
            Name = t.TaxName,
            TaxId = t.Tax.Id,
            Value = t.TaxValue,
            IsEnable = t.Tax.Isenabled,
            IsDefault = t.Tax.Isdefault,
            Type = t.Tax.Type
        }).ToList();

        return new MenuAppOrderDetailsViewModel
        {
            OrderId = orderId,
            Subtotal = order.Subamount,
            Items = itemDetails,
            Taxes = taxDetails,
            Total = order.Totalamount,
            PaymentMethod = order.Payment?.PaymentMethod ?? "",
        };
    }


    public async Task<bool> SaveOrder(MenuAppOrderDetailsViewModel model)
    {
        try
        {
            Order? order = await _menuAppRepository.GetOrderById(model.OrderId);
            if (order == null)
                return false;

            Payment? existingPayment = await _menuAppRepository.GetPaymentByOrderIdAsync(model.OrderId);
            if (existingPayment == null)
            {
                Payment? payment = new()
                {
                    Orderid = model.OrderId,
                    PaymentMethod = model.PaymentMethod,
                    Amount = model.Total
                };
                await _menuAppRepository.InsertPaymentInfoAsync(payment);
                order.Paymentid = payment.Id;
            }
            else if (existingPayment.PaymentMethod != model.PaymentMethod || existingPayment.Amount != model.Total)
            {
                existingPayment.PaymentMethod = model.PaymentMethod;
                existingPayment.Amount = model.Total;
                await _menuAppRepository.UpdatePaymentInfoAsync(existingPayment);
            }

            // Update order info
            order.Subamount = model.Subtotal;
            order.Totalamount = model.Total;
            order.Updatedat = DateTime.Now;
            order.Status = "In Progress";

            await _menuAppRepository.UpdateOrderAsync(order);

            foreach (MenuAppOrderItemViewModel? itemVm in model.Items)
            {
                OrderItemsMapping? existingItem = await _menuAppRepository.GetOrderItemByIdAndOrderIdAsync(itemVm.Id, model.OrderId);

                OrderItemsMapping orderItem;
                if (existingItem != null &&
                    (existingItem.Quantity != itemVm.ItemQuantity))
                {
                    existingItem.Quantity += itemVm.ItemQuantity;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.Price;
                    await _menuAppRepository.UpdateOrderItemAsync(existingItem);

                    orderItem = existingItem;
                }
                else if (existingItem == null)
                {
                    orderItem = new OrderItemsMapping
                    {
                        Orderid = model.OrderId,
                        ItemName = itemVm.ItemName,
                        Menuitemid = itemVm.Id,
                        Quantity = itemVm.ItemQuantity,
                        Price = itemVm.ItemAmount,
                        TotalPrice = itemVm.ItemQuantity * itemVm.ItemAmount
                    };
                    await _menuAppRepository.InsertOrderItemAsync(orderItem);

                    int orderItemId = orderItem.Id;

                    foreach (MenuAppModifierViewModel? modVm in itemVm.SelectedModifiers)
                    {
                        OrderItemModifier? orderMod = new()
                        {
                            Orderitemid = orderItemId,
                            ModifierName = modVm.Name,
                            Price = modVm.Amount,
                            Quantity = itemVm.ItemQuantity,
                            Modifierid = modVm.Id
                        };
                        await _menuAppRepository.InsertOrderModifierAsync(orderMod);
                    }

                    await _menuAppRepository.DecreaseItemQuantityAsync(itemVm.Id, itemVm.ItemQuantity);
                }
            }

            List<OrderTaxesMapping>? tax = await _menuAppRepository.GetOrderTaxesAsync(model.OrderId);
            foreach (MenuAppOrderTaxSummaryViewModel? taxVm in model.Taxes)
            {
                OrderTaxesMapping? existingTax = tax.FirstOrDefault(t => t.TaxId == taxVm.Id);
                if (existingTax != null)
                {
                    existingTax.TaxAmount = taxVm.Amount;
                    await _menuAppRepository.UpdateOrderTaxAsync(existingTax);
                }
            }

            List<Table>? tables = await _menuAppRepository.GetTablesByOrderIdAsync(model.OrderId);
            if (tables == null || !tables.Any())
                return false;

            foreach (Table? table in tables)
            {
                if (table.Status != "Occupied")
                {
                    table.Status = "Occupied";
                    await _menuAppRepository.UpdateTableAsync(table);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string Message)> DeleteOrderItemAsync(int orderItemId)
    {
        OrderItemsMapping? orderItem = await _menuAppRepository.GetOrderItemByItemIdAsync(orderItemId);

        if (orderItem == null)
        {
            return (false, "Item not found");
        }

        if (orderItem.Preparedquantity > 0)
        {

            return (false, $"You cannot delete this item because {orderItem.Preparedquantity} quantity of {orderItem.ItemName} is already ready");

        }

        orderItem.Isdeleted = true;

        bool result = await _menuAppRepository.UpdateOrderItemAsync(orderItem);

        return result
            ? (true, "Item deleted successfully")
            : (false, "Failed to delete item");
    }

    public async Task<MenuAppCustomerViewModel?> GetCustomerDetailsByOrderId(int orderId)
    {
        Order? customerDetails = await _menuAppRepository.GetCustomerDetailsFromOrderId(orderId);
        if (customerDetails == null) return null;

        MenuAppCustomerViewModel? model = new()
        {
            Id = customerDetails.Customer.Id,
            Name = customerDetails.Customer.Name,
            Email = customerDetails.Customer.Email,
            MobileNo = customerDetails.Customer.PhoneNumber,
            NoOfPerson = customerDetails.OrdersTableMappings.Select(ot => ot.NoOfPerson).FirstOrDefault()
        };

        return model;
    }

    public async Task<bool> UpdateCustomerDetailsAsync(MenuAppCustomerViewModel model)
    {
        Customer? customer = await _menuAppRepository.GetCustomerByIdAsync(model.Id);
        if (customer == null) return false;

        customer.Name = model.Name;
        customer.PhoneNumber = model.MobileNo;
        customer.Email = model.Email;

        WaitingToken? waitingToken = customer.WaitingTokens.FirstOrDefault();
        if (waitingToken != null)
        {
            waitingToken.NoOfPersons = model.NoOfPerson;
        }

        return await _menuAppRepository.UpdateCustomerAsync(customer);
    }

    public async Task<MenuAppOrderViewModel?> GetOrderCommentById(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderById(orderId);
        if (order == null) return null;

        return new MenuAppOrderViewModel
        {
            OrderComment = order.Comment
        };
    }

    public async Task<MenuAppItemInstructionViewModel?> GetItemInstructionById(int orderItemId, int orderId)
    {
        OrderItemsMapping? orderItem = await _menuAppRepository.GetOrderItemByIdAndOrderIdAsync(orderItemId, orderId);
        if (orderItem == null) return null;

        return new MenuAppItemInstructionViewModel
        {
            Instruction = orderItem.Instruction,
            ItemId = orderItem.Id

        };
    }

    public async Task<bool> UpdateInstruction(MenuAppItemInstructionViewModel model)
    {
        OrderItemsMapping? orderItem = await _menuAppRepository.GetOrderItemByIdAndOrderIdAsync(model.ItemId, model.OrderId);
        if (orderItem == null) return false;

        orderItem.Instruction = model.Instruction;

        return await _menuAppRepository.UpdateOrderItemAsync(orderItem);
    }

    public async Task<bool> UpdateOrderComment(MenuAppOrderViewModel model)
    {
        Order? order = await _menuAppRepository.GetOrderById(model.Id);
        if (order == null) return false;

        order.Comment = model.OrderComment;

        return await _menuAppRepository.UpdateOrderAsync(order);
    }

    public async Task<(bool Success, string Message)> CompleteOrderAsync(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderById(orderId);
        if (order == null)
            return (false, "Order not found");

        // Get order items
        List<OrderItemsMapping>? orderItems = await _menuAppRepository.GetOrderItemListByOrderIdAsync(orderId);
        if (orderItems == null || orderItems.Count == 0)
            return (false, "No items found for this order");

        List<OrderItemsMapping>? notReadyItems = orderItems.Where(x => x.Preparedquantity < x.Quantity).ToList();
        if (notReadyItems.Any())
            return (false, "All items must be served before completing the orders");

        order.Status = "Completed";
        await _menuAppRepository.UpdateOrderAsync(order);

        List<Table>? tables = await _menuAppRepository.GetTablesByOrderIdAsync(orderId);
        if (tables == null || !tables.Any())
            return (false, "No Tables found for this table");

        foreach (Table? table in tables)
        {
            if (table.Status != "Available")
            {
                table.Status = "Available";
                await _menuAppRepository.UpdateTableAsync(table);
            }
        }

        Payment? payment = await _menuAppRepository.GetPaymentByOrderIdAsync(orderId);
        if (payment != null)
        {
            payment.PaymentStatus = true;
            await _menuAppRepository.UpdatePaymentInfoAsync(payment);
        }

        string invoiceNumber = $"INV{DateTime.Now:yyyyMMddHHmmss}";

        Invoice? invoice = new()
        {
            Orderid = orderId,
            InvoiceNumber = invoiceNumber,
            Createdat = DateTime.Now,

        };
        await _menuAppRepository.AddInvoiceAsync(invoice);

        return (true, "Order completed successfully");
    }

    public async Task<MenuAppOrderViewModel?> GetOrderStatusAsync(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderById(orderId);
        if (order == null) return null;

        return new MenuAppOrderViewModel
        {
            Status = order.Status
        };
    }

    public async Task<bool> AddFeedbackAsync(MenuAppCustomerFeedbackViewModel model)
    {
        double average = (model.ServiceRating + model.FoodRating + model.AmbienceRating) / 3.0;
        int roundedAvg = (int)Math.Round(average);
        int clampedAvg = Math.Clamp(roundedAvg, 1, 5);
        Feedback? feedback = new()
        {
            OrderId = model.OrderId,
            Food = model.FoodRating,
            Service = model.ServiceRating,
            Ambience = model.AmbienceRating,
            Avgrating = clampedAvg,
            Comments = model.Comment
        };

        await _menuAppRepository.AddFeedbackAsync(feedback);
        return true;
    }

    public async Task<OrderDetailsViewModel?> GetOrderDetailsForInvoiceAsync(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderWithDetailsAsync(orderId);

        if (order == null)
            return null;

        // Fetch Payment Date where PaymentStatus is true
        DateTime paidOn = order.Payments?.FirstOrDefault(p => p.PaymentStatus)?.Createdat ?? DateTime.MinValue;

        string? paymentMethod = order.Payment != null ? order.Payment.PaymentMethod : "Pending";

        List<OrderItemViewModel>? orderItems = order.OrderItemsMappings.Select(oi => new OrderItemViewModel
        {
            ItemName = oi.ItemName,
            Quantity = oi.Quantity,
            Price = oi.Price,
            TotalAmount = oi.TotalPrice,

            ItemModifier = oi.OrderItemModifiers?.Select(m => new OrderItemModifierViewModel
            {
                Name = m.ModifierName,
                Quantity = m.Price,
                Price = m.Price,
                TotalAmount = m.Price
            }).ToList() ?? new List<OrderItemModifierViewModel>()

        }).ToList() ?? new List<OrderItemViewModel>();

        decimal subtotal = order.Subamount;

        List<OrderTaxViewModel>? orderTax = order.OrderTaxesMappings
                            .Where(ot => ot.Tax.Isenabled == true)
                            .Select(ot => new OrderTaxViewModel
                            {
                                TaxName = ot.TaxName,
                                TaxAmount = ot.TaxAmount
                            }).ToList() ?? new List<OrderTaxViewModel>();

        List<OrdersTableMapViewModel>? orderTable = order.OrdersTableMappings.Select(ott => new OrdersTableMapViewModel
        {
            TableName = ott.Table.Name,
            SectionName = ott.Table.Section.Name
        }).ToList() ?? new List<OrdersTableMapViewModel>();

        decimal? totalAmount = order.Totalamount;

        TimeSpan orderDuration = order.Updatedat - order.Createdat;

        return new OrderDetailsViewModel
        {
            OrderId = order.Id,
            InvoiceNo = order.Invoices.FirstOrDefault()?.InvoiceNumber,
            Status = order.Status,
            PlacedOn = order.Createdat,
            ModifiedOn = order.Updatedat,
            PaymentMethod = paymentMethod,
            Paidon = paidOn,
            OrderDuration = order.Updatedat - order.Createdat,
            CustomerName = order.Customer.Name,
            CustomerPhone = order.Customer.PhoneNumber,
            CustomerEmail = order.Customer.Email,
            NoOfPerson = order.Customer.NoOfPerson,
            OrderTable = orderTable,
            OrderItems = orderItems,
            OrderTax = orderTax,
            Subtotal = subtotal,
            TotalAmount = totalAmount
        };
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(int orderId)
    {
        OrderDetailsViewModel? model = await GetOrderDetailsForInvoiceAsync(orderId) ?? throw new Exception("Order not found!");

        string htmlContent = await _razorRenderer.RenderViewToStringAsync("Orders/OrderInvoice", model);

        HtmlToPdf? converter = new();

        converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;

        PdfDocument doc = converter.ConvertHtmlString(htmlContent);
        byte[] pdfBytes;
        using (MemoryStream? memoryStream = new())
        {
            doc.Save(memoryStream);
            pdfBytes = memoryStream.ToArray();
        }
        doc.Close();

        return pdfBytes;
    }

    public async Task<(bool Success, string Message)> CancelOrderAsync(int orderId)
    {
        Order? order = await _menuAppRepository.GetOrderById(orderId);

        if (order == null)
            return (false, "Order not found.");
        List<OrderItemsMapping>? orderItems = await _menuAppRepository.GetOrderItemsAsync(orderId);

        if (orderItems.Any(i => i.Preparedquantity > 0))
            return (false, "The order item is ready, cannot cancel the order.");
        order.Status = "Cancelled";

        List<Table>? tables = await _menuAppRepository.GetTablesByOrderIdAsync(orderId);
        if (tables == null || !tables.Any())
            return (false, "No Tables found for this table");

        foreach (Table? table in tables)
        {
            if (table.Status != "Available")
            {
                table.Status = "Available";
                await _menuAppRepository.UpdateTableAsync(table);
            }
        }
        await _menuAppRepository.UpdateOrderAsync(order);
        return (true, "Order cancelled successfully.");
    }

}


using pizzashop_repository.Models;

namespace pizzashop_repository.ViewModels;

public class OrderDetailsViewModel
{
    public int OrderId { get; set; }
    public string? InvoiceNo { get; set; }
    public string? Status { get; set; }
    public DateTime PlacedOn { get; set; }

    public DateTime Paidon { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public string? PaymentMethod  { get; set; }

    public TimeSpan OrderDuration { get; set; }

    // Customer Details (Directly in OrderDetailsViewModel)
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public int? NoOfPerson { get; set; }

    // Table Details
    public string? TableName { get; set; }
    public string? SectionName { get; set; }

    public List<OrdersTableMapViewModel>? OrderTable { get; set;}

    // Order Items
    public List<OrderItemViewModel>? OrderItems { get; set; }

    public List<OrderTaxViewModel>? OrderTax { get; set;}
    public decimal Subtotal { get; set; }
    // public decimal CGST { get; set; }
    // public decimal SGST { get; set; }
    // public decimal GST { get; set; }
    // public decimal OtherCharges { get; set; }
    public decimal? TotalAmount { get; set; }
}

public class OrdersTableMapViewModel
{
    public string? TableName { get; set;}
    public string? SectionName {get; set;}
}

public class OrderTaxViewModel 
{
    public string? TaxName { get; set; }

    public decimal? TaxValue { get; set; }

    public decimal? TaxAmount { get; set; }
}

public class OrderItemViewModel
{
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemModifierViewModel>? ItemModifier { get; set; }
}

public class OrderItemModifierViewModel
{
    public string? Name { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }

}



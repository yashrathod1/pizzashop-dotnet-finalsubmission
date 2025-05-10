namespace pizzashop_repository.ViewModels;

public class MenuAppOrderDetailsViewModel
{
    public List<MenuAppOrderItemViewModel> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Total {get; set; }

    public int OrderId { get; set; }

    public string? PaymentMethod { get; set;}
    public List<MenuAppOrderTaxSummaryViewModel> Taxes { get; set; } = new();
}

public class MenuAppAddOrderItemViewModel
{
    public List<MenuAppOrderItemViewModel> Items { get; set; } = new();
}

public class MenuAppOrderItemViewModel
{
    public int Id { get; set; }
    public string ItemName { get; set; } = null!;

    public decimal ItemAmount { get; set; }

    public int ItemQuantity { get; set; }

    public decimal TotalModifierAmount { get; set; }

    public List<MenuAppModifierViewModel> SelectedModifiers { get; set; } = new();
}

public class MenuAppModifierViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public int Quantity { get; set; }

}


public class MenuAppOrderTaxSummaryViewModel
{
    public int Id { get; set; }

    public int TaxId { get; set; }
    public string Name { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal Value { get; set; }

    public bool IsEnable { get; set; }
    public bool IsDefault { get; set; }

    public string Type { get; set; } = null!;
}
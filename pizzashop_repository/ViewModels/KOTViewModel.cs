namespace pizzashop_repository.ViewModels;

public class KOTViewModel
{   
    public List<KOTCategoryViewModel> KOTCategory { get; set; } = new();

    public List<KOTOrderCardViewModel> OrderCard {get; set;} = new();
}


public class KOTCategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class KOTOrderCardViewModel
{
    public int OrderId { get; set; }
    public List<KOTOrderSectionTableViewModel> SectionTable { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public int? Categoryid { get; set;}

    public string? Status {get; set;}

    public string? OrderInstruction { get; set; }

     public string? ItemInstruction { get; set; }

    public List<KOTOrderItemViewModel> Items { get; set; } = new();
}

public class KOTOrderItemViewModel
{
    public string ItemName { get; set; } = null!;
    public int? Quantity { get; set; }

    public int ItemId {get; set; }

    public int PreparedQuantity {get; set;}
    public List<string> Modifiers { get; set; } = new();
}

public class KOTOrderSectionTableViewModel 
{
    public string TableName {get; set;} = null!;

    public string SectionName {get; set;} = null!;
}
using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ModifierViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Modifier group is required.")]
    public int Modifiergroupid { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = null!;


    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Unit type is required.")]
    public string Unittype { get; set; } = null!;

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    public string? Description { get; set; }

    public bool Isdeleted { get; set; }

    [Required(ErrorMessage = "At least one modifier group must be selected.")]
    public List<int> ModifierGroupIds { get; set; } = new List<int>();

    public string? GroupName { get; set; }



    // public DateTime Createdat { get; set; }

    // public DateTime Updatedat { get; set; }

    // public int? Createdby { get; set; }

    // public int? Updatedby { get; set; }

}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Category is required.")]
        public int? Categoryid { get; set; }

        [Required(ErrorMessage = "Rate is required.")]
        [Range(0.01, 10000, ErrorMessage = "Rate must be between 0.01 and 10,000.")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1,000.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Item Type is required.")]
        public string ItemType { get; set; } = null!;

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(10, ErrorMessage = "Unit cannot exceed 10 characters.")]
        public string Unit { get; set; } = null!;

        [Required(ErrorMessage = "Short Code is required.")]
        [StringLength(10, ErrorMessage = "Short Code must be between 2 and 10 characters.", MinimumLength = 2)]
        public string? ShortCode { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; }

        [Column("IsdefaultTax")]
        public bool IsDefaultTax { get; set; }

        [Range(0, 100, ErrorMessage = "Tax Percentage must be between 0 and 100.")]
        public decimal TaxPercentage { get; set; }

        public IFormFile? ItemPhoto { get; set; }

        public string? ItemImagePath { get; set; }

        public List<ItemModifierGroupViewModel>? ModifierGroups { get; set; } = new List<ItemModifierGroupViewModel>();

        public List<int>? AssignedModifierGroups { get; set; }

        public string ItemTypeIcon
        {
            get
            {
                return ItemType switch
                {
                    "Veg" => "/images/icons/veg-icon.svg",
                    "Non-Veg" => "/images/icons/non-veg-icon.svg",
                    "Vegan" => "/images/icons/vegan-icon.svg",
                    _ => "/images/icons/unknown-icon.svg"
                };
            }
        }
    }
}



using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class TableAndSectionViewModel
{
     public List<SectionsViewModal> Sections { get; set; } = new List<SectionsViewModal>();
     
     [Required]
     public TableViewModel Table { get; set; } = new TableViewModel();

     [Required]
     public SectionsViewModal Section { get; set; } = new SectionsViewModal();
}

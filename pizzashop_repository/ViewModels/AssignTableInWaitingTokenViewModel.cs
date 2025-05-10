using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzashop_repository.ViewModels;

public class AssignTableInWaitingTokenViewModel
{
    public int CustomerId { get; set; }
    public int SectionId { get; set; }
    public List<int>? SelectedTables { get; set; }
    public int NumberOfPersons { get; set; }

}
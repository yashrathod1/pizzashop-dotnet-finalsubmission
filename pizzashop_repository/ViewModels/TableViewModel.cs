using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels
{
    public class TableViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Section is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid section")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Table name is required")]
        public string Name { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        public string Status { get; set; } = null!;

        public bool Isdeleted { get; set; }

        public DateTime? OrderTableTime {get; set;}

    }
}

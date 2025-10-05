using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class UpdateMenuDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Menü adı gereklidir")]
        [StringLength(100, ErrorMessage = "Menü adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "İkon en fazla 50 karakter olabilir")]
        public string? Icon { get; set; }

        [StringLength(200, ErrorMessage = "URL en fazla 200 karakter olabilir")]
        public string? Url { get; set; }

        [StringLength(100, ErrorMessage = "Controller en fazla 100 karakter olabilir")]
        public string? Controller { get; set; }

        [StringLength(100, ErrorMessage = "Action en fazla 100 karakter olabilir")]
        public string? Action { get; set; }

        public int? ParentId { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsVisible { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class MenuDto
    {
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

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public MenuDto? Parent { get; set; }
        public List<MenuDto> Children { get; set; } = new List<MenuDto>();
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    public class CreateMenuDto
    {
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

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class UpdateMenuDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
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

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}

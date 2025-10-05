using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ParentName { get; set; }
    }
}

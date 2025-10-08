using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class PermissionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İzin adı gereklidir")]
        [StringLength(100, ErrorMessage = "İzin adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Kod en fazla 100 karakter olabilir")]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }

    public class CreatePermissionDto
    {
        [Required(ErrorMessage = "İzin adı gereklidir")]
        [StringLength(100, ErrorMessage = "İzin adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Kod en fazla 100 karakter olabilir")]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdatePermissionDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "İzin adı gereklidir")]
        [StringLength(100, ErrorMessage = "İzin adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Kod en fazla 100 karakter olabilir")]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

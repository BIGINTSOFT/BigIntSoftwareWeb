using Entities.Entity;

namespace Entities.Dto
{
    public class UserWithSource
    {
        public User User { get; set; } = null!;
        public string Source { get; set; } = string.Empty;
    }
}

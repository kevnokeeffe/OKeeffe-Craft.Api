using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Entities
{
    public class EmailStatus
    {
        [Key]
        [MaxLength(40)]
        public required string Code { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }
    }
}

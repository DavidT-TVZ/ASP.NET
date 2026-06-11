using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }

        [ForeignKey("Character")]
        public int CharacterId { get; set; }

        public required string FileName { get; set; }

        public required string FilePath { get; set; }

        public string? ContentType { get; set; }

        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Character? Character { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Geekspace.Models
{
    public class UserLearningProgress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int LearningResourceId { get; set; }
        public LearningResource? LearningResource { get; set; }

        public bool IsSaved { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}

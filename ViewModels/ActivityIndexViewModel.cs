using Geekspace.Models;

namespace Geekspace.ViewModels
{
    public class ActivityIndexViewModel
    {
        public List<ResourceComment> Comments { get; set; } = new();
        public List<UserLearningProgress> LearningItems { get; set; } = new();

        public int SavedCount => LearningItems.Count(item => item.IsSaved);
        public int CompletedCount => LearningItems.Count(item => item.IsCompleted);
    }
}

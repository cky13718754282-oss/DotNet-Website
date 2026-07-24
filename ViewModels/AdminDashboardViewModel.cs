using Geekspace.Models;

namespace Geekspace.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int ResourceCount { get; set; }
        public int PublishedResourceCount { get; set; }
        public int CategoryCount { get; set; }
        public int UserCount { get; set; }
        public int CommentCount { get; set; }
        public int SavedCount { get; set; }
        public int CompletedCount { get; set; }

        public List<LearningResource> RecentResources { get; set; } = new();
        public List<ResourceComment> RecentComments { get; set; } = new();
        public Dictionary<string, string> CommentAuthors { get; set; } = new();
    }
}

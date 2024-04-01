using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CrashAnalytics.Models;
using System.Text.Json.Serialization;

namespace Models
{
    [Table("projects")]
    public class ProjectDTO: Project
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public virtual ICollection<CrashDTO> Crashes { get; } = new List<CrashDTO>();
    }
}
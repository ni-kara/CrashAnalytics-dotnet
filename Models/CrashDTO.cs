using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models;
using System.Text.Json.Serialization;

namespace CrashAnalytics.Models
{
    [Table("crashes")]
    public class CrashDTO : Crash
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]

        public Guid Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        [Column("project_id")]
        public Guid ProjectId { get; set; }
        [JsonIgnore]
        public virtual ProjectDTO Project { get; set; } = null!;


        public CrashDTO()
        {        }
        public CrashDTO(Crash crash) : this()
        {
            this.Message = crash.Message;
            this.Version = crash.Version;
            this.Type = crash.Type;
        }
    }
}

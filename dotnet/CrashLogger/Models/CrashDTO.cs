using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models;
using System.Text.Json.Serialization;

namespace CrashAnalytics.Models
{
    public class CrashDTO : Crash
    {
        public Guid Id { get; set; }

       public DateTime CreatedAt { get; set; }

       [JsonIgnore]
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

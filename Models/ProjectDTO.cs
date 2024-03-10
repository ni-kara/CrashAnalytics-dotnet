using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CrashAnalytics.Models;

namespace Models
{
    [Table("projects")]
    public class ProjectDTO: Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]

        public Guid Id { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CrashDTO> Crashes { get; } = new List<CrashDTO>();

    }
}
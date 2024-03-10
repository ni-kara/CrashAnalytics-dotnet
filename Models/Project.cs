using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrashAnalytics.Models
{
    public class Project
    {
        [Required(ErrorMessage = "An Project Name is required")]
        [Column("name")]
        public string Name { get; set; }

        public Project()
        {        }
    }
}

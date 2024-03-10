using Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CrashAnalytics.Models
{
    public class Crash 
    {
        [Required(ErrorMessage = "The DeviceType is required")]
        [Column("type")]
        public DeviceType Type { get; set; }

        [Required(ErrorMessage = "The Version is required")]
        [MaxLength(15)]
        [Column("version")]
        public string Version { get; set; }

        [Required(ErrorMessage = "An Message is required")]
        [Column("message")]
        public string Message { get; set; }

        public enum DeviceType { Android, iOS };

        public Crash()
        {        }
        
    }
}

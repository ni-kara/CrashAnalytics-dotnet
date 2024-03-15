using Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CrashAnalytics.Utils;
using System.ComponentModel;

namespace CrashAnalytics.Models
{
    public class Crash 
    {
        public DeviceType Type { get; set; }

        public string Version { get; set; }

        public string Message { get; set; }

        public enum DeviceType {
            [Description("Android")]
            Android,
            [Description("iOS")]
            iOS
        };

        public Crash()
        {        }
        
    }
}

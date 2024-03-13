using System.Diagnostics;

namespace CrashAnalytics.Utils
{
    public class EnvReader
    {
        private Dictionary<string, string> envKeys;
        public void Build(string filename=".env")
        {
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            
            if (!File.Exists(path))
                throw new Exception();

            envKeys = new Dictionary<string, string>();

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                // Split the line into key and value
                string[] parts = line.Split('=');

                // Check if there are key and value parts
                if (parts.Length == 2)
                {
                    // Trim whitespace and set the environment variable
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Set the environment variable
                    envKeys.Add(key, value);    
                }
            }
        }

        public string GetValue(string key)
        {
            envKeys.TryGetValue(key, out var value);
            return value;
        }
    }
}

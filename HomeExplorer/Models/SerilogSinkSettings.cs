using System.Collections.Generic;

namespace HomeExplorer.Models
{
    class SerilogSinkSettings
    {
        public string Name { get; set; }

        public Dictionary<string, string> Args { get; set; } = new();
    }
}

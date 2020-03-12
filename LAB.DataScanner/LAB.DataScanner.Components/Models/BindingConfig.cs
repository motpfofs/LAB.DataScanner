namespace LAB.DataScanner.Components.Models
{
    using System.Collections.Generic;

    public class BindingConfig
    {
        public string ComponentName { get; set; }
        public List<BindingConfigEntry> Bindings { get; set; }
    }
}
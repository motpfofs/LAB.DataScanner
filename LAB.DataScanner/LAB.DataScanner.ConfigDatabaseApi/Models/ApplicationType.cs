namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    using System.Collections.Generic;

    public partial class ApplicationType
    {
        public ApplicationType()
        {
            ApplicationInstance = new HashSet<ApplicationInstance>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeVersion { get; set; }
        public string ConfigTemplateJson { get; set; }
        
        public virtual ICollection<ApplicationInstance> ApplicationInstance { get; set; }
    }
}

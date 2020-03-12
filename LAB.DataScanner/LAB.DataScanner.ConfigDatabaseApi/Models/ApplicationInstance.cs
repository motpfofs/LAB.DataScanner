namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    using System.Collections.Generic;

    public partial class ApplicationInstance
    {
        public ApplicationInstance()
        {
            BindingConsumerInstance = new HashSet<Binding>();
            BindingPublisherInstance = new HashSet<Binding>();
        }

        public int InstanceId { get; set; }
        public int TypeId { get; set; }
        public string InstanceName { get; set;  }
        public string ConfigJson { get; set;  }

        public virtual ApplicationType Type { get; set;  }
        public virtual ICollection<Binding> BindingConsumerInstance { get; set; }
        public virtual ICollection<Binding> BindingPublisherInstance { get; set; }
    }
}

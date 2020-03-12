namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    public partial class Binding
    {
        public int PublisherInstanceId { get; set; }
        public int ConsumerInstanceId { get; set; }

        public virtual ApplicationInstance ConsumerInstance { get; set; }
        public virtual ApplicationInstance PublisherInstance { get; set; }
    }
}

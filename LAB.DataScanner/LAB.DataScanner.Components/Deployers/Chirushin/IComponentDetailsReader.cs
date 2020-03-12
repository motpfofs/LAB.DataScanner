namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    public interface IComponentDetailsReader
    {
        DeployingComponent GetComponentDetails(int serviceId);
    }
}

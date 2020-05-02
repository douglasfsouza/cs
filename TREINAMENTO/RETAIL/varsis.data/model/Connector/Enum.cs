namespace Varsis.Data.Model.Connector
{
    public enum IntegrationStatus
    {
        Pending = 0,
        Sent = 1,
        Imported = 2,
        Error = 99
    }

    public enum DetailIntegrationStatus
    {
        Imported = 1,
        Error = 99
    }
}

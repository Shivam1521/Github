namespace Entities
{
    public class EBSConfig
    {
        public required string ClientID { get; set; }
        public required string ClientSecret { get; set; }
        public required string Scope { get; set; }
        public required string TokenEndpoint { get; set; }
        public required string APIEndpoint { get; set; }
    }

    public class EBSAPILog
    {
        public required string APIName { get; set; }
        public required string URL { get; set; }
        public required string Request { get; set; }
        public required string Response { get; set; }
        public int RequestedBy { get; set; }
    }
}

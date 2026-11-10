namespace Entities
{
    public class EnvironmentVariables
    {
        public required string Name { get; set; }
        public required EnvironmentVariablesDetails Details { get; set; }
    }

    public class EnvironmentVariablesDetails
    {
        public required string QRURL { get; set; }
    }
}

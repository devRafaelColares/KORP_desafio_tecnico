
namespace EstoqueService.Configurations
{
    public static class ApiConstants
    {
        public const string HttpClientName = "EstoqueServiceClient";
        public const string ApplicationName = "estoque-service";
        public const string Version = "v1";
        public const string Author = "Rafael Colares";
        
        public const int DefaultStatusCode = 200;
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 25;

        public static string ConnectionString { get; set; } = string.Empty;
        public static string FrontendUrl { get; set; } = string.Empty;
        public static string BackendUrl { get; set; } = string.Empty;  
    }
}
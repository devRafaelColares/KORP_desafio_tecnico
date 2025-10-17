
namespace FaturamentoService.Configurations
{
    public static class ApiConstants
    {
        public const string HttpClientName = "FaturamentoServiceClient";
        public const string ApplicationName = "faturamento-service";
        public const string Version = "v1";
        public const string Author = "Rafael Colares";
        
        public const int DefaultStatusCode = 200;
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 25;

        public static string ConnectionString { get; set; } = string.Empty;
        public static string FrontendUrl { get; set; } = string.Empty;
        public static string BackendUrlFaturamentoService { get; set; } = string.Empty;  
    }
}
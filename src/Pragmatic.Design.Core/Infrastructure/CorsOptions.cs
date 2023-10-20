namespace Pragmatic.Design.Core.Infrastructure;

public class CorsOptions
{
    public bool AllowSpecificOrigins { get; set; }
    public string AllowedOrigins { get; set; } = "";
    public bool AllowSpecificHeaders { get; set; }
    public string AllowedHeaders { get; set; } = "";
    public bool AllowSpecificMethods { get; set; }
    public string AllowedMethods { get; set; } = "";
}

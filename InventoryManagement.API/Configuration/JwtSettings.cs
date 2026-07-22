namespace InventoryManagement.API.Configurtion;

public class JwtSettings{
    public string Key{set;get;}=String.Empty;
    public string Issuer{set;get;}=String.Empty;
    public string Audience{set;get;}=String.Empty;
    public int DurationInMinutes{set;get;}
}

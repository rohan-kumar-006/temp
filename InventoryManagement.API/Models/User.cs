namespace InventoryManagement.API.Models;
public class User
{
    public int Id {get;set;}
    public string FullName{get;set;}=string.Empty;
    public string Email{get;set;}=string.Empty;
    public string PassowrdHash{get;set;}=string.Empty;
    public string Role{get;set;}=string.Empty;
    public bool isActive{get;set;}
    public DateTime CreatedAt{get; set;}

}
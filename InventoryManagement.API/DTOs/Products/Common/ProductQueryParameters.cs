using Microsoft.Identity.Client;

namespace InventoryManagement.API.DTOs.Products.Common;

public class ProductQueryParameters
{
    private const int MaxPageSize=100;

    public int Page {get;set;}=1;

    private int _pageSize=10;

    public int pageSize
    {
        get=>_pageSize;

        set=> _pageSize= value > MaxPageSize ? MaxPageSize : value  ;
    }

    public string? Search{get;set;}  
    public decimal? MinPrice{get;set;}
    public decimal? MaxPrice{get;set;}
    public bool? LowStockOnly{get;set;}

    public string SortBy{get;set;}="Name";

    public bool Descending{get;set;}

}

namespace Catalog.API.Products.GetProductByCategory;
//Adding both Request and Response as record type for Minimal API-Carter

//This is masked as parameter comes via request url
//public record GetProductByCategoryRequest();

//Response - Record type - API Return - list of products

/// <summary>
/// Implement Endpoint via Minimal API and Carter to fetch products by category name
/// </summary>
/// <param name="Product"></param>
public record GetProductByCategoryResponse(IEnumerable<Product> Products);
public class GetProductByCategoryEndpoint : ICarterModule
{
    //Implement route for GET Api
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //Carter route handler and builder - Adding a category in path as we already have id api 
        app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
        {
            //Get result via Mediatr - Input Category value comes from request parameter
            var result = await sender.Send(new GetProductByCategoryQuery(category));
            //Using Mapster to perform object mapping with GetProductByCategoryResponse and GetProductByCategoryResult
            var response = result.Adapt<GetProductByCategoryResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductsByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products By Category")
            .WithDescription("Get Products By Category");
    }
}


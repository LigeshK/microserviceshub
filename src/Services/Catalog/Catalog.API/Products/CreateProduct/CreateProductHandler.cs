namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);
internal class CreateProductCommandHandler(IDocumentSession session, ILogger<CreateProductCommandHandler> logger) :
    ICommandHandler<CreateProductCommand, CreateProductResult>
{
    /// <summary>
    /// Add product - via Marten- IDocumentSession to the Postgres database and return its Id
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProductNotFoundException"></exception>
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Add product {@command}", command);
        //Create product entity
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };
        //save to DB
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        //return product object once successful
        return new CreateProductResult(product.Id);
    }
}


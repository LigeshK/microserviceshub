namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

//Fluent validation for Product inputs
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price need to be greater than 0");
    }
}
internal class CreateProductCommandHandler(IDocumentSession session) :
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


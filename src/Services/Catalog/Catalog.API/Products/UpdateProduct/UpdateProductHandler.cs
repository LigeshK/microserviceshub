
namespace Catalog.API.Products.UpdateProduct;

//CQRS - Command Record type for update product
public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<UpdateProductResult>;
//Record type for result
public record UpdateProductResult(bool IsSuccess);

//Fluent validations
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .Length(2,150).WithMessage("Name need to be between 2 and 150 characters");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price need to be greater than 0");
    }
}
/// <summary>
/// Update Product
/// </summary>
/// <param name="session">Interact with Postgres database - dependency injection via Marten library</param>
internal class UpdateProductCommandHandler(IDocumentSession session) :
    ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    /// <summary>
    /// Update product - get the right product via Marten- IDocumentSession from Postgres database and update the data
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProductNotFoundException"></exception>
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken) ?? throw new ProductNotFoundException(command.Id);
        product.Name = command.Name;
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);
        return new UpdateProductResult(true);

    }
}


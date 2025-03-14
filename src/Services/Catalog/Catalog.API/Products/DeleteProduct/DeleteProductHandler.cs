namespace Catalog.API.Products.DeleteProduct;
public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;
public record DeleteProductResult(bool IsSuccess);

//Fluent validations for delete product
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required for deletion");
    }
}
internal class DeleteProductCommandHandler(IDocumentSession session) :
    ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    /// <summary>
    /// Delete Product by Id
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProductNotFoundException"></exception>
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        //var product = await session.LoadAsync<Product>(command.Id, cancellationToken) ?? throw new ProductNotFoundException();
        //session.Delete(product);
        session.Delete<Product>(command.Id);
        await session.SaveChangesAsync(cancellationToken);
        return new DeleteProductResult(true);
    }
}


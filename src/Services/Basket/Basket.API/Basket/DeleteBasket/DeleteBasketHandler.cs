
namespace Basket.API.Basket.DeleteBasket;

public record DeleteBasketCommand(string UserName): ICommand<DeleteBasketResponse>;
public record DeleteBasketResult(bool IsSucess);

public class DeleteBasketValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
    }
}

public class DeleteBasketCommandHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResponse>
{
    public async Task<DeleteBasketResponse> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        //Delete basket from database
        await repository.DeleteBasket(command.UserName, cancellationToken);

        //Clear cache

        return new DeleteBasketResponse(true);
    }
}

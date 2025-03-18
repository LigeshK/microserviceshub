namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StorebasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StorebasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart cannot be null");
        RuleFor(x => x.Cart.UserName).NotNull().WithMessage("UserName is required");
    }
}
public class StoreBasketCommandHandler(IBasketRepository repository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        //Store cart to database (use Marten upsert -if exist -update db else insert db)
        await repository.StoreBasket(command.Cart,cancellationToken);
        //Update cache


        return new StoreBasketResult(command.Cart.UserName);
    }
}

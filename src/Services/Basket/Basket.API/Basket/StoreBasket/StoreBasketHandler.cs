using Discount.Grpc;

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
public class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) :
    ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    /// <summary>
    /// Build shopping cart/basket for user
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        //Calculate discount for items in basket by calling Discount grpc service
        await DeductDiscount(command.Cart, cancellationToken);

        //Store cart to database (use Marten upsert -if exist -update db else insert db)
        await repository.StoreBasket(command.Cart, cancellationToken);
        return new StoreBasketResult(command.Cart.UserName);
    }
    /// <summary>
    /// Deduct Discount for basket items
    /// </summary>
    /// <param name="cart"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        //Communicate with Discount.grpc, check for selected products have discount and apply it
        foreach (var item in cart.Items)
        {
            var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
            item.Price -= coupon.Amount;
        }
    }
    
}

namespace Basket.API.Data;
/// <summary>
/// Implement Repository pattern for Basket Service
/// </summary>
public class BasketRepository(IDocumentSession session) : IBasketRepository
{
    async Task<ShoppingCart> IBasketRepository.GetBasket(string userName, CancellationToken cancellationToken)
    {
        var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);
        return basket is null ? throw new BasketNotFoundException(userName) : basket;
    }

    async Task<ShoppingCart> IBasketRepository.StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        //marten has inbuilt capability to do insert and updatewith same command - upsert
        //so for same function will be used for both operations
        session.Store(basket);
        //check basket already exists , then updates or inserts
        await session.SaveChangesAsync(cancellationToken);
        return basket;
    }
    async Task<bool> IBasketRepository.DeleteBasket(string userName, CancellationToken cancellationToken)
    {
        session.Delete<ShoppingCart>(userName);
        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}

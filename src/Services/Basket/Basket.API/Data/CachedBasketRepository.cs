

using System.Text.Json;

namespace Basket.API.Data;
/// <summary>
/// Implemented CachedBasketRepository which acts as a decorator class for IBasketRepository adding caching functionalities
/// to optimise database performance
/// </summary>
/// <param name="repository"></param>
public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    /// <summary>
    /// Implemented proxy pattern and decorator pattern
    /// Proxy pattern - CachedBasketRepository acts as a proxy forwarding the calls to underlying basket repository
    /// Implement Decorator pattern - extends the functionalites of basket repository to add caching logic
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<ShoppingCart> IBasketRepository.GetBasket(string userName, CancellationToken cancellationToken)
    {
        //GetStringAsync - Key-value pair - Returns a output as JSON value string(Shopping basket) via key input - username
        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
        var basket = await repository.GetBasket(userName, cancellationToken);
        await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    async Task<ShoppingCart> IBasketRepository.StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        await repository.StoreBasket(basket, cancellationToken);
        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }
    async Task<bool> IBasketRepository.DeleteBasket(string userName, CancellationToken cancellationToken)
    {
        await repository.DeleteBasket(userName, cancellationToken);
        await cache.RemoveAsync(userName, cancellationToken);
        return true;
    }
}

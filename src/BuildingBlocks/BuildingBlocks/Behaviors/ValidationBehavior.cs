using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;
/// <summary>
/// Common validation execution for entire project with Fluentvalidation and MediatR Middleware
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="validators"></param>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    /// <summary>
    ///  Implement the IPipelineBehavior interface from MediatR. It validates all incoming requests, collects any validation errors, 
    ///  and throws an exception if any are found. Otherwise, it proceeds to the next middleware.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();
        if (failures.Count != 0)
            throw new ValidationException(failures);
        return await next();
    }
}


using FluentValidation;
using MediatR;
using Security.Domain.Exceptions;
using ValidationException = Security.Domain.Exceptions.ValidationException;

namespace Security.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs every registered FluentValidation
/// validator for a Command/Query before its Handler executes, so Commands and
/// Requests are validated in a single, centralized place instead of inside
/// each Handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

            throw new ValidationException("Uno o más campos no son válidos.", errors);
        }

        return await next();
    }
}

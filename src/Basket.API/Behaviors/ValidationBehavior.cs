using FluentValidation;
using MediatR;

namespace Basket.API.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        private readonly IEnumerable<IValidator> _validators;

        public ValidationBehavior(IEnumerable<IValidator> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = results
                .SelectMany(r => r.Errors)
                .Where(e => e is not null)
                .ToList();

            if (errors.Any())
                throw new ValidationException(errors);

            return await next();
        }
    }
}

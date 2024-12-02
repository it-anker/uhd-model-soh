using FluentValidation;
using FluentValidation.Results;
using Mars.Common.Core.Collections;
using Mars.Numerics.Comparers;
using MediatR;

namespace SOH.Process.Server.Validation;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = ResolveValidationFailures(out var validatorList);
        var ruleSetsExecuted = new HashSet<string>();

        List<Type> executed = [];
        foreach (var validator in validatorList)
        {
            var validatorType = validator.GetType();
            var alreadyExecutedServer = executed.Find(type => type.IsSubclassOf(validatorType));
            if (alreadyExecutedServer == null)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                var distinctSet = result.Errors
                    .DistinctBy(failure => new { failure.ErrorMessage, failure.PropertyName }).ToList();
                validationFailures.AddRange(distinctSet);
                ruleSetsExecuted.AddRange(result.RuleSetsExecuted);
                executed.Add(validatorType);
            }
        }

        var completeResult = new ValidationResult(validationFailures);

        if (completeResult.Errors.Exists(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(completeResult.Errors);
        }

        return await next();
    }

    private HashSet<ValidationFailure> ResolveValidationFailures(out List<IValidator<TRequest>> validatorList)
    {
        var validationFailures = new HashSet<ValidationFailure>([],
            new CustomComparer<ValidationFailure>((failure1, failure2) =>
                failure1 != null && failure2 != null &&
                failure1.PropertyName == failure2.PropertyName &&
                failure1.ErrorMessage == failure2.ErrorMessage
                    ? 0
                    : 1));

        var comparer = new CustomComparer<Type>((typeA, typeB) =>
        {
            if (typeA == null || typeB == null)
                return 1;
            if (typeA == typeB)
                return 0;

            return typeA.IsSubclassOf(typeB) ? -1 : 1;
        });

        validatorList = validators
            .OrderBy(validator => validator.GetType(), comparer)
            .ToList();
        return validationFailures;
    }
}
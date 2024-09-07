using FluentValidation;
using TaskManager.Shareable.Requests;

namespace TaskManager.Shareable.Validators;

public class GetTasksBySearchRequestValidator : AbstractValidator<GetTasksBySearchRequest>
{
    public GetTasksBySearchRequestValidator()
    {
        RuleFor(x => x.Search)
            .NotEmpty().WithMessage("O termo de busca não pode estar vazio.")
            .Length(1, 100).WithMessage("O termo de busca deve ter entre 1 e 100 caracteres.");
    }
}
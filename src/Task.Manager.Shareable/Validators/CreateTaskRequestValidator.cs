using FluentValidation;
using Task.Manager.Shareable.Requests;

namespace Task.Manager.Shareable.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.TaskEntityDTO.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título não pode ter mais de 100 caracteres.");

        RuleFor(x => x.TaskEntityDTO.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição não pode ter mais de 500 caracteres.");

        RuleFor(x => x.TaskEntityDTO.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data de criação não pode estar no futuro.");

        RuleFor(x => x.TaskEntityDTO.IsCompleted)
            .NotNull().WithMessage("O status de conclusão é obrigatório.");
    }
}
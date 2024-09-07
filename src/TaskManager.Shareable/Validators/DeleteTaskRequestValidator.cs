using FluentValidation;
using TaskManager.Shareable.Requests;

namespace TaskManager.Shareable.Validators;

public class DeleteTaskRequestValidator : AbstractValidator<DeleteTaskRequest>
{
    public DeleteTaskRequestValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("O ID da tarefa é obrigatório.")
            .Must(id => id != Guid.Empty).WithMessage("O ID da tarefa deve ser um GUID válido.");
    }
}
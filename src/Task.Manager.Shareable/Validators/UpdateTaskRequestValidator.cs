using FluentValidation;
using Task.Manager.Shareable.Requests;

namespace Task.Manager.Shareable.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.TaskEntityDTO)
            .NotNull().WithMessage("Os dados da tarefa são obrigatórios.");

        RuleFor(x => x.TaskEntityDTO.Id)
            .NotEmpty().WithMessage("O ID da tarefa é obrigatório.");

        RuleFor(x => x.TaskEntityDTO.Title)
            .NotEmpty().WithMessage("O título da tarefa é obrigatório.")
            .MaximumLength(100).WithMessage("O título da tarefa não pode exceder 100 caracteres.");

        RuleFor(x => x.TaskEntityDTO.Description)
            .MaximumLength(500).WithMessage("A descrição da tarefa não pode exceder 500 caracteres.");

        RuleFor(x => x.TaskEntityDTO.UpdatedAt)
            .NotEmpty().WithMessage("A data de atualização é obrigatória.");
    }
}
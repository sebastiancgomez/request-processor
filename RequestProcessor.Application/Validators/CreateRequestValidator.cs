using FluentValidation;
using RequestProcessor.Application.Dtos;

namespace RequestProcessor.Application.Validators;

public class CreateRequestValidator : AbstractValidator<CreateRequestDto>
{
    public CreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be null or blank.");
        RuleFor(x => x.Payload).NotEmpty().WithMessage("Payload can not be null or blank");
    }
}

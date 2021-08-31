using APICatalogo.Models.ViewModels;
using FluentValidation;

namespace APICatalogo.Models.ViewModelsValidators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Digite o e-mail")
                .EmailAddress().WithMessage("Digite um e-mail válido (exemplo: ana@gmail.com)");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Digite a senha");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.ConfirmPassword);
        }
    }
}

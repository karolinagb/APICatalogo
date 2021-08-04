using FluentValidation;

namespace APICatalogo.Models.ViewModelsValidators
{
    public class CategoriaViewModelValidator : AbstractValidator<Categoria>
    {
        public CategoriaViewModelValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome da categoria");
            RuleFor(x => x.ImagemUrl).NotEmpty().WithMessage("Informe a URL da imagem");
        }
    }
}

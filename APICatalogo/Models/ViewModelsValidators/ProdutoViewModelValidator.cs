using APICatalogo.Models.ViewModels;
using FluentValidation;

namespace APICatalogo.Models.ViewModelsValidators
{
    public class ProdutoViewModelValidator : AbstractValidator<ProdutoViewModel>
    {
        public ProdutoViewModelValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome do produto");
            RuleFor(x => x.Descricao).NotEmpty().WithMessage("Informe a descrição");
            RuleFor(x => x.Preco)
                .NotEmpty().WithMessage("Informe o preço")
                .GreaterThan(0).WithMessage("O preço tem que ser maior que 0,00");
            RuleFor(x => x.ImagemUrl).NotEmpty().WithMessage("Informe a URL da imagem");
            RuleFor(x => x.Estoque)
                .NotEmpty().WithMessage("Informe a quantidade em Estoque")
                .GreaterThanOrEqualTo(1).WithMessage("Quantidade em estoque deve ser no mínimo 1");
        }
    }
}

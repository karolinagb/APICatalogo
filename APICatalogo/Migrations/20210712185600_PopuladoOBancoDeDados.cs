using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class PopuladoOBancoDeDados : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Bebidas', 'http://www.macoratti.net/Imagens/1.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Lanches', 'http://www.macoratti.net/Imagens/2.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Sobremesas', 'http://www.macoratti.net/Imagens/3.jpg')");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) " +
                                            "Values('Coca-Cola Diet','Refrigerante de Cola 350 ml',5.45,'http://www.macoratti.net/Imagens/coca.jpg',50,now(),(Select Id from Categorias where Nome='Bebidas'))");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) " +
                                "Values('Lanche de Atum','Lanche de Atum com maionese',8.50,'http://www.macoratti.net/Imagens/atum.jpg',10,now(),(Select Id from Categorias where Nome='Lanches'))");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) " +
                                "Values('Pudim 100 g','Pudim de leite condensado 100g',6.75,'http://www.macoratti.net/Imagens/pudim.jpg',20,now(),(Select Id from Categorias where Nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Products(Name,Description,Price,ImageUrl,Stock,RegistrationDate,IdCategory)" +
            "Values('Coca-Cola Diet','Refrigerante de Cola 350 ml',5.45,'cocacola.jpg',50,now(),1)");

            mb.Sql("Insert into Products(Name,Description,Price,ImageUrl,Stock,RegistrationDate,IdCategory)" +
                "Values('Lanche de Atum','Lanche de Atum com maionese',8.50,'atum.jpg',10,now(),2)");

            mb.Sql("Insert into Products(Name,Description,Price,ImageUrl,Stock,RegistrationDate,IdCategory)" +
               "Values('Pudim 100 g','Pudim de leite condensado 100g',6.75,'pudim.jpg',20,now(),3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Products");
        }
    }
}

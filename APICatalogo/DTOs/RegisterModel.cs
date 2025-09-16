using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Senha é obrigatório")]
    public string? Password { get; set; }
}

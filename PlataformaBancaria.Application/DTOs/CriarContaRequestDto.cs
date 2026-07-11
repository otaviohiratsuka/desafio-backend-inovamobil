namespace PlataformaBancaria.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para receber os dados necessários à criação de uma nova Conta.
    /// </summary>
    public record CriarContaRequestDto(string Cnpj, string RazaoSocial, string Agencia);
}
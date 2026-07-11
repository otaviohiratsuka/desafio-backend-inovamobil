namespace PlataformaBancaria.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para retornar os dados de uma Conta ao consumidor da API,
    /// evitando expor diretamente a Entidade de Domínio.
    /// </summary>
    public record ContaResponseDto(
        Guid Id,
        string Cnpj,
        string RazaoSocial,
        string Agencia,
        decimal Saldo,
        string Status);
}
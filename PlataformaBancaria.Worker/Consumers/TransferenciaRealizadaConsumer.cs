using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Application.Events;
using PlataformaBancaria.Worker.Models;

namespace PlataformaBancaria.Worker.Consumers;

public class TransferenciaRealizadaConsumer : IConsumer<TransferenciaRealizadaEvent>
{
    private readonly IMongoCollection<TransacaoDocument> _transacoes;

    public TransferenciaRealizadaConsumer(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("PlataformaBancariaDb"); 
        _transacoes = database.GetCollection<TransacaoDocument>("Transacoes");
    }

    public async Task Consume(ConsumeContext<TransferenciaRealizadaEvent> context)
    {
        var mensagem = context.Message;

        var transacaoOrigem = new TransacaoDocument
        {
            ContaId = mensagem.ContaOrigemId, 
            Tipo = "Transferencia", 
            Valor = -mensagem.Valor,
            DataOcorrencia = mensagem.DataOcorrencia,
            Descricao = "Transferência enviada"
        };

        var transacaoDestino = new TransacaoDocument
        {
            ContaId = mensagem.ContaDestinoId,
            Tipo = "Transferencia", 
            Valor = mensagem.Valor, // Valor positivo no extrato
            DataOcorrencia = mensagem.DataOcorrencia,
            Descricao = "Transferência recebida"
        };

        // Salva as duas operações no MongoDB de uma vez
        await _transacoes.InsertManyAsync(new[] { transacaoOrigem, transacaoDestino });
    }
}
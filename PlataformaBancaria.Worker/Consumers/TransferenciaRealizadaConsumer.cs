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

        // 1. Lançamento para a Conta de Origem (Saiu o dinheiro)
        var transacaoOrigem = new TransacaoDocument
        {
            // O 'Id' é omitido para que o MongoDB gere o ObjectId automaticamente
            ContaId = mensagem.ContaOrigemId, 
            Tipo = "Transferencia", 
            Valor = -mensagem.Valor, // Valor negativo no extrato
            DataOcorrencia = mensagem.DataOcorrencia,
            Descricao = "Transferência enviada"
        };

        // 2. Lançamento para a Conta de Destino (Entrou o dinheiro)
        var transacaoDestino = new TransacaoDocument
        {
            // O 'Id' é omitido para que o MongoDB gere o ObjectId automaticamente
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
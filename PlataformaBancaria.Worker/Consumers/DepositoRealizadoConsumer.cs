using System;
using System.Threading.Tasks;
using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Application.Events;
using PlataformaBancaria.Worker.Models;

namespace PlataformaBancaria.Worker.Consumers
{
    public class DepositoRealizadoConsumer : IConsumer<DepositoRealizadoEvent>
    {
        private readonly IMongoCollection<TransacaoDocument> _collection;

        public DepositoRealizadoConsumer(IMongoClient mongoClient)
        {
            // O nome do banco corrigido para alinhar com o Saldo e Extrato
            var database = mongoClient.GetDatabase("PlataformaBancariaDb");
            _collection = database.GetCollection<TransacaoDocument>("Transacoes");
        }

        public async Task Consume(ConsumeContext<DepositoRealizadoEvent> context)
        {
            var transacao = new TransacaoDocument
            {
                ContaId = context.Message.ContaId,
                Tipo = "Deposito",
                Valor = context.Message.Valor, // Depósito entra positivo
                DataOcorrencia = context.Message.DataOcorrencia,
                Descricao = "Depósito recebido" // Adicionada a descrição para o extrato
            };

            // Salva fisicamente no MongoDB
            await _collection.InsertOneAsync(transacao);

            Console.WriteLine($"[MongoDB] Sucesso! Depósito de R$ {transacao.Valor} salvo no extrato da conta {transacao.ContaId}");
        }
    }
}
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
            // O Mongo cria o banco e a tabela automaticamente na primeira vez que usamos
            var database = mongoClient.GetDatabase("PlataformaBancariaReadDb");
            _collection = database.GetCollection<TransacaoDocument>("Transacoes");
        }

        public async Task Consume(ConsumeContext<DepositoRealizadoEvent> context)
        {
            var transacao = new TransacaoDocument
            {
                ContaId = context.Message.ContaId,
                Tipo = "Deposito",
                Valor = context.Message.Valor,
                DataOcorrencia = context.Message.DataOcorrencia
            };

            // Salva fisicamente no MongoDB
            await _collection.InsertOneAsync(transacao);

            Console.WriteLine($"[MongoDB] Sucesso! Depósito de R$ {transacao.Valor} salvo no extrato da conta {transacao.ContaId}");
        }
    }
}
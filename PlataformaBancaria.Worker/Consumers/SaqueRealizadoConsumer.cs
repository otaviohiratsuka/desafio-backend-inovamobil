using System;
using System.Threading.Tasks;
using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Application.Events; 
using PlataformaBancaria.Worker.Models;

namespace PlataformaBancaria.Worker.Consumers
{
    public class SaqueRealizadoConsumer : IConsumer<SaqueRealizadoEvent>
    {
        private readonly IMongoCollection<TransacaoDocument> _collection;

        public SaqueRealizadoConsumer(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("PlataformaBancariaReadDb");
            _collection = database.GetCollection<TransacaoDocument>("Transacoes");
        }

        public async Task Consume(ConsumeContext<SaqueRealizadoEvent> context)
        {
            var transacao = new TransacaoDocument
            {
                ContaId = context.Message.ContaId,
                Tipo = "Saque", // O tipo agora é "Saque"
                Valor = context.Message.Valor,
                DataOcorrencia = context.Message.DataOcorrencia
            };

            await _collection.InsertOneAsync(transacao);

            Console.WriteLine($"[MongoDB] Sucesso! Saque de R$ {transacao.Valor} salvo no extrato da conta {transacao.ContaId}");
        }
    }
}
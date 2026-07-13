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
            var database = mongoClient.GetDatabase("PlataformaBancariaDb");
            _collection = database.GetCollection<TransacaoDocument>("Transacoes");
        }

        public async Task Consume(ConsumeContext<SaqueRealizadoEvent> context)
        {
            var transacao = new TransacaoDocument
            {
                ContaId = context.Message.ContaId,
                Tipo = "Saque", 
                Valor = -context.Message.Valor, // Saque deve ser negativo para o /balance funcionar
                DataOcorrencia = context.Message.DataOcorrencia,
                Descricao = "Saque efetuado" // Adicionada a descrição para o extrato
            };

            await _collection.InsertOneAsync(transacao);

            Console.WriteLine($"[MongoDB] Sucesso! Saque de R$ {transacao.Valor} salvo no extrato da conta {transacao.ContaId}");
        }
    }
}
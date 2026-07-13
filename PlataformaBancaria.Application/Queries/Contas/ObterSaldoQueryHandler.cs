using MediatR;
using MongoDB.Driver;
using PlataformaBancaria.Application.Queries; // Necessário para acessar o TransacaoReadModel que você criou no Extrato

namespace PlataformaBancaria.Application.Queries.Contas;

public class ObterSaldoQueryHandler : IRequestHandler<ObterSaldoQuery, decimal>
{
    private readonly IMongoCollection<TransacaoReadModel> _collection;

    public ObterSaldoQueryHandler(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("PlataformaBancariaDb"); 
        
        _collection = database.GetCollection<TransacaoReadModel>("Transacoes");
    }

    public async Task<decimal> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<TransacaoReadModel>.Filter.Eq(t => t.ContaId, request.ContaId);
        
        var transacoes = await _collection.Find(filter).ToListAsync(cancellationToken);

        return transacoes.Sum(t => t.Valor);
    }
}
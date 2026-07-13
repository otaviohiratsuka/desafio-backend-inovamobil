using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PlataformaBancaria.Application.Queries
{
    // 1. Ajustado para string (Id do Mongo) e DataOcorrencia
    public record TransacaoDto(string Id, Guid ContaId, string Tipo, decimal Valor, DateTime DataOcorrencia);

    public class ObterExtratoQuery : IRequest<IEnumerable<TransacaoDto>>
    {
        public Guid ContaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Tipo { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 20;
    }

    public class ObterExtratoQueryHandler : IRequestHandler<ObterExtratoQuery, IEnumerable<TransacaoDto>>
    {
        private const int TamanhoPaginaMaximo = 100;
        private readonly IMongoCollection<TransacaoReadModel> _collection;

        public ObterExtratoQueryHandler(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("PlataformaBancariaDb");
            _collection = database.GetCollection<TransacaoReadModel>("Transacoes");
        }

        public async Task<IEnumerable<TransacaoDto>> Handle(ObterExtratoQuery request, CancellationToken cancellationToken)
        {
            var tamanhoPagina = Math.Min(request.TamanhoPagina, TamanhoPaginaMaximo);
            var pagina = Math.Max(request.Pagina, 1);

            var filtroBuilder = Builders<TransacaoReadModel>.Filter;
            var filtro = filtroBuilder.Eq(t => t.ContaId, request.ContaId);

            if (request.DataInicio is not null)
                filtro &= filtroBuilder.Gte(t => t.DataOcorrencia, request.DataInicio.Value);

            if (request.DataFim is not null)
                filtro &= filtroBuilder.Lte(t => t.DataOcorrencia, request.DataFim.Value);

            if (!string.IsNullOrWhiteSpace(request.Tipo))
                filtro &= filtroBuilder.Eq(t => t.Tipo, request.Tipo);

            var transacoes = await _collection
                .Find(filtro)
                .SortByDescending(t => t.DataOcorrencia)
                .Skip((pagina - 1) * tamanhoPagina)
                .Limit(tamanhoPagina)
                .ToListAsync(cancellationToken);

            return transacoes.Select(t => new TransacaoDto(t.Id!, t.ContaId, t.Tipo, t.Valor, t.DataOcorrencia));
        }
    }

    // 2. Modelo de leitura espelhado exatamente como o Worker gravou
 [BsonIgnoreExtraElements] 
     public class TransacaoReadModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonRepresentation(BsonType.String)] 
        public Guid ContaId { get; set; }
        
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string Descricao { get; set; } = string.Empty; // <-- O campo que estava faltando!
    }
}
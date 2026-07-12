using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlataformaBancaria.Worker.Models
{
    public class TransacaoDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ContaId { get; set; }

        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataOcorrencia { get; set; }
    }
}
using System;

namespace GerenciarTransacoes.Dominio
{
    public class Transaction
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }
}

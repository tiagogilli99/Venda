using System.Collections.Generic;

namespace VendaService.DTO
{
    public class Venda
    {
        public string Identificador { get; set; }
        public int VendaId { get; set; }
        public List<Itens> Itens { get; set; }
        public string NomeVendedor { get; set; }
        public decimal TotalVenda { get; set; }
    }

    public class Itens
    {
        public int ItemId { get; set; }

        public int Quantidade { get; set; }

        public decimal Preco { get; set; }
    }
}

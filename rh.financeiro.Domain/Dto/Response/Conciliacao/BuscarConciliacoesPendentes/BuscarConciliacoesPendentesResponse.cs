using rh.financeiro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Response.Conciliacao.BuscarConciliacoesPendentes
{
    public class BuscarConciliacoesPendentesResponse
    {
        public ParcelasPendetes? parcelasPendetes { get; set; }
        public MovimentosPendentes? movimentosPendentes { get; set; }
    }

    public class ParcelasPendetes
    {
        public int total { get; set; } = 0;
        public decimal? valorTotal { get; set; }
        public List<Parcela> itens { get; set; } = [];
    }

    public class MovimentosPendentes
    {
        public int total { get; set; } = 0;
        public decimal? valorTotal { get; set; }
        public List<MovimentoBancario> itens { get; set; } = [];
    }
}

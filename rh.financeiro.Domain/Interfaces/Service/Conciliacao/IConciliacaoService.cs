using rh.financeiro.Domain.Dto.Response.Conciliacao.BuscarConciliacoesPendentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Conciliacao
{
    public interface IConciliacaoService
    {
        Task<BuscarConciliacoesPendentesResponse> BuscarConciliacoesPendentes();
    }
}

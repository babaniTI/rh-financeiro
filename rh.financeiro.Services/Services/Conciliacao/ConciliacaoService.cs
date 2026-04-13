using Microsoft.EntityFrameworkCore;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Dto.Response.Conciliacao.BuscarConciliacoesPendentes;
using rh.financeiro.Domain.Dto.Response.MovimentacaoBancaria.BuscarMovimentacoesBancarias;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Conciliacao;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Conciliacao
{
    public class ConciliacaoService : IConciliacaoService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public ConciliacaoService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        public async Task<BuscarConciliacoesPendentesResponse> BuscarConciliacoesPendentes()
        {
            #region Fields
            var repoMovimentacaoBancaria = _unitOfWork.Repository<MovimentoBancario>().QueryableObject();
            var repoParcela = _unitOfWork.Repository<Parcela>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                var MovimentacoesPendentes = repoMovimentacaoBancaria
                    .AsEnumerable()
                    .Where(x => x.Status.ToString() == StatusMovimento.PENDENTE.ToString())
                    .ToList();

                var ParcelasPendentes =  repoParcela
                    .AsEnumerable()
                    .Where(x => x.Status.ToString() == StatusParcela.Aberto.ToString())
                    .ToList();

                return new()
                {
                    movimentosPendentes = new()
                    {
                        total = MovimentacoesPendentes.Count,
                        valorTotal = MovimentacoesPendentes.Sum(x => x.ValorLiquido)!,
                        itens = MovimentacoesPendentes
                    },

                    parcelasPendetes = new()
                    {
                        total = ParcelasPendentes.Count,
                        valorTotal = ParcelasPendentes.Sum(x => x.Valor)!,
                        itens = ParcelasPendentes
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            #endregion

        }
        #endregion

        #region Private Methods
        #endregion
    }


}

using Microsoft.EntityFrameworkCore;
using rh.financeiro.CrossCuting;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Domain.Dto.Paginacao;
using rh.financeiro.Domain.Dto.Request.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Dto.Request.Participantes.CriarParticipante;
using rh.financeiro.Domain.Dto.Request.Participantes.EditarParticipante;
using rh.financeiro.Domain.Dto.Response.Participantes.BuscarParticipantes;
using rh.financeiro.Domain.Entities;
using rh.financeiro.Domain.Enums;
using rh.financeiro.Domain.Interfaces.Service.Participantes;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Services.Services.Participantes
{
    public class ParticipantesService : IParticipantesService
    {
        #region Fields
        private readonly IUnitOfWork<DefaultContext> _unitOfWork;
        #endregion

        #region CONS
        public ParticipantesService(IUnitOfWork<DefaultContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods

        public async Task<Participante?> CriarParticipante(CriarParticipanteRequest request,string UsuarioId)
        {
            #region Fields
            var repoParticipante = _unitOfWork.Repository<Participante>();
            var repoUsuarioEmpresa = _unitOfWork.Repository<UsuarioEmpresa>();
            #endregion

            #region Logic
            try
            {
                bool jaExiste = await repoParticipante.QueryableObject()
                    .AnyAsync(x => x.Documento == request.cpfcnpj);

                if (jaExiste)
                    return null;

                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var NovoParticipante = new Participante()
                {
                    EmpresaId = EmpresaId,
                    Tipo = request.tipo switch
                    {
                        "CLIENTE" => TipoParticipante.Cliente,
                        "FORNECEDOR" => TipoParticipante.Fornecedor,
                        "TRANSPORTADORA" => TipoParticipante.Transportadora
                    },
                    Nome = request.razaoSocial,
                    Documento = request.cpfcnpj,
                };

                await repoParticipante.AddAsync(NovoParticipante);
                return NovoParticipante;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }


        public async Task<PaginacaoResponse<BuscarParticipantesResponse>> BuscarParticipantes(BuscarParticipantesRequest request, string UsuarioId)
        {
            #region Fields
            var queryParticipantes = _unitOfWork.Repository<Participante>().QueryableObject();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                // Filtro por Empresa
                queryParticipantes = queryParticipantes
                    .Where(x => x.EmpresaId.ToString() == EmpresaId.ToString());

                // Filtro por tipo
                if (request.tipo.HasValue)
                {
                    queryParticipantes = queryParticipantes
                        .Where(x => x.Tipo == request.tipo.Value);
                }

                // Filtro por busca
                if (!string.IsNullOrEmpty(request.search))
                {
                    queryParticipantes = queryParticipantes
                        .Where(x =>
                            x.Documento.Contains(request.search) ||
                            x.Nome.Contains(request.search)
                        );
                }


                // Total antes da paginação
                var total = await queryParticipantes.CountAsync();

                // Paginação
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = request.pageSize <= 0 ? 10 : request.pageSize;

                var data = await queryParticipantes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new BuscarParticipantesResponse
                    {
                        id = x.Id.ToString(),
                        nome = x.Nome,
                        cpfcnpj = x.Documento,
                        tipo = x.Tipo,
                        Ativo = true,
                        created_At = x.CreatedAt,
                        updated_At = x.UpdatedAt
                    })
                    .OrderByDescending(x => x.updated_At)
                    .ToListAsync();

                return new PaginacaoResponse<BuscarParticipantesResponse>
                {
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    data = data
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<Participante?> BuscarParticipantePorId(string id, string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {

                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                return await _unitOfWork.Repository<Participante>()
                    .QueryableObject()
                    .Where(x => x.Id.ToString() == id)
                    .Where(x => x.EmpresaId.ToString() == EmpresaId.ToString())
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<Participante?> EditarParticipantePorId(EditarParticipanteRequest request,string id, string UsuarioId)
        {
            #region Fields
            var repoParticipante = _unitOfWork.Repository<Participante>();
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                Guid EmpresaId = await BuscarEmpresaIdPorUsarioId(UsuarioId);

                var participante  = await repoParticipante.QueryableObject()
                    .FirstOrDefaultAsync(x => x.Id.ToString() == id && x.EmpresaId.ToString() == EmpresaId.ToString());

                if (participante == null)
                    return null;

                if (request.razaoSocial != null)
                    participante.Nome = request.razaoSocial;

                if (request.cpfcnpj != null)
                    participante.Documento = request.cpfcnpj;

                if (request.tipo != null)
                {
                    participante.Tipo = request.tipo switch
                    {
                        "CLIENTE" => TipoParticipante.Cliente,
                        "FORNECEDOR" => TipoParticipante.Fornecedor,
                        "TRANSPORTADORA" => TipoParticipante.Transportadora
                    };
                }

                await repoParticipante.UpdateAsync(participante);
                return participante;
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
        private async Task<Guid> BuscarEmpresaIdPorUsarioId(string UsuarioId)
        {
            #region Fields
            #endregion

            #region Logic
            try
            {
                // Buscando a EmpresaId pelo usuario autenticado
                return await _unitOfWork.Repository<UsuarioEmpresa>()
                    .QueryableObject()
                    .Where(x => x.UsuarioId.ToString() == UsuarioId)
                    .Select(x => x.EmpresaId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            #endregion
        }
        #endregion
    }
}

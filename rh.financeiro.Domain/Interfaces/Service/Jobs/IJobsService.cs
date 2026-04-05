using rh.financeiro.Domain.Dto.Request.Jobs.ImportarDocumentosAndGerarTitulos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Interfaces.Service.Jobs
{
    public interface IJobsService
    {
        Task<(bool success, string Message)> ImportarDocumentosAndGerarTitulos(ImportarDocumentosAndGerarTitulosRequest request);
        Task<(bool success, string Message)> AtualizarDocumentosAndTitulos(string Cnpj);
    }
}

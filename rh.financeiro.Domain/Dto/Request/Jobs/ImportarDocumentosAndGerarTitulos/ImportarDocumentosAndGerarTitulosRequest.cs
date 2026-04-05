using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Dto.Request.Jobs.ImportarDocumentosAndGerarTitulos
{
    public class ImportarDocumentosAndGerarTitulosRequest
    {
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "O campo início deve estar no formato yyyy-MM-dd.")]
        public string? inicio { get; set; }

        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "O campo fim deve estar no formato yyyy-MM-dd.")]
        public string? fim { get; set;}

        [Required(ErrorMessage = "O campo CNPJ é obrigatório.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter apenas números e ter 14 dígitos.")]
        public string cnpj { get; set; }
    }
}

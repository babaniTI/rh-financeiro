using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Domain.Common.DocumentosNfes
{
    public class NfeProc
    {
        public string? versao { get; set; }
        public NFe? NFe { get; set; }
        public ProtNFe? protNFe { get; set; }
    }

    public class NFe
    {
        public InfNFe? infNFe { get; set; }
    }

    public class InfNFe
    {
        public string? Id { get; set; }
        public string? versao { get; set; }
        public Ide? ide { get; set; }
        public Emit? emit { get; set; }
        public Dest dest { get; set; }
        public List<Det>? det { get; set; }
        public Pag? pag { get; set; }
        public Total? total { get; set; }
    }

    public class Ide
    {
        public string? cUF { get; set; }
        public string? cNF { get; set; }
        public string? natOp { get; set; }
        public string? mod { get; set; }
        public string? serie { get; set; }
        public string? nNF { get; set; }
        public DateTime? dhEmi { get; set; }
    }

    public class Emit
    {
        public string? CNPJ { get; set; }
        public string? xNome { get; set; }
        public string? xFant { get; set; }
        public EnderEmit? enderEmit { get; set; }
    }

    public class EnderEmit
    {
        public string? xLgr { get; set; }
        public string? nro { get; set; }
        public string? xBairro { get; set; }
        public string? xMun { get; set; }
        public string? UF { get; set; }
    }

    public class Dest
    {
        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public string? xNome { get; set; }
        public string? idEstrangeiro { get; set; }
        public EnderDest? enderDest { get; set; }
        public string? indIEDest { get; set; }
    }

    public class EnderDest
    {
        public string? xLgr { get; set; }
        public string? nro { get; set; }
        public string? xBairro { get; set; }
        public string? xMun { get; set; }
        public string? UF { get; set; }
        public string? CEP { get; set; }
    }

    public class Det
    {
        public string? nItem { get; set; }
        public Prod? prod { get; set; }
        public Imposto? imposto { get; set; }
    }

    public class Prod
    {
        public string? cProd { get; set; }
        public string? xProd { get; set; }
        public decimal? qCom { get; set; }
        public decimal? vProd { get; set; }
        public string? NCM { get; set; }
        public string? CFOP { get; set; }
    }

    public class Imposto
    {
        public decimal? vTotTrib { get; set; }
        public ICMS? ICMS { get; set; }
        public PIS? PIS { get; set; }
        public COFINS? COFINS { get; set; }
        public IBSCBS? IBSCBS { get; set; }
    }

    public class ICMS
    {
        public ICMS20? ICMS20 { get; set; }
    }

    public class ICMS20
    {
        public decimal? vICMS { get; set; }
    }

    public class PIS
    {
        public PISAliq? PISAliq { get; set; }
    }

    public class PISAliq
    {
        public decimal? vPIS { get; set; }
    }

    public class COFINS
    {
        public COFINSAliq? COFINSAliq { get; set; }
    }

    public class COFINSAliq
    {
        public decimal? vCOFINS { get; set; }
    }

    public class IBSCBS
    {
        public GIBSCBS? gIBSCBS { get; set; }
    }

    public class GIBSCBS
    {
        public decimal? vBC { get; set; }
        public GIBSUF? gIBSUF { get; set; }
        public GIBSMUN? GIBSMun { get; set; }

        public GCBS? gCBS { get; set; }
    }

    public class GCBS
    {
        public decimal? vCBS { get; set; }
        public decimal? pCBS { get; set; }

    }

    public class GIBSUF
    {
        public decimal? pIBSUF { get; set; }
        public decimal? vIBSUF { get; set; }

    }

    public class GIBSMUN
    {
        public decimal? pIBSMun { get; set; }
        public decimal? vIBSMun { get; set; }
    }

    public class Total
    {
        public ICMSTot? ICMSTot { get; set; }
        public IBSCBSTot? IBSCBSTot { get; set; }
    }

    public class ICMSTot
    {
        public decimal? vNF { get; set; }
    }

    public class IBSCBSTot
    {
        public GIBS? gIBS { get; set; }
        public GCBS? gCBS { get; set; }
    }

    public class GIBS
    {
        public decimal? vIBS { get; set; }
    }

    public class Pag
    {
        public List<DetPag> detpag { get; set; }
    }
    public class DetPag
    {
        public string? tPag { get; set; }
        public decimal? vPag { get; set; }
    }

    public class ProtNFe
    {
        public InfProt? infProt { get; set; }
    }

    public class InfProt
    {
        public string? chNFe { get; set; }
        public string? cStat { get; set; }
        public string? xMotivo { get; set; }
    }
}

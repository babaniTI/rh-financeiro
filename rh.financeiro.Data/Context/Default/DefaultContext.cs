using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rh.financeiro.Data.Context.Default
{
    using Microsoft.EntityFrameworkCore;
    using rh.financeiro.Domain.Entities;

    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

        #region DBSETS
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEmpresa> UsuarioEmpresas { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<ContaFinanceira> ContasFinanceiras { get; set; }
        public DbSet<DocumentoFiscal> DocumentosFiscais { get; set; }
        public DbSet<DocumentoFiscalItem> DocumentoFiscalItens { get; set; }
        public DbSet<TributoItem> TributosItens { get; set; }
        public DbSet<CategoriaFinanceira> CategoriasFinanceiras { get; set; }
        public DbSet<Titulo> Titulos { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }
        public DbSet<MovimentoBancario> MovimentosBancarios { get; set; }
        public DbSet<Conciliacao> Conciliacoes { get; set; }
        public DbSet<DeParaContabil> DeParaContabeis { get; set; }
        public DbSet<LancamentoContabil> LancamentosContabeis { get; set; }
        public DbSet<Rateio> Rateios { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<FilaExcecao> FilasExcecao { get; set; }
        public DbSet<EventoFinanceiro> EventosFinanceiros { get; set; }
        public DbSet<LiquidacaoFinanceira> LiquidacoesFinanceiras { get; set; }
        public DbSet<DeclaracaoImportacao> DeclaracoesImportacao { get; set; }

        #endregion

        #region ON MODEL CREATING

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // ENUM → STRING
            // =========================

            modelBuilder.Entity<Participante>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<ContaFinanceira>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<DocumentoFiscal>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<DocumentoFiscal>()
                .Property(x => x.status)
                .HasConversion<string>();

            modelBuilder.Entity<CategoriaFinanceira>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Titulo>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Titulo>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Parcela>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<MovimentoBancario>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Conciliacao>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<TributoItem>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<EventoFinanceiro>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<LiquidacaoFinanceira>()
                .Property(x => x.Tipo)
                .HasConversion<string>();

            // =========================
            // AJUSTES IMPORTANTES
            // =========================

            // JSONB (opcional - mantém como string)
            modelBuilder.Entity<DocumentoFiscal>()
                .Property(x => x.DadosTributarios)
                .HasColumnType("jsonb");

            modelBuilder.Entity<DeParaContabil>()
                .Property(x => x.NaturezaFiscal)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Auditoria>()
                .Property(x => x.DadosAntes)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Auditoria>()
                .Property(x => x.DadosDepois)
                .HasColumnType("jsonb");
        }

        #endregion
    }
}

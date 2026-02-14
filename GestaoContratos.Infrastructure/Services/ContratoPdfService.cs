using GestaoContratos.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;

namespace GestaoContratos.Infrastructure.Services
{
    public static class ContratoPdfService
    {
        public static void ExportarContrato(Contrato contrato, string caminho)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var cultura = new CultureInfo("pt-BR");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // ==========================
                    // HEADER
                    // ==========================
                    page.Header().Column(col =>
                    {
                        col.Item().Text("RELATÓRIO DE CONTRATO")
                            .FontSize(18)
                            .Bold();

                        col.Item().Text($"Contrato nº {contrato.NumeroContrato}")
                            .FontSize(12);

                        col.Item().Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    // ==========================
                    // CONTENT
                    // ==========================
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        void Secao(string titulo)
                        {
                            col.Item().PaddingTop(10).Text(titulo)
                                .FontSize(14)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);
                        }

                        void Linha(string label, string valor)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem(3)
                                   .Text(label)
                                   .Bold();

                                row.RelativeItem(7)
                                   .Text(valor ?? "");
                            });
                        }

                        // ================= IDENTIFICAÇÃO
                        Secao("IDENTIFICAÇÃO");
                        Linha("Número do Contrato:", contrato.NumeroContrato);
                        Linha("Objeto:", contrato.Objeto);
                        Linha("Modalidade do Contrato:", contrato.ModalidadeContrato);

                        // ================= FINANCEIRO
                        Secao("FINANCEIRO");
                        Linha("Modalidade Licitação:", contrato.ModalidadeLicitacao);
                        Linha("Critério Seleção:", contrato.CriterioSelecao);
                        Linha("Unidade Medida:", contrato.UnidadeMedida);
                        Linha("Valor Total:",
                            contrato.ValorTotal.ToString("C", cultura));
                        Linha("Valor Exercício:",
                            contrato.ValorExercicio.ToString("C", cultura));

                        // ================= CONTRATADA
                        Secao("CONTRATADA");
                        Linha("Razão Social:", contrato.RazaoSocialContratada);
                        Linha("CNPJ:", contrato.CnpjContratada);
                        Linha("Endereço:", contrato.EnderecoContratada);
                        Linha("E-mail:", contrato.EmailContratada);
                        Linha("Telefone:", contrato.TelefoneContratada);

                        // ================= PREPOSTO
                        Secao("PREPOSTO");
                        Linha("Nome:", contrato.NomePreposto);
                        Linha("E-mail:", contrato.EmailPreposto);
                        Linha("Telefone:", contrato.TelefonePreposto);

                        // ================= GESTÃO
                        Secao("GESTÃO E FISCALIZAÇÃO");
                        Linha("Gestor:", contrato.GestorContrato);
                        Linha("Fiscal:", contrato.FiscalContrato);
                        Linha("Fiscal Substituto:", contrato.FiscalSubstituto);
                        Linha("Contato Fiscal:", contrato.ContatoFiscal);

                        // ================= VIGÊNCIA
                        Secao("VIGÊNCIA");
                        Linha("Início:",
                            contrato.InicioVigencia.ToString("dd/MM/yyyy"));
                        Linha("Fim:",
                            contrato.FimVigencia.ToString("dd/MM/yyyy"));
                    });

                    // ==========================
                    // FOOTER
                    // ==========================
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            })
            .GeneratePdf(caminho);
        }
    }
}

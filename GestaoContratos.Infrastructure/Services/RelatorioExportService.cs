using System;
using System.Data;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;

namespace GestaoContratos.Infrastructure.Services
{
    public static class RelatorioExportService
    {
        private static readonly CultureInfo CulturaBrasil = new CultureInfo("pt-BR");

        // ==========================
        // EXPORTAR PARA EXCEL
        // ==========================
        public static void ExportarExcel(DataTable tabela, string caminho, string titulo = "Relatório")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatório");

            int totalColunas = tabela.Columns.Count > 0 ? tabela.Columns.Count : 1;

            // 🔹 Título
            worksheet.Cell(1, 1).Value = titulo;
            worksheet.Range(1, 1, 1, totalColunas).Merge();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;

            // 🔹 Data
            worksheet.Cell(2, 1).Value = $"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range(2, 1, 2, totalColunas).Merge();

            // 🔹 Inserir tabela na linha 4
            worksheet.Cell(4, 1).InsertTable(tabela);

            // 🔹 Formatar colunas numéricas como moeda
            for (int i = 0; i < tabela.Columns.Count; i++)
            {
                var coluna = tabela.Columns[i];

                if (coluna.DataType == typeof(decimal) ||
                    coluna.DataType == typeof(double) ||
                    coluna.DataType == typeof(float))
                {
                    worksheet.Column(i + 1).Style.NumberFormat.Format = "R$ #,##0.00";
                }
            }

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(caminho);
        }

        // ==========================
        // EXPORTAR PARA PDF (QuestPDF)
        // ==========================
        public static void ExportarPdf(DataTable tabela, string caminho, string titulo = "Relatório")
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // 🔹 Cabeçalho
                    page.Header().Column(col =>
                    {
                        col.Item()
                            .Text(titulo)
                            .FontSize(18)
                            .Bold();

                        col.Item()
                            .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    // 🔹 Conteúdo
                    page.Content().PaddingVertical(15).Table(table =>
                    {
                        int colCount = tabela.Columns.Count > 0 ? tabela.Columns.Count : 1;

                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < colCount; i++)
                                columns.RelativeColumn();
                        });

                        // 🔹 Cabeçalho
                        table.Header(header =>
                        {
                            foreach (DataColumn column in tabela.Columns)
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(6)
                                    .Text(column.ColumnName)
                                    .Bold();
                            }
                        });

                        // 🔹 Linhas
                        foreach (DataRow row in tabela.Rows)
                        {
                            for (int i = 0; i < tabela.Columns.Count; i++)
                            {
                                var valor = row[i];
                                var coluna = tabela.Columns[i];

                                var textoFormatado = FormatarValor(valor, coluna.DataType);

                                var cell = table.Cell().Padding(5);

                                // Alinha números à direita
                                if (EhNumero(coluna.DataType))
                                    cell.AlignRight().Text(textoFormatado);
                                else
                                    cell.Text(textoFormatado);
                            }
                        }
                    });

                    // 🔹 Rodapé
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

        // ==========================
        // FORMATADORES
        // ==========================

        private static string FormatarValor(object valor, Type tipo)
        {
            if (valor == null)
                return "";

            // 🔹 Primeiro tenta converter para decimal
            if (decimal.TryParse(valor.ToString(), out var numero))
            {
                return numero.ToString("C", CulturaBrasil);
            }

            // 🔹 Depois tenta converter para inteiro
            if (long.TryParse(valor.ToString(), out var inteiro))
            {
                return inteiro.ToString("N0", CulturaBrasil);
            }

            return valor.ToString();
        }

        private static bool EhNumero(Type tipo)
        {
            return tipo == typeof(decimal) ||
                   tipo == typeof(double) ||
                   tipo == typeof(float) ||
                   tipo == typeof(int) ||
                   tipo == typeof(long);
        }
    }
}

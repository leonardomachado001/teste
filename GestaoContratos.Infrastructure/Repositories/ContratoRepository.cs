using GestaoContratos.Domain.DTOs;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.Interfaces;
using GestaoContratos.Infrastructure.Database;
using GestaoContratos.Domain.Filtros;
using Npgsql;
using System;
using System.Collections.Generic;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class ContratoRepository : IContratoRepository
    {
        // =====================================================
        // SALVAR
        // =====================================================

        public void Salvar(Contrato contrato)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var existe = false;

            using (var checkCmd = new NpgsqlCommand(
                "SELECT COUNT(*) FROM contratos WHERE id = @id", conn))
            {
                checkCmd.Parameters.AddWithValue("@id", contrato.Id);
                existe = (long)checkCmd.ExecuteScalar() > 0;
            }

            if (!existe)
            {
                if (contrato.Id == Guid.Empty)
                    contrato.Id = Guid.NewGuid();

                var insertSql = @"
            INSERT INTO contratos
            (id, numero_contrato, objeto, modalidade_licitacao,
             criterio_selecao, unidade_medida, razao_social_contratada,
             cnpj_contratada, endereco_contratada, email_contratada,
             telefone_contratada, nome_preposto, email_preposto,
             telefone_preposto, valor_total, gestor_contrato,
             fiscal_contrato, fiscal_substituto, inicio_vigencia,
             fim_vigencia, modalidade_contrato, valor_exercicio,
             contato_fiscal, data_criacao, usuario_criacao)
            VALUES
            (@id, @numero, @objeto, @modalidadeLicitacao,
             @criterio, @unidade, @razao,
             @cnpj, @endereco, @emailContratada,
             @telefoneContratada, @nomePreposto, @emailPreposto,
             @telefonePreposto, @valorTotal, @gestor,
             @fiscal, @fiscalSubstituto, @inicio,
             @fim, @modalidadeContrato, @valorExercicio,
             @contatoFiscal, @dataCriacao, @usuarioCriacao);
        ";

                using var insertCmd = new NpgsqlCommand(insertSql, conn);
                PreencherParametros(insertCmd, contrato);
                insertCmd.ExecuteNonQuery();
            }
            else
            {
                var updateSql = @"
            UPDATE contratos SET
                numero_contrato = @numero,
                objeto = @objeto,
                modalidade_licitacao = @modalidadeLicitacao,
                criterio_selecao = @criterio,
                unidade_medida = @unidade,
                razao_social_contratada = @razao,
                cnpj_contratada = @cnpj,
                endereco_contratada = @endereco,
                email_contratada = @emailContratada,
                telefone_contratada = @telefoneContratada,
                nome_preposto = @nomePreposto,
                email_preposto = @emailPreposto,
                telefone_preposto = @telefonePreposto,
                valor_total = @valorTotal,
                gestor_contrato = @gestor,
                fiscal_contrato = @fiscal,
                fiscal_substituto = @fiscalSubstituto,
                inicio_vigencia = @inicio,
                fim_vigencia = @fim,
                modalidade_contrato = @modalidadeContrato,
                valor_exercicio = @valorExercicio,
                contato_fiscal = @contatoFiscal
            WHERE id = @id;
        ";

                using var updateCmd = new NpgsqlCommand(updateSql, conn);
                PreencherParametros(updateCmd, contrato);
                updateCmd.ExecuteNonQuery();
            }
        }
        private void PreencherParametros(NpgsqlCommand cmd, Contrato contrato)
        {
            cmd.Parameters.AddWithValue("@id", contrato.Id);
            cmd.Parameters.AddWithValue("@numero", contrato.NumeroContrato);
            cmd.Parameters.AddWithValue("@objeto", contrato.Objeto);
            cmd.Parameters.AddWithValue("@modalidadeLicitacao", contrato.ModalidadeLicitacao);
            cmd.Parameters.AddWithValue("@criterio", contrato.CriterioSelecao);
            cmd.Parameters.AddWithValue("@unidade", contrato.UnidadeMedida);
            cmd.Parameters.AddWithValue("@razao", contrato.RazaoSocialContratada);
            cmd.Parameters.AddWithValue("@cnpj", contrato.CnpjContratada);
            cmd.Parameters.AddWithValue("@endereco", contrato.EnderecoContratada);
            cmd.Parameters.AddWithValue("@emailContratada", contrato.EmailContratada);
            cmd.Parameters.AddWithValue("@telefoneContratada", contrato.TelefoneContratada);
            cmd.Parameters.AddWithValue("@nomePreposto", contrato.NomePreposto);
            cmd.Parameters.AddWithValue("@emailPreposto", contrato.EmailPreposto);
            cmd.Parameters.AddWithValue("@telefonePreposto", contrato.TelefonePreposto);
            cmd.Parameters.AddWithValue("@valorTotal", contrato.ValorTotal);
            cmd.Parameters.AddWithValue("@gestor", contrato.GestorContrato);
            cmd.Parameters.AddWithValue("@fiscal", contrato.FiscalContrato);
            cmd.Parameters.AddWithValue("@fiscalSubstituto", contrato.FiscalSubstituto);
            cmd.Parameters.AddWithValue("@inicio", contrato.InicioVigencia);
            cmd.Parameters.AddWithValue("@fim", contrato.FimVigencia);
            cmd.Parameters.AddWithValue("@modalidadeContrato", contrato.ModalidadeContrato);
            cmd.Parameters.AddWithValue("@valorExercicio", contrato.ValorExercicio);
            cmd.Parameters.AddWithValue("@contatoFiscal", contrato.ContatoFiscal);
            cmd.Parameters.AddWithValue("@dataCriacao", contrato.DataCriacao);
            cmd.Parameters.AddWithValue("@usuarioCriacao", contrato.UsuarioCriacao);
        }


        public bool ExisteNumeroContrato(string numeroContrato)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT COUNT(*) FROM contratos WHERE numero_contrato = @numero";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@numero", numeroContrato);

            return (long)cmd.ExecuteScalar() > 0;
        }

        public List<Contrato> Filtrar(FiltroContrato filtro)
        {
            var lista = new List<Contrato>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = @"
        SELECT *
        FROM contratos
        WHERE 1=1
    ";

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // ================= FILTRO NÚMERO =================
            if (!string.IsNullOrWhiteSpace(filtro.NumeroContrato))
            {
                sql += " AND numero_contrato ILIKE @numero ";
                cmd.Parameters.AddWithValue("@numero", $"%{filtro.NumeroContrato}%");
            }

            // ================= FILTRO CONTRATADA =================
            if (!string.IsNullOrWhiteSpace(filtro.Contratada))
            {
                sql += " AND razao_social_contratada ILIKE @contratada ";
                cmd.Parameters.AddWithValue("@contratada", $"%{filtro.Contratada}%");
            }

            // ================= FILTRO OBJETO =================
            if (!string.IsNullOrWhiteSpace(filtro.Objeto))
            {
                sql += " AND objeto ILIKE @objeto ";
                cmd.Parameters.AddWithValue("@objeto", $"%{filtro.Objeto}%");
            }

            // ================= FILTRO GESTOR =================
            if (!string.IsNullOrWhiteSpace(filtro.GestorContrato))
            {
                sql += " AND gestor_contrato ILIKE @gestor ";
                cmd.Parameters.AddWithValue("@gestor", $"%{filtro.GestorContrato}%");
            }

            // ================= FILTRO STATUS =================
            if (filtro.Ativo.HasValue)
            {
                if (filtro.Ativo.Value)
                    sql += " AND fim_vigencia >= CURRENT_DATE ";
                else
                    sql += " AND fim_vigencia < CURRENT_DATE ";
            }

            sql += " ORDER BY data_criacao DESC ";

            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearContrato(reader));
            }

            return lista;
        }


        public Contrato? ObterPorId(Guid id)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM contratos WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapearContrato(reader);
        }

        public void Excluir(Guid id)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "DELETE FROM contratos WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        public ContratoResumoDTO ObterResumo()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            return new ContratoResumoDTO
            {
                Total = Count(conn, "SELECT COUNT(*) FROM contratos"),
                Ativos = Count(conn, "SELECT COUNT(*) FROM contratos WHERE fim_vigencia >= CURRENT_DATE"),
                Vencidos = Count(conn, "SELECT COUNT(*) FROM contratos WHERE fim_vigencia < CURRENT_DATE"),
                AVencer30Dias = Count(conn,
                    @"SELECT COUNT(*) FROM contratos
                      WHERE fim_vigencia BETWEEN CURRENT_DATE
                      AND CURRENT_DATE + INTERVAL '30 days'")
            };
        }

        public List<Contrato> ObterContratosVencidos()
        {
            return BuscarPorWhere("fim_vigencia < CURRENT_DATE");
        }

        public List<Contrato> ObterContratosAVencer30Dias()
        {
            return BuscarPorWhere(
                @"fim_vigencia BETWEEN CURRENT_DATE
                  AND CURRENT_DATE + INTERVAL '30 days'");
        }

        public FinanceiroResumoDTO ObterResumoFinanceiro()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            return new FinanceiroResumoDTO
            {
                TotalContratado = Sum(conn, "SELECT COALESCE(SUM(valor_total),0) FROM contratos"),
                TotalAtivo = Sum(conn, "SELECT COALESCE(SUM(valor_total),0) FROM contratos WHERE fim_vigencia >= CURRENT_DATE"),
                TotalVencido = Sum(conn, "SELECT COALESCE(SUM(valor_total),0) FROM contratos WHERE fim_vigencia < CURRENT_DATE"),
                TotalAVencer30Dias = Sum(conn,
                    @"SELECT COALESCE(SUM(valor_total),0)
                      FROM contratos
                      WHERE fim_vigencia BETWEEN CURRENT_DATE
                      AND CURRENT_DATE + INTERVAL '30 days'")
            };
        }

        public List<FinanceiroAgrupadoDTO> ObterFinanceiroPorGestor()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var lista = new List<FinanceiroAgrupadoDTO>();

            var sql = @"
        SELECT 
            gestor_contrato AS nome,
            SUM(valor_total) AS total,
            SUM(CASE WHEN fim_vigencia >= CURRENT_DATE THEN valor_total ELSE 0 END) AS ativo,
            SUM(CASE WHEN fim_vigencia < CURRENT_DATE THEN valor_total ELSE 0 END) AS vencido
        FROM contratos
        GROUP BY gestor_contrato
        ORDER BY gestor_contrato;
    ";

            using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new FinanceiroAgrupadoDTO
                {
                    Nome = reader["nome"]?.ToString() ?? "",
                    Total = reader["total"] != DBNull.Value ? Convert.ToDecimal(reader["total"]) : 0,
                    Ativo = reader["ativo"] != DBNull.Value ? Convert.ToDecimal(reader["ativo"]) : 0,
                    Vencido = reader["vencido"] != DBNull.Value ? Convert.ToDecimal(reader["vencido"]) : 0
                });
            }

            return lista;
        }


        public List<FinanceiroAgrupadoDTO> ObterFinanceiroPorContratada()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var lista = new List<FinanceiroAgrupadoDTO>();

            var sql = @"
        SELECT 
            razao_social_contratada AS nome,
            SUM(valor_total) AS total,
            SUM(CASE WHEN fim_vigencia >= CURRENT_DATE THEN valor_total ELSE 0 END) AS ativo,
            SUM(CASE WHEN fim_vigencia < CURRENT_DATE THEN valor_total ELSE 0 END) AS vencido
        FROM contratos
        GROUP BY razao_social_contratada
        ORDER BY razao_social_contratada;
    ";

            using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new FinanceiroAgrupadoDTO
                {
                    Nome = reader["nome"]?.ToString() ?? "",
                    Total = reader["total"] != DBNull.Value ? Convert.ToDecimal(reader["total"]) : 0,
                    Ativo = reader["ativo"] != DBNull.Value ? Convert.ToDecimal(reader["ativo"]) : 0,
                    Vencido = reader["vencido"] != DBNull.Value ? Convert.ToDecimal(reader["vencido"]) : 0
                });
            }

            return lista;
        }


        public List<RelatorioGestorDTO> ObterResumoPorGestor()
        {
            var lista = new List<RelatorioGestorDTO>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = @"
        SELECT 
            gestor_contrato,
            COUNT(*) AS total_contratos,
            COUNT(*) FILTER (WHERE fim_vigencia >= CURRENT_DATE) AS ativos,
            COUNT(*) FILTER (WHERE fim_vigencia < CURRENT_DATE) AS vencidos
        FROM contratos
        GROUP BY gestor_contrato
        ORDER BY gestor_contrato;
    ";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new RelatorioGestorDTO
                {
                    Gestor = reader["gestor_contrato"]?.ToString() ?? "",
                    TotalContratos = Convert.ToInt32(reader["total_contratos"]),
                    Ativos = Convert.ToInt32(reader["ativos"]),
                    Vencidos = Convert.ToInt32(reader["vencidos"])
                });
            }

            return lista;
        }


        // =====================================================
        // AUXILIARES
        // =====================================================

        private List<Contrato> BuscarPorWhere(string where)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var lista = new List<Contrato>();
            var sql = $"SELECT * FROM contratos WHERE {where}";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(MapearContrato(reader));

            return lista;
        }

        private int Count(NpgsqlConnection conn, string sql)
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private decimal Sum(NpgsqlConnection conn, string sql)
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        private Contrato MapearContrato(NpgsqlDataReader reader)
        {
            return new Contrato
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                NumeroContrato = reader.GetString(reader.GetOrdinal("numero_contrato")),
                Objeto = reader.GetString(reader.GetOrdinal("objeto")),
                ModalidadeLicitacao = reader.GetString(reader.GetOrdinal("modalidade_licitacao")),
                CriterioSelecao = reader.GetString(reader.GetOrdinal("criterio_selecao")),
                UnidadeMedida = reader.GetString(reader.GetOrdinal("unidade_medida")),
                RazaoSocialContratada = reader.GetString(reader.GetOrdinal("razao_social_contratada")),
                CnpjContratada = reader.GetString(reader.GetOrdinal("cnpj_contratada")),
                EnderecoContratada = reader.GetString(reader.GetOrdinal("endereco_contratada")),
                EmailContratada = reader.GetString(reader.GetOrdinal("email_contratada")),
                TelefoneContratada = reader.GetString(reader.GetOrdinal("telefone_contratada")),
                NomePreposto = reader.GetString(reader.GetOrdinal("nome_preposto")),
                EmailPreposto = reader.GetString(reader.GetOrdinal("email_preposto")),
                TelefonePreposto = reader.GetString(reader.GetOrdinal("telefone_preposto")),
                ValorTotal = reader.GetDecimal(reader.GetOrdinal("valor_total")),
                GestorContrato = reader.GetString(reader.GetOrdinal("gestor_contrato")),
                FiscalContrato = reader.GetString(reader.GetOrdinal("fiscal_contrato")),
                FiscalSubstituto = reader.GetString(reader.GetOrdinal("fiscal_substituto")),
                InicioVigencia = reader.GetDateTime(reader.GetOrdinal("inicio_vigencia")),
                FimVigencia = reader.GetDateTime(reader.GetOrdinal("fim_vigencia")),
                ModalidadeContrato = reader.GetString(reader.GetOrdinal("modalidade_contrato")),
                ValorExercicio = reader.GetDecimal(reader.GetOrdinal("valor_exercicio")),
                ContatoFiscal = reader.GetString(reader.GetOrdinal("contato_fiscal")),
                DataCriacao = reader.GetDateTime(reader.GetOrdinal("data_criacao")),
                UsuarioCriacao = reader.GetString(reader.GetOrdinal("usuario_criacao"))
            };
        }
    }
}

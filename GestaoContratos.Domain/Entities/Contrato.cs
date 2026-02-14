using System;
using System.Collections.Generic;

namespace GestaoContratos.Domain.Entities
{
    public class Contrato
    {
        // ===============================
        // IDENTIFICAÇÃO
        // ===============================
        public Guid Id { get; set; }

        public string ModalidadeContrato { get; set; } = string.Empty;

        public string NumeroContrato { get; set; } = string.Empty;
        public string Objeto { get; set; } = string.Empty;

        public string ModalidadeLicitacao { get; set; } = string.Empty;
        public string CriterioSelecao { get; set; } = string.Empty;
        public string UnidadeMedida { get; set; } = string.Empty;

        // ===============================
        // CONTRATADA
        // ===============================
        public string RazaoSocialContratada { get; set; } = string.Empty;
        public string CnpjContratada { get; set; } = string.Empty;
        public string EnderecoContratada { get; set; } = string.Empty;
        public string EmailContratada { get; set; } = string.Empty;
        public string TelefoneContratada { get; set; } = string.Empty;

        // ===============================
        // PREPOSTO
        // ===============================
        public string NomePreposto { get; set; } = string.Empty;
        public string EmailPreposto { get; set; } = string.Empty;
        public string TelefonePreposto { get; set; } = string.Empty;

        // ===============================
        // VALORES
        // ===============================
        public decimal ValorTotal { get; set; }
        public decimal ValorExercicio { get; set; }

        public List<ContratoValorExercicio> ValoresPorExercicio { get; set; }
            = new List<ContratoValorExercicio>();

        // ===============================
        // GESTÃO
        // ===============================
        public string GestorContrato { get; set; } = string.Empty;
        public string FiscalContrato { get; set; } = string.Empty;
        public string FiscalSubstituto { get; set; } = string.Empty;
        public string ContatoFiscal { get; set; } = string.Empty;

        // ===============================
        // VIGÊNCIA
        // ===============================
        public DateTime InicioVigencia { get; set; }
        public DateTime FimVigencia { get; set; }

        // ===============================
        // AUDITORIA
        // ===============================
        public DateTime DataCriacao { get; set; }
        public string UsuarioCriacao { get; set; } = string.Empty;
  

        // ===============================
        // CAMPOS CALCULADOS (NÃO VÃO AO BANCO)
        // ===============================
        public int DiasParaTermino =>
            (FimVigencia - DateTime.Today).Days;

        public bool EstaVencido =>
            DateTime.Today > FimVigencia;
    }
}

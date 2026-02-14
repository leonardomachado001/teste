using System;
using System.Collections.Generic;
using GestaoContratos.Domain.Entities;

namespace GestaoContratos.Domain.Interfaces
{
    public interface IContratoHistoricoRepository
    {
        void RegistrarAlteracao(ContratoHistorico historico);
        List<ContratoHistorico> ObterPorContrato(Guid contratoId);
    }
}

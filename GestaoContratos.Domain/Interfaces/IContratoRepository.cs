using System;
using System.Collections.Generic;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.Filtros;
using GestaoContratos.Domain.DTOs;
using System.Threading.Tasks;


namespace GestaoContratos.Domain.Interfaces
{
    public interface IContratoRepository
    {
        // ===============================
        // CRIAÇÃO / ATUALIZAÇÃO
        // ===============================
        void Salvar(Contrato contrato);
        bool ExisteNumeroContrato(string numeroContrato);

        // ===============================
        // LISTAGEM
        // ===============================
        List<Contrato> Filtrar(FiltroContrato filtro);

        // ===============================
        // AÇÕES DOS ÍCONES
        // ===============================
        Contrato? ObterPorId(Guid id);
        void Excluir(Guid id);

        // ===============================
        // DASHBOARD
        // ===============================
        ContratoResumoDTO ObterResumo();



    }
}

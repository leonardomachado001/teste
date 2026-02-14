using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoContratos.Domain.Enums
{
    public enum Permissao
    {
        Dashboard,

        Usuario_Listar,
        Usuario_EditarPerfil,

        Contrato_Listar,
        Contrato_Detalhar,
        Contrato_Criar,
        Contrato_Editar,
        Contrato_Excluir,
        Relatorio_Visualizar
    }
}

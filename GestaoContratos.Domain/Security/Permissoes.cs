using GestaoContratos.Domain.Enums;

namespace GestaoContratos.Domain.Security
{
    public static class Permissoes
    {
        // ===============================
        // USUÁRIOS
        // ===============================
        public static bool PodeCriarGestor(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master;

        public static bool PodeGerenciarUsuarios(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master || perfil == PerfilUsuario.Gestor;

        // ===============================
        // CONTRATOS
        // ===============================
        public static bool PodeEditarContrato(PerfilUsuario perfil)
            => perfil != PerfilUsuario.Leitor;

        public static bool PodeExcluirContrato(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master || perfil == PerfilUsuario.Gestor;

        // ===============================
        // VISUALIZAÇÃO
        // ===============================
        public static bool PodeAcessarDashboard(PerfilUsuario perfil)
            => perfil != PerfilUsuario.Leitor;

        public static bool PodeAcessarRelatorios(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master || perfil == PerfilUsuario.Gestor;
    }
}

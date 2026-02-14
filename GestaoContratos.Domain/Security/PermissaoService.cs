using GestaoContratos.Domain.Enums;

namespace GestaoContratos.Domain.Security
{
    public static class PermissaoService
    {
        public static bool PodeCriarGestor(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master;

        public static bool PodeGerenciarUsuarios(PerfilUsuario perfil)
            => perfil == PerfilUsuario.Master || perfil == PerfilUsuario.Gestor;

        public static bool PodeEditarContrato(PerfilUsuario perfil)
            => perfil != PerfilUsuario.Leitor;
    }
}

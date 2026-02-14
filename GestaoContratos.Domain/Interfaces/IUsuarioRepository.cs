using System;
using System.Threading.Tasks;
using GestaoContratos.Domain.Entities;

namespace GestaoContratos.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        bool LoginExiste(string login);
        string GerarLogin(string nomeCompleto);

        void Salvar(Usuario usuario);
        void Atualizar(Usuario usuario);
        int ContarMastersAtivos();

        Usuario? ObterPorId(Guid id);

        Task<Usuario?> Autenticar(string login, string senha);
        Task<Usuario?> ObterPorLogin(string login);

    }
}

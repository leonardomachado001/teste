using System;
using System.Collections.Generic;
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
        Usuario? Autenticar(string login, string senha);
        Usuario? ObterPorLogin(string login);

        List<Usuario> ObterTodos();
    }
}

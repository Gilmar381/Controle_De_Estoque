namespace Controle_De_estoqueAPI.Exceptions
{
    /// <summary>Violação de regra de negócio. Vira HTTP 400.</summary>
    public class RegraDeNegocioException : Exception
    {
        public RegraDeNegocioException(string mensagem) : base(mensagem) { }
    }

    /// <summary>Conflito com o estado atual (duplicidade, vínculo existente). Vira HTTP 409.</summary>
    public class ConflitoException : Exception
    {
        public ConflitoException(string mensagem) : base(mensagem) { }
    }
}
    
using Controle_De_estoqueAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Middleware
{
    /// <summary>
    /// Converte exceções em respostas HTTP padronizadas (ProblemDetails),
    /// evitando que erros de negócio virem HTTP 500.
    /// </summary>
    public class TratamentoDeErrosMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TratamentoDeErrosMiddleware> _logger;

        public TratamentoDeErrosMiddleware(RequestDelegate next, ILogger<TratamentoDeErrosMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                // DbUpdateConcurrencyException herda de DbUpdateException: precisa vir primeiro.
                var (status, titulo, detalhe) = ex switch
                {
                    KeyNotFoundException => (404, "Recurso não encontrado", ex.Message),
                    RegraDeNegocioException or ArgumentException => (400, "Requisição inválida", ex.Message),
                    ConflitoException => (409, "Conflito", ex.Message),
                    UnauthorizedAccessException => (401, "Não autorizado", ex.Message),
                    DbUpdateConcurrencyException => (409, "Conflito de concorrência",
                        "O registro foi alterado por outra operação. Tente novamente."),
                    DbUpdateException => (409, "Conflito ao salvar",
                        "Não foi possível salvar: a operação viola uma restrição do banco (duplicidade ou vínculo existente)."),
                    _ => (500, "Erro interno", "Ocorreu um erro inesperado.")
                };

                if (status == 500)
                    _logger.LogError(ex, "Erro não tratado em {Metodo} {Caminho}", context.Request.Method, context.Request.Path);
                else
                    _logger.LogWarning("{Titulo}: {Detalhe}", titulo, detalhe);

                context.Response.Clear();
                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = status,
                    Title = titulo,
                    Detail = detalhe
                });
            }
        }
    }
}

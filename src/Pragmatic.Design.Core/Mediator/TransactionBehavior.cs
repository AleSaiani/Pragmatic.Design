using System.Reflection;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Pragmatic.Design.Core.Persistence;

namespace Pragmatic.Design.Core.Mediator;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly DbContext _dbContext;

    public TransactionBehavior(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        // Controlla se il comando è marcato con l'attributo Transaction
        if (request.GetType().GetCustomAttribute(typeof(TransactionAttribute)) == null)
            return await next(request, cancellationToken);
        var currentTransaction = _dbContext.Database.CurrentTransaction;

        if (currentTransaction != null)
            return await next(request, cancellationToken);

        // Inizia una transazione se non ce ne è una già in corso
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Esegui il comando
            var response = await next(request, cancellationToken);

            // Se il comando è stato eseguito con successo, esegui il commit della transazione
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch (Exception)
        {
            // Se si verifica un'eccezione, annulla la transazione
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        // Se il comando non è marcato con l'attributo Transaction, eseguilo normalmente
        return await next(request, cancellationToken);
    }
}

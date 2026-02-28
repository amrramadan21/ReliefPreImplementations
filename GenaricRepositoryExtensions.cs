// Plan (pseudocode):
// 1. Provide an extension method named `FindAsync` for the existing `IGenaricRepository<TEntity, TKey>`
//    so calls like `repo.FindAsync(predicate)` compile.
// 2. Keep the extension simple and compatible with the repository interface that exposes
//    `Task<IEnumerable<TEntity>> GetAllAsync()` only.
// 3. Implement `FindAsync` to:
//    - Validate incoming `repo` and `predicate` arguments.
//    - Call `repo.GetAllAsync()` to retrieve all entities asynchronously.
//    - Filter the result using the provided `predicate` (in-memory) and return the filtered sequence.
// 4. Place the extension in the same namespace as `IGenaricRepository` so no extra using is required.
// 5. Use a generic constraint `where TEntity : class` matching the repository interface.
//
// Note: This implements a memory-based filter using `Func<TEntity, bool>`.
// If you later want to support expression translation (e.g., EF Core), consider adding an
// overload that accepts `Expression<Func<TEntity, bool>>` and forwarding to repository methods
// that can translate expressions to queries.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Relief.Domain.Contracts
{
    public static class GenaricRepositoryExtensions
    {
        /// <summary>
        /// Finds entities that match the provided predicate by retrieving all entities
        /// from the repository and filtering them in-memory.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TKey">Repository key type.</typeparam>
        /// <param name="repo">The repository instance.</param>
        /// <param name="predicate">Predicate to filter entities.</param>
        /// <returns>A task containing the filtered entities.</returns>
        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity, TKey>(
            this IGenaricRepository<TEntity, TKey> repo,
            Func<TEntity, bool> predicate) where TEntity : class
        {
            if (repo == null) throw new ArgumentNullException(nameof(repo));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var all = await repo.GetAllAsync();
            return all?.Where(predicate) ?? Enumerable.Empty<TEntity>();
        }
    }
}
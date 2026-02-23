using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedApiResponse<TDestination>> ReadPage<TEntity, TDestination>(
            this IQueryable<TEntity> query,
            BaseFilter filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? applyFilters = null,
            Func<IEnumerable<TEntity>, IEnumerable<TDestination>>? mapFunc = null)
            where TEntity : class
        {
            if (applyFilters != null)
            {
                query = applyFilters(query);
            }

            var total = await query.CountAsync();

            int skip = ((filter.Page ?? 1) - 1) * (filter.PageSize ?? 10);
            int take = filter.PageSize ?? 10;

            var data = await query.Skip(skip).Take(take).ToListAsync();

            var dtos = mapFunc != null ? mapFunc(data) : data.Cast<TDestination>();

            var pageData = new PageData
            {
                Page = filter.Page ?? 1,
                PageSize = filter.PageSize ?? 10,
                Total = total
            };

            return new PagedApiResponse<TDestination>(pageData, dtos);
        }
    }

    //public static class IQueryableExtensions
    //{
    //    public static async Task<PagedApiResponse<TDestination>> ReadPage<TEntity, TDestination>(
    //        this IQueryProviderExecutor<TEntity> executor,
    //        BaseFilter filter,
    //        Func<IQueryable<TEntity>, IQueryable<TEntity>>? applyFilters = null,
    //        Func<IEnumerable<TEntity>, IEnumerable<TDestination>>? mapFunc = null,
    //        IQueryable<TEntity>? query = null) // opcional, caso precise aplicar filtros
    //        where TEntity : class
    //    {
    //        if (query != null && applyFilters != null)
    //        {
    //            query = applyFilters(query);
    //        }

    //        // total
    //        var total = await (query != null ? query.CountAsync() : executor.CountAsync());

    //        int skip = ((filter.Page ?? 1) - 1) * (filter.PageSize ?? 10);
    //        int take = filter.PageSize ?? 10;

    //        // dados
    //        var data = await (query != null ? query.Skip(skip).Take(take).ToListAsync() : executor.ToListAsync(skip, take));

    //        // mapeamento
    //        var dtos = mapFunc != null ? mapFunc(data) : data.Cast<TDestination>();

    //        var pageData = new PageData
    //        {
    //            Page = filter.Page ?? 1,
    //            PageSize = filter.PageSize ?? 10,
    //            Total = total
    //        };

    //        return new PagedApiResponse<TDestination>(pageData, dtos);
    //    }
    //}
}
 
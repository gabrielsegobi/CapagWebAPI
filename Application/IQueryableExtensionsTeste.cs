using Application.Filters;
using Domain.Contracts.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class IQueryableExtensionsTeste
    {
        public static async Task<PagedApiResponse<TDestination>> ReadPage<TEntity, TDestination>(
            this IQueryable<TEntity> query,
            BaseFilter filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? applyFilters = null,           // filtros SQL
            Func<IEnumerable<TEntity>, IEnumerable<TEntity>>? postFilter = null,           // filtros em memória
            Func<IEnumerable<TEntity>, IEnumerable<TDestination>>? mapFunc = null)         // mapeamento DTO
            where TEntity : class
        {
            // 🔹 1. Filtros SQL (traduzíveis)
            if (applyFilters != null)
                query = applyFilters(query);

            // 🔹 2. Executa a query (carrega da base)
            var data = await query.ToListAsync();

            // 🔹 3. Aplica filtro em memória (ex: Grupo chumbado)
            if (postFilter != null)
                data = postFilter(data).ToList();

            // 🔹 4. Calcula total pós-filtro
            var total = data.Count;

            // 🔹 5. Aplica paginação manual
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;

            var paged = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 6. Mapeia (DTO)
            var dtos = mapFunc != null ? mapFunc(paged) : paged.Cast<TDestination>();

            // 🔹 7. Monta retorno
            var pageData = new PageData
            {
                Page = page,
                PageSize = pageSize,
                Total = total
            };

            return new PagedApiResponse<TDestination>(pageData, dtos);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Maruko.Core.Application.Servers;
using Maruko.Core.Application.Servers.Dto;
using Maruko.Core.Domain.Entities;
using Maruko.Core.FreeSql.Internal.Repos;
using Maruko.Core.ObjectMapping;

namespace Maruko.Core.FreeSql.Internal.AppService
{
    public class CurdAppService<TEntity, TEntityDto, TSearch> : CurdAppServiceBase<TEntity, TEntityDto, TEntityDto, TEntityDto>, ICurdAppService<TEntity, TEntityDto, TSearch>
        where TEntity : Entity
        where TEntityDto : EntityDto
        where TSearch : PageDto
    {
        private readonly IFreeSqlRepository<TEntity> Table;

        public CurdAppService(IObjectMapper objectMapper, IFreeSqlRepository<TEntity> repository) : base(objectMapper, repository)
        {
            Table = repository;
        }

        public virtual PagedResultDto PageSearch(TSearch search)
        {
            var query = Table.GetAll().Select<TEntity>();

            if (SearchFilter(search) != null)
                query = query.Where(SearchFilter(search));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var result = query
                .Count(out var total)
                .Page(search.PageIndex, search.PageMax)
                .ToList();

            return new PagedResultDto(total, ConvertToEntityDTOs(result));
        }

        public virtual TEntityDto CreateOrEdit(TEntityDto request)
        {
            TEntity data = null;
            if (request.Id == 0)
            {
                request.CreateTime = DateTime.Now;
                data = Table.Insert(MapToEntity(request));
            }
            else
            {
                data = Table.FirstOrDefault(item => item.Id == request.Id);
                data = MapToEntity(request);
                data.CreateTime = DateTime.Now;
                data = Table.Update(data);
            }

            return data == null
                ? null
                : ObjectMapper.Map<TEntityDto>(data);
        }

        protected virtual Expression<Func<TEntity, bool>> SearchFilter(TSearch search)
        {
            return null;
        }

        protected virtual Expression<Func<TEntity, int>> OrderFilter()
        {
            return null;
        }

        protected virtual List<TEntityDto> ConvertToEntityDTOs(List<TEntity> entities)
        {
            return ObjectMapper.Map<List<TEntityDto>>(entities);
        }
    }
}
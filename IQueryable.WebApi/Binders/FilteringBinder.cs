﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IQueryableFilter.Infrastructure.Filtering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IQueryableFilter.WebApi.Binders
{
    public class FilteringBinder : IModelBinder
    {
        private readonly IFilterExpressionFactory _filterExpressionFactory;
        private readonly INamedFilterExpressionFactory _namedFilterExpressionFactory;

        public FilteringBinder(IFilterExpressionFactory filterExpressionFactory,
            INamedFilterExpressionFactory namedFilterExpressionFactory)
        {
            _filterExpressionFactory = filterExpressionFactory ??
                                       throw new ArgumentNullException(nameof(filterExpressionFactory));
            _namedFilterExpressionFactory = namedFilterExpressionFactory ??
                                            throw new ArgumentNullException(nameof(namedFilterExpressionFactory));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            HttpRequest request = bindingContext.ActionContext.HttpContext.Request;

            Dictionary<string, string[]> queryCollection =
                request.Query.ToDictionary(q => q.Key, q => q.Value.ToArray());

            bindingContext.Result =
                ModelBindingResult.Success(new Infrastructure.Filtering.Filtering(queryCollection,
                    _filterExpressionFactory, _namedFilterExpressionFactory));
            return Task.CompletedTask;
        }
    }
}
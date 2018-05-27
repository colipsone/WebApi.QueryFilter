using System;
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
        private readonly IFilterExpressionBuilder _expressionBuilder;

        public FilteringBinder(IFilterExpressionBuilder expressionBuilder)
        {
            _expressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            HttpRequest request = bindingContext.ActionContext.HttpContext.Request;

            Dictionary<string, string[]> queryCollection =
                request.Query.ToDictionary(q => q.Key, q => q.Value.ToArray());

            bindingContext.Result =
                ModelBindingResult.Success(new Infrastructure.Filtering.Filtering(queryCollection, _expressionBuilder));
            return Task.CompletedTask;
        }
    }
}
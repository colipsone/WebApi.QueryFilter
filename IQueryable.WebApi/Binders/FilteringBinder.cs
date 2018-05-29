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
        private readonly IFiltering _filtering;

        public FilteringBinder(IFiltering filtering)
        {
            _filtering = filtering ?? throw new ArgumentNullException(nameof(filtering));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            HttpRequest request = bindingContext.ActionContext.HttpContext.Request;

            Dictionary<string, string[]> queryCollection =
                request.Query.ToDictionary(q => q.Key, q => q.Value.ToArray());

            _filtering.Initialize(queryCollection);

            bindingContext.Result = ModelBindingResult.Success(_filtering);
            return Task.CompletedTask;
        }
    }
}
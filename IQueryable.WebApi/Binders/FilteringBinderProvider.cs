using System;
using IQueryableFilter.Infrastructure.Filtering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace IQueryableFilter.WebApi.Binders
{
    public class FilteringBinderProvider: IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.ModelType == typeof(IFiltering) ? new BinderTypeModelBinder(typeof(FilteringBinder)) : null;
        }
    }
}
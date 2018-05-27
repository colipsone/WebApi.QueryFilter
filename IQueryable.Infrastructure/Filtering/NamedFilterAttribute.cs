using System;
using System.Composition;

namespace IQueryableFilter.Infrastructure.Filtering
{
    [MetadataAttribute]
    public sealed class NamedFilterAttribute : Attribute
    {
        public NamedFilterAttribute(string filterName)
        {
            FilterName = filterName;
        }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName { get; }
    }
}
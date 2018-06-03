using System.Collections.Generic;
using System.Linq;
using IQueryableFilter.Data.Extensions;
using IQueryableFilter.Infrastructure.Filtering;
using IQueryableFilter.WebApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace IQueryableFilter.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IEnumerable<TestEntity> GetTestEntities(IFiltering filtering)
        {
            IQueryable<TestEntity> items = new[]
            {
                new TestEntity
                {
                    UniqueName = "test",
                    Sub = new SubClass
                    {
                        FirstName = "sub test"
                    },
                    Number = 2
                },
                new TestEntity
                {
                    UniqueName = "not test",
                    Sub = new SubClass
                    {
                        FirstName = "not sub test"
                    },
                    Number = 3
                }
            }.AsQueryable();

            return items
                .FilterByNamed(filtering)
                .FilterBy(filtering);
        }
    }
}
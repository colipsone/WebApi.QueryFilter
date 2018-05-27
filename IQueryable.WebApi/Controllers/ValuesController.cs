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
                    Name = "test"
                },
                new TestEntity
                {
                    Name = "not test"
                }
            }.AsQueryable();

            return items
                .FilterByNamed(filtering)
                .FilterBy(filtering);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
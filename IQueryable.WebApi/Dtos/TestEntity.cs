using Newtonsoft.Json;

namespace IQueryableFilter.WebApi.Dtos
{
    public class TestEntity
    {
        public string UniqueName { get; set; }

        public SubClass Sub { get; set; }

        [JsonProperty("test_number")]
        public int Number { get; set; }
    }

    public class SubClass
    {
        public string FirstName { get; set; }
    }
}
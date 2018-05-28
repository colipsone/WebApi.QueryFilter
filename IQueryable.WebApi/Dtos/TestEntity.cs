namespace IQueryableFilter.WebApi.Dtos
{
    public class TestEntity
    {
        public string Name { get; set; }

        public SubClass Sub { get; set; }
    }

    public class SubClass
    {
        public string Name { get; set; }
    }
}
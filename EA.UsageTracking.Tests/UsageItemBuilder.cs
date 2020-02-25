using EA.UsageTracking.Core.Entities;

namespace EA.UsageTracking.Tests
{
    public class UsageItemBuilder

    {

        private UsageItem _todo = new UsageItem();



        public UsageItemBuilder Id(int id)

        {

            _todo.Id = id;

            return this;

        }



        public UsageItemBuilder Title(string title)

        {
            return this;
        }



        public UsageItemBuilder Description(string description)

        {
            return this;
        }



        public UsageItemBuilder WithDefaultValues()

        {
            _todo = new UsageItem() { Id = 1 };


            return this;

        }



        public UsageItem Build() => _todo;

    }
}

using System.Collections;

namespace Climb.Test.Extensions
{
    public static class CollectionExtensions
    {
        public static T Init<T>(this T collection, int count) where T : IList
        {
            for(int i = 0; i < count; i++)
            {
                collection.Add(default(T));
            }

            return collection;
        }
    }
}
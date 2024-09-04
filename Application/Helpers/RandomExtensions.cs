using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Helpers
{
    public static class RandomExtensions
    {
        private static readonly Random _random = new();

        public static T GetRandomItem<T>(IEnumerable<T> items)
        {
            int randomIndex = _random.Next(items.Count());
            return items.ElementAt(randomIndex);
        }

        public static T GetRandom<T>(T first, T second)
                => _random.Next(0, 2) == 1 ? first : second;
    }
}

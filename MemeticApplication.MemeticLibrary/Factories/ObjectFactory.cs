using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Factories
{
    public class ObjectFactory<K, V>
    {
        protected ObjectFactory() { }

        static readonly Dictionary<K, Func<V>> _dict
             = new Dictionary<K, Func<V>>();

        public static V Create(K id)
        {
            Func<V> constructor = null;
            if (_dict.TryGetValue(id, out constructor))
                return constructor();

            throw new ArgumentException("No type registered for this id");
        }

        public static bool Register(K id, Func<V> ctor)
        {
            if (!_dict.ContainsKey(id))
            {
                _dict.Add(id, ctor);
                return true;
            }
            return false;
        }
    }
}

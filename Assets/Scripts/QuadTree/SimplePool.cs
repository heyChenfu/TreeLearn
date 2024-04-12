
using System;
using System.Collections.Generic;

namespace QuadTree 
{ 
    public class SimplePool<T> where T : class
    {

        IList<T> _pool = new List<T>();
        private Func<T> _createDelegate;
        public Action<T> _returnDelegate;

        public SimplePool(Func<T> createDelegate, Action<T> returnDelegate)
        {
            _createDelegate = createDelegate;
            _returnDelegate = returnDelegate;

        }

        public T Borrow()
        {
            if (_pool.Count == 0)
            {
                return _createDelegate?.Invoke();
            }
            else
            {
                T obj = _pool[_pool.Count - 1];
                _pool.RemoveAt(_pool.Count - 1);
                return obj;
            }

        }

        public void Return(T obj)
        {
            _pool.Add(obj);
            _returnDelegate?.Invoke(obj);

        }

    }

}

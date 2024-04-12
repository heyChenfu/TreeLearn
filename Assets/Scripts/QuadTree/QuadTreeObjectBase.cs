
using UnityEngine;

namespace QuadTree
{

    public abstract class QuadTreeObjectBase : IQuadTreeObject
    {
        private Bounds _lastBounds;

        public IQuadTreeNode StorageNode { get; set; }

        public abstract Bounds GetBounds();

        /// <summary>
        /// 物体Bounds发生变化(位置,缩放)
        /// </summary>
        public void BoundsChange()
        {
            if (StorageNode == null)
                return;
            Bounds currBounds = GetBounds();
            if (_lastBounds == null || !_lastBounds.Equals(currBounds))
            {
                bool bReinsert = false;
                if (!_lastBounds.Intersects(currBounds) || _lastBounds.size != currBounds.size)
                {
                    bReinsert = true;
                }

            }

        }

    }

}
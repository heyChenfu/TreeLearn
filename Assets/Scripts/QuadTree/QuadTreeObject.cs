
using System;
using UnityEngine;

namespace QuadTree
{

    public class QuadTreeObject : IQuadTreeObject
    {
        private Func<Bounds> GetBoundsCall;
        private Bounds _lastBounds;

        public IQuadTreeNode StorageNode { get; set; }

        public QuadTreeObject(Func<Bounds> getBoundsCall)
        {
            GetBoundsCall = getBoundsCall;

        }

        public Bounds GetBounds()
        {
            if(GetBoundsCall == null)
            {
                Debug.LogError("GetBounds Failed!");
                return default;
            }
            return GetBoundsCall.Invoke();
        }

        public void DrawGizmos(Bounds bounds)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

        }

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
                StorageNode?.UpdateByDataBoundsChange(this, bReinsert);
                _lastBounds = currBounds;
            }

        }

    }

}
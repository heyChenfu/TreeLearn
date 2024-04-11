
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeRoot : IQuadTreeRoot
    {
        private int _maxChidNodeCount;
        private float _minTreeNodeSize;
        private IQuadTreeNode _rootNode;

        public int MaxChidNodeCount { get { return _maxChidNodeCount; } }
        public float MinTreeNodeSize { get { return _minTreeNodeSize; } }
        public IQuadTreeNode RootNode { get { return _rootNode; } }

        public QuadTreeRoot(Vector3 center, Vector3 size, float minTreeNodeSize) 
        {
            _maxChidNodeCount = 4;
            _minTreeNodeSize = minTreeNodeSize;
            _rootNode = new QuadTreeNode() {
                Root = this,
                NodeBounds = new Bounds(center, size),
                Parent = null,
            };

        }

        /// <summary>
        /// 给定Bounds查找目标
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        public IList<IQuadTreeObject> Query(Bounds bound)
        {
            return null;

        }

        /// <summary>
        /// 插入物体
        /// </summary>
        /// <param name="item"></param>
        public void Insert(IQuadTreeObject item)
        {
            Bounds itemBounds = item.GetBounds();
            if (!_rootNode.Contains(itemBounds))
            {
                Debug.Log("Outside the tree bounds, insert failed!");
                return;
            }
            _rootNode.Insert(item);
        }

        public void Reset()
        {
            _rootNode?.Reset();
        }

    }


}
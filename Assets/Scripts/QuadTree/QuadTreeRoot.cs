
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeRoot : IQuadTreeRoot
    {
        private int _maxChidNodeCount;
        private float _minTreeNodeSize;
        private IQuadTreeNode _rootNode;
        private bool _bSpreadingDirectionRight = true;

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

        public void Reset()
        {
            _rootNode?.Reset();
        }

        /// <summary>
        /// 插入物体
        /// </summary>
        /// <param name="item"></param>
        public void Insert(IQuadTreeObject item)
        {
            Bounds itemBounds = item.GetBounds();
            int whileBreak = 10;
            while (whileBreak > 0 && !_rootNode.Contains(itemBounds))
            {
                whileBreak--;
                //尝试扩展四叉树将物体包含
                Expand();
            }
            if (whileBreak <= 0)
                Debug.LogError("QuadTree Expand to insert object failed!");
            else
                _rootNode.Insert(item);

        }

        /// <summary>
        /// 给定Bounds查找目标
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        public IList<IQuadTreeObject> Find(Bounds bound)
        {
            IList<IQuadTreeObject> findResult = new List<IQuadTreeObject>();
            _rootNode.FindDataWithBounds(bound, ref findResult);
            return findResult;
        }

        /// <summary>
        /// 扩展四叉树
        /// </summary>
        public void Expand()
        {
            Vector3 oldBoundsSize = _rootNode.NodeBounds.size;
            Vector3 oldBoundsExtents = _rootNode.NodeBounds.extents;
            Vector3 newCenter = _bSpreadingDirectionRight ? _rootNode.NodeBounds.max : _rootNode.NodeBounds.min;
            IQuadTreeNode newRootNode = new QuadTreeNode()
            {
                Root = this,
                NodeBounds = new Bounds(newCenter, _rootNode.NodeBounds.size * 2),
                Parent = null,
            };
            _rootNode.Parent = newRootNode;

            IQuadTreeNode[] newNodeArr = new IQuadTreeNode[_maxChidNodeCount];
            oldBoundsExtents.x *= -1;
            newNodeArr[(int)QuadTreeChildDefine.LEFT_UP] = new QuadTreeNode() {
                Root = this,
                NodeBounds = new Bounds(newCenter + oldBoundsExtents, oldBoundsSize),
                Parent = newRootNode,
            };
            oldBoundsExtents.x *= -1;
            newNodeArr[(int)QuadTreeChildDefine.RIGHT_UP] = !_bSpreadingDirectionRight ? _rootNode :
            new QuadTreeNode()
            {
                Root = this,
                NodeBounds = new Bounds(newCenter + oldBoundsExtents, oldBoundsSize),
                Parent = newRootNode,
            };
            oldBoundsExtents.z *= -1;
            newNodeArr[(int)QuadTreeChildDefine.RIGHT_DOWN] = new QuadTreeNode()
            {
                Root = this,
                NodeBounds = new Bounds(newCenter + oldBoundsExtents, oldBoundsSize),
                Parent = newRootNode,
            };
            oldBoundsExtents.x *= -1;
            newNodeArr[(int)QuadTreeChildDefine.LEFT_DOWN] = _bSpreadingDirectionRight ? _rootNode : 
            new QuadTreeNode()
            {
                Root = this,
                NodeBounds = new Bounds(newCenter + oldBoundsExtents, oldBoundsSize),
                Parent = newRootNode,
            };
            newRootNode.SetChid(newNodeArr);
            _rootNode = newRootNode;
            //反向扩展
            _bSpreadingDirectionRight = !_bSpreadingDirectionRight;

        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            _rootNode?.Draw();

        }

    }

}
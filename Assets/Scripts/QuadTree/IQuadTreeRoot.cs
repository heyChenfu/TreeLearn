
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    /// <summary>
    /// 四叉树基类接口
    /// </summary>
    public interface IQuadTreeRoot
    {
        /// <summary>
        /// 每个节点最大子节点数目
        /// </summary>
        int MaxChidNodeCount { get; }
        /// <summary>
        /// 最小划分大小
        /// </summary>
        float MinTreeNodeSize { get; }
        /// <summary>
        /// 四叉树根节点
        /// </summary>
        IQuadTreeNode RootNode { get; }
        SimplePool<IQuadTreeNode> Pool { get; }

        void Reset();
        void Insert(IQuadTreeObject item);
        IList<IQuadTreeObject> Find(Bounds bound);
        void Expand();
        void DrawGizmos();

    }

}

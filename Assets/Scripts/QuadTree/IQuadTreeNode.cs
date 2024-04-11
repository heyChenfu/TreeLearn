
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public enum QuadTreeChildDefine
    {
        LEFT_UP = 0,
        RIGHT_UP = 1,
        LEFT_DOWN = 2,
        RIGHT_DOWN = 3,
        NONE = 4,
    }

    public interface IQuadTreeNode
    {
        IQuadTreeRoot Root { get; set; }
        /// <summary>
        /// AABB包围盒
        /// </summary>
        Bounds NodeBounds { get; set; }
        /// <summary>
        /// 树父节点
        /// </summary>
        IQuadTreeNode Parent { get; set; }
        /// <summary>
        /// 树子节点
        /// </summary>
        IList<IQuadTreeNode> Child { get; }
        HashSet<IQuadTreeObject> DataSet { get; }

        void Reset();
        bool Contains(Bounds bounds);
        void Insert(IQuadTreeObject obj);
        bool Remove(IQuadTreeObject obj);
        bool IsDataEmpty();
        void Draw();

    }


}

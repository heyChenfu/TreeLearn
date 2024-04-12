
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
        //同时多个子节点情况
        ACROSS_MULTIPLE_LEFT,
        ACROSS_MULTIPLE_RIGHT,
        ACROSS_MULTIPLE_UP,
        ACROSS_MULTIPLE_DOWN,
        ACROSS_MULTIPLE_ALL,

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
        void SetChid(IQuadTreeNode[] childArr);
        bool Remove(IQuadTreeObject obj);
        void UpdateByDataBoundsChange(IQuadTreeObject treeObj, bool bReinsert);
        void FindDataWithBounds(Bounds targetBounds, ref IList<IQuadTreeObject> findResult);
        bool IsDataEmpty();
        void Draw();

    }


}

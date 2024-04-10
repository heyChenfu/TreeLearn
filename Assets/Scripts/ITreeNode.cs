
using System.Collections.Generic;
using UnityEngine;

namespace Tree
{
    public interface ITreeNode
    {
        /// <summary>
        /// AABB包围盒
        /// </summary>
        Bounds TreeBounds { get; set; }
        /// <summary>
        /// 树父节点
        /// </summary>
        ITreeNode Parent { get; set; }
        /// <summary>
        /// 树子节点
        /// </summary>
        IList<ITreeNode> Child { get; set; }
        HashSet<ITreeObject> DataSet { get; set; }

        void Reset();
        bool Contains(Bounds bounds);
        void Insert(ITreeObject obj);
        void Remove(ITreeObject obj);
        bool IsEmpty();
        void DrawTreeBounds();

    }


}

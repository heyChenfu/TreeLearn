
using UnityEngine;

namespace QuadTree
{
    public interface IQuadTreeObject
    {

        IQuadTreeNode StorageNode { get; set; }
        /// <summary>
        /// 获取物体的Bounds
        /// </summary>
        /// <returns></returns>
        Bounds GetBounds();

    }


}

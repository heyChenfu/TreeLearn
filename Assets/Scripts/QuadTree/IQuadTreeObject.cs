
using UnityEngine;

namespace QuadTree
{
    public interface IQuadTreeObject
    {

        IQuadTreeNode StorageNode { get; set; }
        /// <summary>
        /// 物体的Bounds
        /// </summary>
        /// <returns></returns>
        Bounds GetBounds();

    }


}

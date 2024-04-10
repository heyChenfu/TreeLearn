
using UnityEngine;

namespace Tree
{
    public interface ITreeObject
    {

        ITreeNode StorageNode { get; set; }
        Bounds GetBounds();

    }


}

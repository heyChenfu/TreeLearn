
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tree 
{
    public interface ITreeRoot
    {

        ITreeNode RootNode { get; }

        void Reset();
        void Insert(ITreeObject item);
        IList<ITreeObject> Find(Bounds bound);

    }

}


using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeNode : IQuadTreeNode
    {
        private IList<IQuadTreeNode> _children;
        private HashSet<IQuadTreeObject> _dataSet;

        public IQuadTreeRoot Root { get; set; }
        public Bounds NodeBounds { get; set; }
        public IQuadTreeNode Parent { get; set; }
        public IList<IQuadTreeNode> Child { get { return _children; } }
        public HashSet<IQuadTreeObject> DataSet { get { return _dataSet; } }

        public QuadTreeNode()
        {
            _children = new List<IQuadTreeNode>(4);
            _dataSet = new HashSet<IQuadTreeObject>();

        }

        /// <summary>
        /// ���õ�ǰ�ڵ�, ����ӽڵ������
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < _dataSet.Count; ++i)
            {
                if (_dataSet.ElementAt(i) != null)
                    _dataSet.ElementAt(i).StorageNode = null;
            }
            _dataSet.Clear();

            for (int i = 0; _children != null && i < _children.Count; ++i)
            {
                _children[i].Reset();
            }
            _children.Clear();

        }

        public bool Contains(Bounds bounds)
        {
            return NodeBounds != null && NodeBounds.Intersects(bounds);
        }

        public void Insert(IQuadTreeObject obj)
        {
            if (_children.Count == 0)
                CreateChildrenNode();

            if(_children.Count == 0)
            {
                _dataSet.Add(obj);
                obj.StorageNode = this;
            }
            else
            {
                QuadTreeChildDefine childDefine = ChildCoverJudge(obj.GetBounds());
                if (childDefine != QuadTreeChildDefine.NONE)
                {
                    _children[(int)childDefine].Insert(obj);
                }
                else
                {
                    //���Ƕ���ӽڵ�ʱ�����ڵ�ǰ�ڵ�
                    _dataSet.Add(obj);
                    obj.StorageNode = this;
                }
            }

        }

        public bool Remove(IQuadTreeObject obj)
        {
            bool bRemove = false;
            if (_dataSet.Contains(obj))
            {
                bRemove = true;
                _dataSet.Remove(obj);
                obj.StorageNode = null;

                if (IsDataEmpty())
                {
                    Reset();
                }
            }
            else
            {
                for (int i = 0; _children != null && i < _children.Count; ++i)
                {
                    bRemove |= _children[i].Remove(obj);
                    if (bRemove)
                        break;
                }
            }
            return bRemove;
        }

        /// <summary>
        /// �Լ����ӽڵ��Ƿ����ݶ�Ϊ��
        /// </summary>
        /// <returns></returns>
        public bool IsDataEmpty()
        {
            if (_dataSet.Count > 0)
                return false;
            for (int i = 0; _children != null && i < _children.Count; ++i)
            {
                if (!_children[i].IsDataEmpty())
                    return false;
            }
            return true;
        }

        public void Draw()
        {
            Gizmos.DrawWireCube(NodeBounds.center, NodeBounds.size);

            for (int i = 0; _children != null && i < _children.Count; ++i)
            {
                _children[i]?.Draw();
            }
        }

        /// <summary>
        /// ���������ӽڵ�
        /// </summary>
        private void CreateChildrenNode()
        {
            Vector3 currBoundsHalf = NodeBounds.size * 0.5f;
            //�ӽڵ��С�Ƿ��Ѿ��ﵽ��������
            if (currBoundsHalf.x < Root.MinTreeNodeSize || currBoundsHalf.z < Root.MinTreeNodeSize)
            {
                return;
            }

            Vector3 childCenterOffset = currBoundsHalf * 0.5f;
            Vector3[] childCenterArr = new Vector3[4];
            childCenterArr[(int)QuadTreeChildDefine.LEFT_UP] = new Vector3(
                NodeBounds.center.x - childCenterOffset.x, NodeBounds.center.y, NodeBounds.center.z + childCenterOffset.z);
            childCenterArr[(int)QuadTreeChildDefine.LEFT_DOWN] = new Vector3(
                NodeBounds.center.x - childCenterOffset.x, NodeBounds.center.y, NodeBounds.center.z - childCenterOffset.z);
            childCenterArr[(int)QuadTreeChildDefine.RIGHT_UP] = new Vector3(
                NodeBounds.center.x + childCenterOffset.x, NodeBounds.center.y, NodeBounds.center.z + childCenterOffset.z);
            childCenterArr[(int)QuadTreeChildDefine.RIGHT_DOWN] = new Vector3(
                NodeBounds.center.x + childCenterOffset.x, NodeBounds.center.y, NodeBounds.center.z - childCenterOffset.z);

            for (int i = 0; i < childCenterArr.Length; ++i)
            {
                _children.Add(new QuadTreeNode()
                {
                    Root = this.Root,
                    NodeBounds = new Bounds(childCenterArr[i], currBoundsHalf),
                    Parent = this,
                });
            }

        }

        /// <summary>
        /// �鿴Ŀ��Bounds���ĸ��ӽڵ��Bounds��
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <returns></returns>
        private QuadTreeChildDefine ChildCoverJudge(Bounds targetBounds)
        {
            if (targetBounds.min.z >= NodeBounds.center.z)
            {
                //��Сz���ڵ�ǰBounds����z, ���Ѿ����ϰ벿��
                if (targetBounds.max.x < NodeBounds.center.x)
                {
                    //���xС������x, ��ֻ��������
                    return QuadTreeChildDefine.LEFT_UP;
                }
                else if (targetBounds.min.x >= NodeBounds.center.x)
                {
                    //��Сx��������x, ��ֻ��������
                    return QuadTreeChildDefine.RIGHT_UP;
                }
                else
                {
                    //�����������Ϊ���Ƕ��
                    return QuadTreeChildDefine.NONE;
                }
            }
            else if (targetBounds.max.z < NodeBounds.center.z)
            {
                //���zС�ڵ�ǰBounds����z, ���Ȼ���°벿��
                if (targetBounds.max.x < NodeBounds.center.x)
                {
                    return QuadTreeChildDefine.LEFT_DOWN;
                }
                else if (targetBounds.min.x >= NodeBounds.center.x)
                {
                    return QuadTreeChildDefine.RIGHT_DOWN;
                }
                else
                {
                    //�����������Ϊ���Ƕ��
                    return QuadTreeChildDefine.NONE;
                }
            }
            else
            {
                //�����������Ϊ���Ƕ��
                return QuadTreeChildDefine.NONE;
            }

        }

    }

}

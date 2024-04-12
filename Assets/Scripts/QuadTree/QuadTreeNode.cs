
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
        /// 重置当前节点, 清除子节点和数据
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < _dataSet.Count; ++i)
            {
                if (_dataSet.ElementAt(i) != null)
                    _dataSet.ElementAt(i).StorageNode = null;
            }
            _dataSet.Clear();

            for (int i = 0; i < _children.Count; ++i)
            {
                _children[i].Reset();
            }
            _children.Clear();

        }

        public bool Contains(Bounds bounds)
        {
            return NodeBounds != null && NodeBounds.Intersects(bounds);
        }

        /// <summary>
        /// 插入一个物体
        /// </summary>
        /// <param name="obj"></param>
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
                if (childDefine <= QuadTreeChildDefine.RIGHT_DOWN)
                {
                    _children[(int)childDefine].Insert(obj);
                }
                else
                {
                    //覆盖多个子节点时保存在当前节点
                    _dataSet.Add(obj);
                    obj.StorageNode = this;
                }
            }

        }

        /// <summary>
        /// 重置子节点
        /// </summary>
        /// <param name="childArr"></param>
        public void SetChid(IQuadTreeNode[] childArr)
        {
            for (int i = 0; i < childArr.Length; ++i)
            {
                if(i >= _children.Count)
                    _children.Add(childArr[i]);
                else
                    _children[i] = childArr[i];
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
                for (int i = 0; i < _children.Count; ++i)
                {
                    bRemove |= _children[i].Remove(obj);
                    if (bRemove)
                        break;
                }
            }
            return bRemove;
        }

        /// <summary>
        /// 根据Bounds查找物体
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <param name="findResult"></param>
        public void FindDataWithBounds(Bounds targetBounds, ref IList<IQuadTreeObject> findResult)
        {
            if (NodeBounds.Intersects(targetBounds))
                findResult.AddRange(DataSet);

            if (_children.Count == 0)
                return;
            QuadTreeChildDefine coverCondition = ChildCoverJudge(targetBounds);
            switch (coverCondition) 
            {
                case QuadTreeChildDefine.LEFT_UP:
                case QuadTreeChildDefine.RIGHT_UP:
                case QuadTreeChildDefine.LEFT_DOWN:
                case QuadTreeChildDefine.RIGHT_DOWN:
                    _children[(int)coverCondition].FindDataWithBounds(targetBounds, ref findResult);
                    break;
                case QuadTreeChildDefine.ACROSS_MULTIPLE_LEFT:
                    _children[(int)QuadTreeChildDefine.LEFT_UP].FindDataWithBounds(targetBounds, ref findResult);
                    _children[(int)QuadTreeChildDefine.LEFT_DOWN].FindDataWithBounds(targetBounds, ref findResult);
                    break;
                case QuadTreeChildDefine.ACROSS_MULTIPLE_RIGHT:
                    _children[(int)QuadTreeChildDefine.RIGHT_UP].FindDataWithBounds(targetBounds, ref findResult);
                    _children[(int)QuadTreeChildDefine.RIGHT_DOWN].FindDataWithBounds(targetBounds, ref findResult);
                    break;
                case QuadTreeChildDefine.ACROSS_MULTIPLE_UP:
                    _children[(int)QuadTreeChildDefine.LEFT_UP].FindDataWithBounds(targetBounds, ref findResult);
                    _children[(int)QuadTreeChildDefine.RIGHT_UP].FindDataWithBounds(targetBounds, ref findResult);
                    break;
                case QuadTreeChildDefine.ACROSS_MULTIPLE_DOWN:
                    _children[(int)QuadTreeChildDefine.LEFT_DOWN].FindDataWithBounds(targetBounds, ref findResult);
                    _children[(int)QuadTreeChildDefine.RIGHT_DOWN].FindDataWithBounds(targetBounds, ref findResult);
                    break;
                case QuadTreeChildDefine.ACROSS_MULTIPLE_ALL:
                default:
                    for (int i = 0; i < _children.Count; ++i)
                    {
                        _children[i].FindDataWithBounds(targetBounds, ref findResult);
                    }
                    break;
            }

        }

        /// <summary>
        /// 自己和子节点是否数据都为空
        /// </summary>
        /// <returns></returns>
        public bool IsDataEmpty()
        {
            if (_dataSet.Count > 0)
                return false;
            for (int i = 0; i < _children.Count; ++i)
            {
                if (!_children[i].IsDataEmpty())
                    return false;
            }
            return true;
        }

        public void Draw()
        {
            Gizmos.DrawWireCube(NodeBounds.center, NodeBounds.size);

            for (int i = 0; i < _children.Count; ++i)
            {
                _children[i]?.Draw();
            }
        }

        /// <summary>
        /// 创建所有子节点
        /// </summary>
        private void CreateChildrenNode()
        {
            Vector3 currBoundsHalf = NodeBounds.size * 0.5f;
            //子节点大小是否已经达到划分限制
            if (currBoundsHalf.x < Root.MinTreeNodeSize || currBoundsHalf.z < Root.MinTreeNodeSize)
            {
                return;
            }

            Vector3 childCenterOffset = currBoundsHalf * 0.5f;
            Vector3[] childCenterArr = new Vector3[Root.MaxChidNodeCount];
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
        /// 查看目标Bounds在哪个子节点的Bounds中
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <returns></returns>
        private QuadTreeChildDefine ChildCoverJudge(Bounds targetBounds)
        {
            if (targetBounds.min.z >= NodeBounds.center.z)
            {
                //最小z大于当前Bounds中心z, 则已经在上半部分
                if (targetBounds.max.x < NodeBounds.center.x)
                {
                    //最大x小于中心x, 则只能在左上
                    return QuadTreeChildDefine.LEFT_UP;
                }
                else if (targetBounds.min.x >= NodeBounds.center.x)
                {
                    //最小x大于中心x, 则只能在右上
                    return QuadTreeChildDefine.RIGHT_UP;
                }
                else
                {
                    //其他情况则认为覆盖多个
                    return QuadTreeChildDefine.ACROSS_MULTIPLE_UP;
                }
            }
            else if (targetBounds.max.z < NodeBounds.center.z)
            {
                //最大z小于当前Bounds中心z, 则必然在下半部分
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
                    //其他情况则认为覆盖多个
                    return QuadTreeChildDefine.ACROSS_MULTIPLE_DOWN;
                }
            }
            else
            {
                //其他情况则认为覆盖多个
                if (targetBounds.min.x >= NodeBounds.center.x)
                {
                    return QuadTreeChildDefine.ACROSS_MULTIPLE_RIGHT;
                }
                else if (targetBounds.max.x < NodeBounds.center.x)
                {
                    return QuadTreeChildDefine.ACROSS_MULTIPLE_LEFT;
                }
                else
                {
                    return QuadTreeChildDefine.ACROSS_MULTIPLE_ALL;
                }
            }

        }

    }

}

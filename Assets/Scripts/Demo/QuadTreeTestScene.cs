
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeTestScene : MonoBehaviour 
    {
        [SerializeField]
        Bounds _quadTreeInitBounds;
        [SerializeField]
        int _objectCount;
        [SerializeField]
        GameObject _quadTreeSourceObject;

        QuadTreeRoot _treeRoot;
        List<QuadTreeObjectMono> _objectList = new List<QuadTreeObjectMono>();

        void Awake()
        {
            _treeRoot = new QuadTreeRoot(_quadTreeInitBounds.center, _quadTreeInitBounds.size, 1);

            UnityEngine.Random.InitState(DateTime.UtcNow.Millisecond);
            if (_quadTreeSourceObject != null)
            {
                for (int i = 0; i < _objectCount; ++i)
                {
                    GameObject newObj = GameObject.Instantiate<GameObject>(_quadTreeSourceObject);
                    float randomX = UnityEngine.Random.Range(0, _quadTreeInitBounds.extents.x);
                    float randomZ = UnityEngine.Random.Range(0, _quadTreeInitBounds.extents.z);
                    newObj.transform.position = new Vector3(randomX, 0, randomZ);
                    QuadTreeObjectMono monoObj = newObj.GetComponent<QuadTreeObjectMono>();
                    monoObj.RandomMoveBounds = _quadTreeInitBounds;
                    monoObj.SetRandomTargetPosition();
                    _treeRoot.Insert(monoObj.QuadTreeObj);
                    _objectList.Add(monoObj);
                }
            }

        }

        private void OnDrawGizmos()
        {
            _treeRoot?.DrawGizmos();
        }

    }


}
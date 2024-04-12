
using System;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeObjectMono : MonoBehaviour
    {
        [SerializeField]
        Bounds _myBounds;
        private Vector3 targetPosition; // 目标位置
        private float timer; // 计时器

        public QuadTreeObject QuadTreeObj { get; private set; }
        public float MoveSpeed = 3f; // 移动速度
        public float ChangeDirectionInterval = 5f; // 更改方向的间隔时间
        public Bounds RandomMoveBounds = default;

        void Awake()
        {
            QuadTreeObj = new QuadTreeObject(GetBounds);
            timer = ChangeDirectionInterval;

        }

        void Update()
        {
            // 更新计时器
            timer -= Time.deltaTime;
            // 如果计时器小于等于0，重新设置目标位置并重置计时器
            if (timer <= 0f)
            {
                SetRandomTargetPosition();
                timer = ChangeDirectionInterval;
                return;
            }

            // 计算朝向目标位置的方向
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            // 移动物体
            transform.Translate(moveDirection * MoveSpeed * Time.deltaTime);
            //更新树
            QuadTreeObj.BoundsChange();
        }

        private void OnDrawGizmos()
        {
            QuadTreeObj?.DrawGizmos(GetBounds());
        }

        private Bounds GetBounds()
        {
            Bounds newBounds = _myBounds;
            newBounds.center += transform.position;
            return newBounds;
        }

        /// <summary>
        /// 设置随机目标位置
        /// </summary>
        public void SetRandomTargetPosition()
        {
            // 生成随机目标位置
            float randomX = UnityEngine.Random.Range(-RandomMoveBounds.extents.x, RandomMoveBounds.extents.x);
            float randomZ = UnityEngine.Random.Range(-RandomMoveBounds.extents.z, RandomMoveBounds.extents.z);
            targetPosition = new Vector3(randomX, transform.position.y, randomZ);
        }

    }

}

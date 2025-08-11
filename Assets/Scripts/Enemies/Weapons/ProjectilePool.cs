using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Enemies
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private int _initPoolSize = 10;
        [SerializeField] private ProjectileBase _projectileTemplate;

        private Stack<ProjectileBase> _poolStack;
        private List<ProjectileBase> _projectiles =  new List<ProjectileBase>();
        private List<Transform> _projectileParents;
        private int _parentIndex = 0;

        public void Setup(List<Transform> projectileParents)
        {
            _projectileParents = projectileParents;
            _setupPool();
        }

        public ProjectileBase GetProjectile()
        {
            if(_poolStack.Count == 0)
            {
                ProjectileBase instance = _createPoolInstance();
                _getNextParent();
                return instance;
            }

            ProjectileBase nextInstance = _poolStack.Pop();
            nextInstance.gameObject.SetActive(true);
            _resetProjectilePos(nextInstance);
            _getNextParent();
            nextInstance.transform.parent = transform.parent;
            return nextInstance;
        }

        public void ReturnToPool(ProjectileBase instance)
        {
            _poolStack.Push(instance);

            _resetProjectilePos(instance, false);
            instance.gameObject.SetActive(false);
        }

        public void ResetPool()
        {
            _projectiles.ForEach(p => p.ReturnToPool());
        }

        private void _setupPool()
        {
            _poolStack = new Stack<ProjectileBase>();
            for(int i = 0; i < _initPoolSize; i++)
            {
                ProjectileBase instance = _createPoolInstance();
                instance.gameObject.name = $"Projectile_{i}";
                instance.gameObject.SetActive(false);
                _poolStack.Push(instance);
            }
        }

        private ProjectileBase _createPoolInstance()
        {
            ProjectileBase instance = Instantiate(_projectileTemplate, _projectileParents[_parentIndex]);
            instance.Init(this);
            instance.transform.parent = transform;
            _projectiles.Add(instance);
            return instance;
        }

        private void _resetProjectilePos(ProjectileBase instance, bool setNewParent = true)
        {
            instance.transform.parent = setNewParent ? _projectileParents[_parentIndex] : _projectileParents[0];
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }

        private void _getNextParent()
        {
            _parentIndex++;
            if(_parentIndex >= _projectileParents.Count)
                _parentIndex = 0;
        }
    }
}
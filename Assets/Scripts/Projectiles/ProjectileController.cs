using System;
using System.Collections;
using System.Collections.Generic;
using TankU.PlayerInput;
using TankU.PlayerObject;
using UnityEngine;

namespace TankU.Projectile
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private PauseController _pauseController;
        [SerializeField] private PlayerController[] _playerControllers;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private int _damageGivenPerProjectile;

        public event Action<int, int> OnPlayerHit;
        private List<GameObject> _projectilePool = new List<GameObject>();


        private void OnEnable()
        {
            foreach(PlayerController _controller in _playerControllers)
            {
                _controller.OnPlayerShoot += OnPlayerShoot;
            }
        }
        private void OnDisable()
        {
            foreach (PlayerController _controller in _playerControllers)
            {
                _controller.OnPlayerShoot -= OnPlayerShoot;
            }
        }

        private void OnHitPlayer(int _index)
        {
            OnPlayerHit?.Invoke(_index, _damageGivenPerProjectile);
        }

        private void OnPlayerShoot(bool _isPoweredUp, Transform _player, int _index)
        {
            GameObject _projectileReady = GetObjectFromPool(_projectilePool);
            if(_projectileReady == null)
            {
                _projectileReady = Instantiate(_projectilePrefab, _player.position, _player.rotation);
                IHitable _hitableInterface = _projectileReady.GetComponent<IHitable>();
                _hitableInterface.OnHitPlayer += OnHitPlayer;
                _projectilePool.Add(_projectileReady);
            }
            else
            {
                _projectileReady.transform.position = _player.position;
                _projectileReady.transform.rotation = _player.rotation;
            }

            _projectileReady.GetComponent<RocketController>().SetController(_pauseController, _playerControllers[_index]);
            _projectileReady.GetComponent<RocketController>()._isPoweredUp = _isPoweredUp;
            _projectileReady.SetActive(true);
        }
        private GameObject GetObjectFromPool(List<GameObject> _pool)
        {
            return _pool.Find(go => !go.activeInHierarchy);
        }
    }

}
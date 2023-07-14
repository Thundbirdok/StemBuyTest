using UnityEngine;

namespace Coin
{
    using System.Collections;
    using Network;
    using Unity.Netcode;
    using Random = UnityEngine.Random;

    public class CoinsSpawner : MonoBehaviour
    {
        [SerializeField]
        private Coin coinPrefab;

        [SerializeField]
        private int coinsNumber;

        [SerializeField]
        private float spawnOverTimeDelay = 2.5f;
        
        [SerializeField]
        private Transform leftBottomCorner;
        
        [SerializeField]
        private Transform rightTopCorner;

        [SerializeField]
        private Vector2 offset;

        private Coroutine _spawnCoroutine;
        
        private void Start()
        {
            if (NetworkObjectPool.IsPrefabsRegistered)
            {
                Spawn();
                
                return;
            }

            NetworkObjectPool.OnPrefabsRegistered += Spawn;
        }

        private void OnDestroy()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            
            NetworkObjectPool.OnPrefabsRegistered -= Spawn;
        }

        private void Spawn()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            SpawnCoins();
        }

        private void SpawnCoins()
        {
            _spawnCoroutine ??= StartCoroutine(SpawnOveTime());

            for (var i = 0; i < coinsNumber; ++i)
            {
                SpawnCoin();
            }
        }

        private void SpawnCoin()
        {
            var networkObject = NetworkObjectPool.Singleton.GetNetworkObject
            (
                coinPrefab.gameObject,
                GetRandomPosition(),
                Quaternion.identity
            );
            
            networkObject.Spawn(true);
        }

        private Vector3 GetRandomPosition()
        {
            var leftBottomPosition = leftBottomCorner.position;
            var rightTopPosition = rightTopCorner.position;
            
            var x = Random.Range(leftBottomPosition.x + offset.x, rightTopPosition.x - offset.x);
            var y = Random.Range(leftBottomPosition.y  + offset.y, rightTopPosition.y - offset.y);
            
            return new Vector3(x, y);
        }

        private IEnumerator SpawnOveTime()
        {
            var delay = new WaitForSeconds(spawnOverTimeDelay);
            
            while (enabled)
            {
                yield return delay;

                if (NetworkManager.Singleton)
                {
                    yield break;
                }
                
                SpawnCoin();
            }
        }
    }
}

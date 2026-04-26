using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;

namespace Assignment8
{
    public class LevelGenerator : MonoBehaviour
    {
        public static LevelGenerator Instance;

        [System.Serializable]
        public struct FloorConfig
        {
            public string floorName;
            public string startRoomScene;
            public string finishRoomScene;
            public string[] normalRoomScenes;
            public int roomCount;
        }

        [Header("Configurations")]
        public List<FloorConfig> floorConfigs;
        public float roomWidth = 20f;
        public float roomHeight = 12f;

        [Header("References")]
        [SerializeField] private GameObject player;
        // Сюда нужно перетянуть объект NavMeshManager из ГЛАВНОЙ сцены
        [SerializeField] private NavMeshSurface navMeshSurface; 

        public Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();
        private readonly List<Vector2Int> takenPositions = new List<Vector2Int>();
        private int currentFloorIndex;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (floorConfigs.Count > 0)
                Generate();
        }

        public void NextFloor()
        {
            currentFloorIndex++;

            if (currentFloorIndex >= floorConfigs.Count)
                currentFloorIndex = 0;

            StartCoroutine(ResetRoutine());
        }

        private IEnumerator ResetRoutine()
        {
            foreach (Room room in spawnedRooms.Values)
            {
                if (room != null)
                    yield return SceneManager.UnloadSceneAsync(room.gameObject.scene);
            }

            spawnedRooms.Clear();
            takenPositions.Clear();

            Generate();
        }

        private void Generate()
        {
            StartCoroutine(GenerateRoutine());
        }

        private IEnumerator GenerateRoutine()
        {
            FloorConfig config = floorConfigs[currentFloorIndex];

            yield return LoadRoomScene(config.startRoomScene, Vector2Int.zero);

            for (int i = 0; i < config.roomCount; i++)
            {
                Vector2Int pos = GetRandomAdjacent();
                string roomScene = config.normalRoomScenes[
                    Random.Range(0, config.normalRoomScenes.Length)];

                yield return LoadRoomScene(roomScene, pos);
            }

            Vector2Int finishPos = GetFarthestPosition();
            yield return LoadRoomScene(config.finishRoomScene, finishPos);

            foreach (Room room in spawnedRooms.Values)
                room.SetupDoors();

            // Ждем обновления физики, чтобы все коллайдеры комнат заняли свои места
            yield return new WaitForFixedUpdate(); 

            // Запекаем ОДНУ общую сетку для всех загруженных сцен
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
                Debug.Log("<color=green>NavMesh успешно запечен для всего уровня!</color>");
            }
            else
            {
                Debug.LogError("NavMeshSurface не назначен в LevelGenerator!");
            }

            SpawnPlayer();

            if (spawnedRooms.TryGetValue(Vector2Int.zero, out Room startRoom))
                startRoom.OnEnter();

            if (MinimapManager.Instance != null)
            {
                MinimapManager.Instance.InitializeMap(
                    new List<Vector2Int>(spawnedRooms.Keys));

                MinimapManager.Instance.UpdateMap(Vector2Int.zero);
            }

            Debug.Log($"Floor {config.floorName} generated. Rooms: {spawnedRooms.Count}");
        }

        private void SpawnPlayer()
        {
            if (player == null) return;
            if (!spawnedRooms.TryGetValue(Vector2Int.zero, out Room startRoom)) return;

            Transform spawn = startRoom.transform.Find("SpawnPoint");

            player.transform.position =
                spawn != null ? spawn.position : startRoom.transform.position;
        }

        private IEnumerator LoadRoomScene(string sceneName, Vector2Int gridPos)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!op.isDone)
                yield return null;

            Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            Vector3 offset = new Vector3(
                gridPos.x * roomWidth,
                gridPos.y * roomHeight,
                0f);

            Room room = null;

            foreach (GameObject root in loadedScene.GetRootGameObjects())
            {
                root.transform.position += offset;

                if (room == null)
                    room = root.GetComponentInChildren<Room>();
            }

            if (room == null)
            {
                Debug.LogError($"Room component not found in scene: {sceneName}");
                yield break;
            }

            room.gridPos = gridPos;
            spawnedRooms.Add(gridPos, room);
            takenPositions.Add(gridPos);
        }

        private Vector2Int GetRandomAdjacent()
        {
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            for (int attempts = 0; attempts < 1000; attempts++)
            {
                Vector2Int basePos = takenPositions[Random.Range(0, takenPositions.Count)];
                Vector2Int newPos = basePos + dirs[Random.Range(0, dirs.Length)];

                if (!spawnedRooms.ContainsKey(newPos))
                    return newPos;
            }

            return Vector2Int.zero;
        }

        private Vector2Int GetFarthestPosition()
        {
            Vector2Int farthest = Vector2Int.zero;
            float maxDistance = -1f;

            foreach (Vector2Int pos in takenPositions)
            {
                float distance = Vector2Int.Distance(Vector2Int.zero, pos);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthest = pos;
                }
            }

            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            for (int i = 0; i < dirs.Length; i++)
            {
                int randomIndex = Random.Range(i, dirs.Length);
                (dirs[i], dirs[randomIndex]) = (dirs[randomIndex], dirs[i]);
            }

            foreach (Vector2Int dir in dirs)
            {
                Vector2Int candidate = farthest + dir;

                if (!spawnedRooms.ContainsKey(candidate))
                    return candidate;
            }

            return GetRandomAdjacent();
        }
    }
}
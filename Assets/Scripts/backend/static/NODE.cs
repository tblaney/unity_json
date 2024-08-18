using UnityEngine;
namespace snorri
{
    public static class NODE
    {
        public static Map Stages {get; set;} // stages

        public static Task<string, GameObject> TaskToSpawnPrefab {get; set;}

        public static void Init(Task<string, GameObject> taskSpawnPrefab)
        {
            Stages = Map.FromJson("node_stages");
            Stages.Log();

            TaskToSpawnPrefab = taskSpawnPrefab;
        }
    }
}
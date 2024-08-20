using UnityEngine;

// serves as the sole script you should need in a unity scene
namespace snorri
{
    using UnityEngine.EventSystems;

    [UnityEngine.DisallowMultipleComponent]
    public class GameSystem : MonoBehaviour
    {
        bool firstUpdate = false;
        public EventSystem eventSystem;

        void Awake()
        {
            GAME.Init();    

            GAME.Vars.Set<EventSystem>("event_system", eventSystem);

            this.gameObject.AddComponent<NodeSystem>();

            firstUpdate = true;
        }

        void Start()
        {
            // should initiate the starting scene

            GAME.Stage = GAME.Vars.Get<string>("stage_setup", "");
        }

        void Update()
        {
            if (firstUpdate)
            {
                GAME.Stage = GAME.Vars.Get<string>("stage_start", "");
                firstUpdate = false;
            }
        }
    }
}
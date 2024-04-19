# Unity Json Workflow
## This respository provides methods and examples that I have used to create an easy Blender --> Unity workflow, by employing json files.

Drag the contents of the 'resources' folder into your unity project in 'Assets/Resources'. The rest of the contents of this repository can go anywhere in 'Assets/'.

You should view example .json files in 'resources/nodes/' to establish the workflow. Essentially, gameobjects in Unity are created from these node json files, which allows you to create a gameobject hierarchy just using json files. This is effective for transferring meshes from Blender to Unity, as you can write Python scripts to generate these json files within Blender, and then only really need to import mesh objects into the 'Resources/prefabs/' folder within Unity to spawn in mesh objects.

Open scene '_game' within 'scenes/' in Unity, and run the game to start the process. GameSystem.cs is the originator of everything, but NodeSystem.cs is responsible for deciding what objects are to be created. json files within 'resources/json/nodes/stage/' are responsible for spawning in other nodes, so if you want to add gameobjects to the default setup you should link them here. I would recommend reviewing all the C# scrips within 'scripts/common/base/' to understand the workflow (specifically scrips with Node prefix). Node json files should be ordered by their prefix in the 'resources/json/nodes/' folder, otherwise they will not be found by the node system (IE camera_player.json might represent a camera player follower object, and should be placed in 'resources/json/nodes/camera/).

The fields that can (not mandatory) be filled within a node json are:
* "is_prefab" (boolean): Decides if we should create an empty gameobject for this, or use a prefab. If true, the node system will search for a prefab by name using the "prefab_name" field.
* "prefab_name" (string): See above.
* "inherit_from" (string): Should this node inherit from another node? If this is empty, no inheritance will take place, otherwise it will inherit from a node by name of this string value. Anything you set in this json file will override the inherited node.
* "actors" (list): This field represents how you add scripts to the gameobject you have created. The node system will search for scripts you have created that inherit in some form from Actor.cs (so review its contents), and add these scripts to the gameobject in Unity. This functionality also allows you to pass in information to the class instance on the gameobject, which are always included in the Vars property of an Actor instance. You can review "resources/json/nodes/player/player_main.json" to see an extensive usage of this functionality.
* "children" (list): This field is how you setup child gameobjects. You can basically add any type of field mentioned above within each child node and it will function appropriately. If you just want to link another json node, use the "inherit_from" field.

Another useful and commonly used script within the node system is Map.cs. A Map is essentially a generalized dictionary, capable of storing a lot of unique value types and able to read/write directly to a json file. This is often used instead of direct variables, but you should be careful to not overuse this script as it will cause performance issues with a huge amount of entires.

There are lots of other useful scripts in this repository for in-game functionality, but I will not review them here.

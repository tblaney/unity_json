# Unity Json Workflow
## This respository provides methods and examples that I have used to create an easy Blender --> Unity workflow, by employing json files.

Drag the contents of the 'resources' folder into your unity project in 'Assets/Resources'. The rest of the contents of this repository can go anywhere in 'Assets/'.

You should view example .json files in 'resources/nodes/' to establish the workflow. Essentially, gameobjects in Unity are created from these node json files, which allows you to create a gameobject hierarchy just using json files. This is effective for transferring meshes from Blender to Unity, as you can write Python scripts to generate these json files within Blender, and then only really need to import mesh objects into the 'Resources/prefabs/' folder within Unity to spawn in mesh objects.

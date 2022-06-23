using UnityEditor;
using UnityEngine;

public class ObjectSpawner : EditorWindow
{
    private GameObject objectToSpawn;
    private int spawnNumber = 1;
    private int spawnCount = 1;
    private int objectID = 1;
    private Vector2 scrollPos;
    private GameObject[] ghostArray;
    private int startObjID;
    private int endObjID;

    //Spawn Area
    private GameObject spawnArea;
    private float spawnCentreX;
    private float spawnCentreY;
    private float spawnCentreZ;

    //Position values X Axis
    float minPosValX = -0;
    float minPosLimitX = -500;
    float maxPosValX = 0;
    float maxPosLimitX = 500;

    //Position values Y Axis
    float minPosValY = -0;
    float minPosLimitY = -500;
    float maxPosValY = 0;
    float maxPosLimitY = 500;

    //Position values Z Axis
    float minPosValZ = -0;
    float minPosLimitZ = -500;
    float maxPosValZ = 0;
    float maxPosLimitZ = 500;

    //rotation values X Axis
    float minValX = -0;
    float minLimitX = -360;
    float maxValX = 0;
    float maxLimitX = 360;

    //rotation values Y Axis
    float minValY = -0;
    float minLimitY = -360;
    float maxValY = 0;
    float maxLimitY = 360;

    //rotation values Z Axis
    float minValZ = -0;
    float minLimitZ = -360;
    float maxValZ = 0;
    float maxLimitZ = 360;

    //scale values
    float minScaleVal = 1;
    float minScaleLimit = 0;
    float maxScaleVal = 1;
    float maxScaleLimit = 100;


    [MenuItem("Editor Tools/Object Spawner")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ObjectSpawner));
    }
    private void OnGUI()
    {
        if (GameObject.Find("SpawnArea") == null) //if spawn area doesn't already exist 
        {
            spawnArea = Instantiate(Resources.Load("SpawnArea") as GameObject); //add it
            spawnArea.name = "SpawnArea";
        }

        AddGhostTag();
        DeleteGhostObjects();

        GUILayout.Label("Object Spawner", EditorStyles.whiteLargeLabel); //header
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); //creates scrollbar
        objectToSpawn = EditorGUILayout.ObjectField("Object to spawn", objectToSpawn, typeof(GameObject), true) as GameObject; //object to spawn field

        spawnNumber = EditorGUILayout.IntField("Spawn Count", spawnNumber); //asks how many to spawn
        objectID = EditorGUILayout.IntField("Object ID", objectID); //asks what appended ID will start counting from

        //POSITION VALUES
        //X Axis -- float fields min/max slider for position X axis
        GUILayout.Label("");
        GUILayout.Label("Position Range X Axis", EditorStyles.boldLabel);
        minPosValX = EditorGUILayout.FloatField("Lower Limit X Axis", minPosValX);
        maxPosValX = EditorGUILayout.FloatField("Upper Limit X Axis", maxPosValX);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minPosValX, ref maxPosValX, minPosLimitX, maxPosLimitX);
        GUILayout.Label("");

        //Y Axis -- float fields min/max slider for position Y axis
        GUILayout.Label("Position Range Y Axis", EditorStyles.boldLabel);
        minPosValY = EditorGUILayout.FloatField("Lower Limit Y Axis", minPosValY);
        maxPosValY = EditorGUILayout.FloatField("Upper Limit Y Axis", maxPosValY);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minPosValY, ref maxPosValY, minPosLimitY, maxPosLimitY);
        GUILayout.Label("");

        //Z Axis-- float fields min/max slider for position Z axis
        GUILayout.Label("Position Range Z Axis", EditorStyles.boldLabel);
        minPosValZ = EditorGUILayout.FloatField("Lower Limit Z Axis", minPosValZ);
        maxPosValZ = EditorGUILayout.FloatField("Upper Limit Z Axis", maxPosValZ);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minPosValZ, ref maxPosValZ, minPosLimitZ, maxPosLimitZ);
        GUILayout.Label("");

        //Spawn area
        spawnArea = GameObject.Find("SpawnArea");
        spawnCentreX = (minPosValX + maxPosValX) / 2; //get centre of defined spawn range - this tells us where the pivot of the spawn area game object goes
        spawnCentreY = (minPosValY + maxPosValY) / 2;
        spawnCentreZ = (minPosValZ + maxPosValZ) / 2;
        spawnArea.transform.localScale = new Vector3(Mathf.Abs(minPosValX - maxPosValX), Mathf.Abs(minPosValY - maxPosValY), Mathf.Abs(minPosValZ - maxPosValZ));
        //^sets the scale of the spawn area by taking the min/max position range values
        spawnArea.transform.position = new Vector3(spawnCentreX, spawnCentreY, spawnCentreZ); //sets the position transform

        //ROTATION VALUES
        //X Axis
        GUILayout.Label("Rotation Range X Axis", EditorStyles.boldLabel);
        minValX = EditorGUILayout.FloatField("Lower Limit X Axis", minValX);
        maxValX = EditorGUILayout.FloatField("Upper Limit X Axis", maxValX);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minValX, ref maxValX, minLimitX, maxLimitX);
        GUILayout.Label("");

        //Y Axis
        GUILayout.Label("Rotation Range Y Axis", EditorStyles.boldLabel);
        minValY = EditorGUILayout.FloatField("Lower Limit Y Axis", minValY);
        maxValY = EditorGUILayout.FloatField("Upper Limit Y Axis", maxValY);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minValY, ref maxValY, minLimitY, maxLimitY);
        GUILayout.Label("");

        //Z Axis
        GUILayout.Label("Rotation Range Z Axis", EditorStyles.boldLabel);
        minValZ = EditorGUILayout.FloatField("Lower Limit Z Axis", minValZ);
        maxValZ = EditorGUILayout.FloatField("Upper Limit Z Axis", maxValZ);
        EditorGUILayout.MinMaxSlider("Lower/Upper Slider", ref minValZ, ref maxValZ, minLimitZ, maxLimitZ);
        GUILayout.Label("");

        //Scale
        GUILayout.Label("Scale Range", EditorStyles.boldLabel);
        minScaleVal = EditorGUILayout.FloatField("Minimum Scale", minScaleVal);
        maxScaleVal = EditorGUILayout.FloatField("Maximum Scale", maxScaleVal);
        EditorGUILayout.MinMaxSlider("Min/Max Slider", ref minScaleVal, ref maxScaleVal, minScaleLimit, maxScaleLimit);
        GUILayout.Label("");

        EditorGUILayout.EndScrollView();

        GhostObject();

        if (GUILayout.Button("Spawn Object") && objectToSpawn != null) //if spawn button is pressed
        {
            DeleteGhostObjects();
            SpawnObject();
        }
        if (GUILayout.Button("Delete Last Spawn") && objectToSpawn !=null)
        {
            DeleteLastSpawn();
        }

    }
    void OnDestroy()
    {
        DeleteGhostObjects();
        DestroyImmediate(spawnArea); //destroys the spawn area object when window is closed

    }

    private void SpawnObject() //method to spawn objects
    {
        spawnCount = spawnNumber;
        startObjID = objectID;

        while (spawnCount > 0) //counts down from spawn count (number to spawn) to 0
        {
            GameObject newObject = Instantiate(objectToSpawn, new Vector3(Random.Range(minPosValX, maxPosValX), Random.Range(minPosValY, maxPosValY), Random.Range(minPosValZ, maxPosValZ)),
                Quaternion.Euler(new Vector3(Random.Range(minValX, maxValX), Random.Range(minValY, maxValY), Random.Range(minValZ, maxValZ))));
            //^creates each object within a random range and rotation based on the set limits
            newObject.name = objectToSpawn.name + objectID; //renames the new object and appends object ID
            newObject.transform.localScale = Vector3.one * Random.Range(minScaleVal, maxScaleVal); //changes object scale randomly in range set
            spawnCount = spawnCount - 1; //decrease spawn count
            objectID = objectID + 1; //increase object ID
            endObjID = objectID;
        }

    }

    private void DeleteLastSpawn()
    {
        //find all objects called objectname + ID from start to end ID number
        while (startObjID <= endObjID)
        {
            GameObject deleteObject = GameObject.Find(objectToSpawn.name + startObjID);
            DestroyImmediate(deleteObject);
            startObjID++;
        }
    }
    private void GhostObject() //method to spawn objects
    {
        spawnCount = spawnNumber;
        if (objectToSpawn != null)
        {
            while (spawnCount > 0) //counts down from spawn count (number to spawn) to 0
            {
                GameObject newObject = Instantiate(objectToSpawn, new Vector3(Random.Range(minPosValX, maxPosValX), Random.Range(minPosValY, maxPosValY), Random.Range(minPosValZ, maxPosValZ)),
                    Quaternion.Euler(new Vector3(Random.Range(minValX, maxValX), Random.Range(minValY, maxValY), Random.Range(minValZ, maxValZ))));
                //^creates each object within a random range and rotation based on the set limits
                newObject.transform.localScale = Vector3.one * Random.Range(minScaleVal, maxScaleVal); //changes object scale randomly in range set
                newObject.tag = "ghost";
                Renderer rend = newObject.GetComponent<Renderer>();
                rend.material = Resources.Load("Ghost_MAT", typeof(Material)) as Material;
                spawnCount = spawnCount - 1; //decrease spawn count
            }
        }


    }
    private void DeleteGhostObjects()
    {
        ghostArray = GameObject.FindGameObjectsWithTag("ghost");
        if (ghostArray.Length > 0)
        {
            foreach (GameObject ghost in ghostArray)
            {
                DestroyImmediate(ghost);
            }
        }

    }
    private void AddGhostTag()
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // For Unity 5 we need this too
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        // Adding a Tag
        string s = "ghost";

        // First check if it is not already present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(s)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
        }

        // Setting a Layer (Let's set Layer 10)
        string layerName = "ghost";

        // --- Unity 5 ---
        SerializedProperty sp = layersProp.GetArrayElementAtIndex(10);
        if (sp != null) sp.stringValue = layerName;
        // and to save the changes
        tagManager.ApplyModifiedProperties();
    }
}
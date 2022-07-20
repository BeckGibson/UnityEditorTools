using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AssignMaterials : EditorWindow
{

    public List<string> modelsList = new List<string>();
    List<string> selectedModels = new List<string>();
    List<string> modelsListToShow = new List<string>();

    [MenuItem("Editor Tools/Auto Assign Materials")]


    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssignMaterials));
    }
    private void OnGUI()
    {
        GUIStyle GetBtnStyle() //button style for the X button in the model list
        {
            var style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.green;
            style.margin.left = 10;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        modelsListToShow.Clear(); //wipes the list to display
        foreach (string file in modelsList)
        {
            if (!modelsListToShow.Contains(file)) //adds each file from the model list to the display list if it's not shown already
            {
                modelsListToShow.Add(file);
            }
        }
        foreach (string file in modelsListToShow)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("x", GetBtnStyle(), GUILayout.Width(20))) //adds a label and button to display each of the files from the display list
            {
                modelsList.RemoveAll(x => x == file); //button removes all instances of the file from the texture list
            }
            GUILayout.Label(file);

            EditorGUILayout.EndHorizontal();
        }
        if (modelsListToShow.Count == 0)
        {
            GUILayout.Label("No models added.", GUILayout.Height(100));
        }

        if (GUILayout.Button("Add Models", GUILayout.Height(30)))
        {
            modelsList = GetSelectedModels(selectedModels);

        }
        if (GUILayout.Button("Clear List", GUILayout.Height(30)))
        {
            int count = modelsList.Count;
            modelsList.Clear();
        }
        if (GUILayout.Button("Search and Remap Materials", GUILayout.Height(50)))
        {
            modelsList = EditModelImportSettings(modelsList);
        }
    }

    public static List<string> GetSelectedModels(List<string> selectedModels) //method to get the selected files from the asset window
    {
        string path = null;
        int count = 0;

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if(path.EndsWith(".fbx")|| path.EndsWith(".obj"))
            {
                selectedModels.Add(path);
            }
            count++;
        }
        return selectedModels;
    }
    private List<string> EditModelImportSettings(List<string> modelsList)
    {
        foreach (string file in modelsList)
        {
            var assetImporter = AssetImporter.GetAtPath(file);
            ModelImporter modelImporter = assetImporter as ModelImporter;
            try
            {
                modelImporter.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Everywhere);
                assetImporter.SaveAndReimport();
            }
            catch
            {

            }
        }
        modelsList.Clear();
        return modelsList;
    }
}

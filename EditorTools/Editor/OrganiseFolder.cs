using UnityEditor;
using UnityEngine;
using System.IO;

public class OrganiseFolder : EditorWindow
{
    private string pathDirectory;
    private object[] folderAssets;
    private string projectPath;
    private string folderPath;
    private string matFolder;
    private string texFolder;
    private string modFolder;
    private bool rootFolder = false;

    [MenuItem("Editor Tools/Organise Folder")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(OrganiseFolder));
    }
    private void OnGUI()
    {
        projectPath = Application.dataPath;
        pathDirectory = GetSelectedPathOrFallback();
        pathDirectory = pathDirectory.Remove(0, 6);
        folderPath = projectPath + pathDirectory;

        GUILayout.Label("Organise Art Files", EditorStyles.whiteLargeLabel);
        GUILayout.Label("Select Folder from Project Window");
        GUILayout.TextArea(folderPath);
        GUILayout.Label("");

        if (rootFolder == true)
        {
            projectPath = folderPath;
        }

        //CHECK IF FOLDERS EXISTS 
        bool matDir = Directory.Exists(projectPath + "/Materials");
        bool texDir = Directory.Exists(projectPath + "/Textures");
        bool modDir = Directory.Exists(projectPath + "/Models");

        //Warning messages display if folders don't exist already
        if (matDir == false)
        {
            GUILayout.Label("Materials Folder Not found: New Folder Will be added", EditorStyles.whiteMiniLabel);
        }
        if (texDir == false)
        {
            GUILayout.Label("Textures Folder Not found: New Folder Will be added", EditorStyles.whiteMiniLabel);
        }
        if (modDir == false)
        {
            GUILayout.Label("Models Folder Not found: New Folder Will be added", EditorStyles.whiteMiniLabel);
        }
        GUILayout.Label("");
        rootFolder = GUILayout.Toggle(rootFolder, "Organise Files Within Selected Folder");

        if (GUILayout.Button("Organise") && folderPath != null)
        {
            //DIVIDING FILES IN FOLDER INTO ARRAYS BY FILE TYPE
            //get mat files
            string[] matFiles = Directory.GetFiles(folderPath, "*.mat*");
            //get png files
            string[] pngFiles = Directory.GetFiles(folderPath, "*.png*");
            //get jpg files
            string[] jpgFiles = Directory.GetFiles(folderPath, "*.jpg*");
            //get tga files
            string[] tgaFiles = Directory.GetFiles(folderPath, "*.tga*");
            //get tif files
            string[] tifFiles = Directory.GetFiles(folderPath, "*.tif*");
            //get tiff files
            string[] tiffFiles = Directory.GetFiles(folderPath, "*.tiff*");
            //get jpeg files
            string[] jpegFiles = Directory.GetFiles(folderPath, "*.jpeg*");
            //get fbx files
            string[] fbxFiles = Directory.GetFiles(folderPath, "*.fbx*");
            //get obj files
            string[] objFiles = Directory.GetFiles(folderPath, "*.obj*");


            //create folders if they don't exist already
            if (matDir == false)
            {
                matFolder = projectPath + "/Materials/";
                Directory.CreateDirectory(matFolder);
            }
            if (texDir == false)
            {
                texFolder = projectPath + "/Textures/";
                Directory.CreateDirectory(texFolder);
            }
            if (modDir == false)
            {
                modFolder = projectPath + "/Models/";
                Directory.CreateDirectory(modFolder);
            }

            AssetDatabase.Refresh(); //refreshes the asset folder after new folders created
            try
            {
                AssetDatabase.StartAssetEditing();
                //Organise material files
                OrganiseFolderFiles(matFiles, matFolder);
                //Organise model files
                OrganiseFolderFiles(fbxFiles, modFolder);
                OrganiseFolderFiles(objFiles, modFolder);
                //Organise texture files
                OrganiseFolderFiles(pngFiles, texFolder);
                OrganiseFolderFiles(jpgFiles, texFolder);
                OrganiseFolderFiles(tgaFiles, texFolder);
                OrganiseFolderFiles(tifFiles, texFolder);
                OrganiseFolderFiles(tiffFiles, texFolder);
                OrganiseFolderFiles(jpegFiles, texFolder);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();


        }

    }
    //gets the selected folder in unity
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    //moves the files in the given array to the given folder path
    private void OrganiseFolderFiles(string[] fileList, string folder)
    {
        int count = 0;
        if (fileList.Length != 0 || fileList != null)
        {
            foreach (string file in fileList) //for each file in the array
            {
                string destinationPath = folder + "/" + Path.GetFileName(fileList[count]); //sets the destination path with the folder name and file name from the array
                if (File.Exists(destinationPath)) //checks if the path exists already
                {
                    int fileNo = 1;
                    while (File.Exists(destinationPath)) //if the path exists 
                    {
                        destinationPath = folder + "/" + fileNo + "_" + Path.GetFileName(fileList[count]); //appends a number to the file name
                        fileNo++; //increase file name number
                    }
                    File.Move(fileList[count], destinationPath);
                    count++;
                }
                else
                {
                    File.Move(fileList[count], destinationPath);
                    count++;
                }
            }
        }

    }
}



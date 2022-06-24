using UnityEditor;
using UnityEngine;
using System.IO;


public class CreateMaterial : EditorWindow
{
    [MenuItem("Editor Tools/Create Material")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreateMaterial));
    }
    private void OnGUI()
    {
    }
}
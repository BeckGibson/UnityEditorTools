using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

public class CreateMaterial : EditorWindow
{
    private Shader shader;
    private Texture2D baseColor;
    private Texture2D normal;
    private Texture2D detail;
    private Texture2D metallicSmoothness;
    private Texture2D metallic;
    private Texture2D ao;
    private string folderPath;
    private string materialName;
    private string baseString;
    private string baseString2;
    public List<string> textureList = new List<string>();
    public List<string> sortedList;
    public List<List<string>> masterlist;
    List<string> selectedTextures = new List<string>();
    List<string> textureListToShow = new List<string>();
    private int materialOverwriteOption = 3;

    [MenuItem("Editor Tools/Create Material")]


    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreateMaterial));
    }
    private void OnGUI()
    {
        GUIStyle GetBtnStyle()
        {
            var style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.green;
            style.margin.left = 10;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        textureListToShow.Clear();
        foreach (string file in textureList)
        {
            if (!textureListToShow.Contains(file))
            {
                textureListToShow.Add(file);
            }
        }
        foreach (string file in textureListToShow)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("x", GetBtnStyle(), GUILayout.Width(20)))
            {
                textureList.RemoveAll(x => x == file);
            }
            GUILayout.Label(file);

            EditorGUILayout.EndHorizontal();
        }
        if (textureListToShow.Count == 0)
        {
            GUILayout.Label("No textures added.", GUILayout.Height(100));
        }


        if (GUILayout.Button("Add Textures", GUILayout.Height(30)))
        {
            textureList = GetSelectedTextures(selectedTextures);

        }
        if (GUILayout.Button("Clear List", GUILayout.Height(30)))
        {
            int count = textureList.Count;
            textureList.Clear();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Overwrite Shader:");
        shader = (Shader)EditorGUILayout.ObjectField(shader, typeof(Shader), false);
        EditorGUILayout.EndHorizontal();

        if (shader != null)
        {
            if (GUILayout.Button("Create Materials", GUILayout.Height(30)))
            {
                while (textureList.Count > 0)
                {
                    masterlist = SeperateTextures(textureList); //divides textures into one sorted list for one material and the rest
                    textureList = masterlist[0]; //returned unsorted textures
                    sortedList = masterlist[1]; //returned sorted textures
                    sortedList = masterlist[1]; //returned sorted textures
                    sortedList = CreateNewMaterialFromList(sortedList, shader, materialOverwriteOption); //creates new material with sorted list, returns anything unused (but not textures that are accepted but unassigned due to shader constraints).
                    if (sortedList.Count != 0) //if the returned list isn't empty
                    {
                        textureList.AddRange(sortedList); //add it back to the original texture list with the rest
                    }
                }
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Quest Materials", GUILayout.Height(75)))
        {
            shader = Shader.Find("Optimised/DetailTexture");

            while (textureList.Count > 0)
            {
                masterlist = SeperateTextures(textureList); //divides textures into one sorted list for one material and the rest
                textureList = masterlist[0]; //returned unsorted textures
                sortedList = masterlist[1]; //returned sorted textures
                sortedList = CreateNewMaterialFromList(sortedList, shader, materialOverwriteOption); //creates new material with sorted list, returns anything unused (but not textures that are accepted but unassigned due to shader constraints).
                if (sortedList.Count != 0) //if the returned list isn't empty
                {
                    textureList.AddRange(sortedList); //add it back to the original texture list with the rest
                }
            }
            shader = null;
            baseColor = null;
        }
        if (GUILayout.Button("Create Standard Materials", GUILayout.Height(75)))
        {
            shader = Shader.Find("Standard");
            while (textureList.Count > 0)
            {

                masterlist = SeperateTextures(textureList); //divides textures into one sorted list for one material and the rest
                textureList = masterlist[0]; //returned unsorted textures
                sortedList = masterlist[1]; //returned sorted textures
                sortedList = CreateNewMaterialFromList(sortedList, shader, materialOverwriteOption); //creates new material with sorted list, returns anything unused (but not textures that are accepted but unassigned due to shader constraints).
                if (sortedList.Count != 0) //if the returned list isn't empty
                {
                    textureList.AddRange(sortedList); //add it back to the original texture list with the rest
                }
            }
            shader = null;
            baseColor = null;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Material Overwrite Handling:");
        var text = new string[] { " Append textures to existing materials", " Wipe and replace existing materials", " Create new copy of material and keep original" };
        materialOverwriteOption = GUILayout.SelectionGrid(materialOverwriteOption, text, 1, EditorStyles.radioButton);

    }

    private List<List<string>> SeperateTextures(List<string> textureList)
    {
        //get string 1, remove everything after _
        //add original string 1 to new array and delete
        //loop through array and check if anything matches string
        //if yes, copy to new string 1 array and delete
        //after all checked, move to check string 2

        List<string> sortedTextures = new List<string>(); //new list
        List<List<string>> masterList = new List<List<string>>(); //master list of lists!
        string pattern = @"(?<=_).[A-Za-z\d]+";
        baseString = textureList[0];

        //removes everything from first string after prefix
        var matches = Regex.Matches(baseString, pattern);

        if (matches.Count > 0)
        {
            var match = matches[matches.Count - 1];
            var group = match.Groups[match.Groups.Count - 1];
            baseString = baseString.Remove(group.Index);
        }

        sortedTextures.Add(textureList[0]); //adds original first string to new list
        textureList.RemoveAt(0); //removes string from original list

        int loopLength = textureList.Count;
        int count = 0;
        while (count <= loopLength)
        {
            int index = textureList.FindIndex(x => x.StartsWith(baseString)); //searches textureList for basestring
            if (index != -1)
            {
                sortedTextures.Add(textureList[index]); //adds it to the list if it finds something
                textureList.RemoveAt(index);
            }
            count++;
        }

        masterList.Add(textureList);
        masterList.Add(sortedTextures);

        return masterList;
    }

    private List<string> CreateNewMaterialFromList(List<string> textureList, Shader shader, int materialOverwriteOption)
    {

        //gets list of related textures
        //checks first file for prefix name
        //creates new materials with found prefix name
        //loads all files from list into texture files -> search list for prefix name + suitable suffixes (normal, AO, ect.) -> removes file from list when found
        //adds loaded files into material
        //returns list (in case files aren't added to material)

        string pattern = @"(?<=_).[A-Za-z\d]+";
        string pattern2 = @"(?<= ).[A-Za-z\d]+";
        string prefixName = null;
        string normalPath;
        string baseColorPath;
        string detailPath;
        string metallicSmoothnessPath;
        string metallicPath;
        string aoPath;
        List<string> textureListLower;
        bool matFolderExists = false;

        textureListLower = textureList; //indentical in every way but one: lower case. So we can ignore case when searching for file names without compromising the One True File Path.
        int count = 0;
        int length = textureListLower.Count - 1;

        while (length >= count)
        {
            textureListLower[count] = textureListLower[count].ToLower();
            count++;
        }

        int index = textureListLower.FindIndex(x => x.Contains("basecolor")); //find the BaseColor map

        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("basecolour"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("base"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("colour"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("color"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("albedo"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("diffuse"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("_al."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("_dif."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains("_d."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(" al."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(" dif."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(" d."));
        }

        if (index != -1)
        {
            baseColorPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            baseColorPath = null;
        }

        if (baseColorPath != null)
        {
            baseColor = (Texture2D)AssetDatabase.LoadAssetAtPath(baseColorPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            baseColor = null;
        }

        if (baseColor != null) //gets material name and prefix name from base color map
        {
            materialName = baseColor.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else if (matches.Count == 0 || matches2.Count == 0)
            {
                int start = materialName.IndexOf("_d");
                if (start == -1)
                {
                    start = materialName.IndexOf(" d");
                }
                materialName = materialName.Remove(start, 2);
                prefixName = materialName.ToLower() + "_";
                materialName = materialName + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            if (materialName.Contains("_Base_")) //if the material name contains Base_ the base color map was named with an _ incorrectly, this fixes that mistake in the material name
            {
                int start = materialName.IndexOf("_Base_");
                materialName = materialName.Remove(start, 5);
                prefixName = materialName.Remove(materialName.Length - 3);
                prefixName = prefixName.ToLower();
            }
            else if (materialName.Contains("_base_")) //in case it wasn't capitalised
            {
                int start = materialName.IndexOf("_base_");
                materialName = materialName.Remove(start, 5);
                prefixName = materialName.Remove(materialName.Length - 3);
                prefixName = prefixName.ToLower();
            }
            else if (materialName.Contains(" Base_")) //in case it was a space and not _
            {
                int start = materialName.IndexOf(" Base_");
                materialName = materialName.Remove(start, 5);
                prefixName = materialName.Remove(materialName.Length - 3);
                prefixName = prefixName.ToLower();
            }
            else if (materialName.Contains(" base_")) //in case it also wasn't capitalised
            {
                int start = materialName.IndexOf(" base_");
                materialName = materialName.Remove(start, 5);
                prefixName = materialName.Remove(materialName.Length - 3);
                prefixName = prefixName.ToLower();
            }
            prefixName = prefixName.Remove(prefixName.Length - 1); //removes the _ or space in the prefix name, if one has been set

        }
        if (baseColor == null) //couldn't find a base color map
        {
            prefixName = "";
        }
        //
        index = textureListLower.FindIndex(x => x.Contains(prefixName + "_normal")); //find the normal map
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " normal"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_no."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_n."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_nm."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " no."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " n."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " nm."));
        }

        if (index != -1)
        {
            normalPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            normalPath = null;
        }

        if (normalPath != null)
        {
            normal = (Texture2D)AssetDatabase.LoadAssetAtPath(normalPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            normal = null;
        }

        if (baseColor == null && normal != null) //gets material name and prefix name from normal map IF basecolor doesn't exist
        {
            materialName = normal.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else if (matches.Count == 0 || matches2.Count == 0)
            {
                int start = materialName.IndexOf("_n");
                if (start == -1)
                {
                    start = materialName.IndexOf(" n");
                }
                materialName = materialName.Remove(start, 2);
                prefixName = materialName.ToLower() + "_";
                materialName = materialName + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            prefixName = prefixName.Remove(prefixName.Length - 1);
        }

        //
        index = textureListLower.FindIndex(x => x.Contains(prefixName + "_detail")); //find the detail map
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " detail"));
        }
        if (index != -1)
        {
            detailPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            detailPath = null;
        }

        if (detailPath != null)
        {
            detail = (Texture2D)AssetDatabase.LoadAssetAtPath(detailPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            detail = null;
        }

        if (baseColor == null && normal == null && detail != null) //gets material name and prefix name from detaiil map IF basecolor and normal doesn't exist
        {
            materialName = detail.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            prefixName = prefixName.Remove(prefixName.Length - 1);
        }
        //
        index = textureListLower.FindIndex(x => x.Contains(prefixName + "_metallicsmoothness"));//find the metallicSmoothness map
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " metallicsmoothness"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " me."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_me."));
        }
        if (index != -1)
        {
            metallicSmoothnessPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            metallicSmoothnessPath = null;
        }

        if (metallicSmoothnessPath != null)
        {
            metallicSmoothness = (Texture2D)AssetDatabase.LoadAssetAtPath(metallicSmoothnessPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            metallicSmoothness = null;
        }

        if (baseColor == null && normal == null && detail == null && metallicSmoothness != null) //gets material name and prefix name from metSmooth map IF basecolor, normal, and detail don't exist
        {
            materialName = metallicSmoothness.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            prefixName = prefixName.Remove(prefixName.Length - 1);
        }
        //
        index = textureListLower.FindIndex(x => x.Contains(prefixName + "_metallic")); //find the metallic map
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " metallic"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " metal."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_metal."));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " metalness"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_metalness"));
        }
        if (index != -1)
        {
            metallicPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            metallicPath = null;
        }

        if (metallicPath != null)
        {
            metallic = (Texture2D)AssetDatabase.LoadAssetAtPath(metallicPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            metallic = null;
        }

        if (baseColor == null && normal == null && detail == null && metallicSmoothness == null && metallic != null) //gets material name and prefix name from metSmooth map IF basecolor, normal, and detail don't exist
        {
            materialName = metallic.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            prefixName = prefixName.Remove(prefixName.Length - 1);
        }
        //
        index = textureListLower.FindIndex(x => x.Contains(prefixName + "_ao")); //find the ao map
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " ao"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_occlusion"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " occlusion"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + "_ambient"));
        }
        if (index == -1)
        {
            index = textureListLower.FindIndex(x => x.Contains(prefixName + " ambient"));
        }

        if (index != -1)
        {
            aoPath = textureList[index];
            textureList.RemoveAt(index);
        }
        else
        {
            aoPath = null;
        }

        if (aoPath != null)
        {
            ao = (Texture2D)AssetDatabase.LoadAssetAtPath(aoPath, typeof(Texture2D)); //loads the texture at path
        }
        else
        {
            ao = null;
        }

        if (baseColor == null && normal == null && detail == null && metallicSmoothness == null && metallic == null && ao != null) //gets material name and prefix name from metSmooth map IF basecolor, normal, and detail don't exist
        {
            materialName = ao.name;
            var matches = Regex.Matches(materialName, pattern); //looking for _
            var matches2 = Regex.Matches(materialName, pattern2); //looking for space
            if (matches.Count > 0)
            {
                var match = matches[matches.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();

                materialName = materialName + "MAT";
            }
            else if (matches2.Count > 0)
            {
                var match = matches2[matches2.Count - 1];
                var group = match.Groups[match.Groups.Count - 1];
                materialName = materialName.Remove(group.Index);
                prefixName = materialName.ToLower();
                materialName = materialName.Remove(materialName.Length - 1) + "_MAT";
            }
            else
            {
                prefixName = materialName.ToLower();
                materialName = materialName + "MAT";
            }
            prefixName = prefixName.Remove(prefixName.Length - 1);
        }

        folderPath = null;
        if (baseColor != null)
        {
            folderPath = AssetDatabase.GetAssetPath(baseColor);
        }
        else if (normal != null)
        {
            folderPath = AssetDatabase.GetAssetPath(normal);
        }
        else if (detail != null)
        {
            folderPath = AssetDatabase.GetAssetPath(detail);
        }
        else if (metallicSmoothness != null)
        {
            folderPath = AssetDatabase.GetAssetPath(metallicSmoothness);
        }
        else if (metallic != null)
        {
            folderPath = AssetDatabase.GetAssetPath(metallic);
        }
        else if (ao != null)
        {
            folderPath = AssetDatabase.GetAssetPath(ao);
        }
        else
        {
            if (textureList.Count >= 0)
            {
                textureList.Clear(); //if no textures named correctly are found, clears the whole list
            }
        }
        if (folderPath != null)
        {
            folderPath = folderPath.Remove(folderPath.IndexOf("Textures"));
            Material material;

            if (Directory.Exists(folderPath + "Materials/")) //does the materials folder we want exist?
            {
                matFolderExists = true;
            }
            else if (Directory.Exists(folderPath)) //if no, does the asset folder path we want exist?
            {
                string matFolder = folderPath + "/Materials/";
                Directory.CreateDirectory(matFolder); //creates a material folder if one doesn't exist
                matFolderExists = true;
            }
            if (matFolderExists == true)
            {
                folderPath = folderPath + "Materials/" + materialName + ".mat";

                if (File.Exists(folderPath) && materialOverwriteOption == 0) //if the file exists and we don't want to copy/replace it, load it
                {
                    material = (Material)AssetDatabase.LoadAssetAtPath(folderPath, typeof(Material));
                }
                else if (!File.Exists(folderPath)) //if it doesn't exist, we create a new material
                {
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folderPath);
                }
                else if (File.Exists(folderPath) && materialOverwriteOption == 1) //if it exists and we want to wipe/replace the materials
                {
                    AssetDatabase.DeleteAsset(folderPath); //deletes the file at the path
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folderPath);
                }
                else if (File.Exists(folderPath) && materialOverwriteOption == 2) //if it exists and we want to keep both copies
                {
                    int increment = 2;
                    folderPath = folderPath.Remove(folderPath.Length - 8) + increment as string + "_MAT.mat"; //we add "2" to the material name
                    if (File.Exists(folderPath)) //if it still exists
                    {
                        increment++;
                        while (File.Exists(folderPath)) //keep looping until the path doesn't exist
                        {
                            folderPath = folderPath.Remove(folderPath.Length - 9) + increment as string + "_MAT.mat";
                            increment++;
                        }
                    }

                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folderPath);
                }
                else
                {
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folderPath);
                }

                if (material.HasProperty("_MainTex") && baseColor != null)
                {
                    material.SetTexture("_MainTex", baseColor);
                }
                if (material.HasProperty("_DetailMap") && detail != null)
                {
                    material.SetTexture("_DetailMap", detail);
                }
                if (material.HasProperty("_MetallicGlossMap") && metallicSmoothness != null)
                {
                    material.SetTexture("_MetallicGlossMap", metallicSmoothness);
                }
                else if (material.HasProperty("_MetallicGlossMap") && metallic != null)
                {
                    material.SetTexture("_MetallicGlossMap", metallic);
                }
                if (material.HasProperty("_BumpMap") && normal != null)
                {
                    material.SetTexture("_BumpMap", normal);
                }
                if (material.HasProperty("_OcclusionMap") && ao != null)
                {
                    material.SetTexture("_OcclusionMap", ao);
                }

            }
        }
        return textureList;
    }

    public static List<string> GetSelectedTextures(List<string> selectedTextures)
    {
        string path = null;
        int count = 0;

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            selectedTextures.Add(path);
            count++;
        }
        return selectedTextures;
    }
}
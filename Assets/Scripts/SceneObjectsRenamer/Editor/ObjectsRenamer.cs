using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Unity editor window.
/// Allows to rename and/or manipulate the names of multiple selected objects in the scene's 'Hierarchy' window.
/// </summary>
public class ObjectsRenamer : EditorWindow 
{
    private string newName;
    private string addToName;

    private int startNumber;
    private int selectedFormatting = 2;
    private string[] options = new string[] {"#", "##", "###"};

    private string replaceInName;
    private string replaceWith;

    private Vector2 scrollPos = Vector2.zero;


    
    // Display the custom window in the Unity's 'Window' menu and opens it when clicked.
    [MenuItem("Window/Custom/Renamer")]
    public static void ShowWindow() {
        GetWindow<ObjectsRenamer>("Renamer");
    }
   
    // Display a small icon in front of the custom window name of the tab title.
    private void OnEnable() {
        Texture2D myIcon = EditorGUIUtility.Load("Assets/Scripts/Editor/SceneObjectsRenamerTool/Icon.png") as Texture2D;
        titleContent.image = myIcon;
    }
    
    // Display logic of the custom window.
    private void OnGUI() {
        // Set up to make the custom window into an auto adjustable a scrollable window
        float width = this.position.width;
        float height = this.position.height;
        
        // Beginning of the scrollable window code
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(width), GUILayout.Height(height)); 

        #region Window Display
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        #region Rename the Whole Name
        // Display a text field in the inspector that will collect the name that should replace the old one.
        newName = EditorGUILayout.TextField(newName); 

        // Button
        if(GUILayout.Button("Rename")) {
            ChangeTheWholeName(Selection.gameObjects, newName);
        }
        #endregion Rename the Whole Name

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region Prefixes and Suffixes
        // Display a text field in the inspector that will collect the string that should be place before or after the current name.
        addToName = EditorGUILayout.TextField("Add to Name", addToName); 

        // Button
        if (GUILayout.Button("Add Prefix")) {
            AddPrefix(Selection.gameObjects, addToName);
        }

        // Button
        if (GUILayout.Button("Add Suffix")) {
            AddSuffix(Selection.gameObjects, addToName);
        }
        #endregion Prefixes and Suffixes

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region Numbering Options
        // Input field that collects the number that begins the numbering sequence.
        startNumber = EditorGUILayout.IntField("Start Number", startNumber);  
        // Select Rollup that collects the formatting option that should be used.
        selectedFormatting = EditorGUILayout.Popup("Formatting", selectedFormatting, options); 

        //Button
        if (GUILayout.Button("Prefix Numbering")) {
            AddNumbering(Selection.gameObjects, startNumber, selectedFormatting, false);
        }

        //Button
        if (GUILayout.Button("Suffix Numbering")) {
            AddNumbering(Selection.gameObjects, startNumber, selectedFormatting, true);
        }
        #endregion Numbering Options

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #region First and Last Character Removal
        // Button
        if (GUILayout.Button("Remove First Character")) {
            RemoveFirstChar(Selection.gameObjects);
        }

        // Button
        if (GUILayout.Button("Remove Last Character")) {
            RemoveLastChar(Selection.gameObjects);
        }
        #endregion First and Last Character Removal

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        #endregion Window Display
        // Indicate the end of the scrollable window.
        GUILayout.EndScrollView(); 
    }

    #region Buttons Logics
    /// <summary>
    /// Change the whole name of every GameObject in an array.
    /// </summary>
    private void ChangeTheWholeName(GameObject[] selection, string newName) {
        foreach (GameObject obj in selection) {
            obj.name = newName;
        }
    }

    /// <summary>
    /// Add a string in front of the name of every GameObject in an array.
    /// </summary>
    private void AddPrefix(GameObject[] selection, string addition) {
        foreach (GameObject obj in selection) {
            obj.name = addition + obj.name;
        }
    }

    /// <summary>
    /// Add a string at the end of the name of every GameObject in an array.
    /// </summary>
    private void AddSuffix(GameObject[] selection, string addition) {
        foreach (GameObject obj in selection) {
            obj.name = obj.name + addition;
        }
    }

    /// <summary>
    /// Number a selection of objects. /!\ Careful! Only works with objects on the same level of hierarchy.
    /// </summary>
    /// <param name="selection">The array of GameObjects that will receive the numbering.</param>
    /// <param name="startingNumber">The number at which the numbering should start.</param>
    /// <param name="asSuffix">If true, the numbering will be added at the end of the name. Otherwise it will be added in front of the name</param>
    private void AddNumbering(GameObject[] selection, int startingNumber, int selectedFormat, bool asSuffix) {
        // First we reorder the selected objects so that it matches their order in the hierarchy.
        // As "Selection.gameObjects" tends to not respect the hierarchy order but rather the order in which the objects were created.
        BubbleSort(selection);
        int index = startingNumber;

        for (int i = 0; i < selection.Length; i++) {
            // We use the string decimal format specifier: ("D", "D2", "D3"). It specifies how many characters the displayed integer should have at minimum.
            // For example: formatting an int of value 3 using the "D3" formatting will output "003".
            switch (selectedFormat) {
                case 0:
                    if(asSuffix) {
                        selection[i].name = selection[i].name + index.ToString();
                    }
                    else {
                        selection[i].name = index.ToString() + selection[i].name;
                    }
                    break;

                case 1:
                    if (asSuffix) {
                        selection[i].name = selection[i].name + index.ToString("D2"); 
                    }
                    else {
                        selection[i].name = index.ToString("D2") + selection[i].name;
                    }
                    break;

                case 2:
                    if (asSuffix) {
                        selection[i].name = selection[i].name + index.ToString("D3");
                    }
                    else {
                        selection[i].name = index.ToString("D3") + selection[i].name;
                    }
                    break;

                default:
                    Debug.LogError("Numbering Error : No matched case.");
                    break;
            }

            index++;
        }
    }
    
    /// <summary>
    /// Remove the first character of the name of every selected objects.
    /// </summary>
    private void RemoveFirstChar(GameObject[] selection) {
        foreach (GameObject obj in selection) {
            // We put all the characters of the name into an array.
            // We clear the name.
            // And then we add back all the letters but we skip the first one.
            char[] characters;
            characters = obj.name.ToCharArray();
            obj.name = "";

            for (int i = 1; i < characters.Length; i++) {
                obj.name += characters[i];
            }
        }
    }

    /// <summary>
    /// Remove the last character of the name of every selected objects.
    /// </summary>
    private void RemoveLastChar(GameObject[] selection) {
        foreach (GameObject obj in selection) {
            // We put all the characters of the name into an array.
            // We clear the name.
            // And then we add back all the letters except for the last one.
            char[] characters;
            characters = obj.name.ToCharArray();
            obj.name = "";

            for (int i = 0; i < characters.Length - 1; i++) {
                obj.name += characters[i];
            }
        }
    }
    #endregion Buttons Logics


    /// <summary>
    /// Sort an array of GameObjects based on their hierarchy position (using a bubble-sort algorithm).
    ///</summary>
    public static void BubbleSort(GameObject[] array) {
        // We push the object with the largest SiblingIndex to the end of the array by swapping it with its neighbour if its SiblingIndex is larger.
        // We don't check the last one as we are comparing the objects to their next neighbour and the last spot of the array does not have any.
        // As we move along the iterrations, we need to check fewer and fewer positions as we are sure the objects at the end of the array are already sorted correctly.
        for (int i = 0; i < array.Length - 1; i++) {
            for (int j = 0; j < array.Length - i - 1; j++) {
                if (array[j].transform.GetSiblingIndex() > array[j + 1].transform.GetSiblingIndex()) {
                    GameObject temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
}

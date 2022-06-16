# UnitySceneObjectsRenamer
Unity Editor window that allows you to rename and/or manipulate names of multiple selected GameObjects in a scene.

-Features:
    -Full name change
    -Removing first or last letters of the names
    -Adding a string to the names (suffix or affix)
    -Numbering in sequence (suffix or affix)
    -3 numbering format: 0, 00, 000
    /!\ Careful, numbering only works if every object selected is on the same hierarchy level (will not work if a child or parent is present in the selection of GameObjects).

    To implement into your editor, simply copy the code (must be in an 'Editor' folder inside /Assets) or download and import the .unitypackage.
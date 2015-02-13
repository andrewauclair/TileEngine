using UnityEditor;
using UnityEngine;

public class CEditorTools : MonoBehaviour
{
    public const string msc_strToolsName = "Tools";

    [MenuItem(msc_strToolsName + "/Clear Prefs")]
    private static void vClearPrefs()
    {
        Debug.Log("Clearing Player Prefs");
        PlayerPrefs.DeleteAll();
    }
}

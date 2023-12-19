using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[InitializeOnLoad]
public static class RFX1LayerUtilsDistortion
{
    static RFX1LayerUtilsDistortion()
    {
        CreateLayer("BloomMobileEffect");
    }

    public static void CreateLayer(string layerName)
    {
        SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = manager.FindProperty("layers");

        // check if layer is present
        bool found = false;
        for (int i = 0; i <= 31; i++) {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);

            if (sp!=null && layerName.Equals(sp.stringValue)) {
                found = true;
                break;
            }
        }

        // not found, add into 1st open slot
        if (!found) {
            SerializedProperty slot = null;
            for (int i = 8; i <= 31; i++) {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (sp!=null && string.IsNullOrEmpty(sp.stringValue)) {
                    slot = sp;
                    break;
                }
            }

            if (slot!=null)
                slot.stringValue = layerName;
            else
                Debug.LogError("Could not find an open Layer Slot for: " + layerName);
        }


        // save
        manager.ApplyModifiedProperties();
    }

}
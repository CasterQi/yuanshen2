// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace FAE
{
    [CustomEditor(typeof(CliffAppearance))]
    public class CliffAppearanceInspector : Editor
    {
        CliffAppearance ca;
        private bool showHelp = false;
        private GameObject selection;

        new SerializedObject serializedObject;

        public SerializedProperty targetMaterials;
        public SerializedProperty objectColor;
        public SerializedProperty roughness;

        public SerializedProperty detailNormalMap;
        public SerializedProperty detailNormalStrength;

        public SerializedProperty globalColorMap;
        public SerializedProperty globalColor;
        public SerializedProperty globalTiling;

        public SerializedProperty useCoverageShader;
        public SerializedProperty coverageColorMap;
        public SerializedProperty coverageNormalMap;
        public SerializedProperty coverageAmount;
        public SerializedProperty coverageTiling;
        public SerializedProperty coverageMap;

#if UNITY_EDITOR
        void OnEnable()
        {
            selection = Selection.activeGameObject;
            if (selection)
            {
                ca = Selection.activeGameObject.GetComponent<CliffAppearance>();
            }

            serializedObject = new SerializedObject(ca);

            targetMaterials = serializedObject.FindProperty("targetMaterials");

            objectColor = serializedObject.FindProperty("objectColor");
            roughness = serializedObject.FindProperty("roughness");

            detailNormalMap = serializedObject.FindProperty("detailNormalMap");
            detailNormalStrength = serializedObject.FindProperty("detailNormalStrength");

            globalColorMap = serializedObject.FindProperty("globalColorMap");
            globalColor = serializedObject.FindProperty("globalColor");
            globalTiling = serializedObject.FindProperty("globalTiling");

            useCoverageShader = serializedObject.FindProperty("useCoverageShader");
            coverageColorMap = serializedObject.FindProperty("coverageColorMap");
            coverageNormalMap = serializedObject.FindProperty("coverageNormalMap");
            coverageAmount = serializedObject.FindProperty("coverageAmount");
            coverageTiling = serializedObject.FindProperty("coverageTiling");
            coverageMap = serializedObject.FindProperty("coverageMap");

        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(this, "Component");
            if (selection) Undo.RecordObject(selection, "CliffAppearance");

            if (ca.cliffShader == null)
            {
                EditorGUILayout.HelpBox("FAE/Cliff shader could not be found!", MessageType.Error);
            }
            if (ca.cliffCoverageShader == null)
            {
                EditorGUILayout.HelpBox("FAE/Cliff Coverage shader could not be found!", MessageType.Error);
            }

            serializedObject.Update();

            DrawFields();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed || EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selection);
                EditorUtility.SetDirty((CliffAppearance)target);
                EditorUtility.SetDirty(this);
                ca.Apply();
            }

        }

        private void DrawFields()
        {
            DoHeader();

            EditorGUILayout.PropertyField(targetMaterials, true);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Coverage", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(useCoverageShader, new GUIContent("Enable"));

            if (showHelp) EditorGUILayout.HelpBox("Covers the objects from the Y-axis", MessageType.Info);

            if (ca.useCoverageShader)
            {
                EditorGUILayout.HelpBox("Currently this feature requires you to have the PigmentMapGenerator script on your terrain", MessageType.Info);

                EditorGUILayout.PropertyField(coverageMap, new GUIContent("Coverage map"));

                if (showHelp) EditorGUILayout.HelpBox("This grayscale map represents the coverage amount on the terrain \n\nThe bottom left of the texture equals the pivot point of the terrain", MessageType.Info);

                EditorGUILayout.PropertyField(coverageColorMap, new GUIContent("Albedo"));
                EditorGUILayout.PropertyField(coverageNormalMap, new GUIContent("Normals"));

                coverageAmount.floatValue = EditorGUILayout.Slider("Amount", coverageAmount.floatValue, 0f, 1f);
                coverageTiling.floatValue = EditorGUILayout.Slider("Tiling", coverageTiling.floatValue, 0f, 20f);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Object", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            objectColor.colorValue = EditorGUILayout.ColorField("Color", objectColor.colorValue);
            roughness.floatValue = EditorGUILayout.Slider("Roughness", roughness.floatValue, 0f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            EditorGUILayout.LabelField("Detail", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            if (showHelp) EditorGUILayout.HelpBox("Normal details visible up close", MessageType.Info);


            EditorGUILayout.PropertyField(detailNormalMap, new GUIContent("Detail normal map"));

            detailNormalStrength.floatValue = EditorGUILayout.Slider("Normal strength", detailNormalStrength.floatValue, 0f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Global", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            if (showHelp) EditorGUILayout.HelpBox("A tri-planar projected color map which tiles across all the objects seamlessly", MessageType.Info);

            EditorGUILayout.PropertyField(globalColorMap, new GUIContent("Global color map"));

            globalColor.colorValue = EditorGUILayout.ColorField("Color", globalColor.colorValue);
            globalTiling.floatValue = EditorGUILayout.Slider("Tiling", globalTiling.floatValue, 0f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            GUIHelper.DrawFooter();
        }

        private void DoHeader()
        {

            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Cliff Appearance", GUIHelper.Header);
            EditorGUILayout.EndHorizontal();
            if (showHelp) EditorGUILayout.HelpBox("This script allows you to edit multiple materials that use the FAE/Cliff shader.", MessageType.Info);

        }


#endif
    }
}

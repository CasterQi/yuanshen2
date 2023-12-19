// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FAE
{
    public class SubstanceBakerGUI : ShaderGUI
    {
#if !UNITY_2018_1_OR_NEWER

        MaterialEditor m_MaterialEditor;
        Material material;

        bool showHelp;

        MaterialProperty _MainTex;
        MaterialProperty _BumpMap;
        MaterialProperty _ParallaxMap;
        MaterialProperty _Tessellation;

        private SubstanceBaker sb;
        private ProceduralMaterial substance;
        private string savedTargetFolder;
        private string targetFolder;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (!sb) sb = ScriptableObject.CreateInstance<SubstanceBaker>();

            this.m_MaterialEditor = materialEditor;
            this.substance = m_MaterialEditor.target as ProceduralMaterial;


            this.FindProperties(props);

            //Style similar to Standard shader
            m_MaterialEditor.SetDefaultGUIWidths();
            m_MaterialEditor.UseDefaultMargins();
            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();

            //Draw fields
            DoHeader();
            DrawFields();

            if (!substance || substance.GetType() != typeof(ProceduralMaterial))
            {
                EditorGUILayout.HelpBox("This is material is not a Substance material\n\nSubstances can be recognized by a small red flame in the icon", MessageType.Error);
            }
            else
            {

                //Retreive the last used folder in the registry
                savedTargetFolder = EditorPrefs.GetString("FAE_S2T_TARGETFOLDER");
                if (targetFolder == null) targetFolder = savedTargetFolder;
                if (targetFolder == null) targetFolder = "Assets/Fantasy Adventure Environment/Terrain/Textures";

                EditorGUILayout.LabelField("Target folder: ", EditorStyles.boldLabel);
                targetFolder = EditorGUILayout.TextField(targetFolder);

                if (targetFolder == null) EditorGUILayout.HelpBox("This field cannot be empty", MessageType.Error);

                if (showHelp) EditorGUILayout.HelpBox("A separate folder will be created with the name of the current Substance material\n\nExample: /" + substance.name + "/texture_name.png", MessageType.Info);

                if (GUILayout.Button("Bake textures") && targetFolder != null)
                {
                    sb.BakeSubstance(substance, targetFolder);
                }
            }

            GUIHelper.DrawFooter();
        }

        private void DrawFields()
        {
            this.m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), this._MainTex);
            this.m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Normals"), this._BumpMap);
            this.m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Height"), this._ParallaxMap);

            this.m_MaterialEditor.ShaderProperty(_Tessellation, _Tessellation.displayName);

            EditorGUILayout.Space();
        }

        public void FindProperties(MaterialProperty[] props)
        {

            this._MainTex = ShaderGUI.FindProperty("_MainTex", props);
            this._BumpMap = ShaderGUI.FindProperty("_BumpMap", props);
            this._ParallaxMap = ShaderGUI.FindProperty("_ParallaxMap", props);

            this._Tessellation = ShaderGUI.FindProperty("_Tessellation", props);
        }

        void DoHeader()
        {
            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Substance baking tool", GUIHelper.Header);
            EditorGUILayout.EndHorizontal();

            if (showHelp) EditorGUILayout.HelpBox("This GUI allows you to bake the Substance material outputs to PNG files, in order to use them as terrain textures", MessageType.Info);

            if (showHelp) EditorGUILayout.HelpBox("If the output textures are black, you must set the \"Format\" to \"RAW\"\n\nYou can find this option at the far bottom of the inspector", MessageType.Warning);

            EditorGUILayout.Space();
        }

#else
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            EditorGUILayout.HelpBox("This functionality is not supported in Unity 2018.1 and newer", MessageType.Error);
        }
#endif
    }
}
// Fantasy Adventure Environment
// Staggart Creations
// http://staggart.xyz

using UnityEngine;
using System.IO;

//Make this entire class is editor-only without requiring it to be in an "Editor" folder
#if UNITY_EDITOR
using UnityEditor;

namespace FAE
{
    public class FAE_Core : Editor
    {
        public const string ASSET_NAME = "Fantasy Adventure Environment";
        public const string ASSET_ABRV = "FAE";
        public const string ASSET_ID = "70354";

        public const string PACKAGE_VERSION = "20171";
        public static string INSTALLED_VERSION = "1.3.1";
        public const string MIN_UNITY_VERSION = "2017.1";

        public static string DOC_URL = "http://staggart.xyz/unity/fantasy-adventure-environment/fae-documentation/";
        public static string FORUM_URL = "https://forum.unity3d.com/threads/486102";

        public static void OpenStorePage()
        {
            Application.OpenURL("com.unity3d.kharma:content/" + ASSET_ID);
        }

#if UNITY_2018_1_OR_NEWER
        public enum MaterialInstallation
        {
            Regular,
            Substance
        }
        public static MaterialInstallation materialInstallation;

        public static bool SubstanceInstalled()
        {
            string[] assets = AssetDatabase.FindAssets("libsubstance_sse2_blend");

            return (assets.Length > 0) ? true : false;
        }

        public static void InstallMaterials(MaterialInstallation type)
        {
            if (type == MaterialInstallation.Regular)
            {
                InstallRegularMaterials();
            }
            else
            {
                InstallSubstance();
            }
        }

        public static void InstallRegularMaterials()
        {
            string[] asset = AssetDatabase.FindAssets("RegularMaterials");

            if (asset.Length > 0)
            {
                if (EditorUtility.DisplayDialog("Warning", "All your vegetation material settings will be reverted to default", "Continue", "Cancel"))
                {
                    FileUtil.DeleteFileOrDirectory(SessionState.GetString("PATH", "") + "/Source/Substances");
                    FileUtil.DeleteFileOrDirectory(SessionState.GetString("PATH", "") + "/Terrain/Substance");

                    AssetDatabase.Refresh();

                    asset[0] = AssetDatabase.GUIDToAssetPath(asset[0]);
                    AssetDatabase.ImportPackage(asset[0], false);
                }
            }
            else
            {
                Debug.LogError("[FAE] Package for regular materials could not be found! Be sure to import the complete package.");
            }
        }

        public static void InstallSubstance()
        {
            string[] asset = AssetDatabase.FindAssets("SubstanceMaterials");

            if (asset.Length > 0)
            {
                if (EditorUtility.DisplayDialog("Warning", "All your vegetation material settings will be reverted to default\n\nPlease note that Substance materials are to be removed in v2 of this package.", "Continue", "Cancel"))
                {
                    FileUtil.DeleteFileOrDirectory(SessionState.GetString("PATH", "") + "/Source/Textures");

                    AssetDatabase.Refresh();

                    asset[0] = AssetDatabase.GUIDToAssetPath(asset[0]);
                    AssetDatabase.ImportPackage(asset[0], false);
                }
            }
            else
            {
                Debug.LogError("[FAE] Package for Substance materials could not be found! Be sure to import the complete package.");
            }
        }
#endif
    }

    public class FAE_Window : EditorWindow
    {
        //Window properties
        private static int width = 440;
        private static int height = 300;

        //Tabs
#if UNITY_2018_1_OR_NEWER
        private bool isTabInstallation = true;
        private bool isTabGettingStarted = false;
#else
        private bool isTabGettingStarted = true;
#endif
        private bool isTabSupport = false;


        [MenuItem("Help/Fantasy Adventure Environment", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow<FAE_Window>(false, "About", true);
            editorWindow.titleContent = new GUIContent("Help " + FAE_Core.INSTALLED_VERSION);
            editorWindow.autoRepaintOnSceneChange = true;

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.width) / 2f, (Screen.height) / 2f, width, height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, 200);

            Init();

            editorWindow.Show();

        }

        private void SetWindowHeight(float height)
        {
            this.maxSize = new Vector2(width, height);
            this.minSize = new Vector2(width, height);
        }

        //Store values in the volatile SessionState
        static void Init()
        {
            GetRootFolder();

            //Check Substance installation
#if UNITY_2018_1_OR_NEWER
            SessionState.SetBool("SUBSTANCE_INSTALLED", FAE_Core.SubstanceInstalled());
#endif
        }

        public static void GetRootFolder()
        {
            //Get script path
            string[] scriptGUID = AssetDatabase.FindAssets("FAE_CORE t:script");
            string scriptFilePath = AssetDatabase.GUIDToAssetPath(scriptGUID[0]);

            //Truncate to get relative path
            string PACKAGE_ROOT_FOLDER = scriptFilePath.Replace("Scripts/Editor/FAE_Core.cs", string.Empty);

            SessionState.SetString("PATH", PACKAGE_ROOT_FOLDER);
        }

        void OnGUI()
        {

            DrawHeader();

            GUILayout.Space(5);
            DrawTabs();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

#if UNITY_2018_1_OR_NEWER
            if (isTabInstallation) DrawInstallation();
#endif

            if (isTabGettingStarted) DrawGettingStarted();

            if (isTabSupport) DrawSupport();

            //DrawActionButtons();

            EditorGUILayout.EndVertical();

            DrawFooter();

        }

        void DrawHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b><size=24>Fantasy Adventure Environment</size></b>", Header);

            GUILayout.Label("Version: " + FAE_Core.INSTALLED_VERSION, Footer);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

#if UNITY_2018_1_OR_NEWER

            if (GUILayout.Toggle(isTabInstallation, "Installation", Tab))
            {
                isTabInstallation = true;
                isTabGettingStarted = false;
                isTabSupport = false;
            }
#endif

            if (GUILayout.Toggle(isTabGettingStarted, "Getting started", Tab))
            {
#if UNITY_2018_1_OR_NEWER
                isTabInstallation = false;
#endif
                isTabGettingStarted = true;
                isTabSupport = false;
            }

            if (GUILayout.Toggle(isTabSupport, "Support", Tab))
            {
#if UNITY_2018_1_OR_NEWER
                isTabInstallation = false;
#endif
                isTabGettingStarted = false;
                isTabSupport = true;
            }

            EditorGUILayout.EndHorizontal();
        }

#if UNITY_2018_1_OR_NEWER
        void DrawInstallation()
        {

            //Compiling
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent(" Compiling scripts...", EditorGUIUtility.FindTexture("cs Script Icon")), Header);

                EditorGUILayout.Space();
                return;
            }

            if (SessionState.GetBool("SUBSTANCE_INSTALLED", true)) { SetWindowHeight(400f); }
            else { SetWindowHeight(400f); }

            //Substance
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Substance support:");


            Color defaultColor = GUI.contentColor;

            if (SessionState.GetBool("SUBSTANCE_INSTALLED", true))
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Plugin Installed");
                GUI.contentColor = defaultColor;
            }
            else
            {
                GUI.contentColor = Color.yellow;
                EditorGUILayout.LabelField("Plugin not installed", EditorStyles.boldLabel);
                GUI.contentColor = defaultColor;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("In version 2.0.0 of this package Substances are to be removed.", MessageType.Error);

            //Substance not installed, display instructions
            if (!SessionState.GetBool("SUBSTANCE_INSTALLED", true))
            {
                EditorGUILayout.HelpBox("In order to install Substance materials, the \"Substance in Unity\" plugin must be installed", MessageType.Info);
                EditorGUILayout.Space();

                if (GUILayout.Button("<b><size=16>Install free plugin</size></b>\n<i>Opens Asset Store page</i>", Button))
                {
                    Application.OpenURL("com.unity3d.kharma:content/110555");

                    this.Close();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Once installed, re-open this window to install the materials");

                return;
            }
            else
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("The \"Substance in Unity\" plugin is installed.\n\nYou can choose to convert all materials to use Substance textures and enable more customization options.", MessageType.Info);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Choose material installation:", EditorStyles.boldLabel);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b><size=14>Regular</size></b>\n<i>Non-customizable Unity textures</i>", Button))
            {
                FAE_Core.InstallMaterials(FAE_Core.MaterialInstallation.Regular);
            }
            if (GUILayout.Button("<b><size=14>Substance</size></b>\n<i>Customizable procedural textures</i>", Button))
            {
                FAE_Core.InstallMaterials(FAE_Core.MaterialInstallation.Substance);
            }

            EditorGUILayout.EndHorizontal();

        }
#endif
        void DrawGettingStarted()
        {
            SetWindowHeight(335);

            EditorGUILayout.HelpBox("Please view the documentation for further details about this package and its workings.", MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button("<b><size=16>Online documentation</size></b>\n<i>Set up, best practices and troubleshooting</i>", Button))
            {
                Application.OpenURL(FAE_Core.DOC_URL + "#getting-started-3");
            }

        }

        void DrawSupport()
        {
            SetWindowHeight(350f);

            EditorGUILayout.BeginVertical(); //Support box

            EditorGUILayout.HelpBox("If you have any questions, or ran into issues, please get in touch!", MessageType.Info);

            EditorGUILayout.Space();

            //Buttons box
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<b><size=12>Email</size></b>\n<i>Contact</i>", Button))
            {
                Application.OpenURL("mailto:contact@staggart.xyz");
            }
            if (GUILayout.Button("<b><size=12>Twitter</size></b>\n<i>Follow developments</i>", Button))
            {
                Application.OpenURL("https://twitter.com/search?q=staggart%20creations");
            }
            if (GUILayout.Button("<b><size=12>Forum</size></b>\n<i>Join the discussion</i>", Button))
            {
                Application.OpenURL(FAE_Core.FORUM_URL);
            }
            EditorGUILayout.EndHorizontal();//Buttons box

            EditorGUILayout.EndVertical(); //Support box
        }

        //TODO: Implement after Beta
        private void DrawActionButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("<size=12>Rate</size>", Button))
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/account/downloads/search=");

            if (GUILayout.Button("<size=12>Review</size>", Button))
                Application.OpenURL("");


            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DrawFooter()
        {
            //EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.Label("- Staggart Creations -", Footer);
        }

        #region Styles

        private static GUIStyle _Footer;
        public static GUIStyle Footer
        {
            get
            {
                if (_Footer == null)
                {
                    _Footer = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _Footer;
            }
        }

        private static GUIStyle _Button;
        public static GUIStyle Button
        {
            get
            {
                if (_Button == null)
                {
                    _Button = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Button;
            }
        }

        private static GUIStyle _Header;
        public static GUIStyle Header
        {
            get
            {
                if (_Header == null)
                {
                    _Header = new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 18,
                        fontStyle = FontStyle.Bold
                    };
                }

                return _Header;
            }
        }

        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }
                return _HelpIcon;
            }
        }


        private static GUIStyle _Tab;
        public static GUIStyle Tab
        {
            get
            {
                if (_Tab == null)
                {
                    _Tab = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Tab;
            }
        }

        #endregion //Stylies
    }//Window Class
}//namespace
#endif //If Unity Editor
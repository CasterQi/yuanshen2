// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
namespace FAE
{
    public class SubstanceBaker : ScriptableObject
    {
#if !UNITY_2018_1_OR_NEWER
        private SubstanceImporter si;
        private Texture[] substanceOutputs;

        private ProceduralMaterial substance;
        private string targetFolder;

        /// <summary>
        /// Bake the Substance outputs to PNG textures in the target folder
        /// A separate folder will be created using the material name
        /// </summary>
        public void BakeSubstance(ProceduralMaterial substance, string targetFolder)
        {
            this.substance = substance;
            this.targetFolder = targetFolder;

            SetupFolder();

            //Substance .sbsar container
            string substancePath = AssetDatabase.GetAssetPath(substance.GetInstanceID());
            si = AssetImporter.GetAtPath(substancePath) as SubstanceImporter;

            //Make readable
            substance.isReadable = true;

            ConfigureSubstance(si);
            AssetDatabase.Refresh();
            ConfigureSubstance(si);

            //Generate textures
            substance.RebuildTexturesImmediately();

            substanceOutputs = substance.GetGeneratedTextures();

            foreach (Texture texture in substanceOutputs)
            {
                {
                    ConvertProceduralTexture(texture);
                }
            }

            AssetDatabase.Refresh();
        }

        private void SetupFolder()
        {
            if (!Directory.Exists(targetFolder + "/" + substance.name + "/"))
            {
                Directory.CreateDirectory(targetFolder + "/" + substance.name + "/");

                AssetDatabase.Refresh();
            }

            //Store the last used folder in the registry
            EditorPrefs.SetString("FAE_S2T_TARGETFOLDER", targetFolder);
        }


        private void ConfigureSubstance(SubstanceImporter si)
        {
            //Set the format mode to RAW
            //Fails to work for an unknown reason, still looking into it!
            int maxTextureWidth;
            int maxTextureHeight;
            int textureFormat;
            int loadBehavior;

            si.GetPlatformTextureSettings(substance.name, "Standalone", out maxTextureWidth, out maxTextureHeight, out textureFormat, out loadBehavior);
            si.SetPlatformTextureSettings(substance, "Standalone", maxTextureWidth, maxTextureHeight, 1, loadBehavior);

            //Also generate heightmap and roughness
            si.SetGenerateAllOutputs(substance, true);
        }

        private void ConvertProceduralTexture(Texture sourceTex)
        {
            //Debug.Log("Converting " + sourceTex.name);

            ProceduralTexture sourceTexture = (Texture)sourceTex as ProceduralTexture;

            Color32[] pixels = sourceTexture.GetPixels32(0, 0, sourceTex.width, sourceTex.height);

            Texture2D destTex = new Texture2D(sourceTexture.width, sourceTexture.height)
            {
                //Copy options from substance texture
                name = sourceTexture.name,
                anisoLevel = sourceTexture.anisoLevel,
                filterMode = sourceTexture.filterMode,
                mipMapBias = sourceTexture.mipMapBias,
                wrapMode = sourceTexture.wrapMode
            };

            destTex.SetPixels32(pixels);

            //Convert normal map to Unity format
            if (sourceTex.name.Contains("_normal"))
            {
                Color targetColor = new Color();
                for (int x = 0; x < sourceTex.width; x++)
                {
                    for (int y = 0; y < sourceTex.height; y++)
                    {
                        //Red is stored in the alpha component
                        targetColor.r = destTex.GetPixel(x, y).a;
                        //Green channel, already inverted in Substance Designer
                        targetColor.g = destTex.GetPixel(x, y).g;
                        //Invert blue channel
                        targetColor.b = 1 - destTex.GetPixel(x, y).b;
                        destTex.SetPixel(x, y, targetColor);
                    }
                }

            }

            destTex.Apply();

            string path = targetFolder + "/" + substance.name + "/" + destTex.name + ".png";
            File.WriteAllBytes(path, destTex.EncodeToPNG());

            //Refresh the database so the NormalMapImporter runs
            if (sourceTex.name.Contains("_normal")) AssetDatabase.Refresh();

            //Debug.Log("Written file to: " + path);
        }
    }

    //Catch the normal map when it is being imported and flag it accordingly
    internal sealed class NormalMapImporter : AssetPostprocessor
    {

        TextureImporter textureImporter;

        private void OnPreprocessTexture()
        {
            //Look for the given name, this will also apply to textures outside of the FAE package, but this behaviour is desirable anyway
            if (!assetPath.Contains("_normal")) return;

            textureImporter = assetImporter as TextureImporter;

#if UNITY_5_5_OR_NEWER

            textureImporter.textureType = TextureImporterType.NormalMap;
#else
            textureImporter.normalmap = true;
#endif

        }

#endif
    }
}
#endif
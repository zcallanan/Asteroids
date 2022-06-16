#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Utils.SpriteMaker
{
    [ExecuteInEditMode]
    public class SpriteMaker : MonoBehaviour
    {
        [SerializeField] private  bool createSprite;
        [SerializeField] private  RenderTexture renTex;
        [SerializeField] private  Camera bakeCam;
        [SerializeField] private  string spriteName;

        private void Update()
        {
            if (createSprite)
            {
                createSprite = false;
                CreateSprite();
            }
        }

        private void CreateSprite()
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                spriteName = "unnamed sprite";
            }

            var path = SaveLocation();
            path += spriteName;
            
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = renTex;

            var texture = bakeCam.targetTexture;
            var imgPng = new Texture2D(texture.width, texture.height,
                TextureFormat.ARGB32, false);
            imgPng.ReadPixels(new Rect(0,0,texture.width, texture.height),0,0);
            imgPng.Apply();
            RenderTexture.active = currentActiveRT;
            var bytesPng = imgPng.EncodeToPNG();
            System.IO.File.WriteAllBytes($"{path}.png", bytesPng);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            Debug.Log($"<color=white>{spriteName} created</color>");
        }

        private static string SaveLocation()
        {
            var saveLocation = $"{Application.dataPath}/Textures/Sprites/";
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }

            return saveLocation;
        }
    }
}
#endif

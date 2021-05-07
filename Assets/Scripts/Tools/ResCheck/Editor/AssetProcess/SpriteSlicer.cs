using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpriteSlicer
{
    public static void ProcessToSprite()
    {
        var image = Selection.activeObject as Texture2D;
        if (image == null)
        {
            EditorUtility.DisplayDialog("Error", "请选择有效的图片", "OK");
            return;
        }
        
        var rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image));
        var path = rootPath + "/" + image.name + ".PNG";

        var texImp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (texImp != null && texImp.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorUtility.DisplayDialog("Error", "当前图片的Sprite Mode不是Multiple, 无法切分", "OK");
            return;
        }
        if (texImp != null && texImp.isReadable == false)
        {
            if (EditorUtility.DisplayDialog("Warning", "当前选中的图片时不可读的, 是否更改?", "OK", "Cancel"))
            {
                texImp.isReadable = true;
                texImp.SaveAndReimport();
            }
            else
            {
                return;
            }
        }
        AssetDatabase.CreateFolder(rootPath, image.name);

        //遍历图集  
        if (texImp == null) return;
        foreach (var metaData in texImp.spritesheet)
        {
            var myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

            for (var y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) 
            {
                for (var x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                    myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
            }

            //转换纹理到EncodeToPNG兼容格式  
            if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
            {
                var newTexture = new Texture2D(myimage.width, myimage.height);
                newTexture.SetPixels(myimage.GetPixels(0), 0);
                myimage = newTexture;
            }
            var pngData = myimage.EncodeToPNG();

            File.WriteAllBytes(rootPath + "/" + image.name + "/" + metaData.name + ".PNG", pngData);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Texture/Test Corp")]
    private static void TestCorp()
    {
        var obj = Selection.activeObject;
        if (obj)
        {
            CorpTexture(AssetDatabase.GetAssetPath(obj), new RectInt(0, 0, 100, 100), new Vector2Int(64, 64), "Assets/test.png");
            AssetDatabase.Refresh();
        }
    }

    public static void CorpTexture(string source, RectInt corp, Vector2Int size, string savePath)
    {
        var stex = AssetDatabase.LoadAssetAtPath<Texture2D>(source);
        var simp = AssetImporter.GetAtPath(source) as TextureImporter;
        if (stex == null || simp == null)
        {
            return;
        }
        
        var sReadable = simp.isReadable;
        if (!sReadable)
        {
            simp.isReadable = true;
            simp.SaveAndReimport();
        }
        
        var tex = new Texture2D(corp.width, corp.height);

        var corped = stex.GetPixels(corp.x, corp.y, corp.width, corp.height);
        tex.SetPixels(corped);

        tex = ScaleTexture(tex, size);

        var lSavePath = savePath.ToLower();
        byte[] data;
        if (lSavePath.EndsWith(".jpg"))
        {
            data = tex.EncodeToJPG();
        }
        else if (lSavePath.EndsWith(".png"))
        {
            data = tex.EncodeToPNG();
        }
        else
        {
            Debug.LogError($"unsupported file format: {savePath}");
            return;
        }
        
        File.WriteAllBytes(savePath, data);
        Debug.Log($"generate: {savePath}");

        if (!sReadable)
        {
            simp.isReadable = false;
            simp.SaveAndReimport();
        }
    }
    
    private static Texture2D ScaleTexture(Texture2D source, Vector2Int size)
    {
        var result = new Texture2D(size.x, size.y, source.format, false);
        
        for (var i = 0; i < result.height; ++i)
        {
            for (var j = 0; j < result.width; ++j)
            {
                var newColor = source.GetPixelBilinear(j / (float)result.width, i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }
    
    public enum ImageFilterMode
    {
        Nearest = 0,
        Biliner = 1,
        Average = 2
    }
    
    public static Texture2D ScaleTexture(Texture2D source, ImageFilterMode mode, Vector2Int size)
    {
        var sourceColor = source.GetPixels(0);
        var sourceSize = new Vector2(source.width, source.height);

        var tex = new Texture2D(size.x, size.y, source.format, false);
        
        var length = size.x * size.y;
        var colors = new Color[length];
     
        var pixelSize = new Vector2(sourceSize.x / size.x, sourceSize.y / size.y);
        var center = new Vector2();
        
        for (var i = 0; i < length; i++)
        {
            var x = i % size.x;
            var y = i / size.x;
            center.x = x / (float)size.x * sourceSize.x;
            center.y = y / (float)size.y * sourceSize.y;
            
            switch (mode)
            {
                case ImageFilterMode.Nearest:
                    //*** Calculate source index
                    var sourceIndex = (int)(Mathf.Round(center.y) * sourceSize.x + Mathf.Round(center.x));
                    colors[i] = sourceColor[sourceIndex];
                    break;
                case ImageFilterMode.Biliner:
                    //*** Get Ratios
                    var ratioX = center.x - Mathf.Floor(center.x);
                    var ratioY = center.y - Mathf.Floor(center.y);
     
                    //*** Get Pixel index's
                    var indexTl = (int)(Mathf.Floor(center.y) * sourceSize.x + Mathf.Floor(center.x));
                    var indexTr = (int)(Mathf.Floor(center.y) * sourceSize.x + Mathf.Ceil(center.x));
                    var indexBl = (int)(Mathf.Ceil(center.y) * sourceSize.x + Mathf.Floor(center.x));
                    var indexBr = (int)(Mathf.Ceil(center.y) * sourceSize.x + Mathf.Ceil(center.x));
     
                    //*** Calculate Color
                    colors[i] = Color.Lerp(
                        Color.Lerp(sourceColor[indexTl], sourceColor[indexTr], ratioX),
                        Color.Lerp(sourceColor[indexBl], sourceColor[indexBr], ratioX),
                        ratioY
                    );
                    break;
                case ImageFilterMode.Average:
                    //*** Calculate grid around point
                    var xFrom = Mathf.Max(Mathf.FloorToInt(center.x - pixelSize.x * 0.5f), 0);
                    var xTo = (int)Mathf.Min(Mathf.CeilToInt(center.x + pixelSize.x * 0.5f), sourceSize.x);
                    var yFrom = Mathf.Max(Mathf.FloorToInt(center.y - pixelSize.y * 0.5f), 0);
                    var yTo = (int)Mathf.Min(Mathf.Ceil(center.y + pixelSize.y * 0.5f), sourceSize.y);
     
                    //*** Loop and accumulate
                    var colorTemp = new Color();
                    float gridCount = 0;
                    for(var iy = yFrom; iy < yTo; iy++)
                    {
                        for(var ix = xFrom; ix < xTo; ix++)
                        {
                            //*** Get Color
                            colorTemp += sourceColor[(int)(iy * sourceSize.x + ix)];
                            //*** Sum
                            gridCount++;
                        }
                    }
                    //*** Average Color
                    colors[i] = colorTemp / gridCount;
                    break;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return tex;
    }
    

}
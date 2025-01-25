using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class AssetHandling : MonoBehaviour
{

    #region Character sprites
    [MenuItem("AssetImport/Import Character Sprites")]
    public static void ImportCharacterSprites()
    {
        string direct = "Assets/Resources/Entities";
        var files = Directory.GetFiles(direct)
                .Where(x => new string[] { ".png", ".jpg" }.Contains(Path.GetExtension(x))).ToList();

        foreach (var file in files)
        {
            GenerateAnimations(file);
        }
    }

    private static void GenerateAnimations(string file)
    {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(file).Where(z => z is Sprite).Cast<Sprite>().ToList();

        var name = Path.GetFileNameWithoutExtension(file);

        var asset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();

        for (int idx = 0; idx < 32; idx++)
        {
            Sprite[] toanimate = new Sprite[13];
            int instance = idx * 13;

            for (int jdx = 0; jdx < 13; jdx++)
            {

                toanimate[jdx] = sprites[instance + jdx];
            }
            DefineAnimation(toanimate, asset, idx);
        }



        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath("Assets/SpriteLibraries/" + $"{name}Library.asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(asset);
    }

    static void DefineAnimation(Sprite[] sprites, SpriteLibraryAsset spriteLibrary, int row)
    {
        string name;
        switch (row)
        {
            case 0:
                name = "spellup";
                break;
            case 1:
                name = "spellleft";
                break;
            case 2:
                name = "spelldown";
                break;
            case 3:
                name = "spellright";
                break;
            case 4:
                name = "thrustup";
                break;
            case 5:
                name = "thrustleft";
                break;
            case 6:
                name = "thrustdown";
                break;
            case 7:
                name = "thrustright";
                break;
            case 8:
                name = "walkup";
                break;
            case 9:
                name = "walkleft";
                break;
            case 10:
                name = "walkdown";
                break;
            case 11:
                name = "walkright";
                break;
            case 12:
                name = "slashup";
                break;
            case 13:
                name = "slashleft";
                break;
            case 14:
                name = "slashdown";
                break;
            case 15:
                name = "slashright";
                break;
            case 16:
                name = "bowup";
                break;
            case 17:
                name = "bowleft";
                break;
            case 18:
                name = "bowdown";
                break;
            case 19:
                name = "bowright";
                break;
            case 20:
                name = "death";
                break;
            default:
                name = $"{row}";
                break;
        }

        int idx = 0;
        foreach (Sprite sprite in sprites)
        {
            spriteLibrary.AddCategoryLabel(sprite, name, $"{name}_{idx}");
            idx++;
        }

    }

    #endregion

    #region Tilemap

    [MenuItem("AssetImport/Import All Tiles")]
    public static void ImportTiles()
    {
        string direct = "Assets/Resources/Tilemaps";
        var files = Directory.GetFiles(direct)
                .Where(x => new string[] { ".png", ".jpg" }.Contains(Path.GetExtension(x))).ToList();
        foreach (var file in files)
        {
            ImportTile(file);
        }
    }


    [MenuItem("AssetImport/Import Selected Tiles")]
    public static void ImportSelectedTiles()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                ImportTile(AssetDatabase.GetAssetPath(obj)); 
            }
        }
    }


    static void ImportTile(string file)
    {

        var name = Path.GetFileNameWithoutExtension(file);


        TextureImporter ti = AssetImporter.GetAtPath(file) as TextureImporter;
        ti.isReadable = true;
        ti.spritePixelsPerUnit = 32;
        ti.filterMode = FilterMode.Point;
        ti.spriteImportMode = SpriteImportMode.Multiple;

        AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);


        var myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(file);


        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(myTexture);
        dataProvider.InitSpriteEditorDataProvider();

        /* Use the data provider */


        int SliceWidth = 32;
        int SliceHeight = 32;
            
        int idx = 0; 

        var spriteRects = new List<SpriteRect>();
        for (int i = 0; i < myTexture.width; i += SliceWidth)
        {
            int jdx = 0;

            for (int j = 0; j < myTexture.height; j += SliceHeight)
            {
                var newSprite = new SpriteRect()
                {
                    name = $"{name}_{idx}_{jdx}",
                    spriteID = GUID.Generate(),
                    rect = new Rect(i, j, SliceWidth, SliceHeight)
                };
                spriteRects.Add(newSprite);
                // Note: This section is only for Unity 2021.2 and newer
                // Register the new Sprite Rect's name and GUID with the ISpriteNameFileIdDataProvider
                var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                var nameFileIdPairs = spriteNameFileIdDataProvider.GetNameFileIdPairs().ToList();
                nameFileIdPairs.Add(new SpriteNameFileIdPair(newSprite.name, newSprite.spriteID));
                spriteNameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
                // End of Unity 2021.2 and newer section
                jdx++; 
            }
            idx++;
        }

        // Write the updated data back to the data provider
        dataProvider.SetSpriteRects(spriteRects.ToArray());





        // Apply the changes made to the data provider
        dataProvider.Apply();

        // Reimport the asset to have the changes applied
        var assetImporter = dataProvider.targetObject as AssetImporter;
        assetImporter.SaveAndReimport();

    }



    #endregion
}

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class AssetHandling : MonoBehaviour
{


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
            Sprite[] toanimate = new Sprite[12];
            int instance = idx * 12; 
            for (int jdx = 0; jdx < 12; jdx++)
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
}

using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RhythmRift.Enemies;
using RhythmRift;

namespace ShadowMod;

public class ShadowDefinition
{
    public ShadowDefinition( int lower, int higher, Sprite spr ){
        from = lower/192.0f;
        to = higher/192.0f;
        sprite = spr;
    }
    public float from = 0.0f;
    public float to = 0.0f;
    public Sprite sprite;
}

[BepInPlugin("main.rotn.plugins.shadow_mod", "Shadow Mod", "1.0.0.0")]
public class ShadowPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    static string shadows_path;

    public static Texture2D load_texture_file( string path ){
        if ( !File.Exists( path ) ){
            return null;
        }
        Texture2D ret = new Texture2D(1, 1);
        ret.LoadImage( System.IO.File.ReadAllBytes(path), true );
        return ret;
    }

    public static Texture2D load_shadow( string filename ){
        var path = System.IO.Path.Join(shadows_path, filename); 
        return load_texture_file( path );
    }

    public static List<ShadowDefinition> shadows = new List<ShadowDefinition>();

    [HarmonyPatch(typeof(RRStageController), "BeginPlay")]
    [HarmonyPrefix]
    public static bool ReloadShadows(){

        shadows.Clear();

        DirectoryInfo d = new DirectoryInfo( shadows_path );
        string[] files = Directory.GetFiles( shadows_path, "*.png", SearchOption.TopDirectoryOnly );

        foreach( string file in files ){
            int i = 0;
            var str = Path.GetFileNameWithoutExtension( file );
            if( int.TryParse( str, out i ) ){
                var from = i - 1;
                var to = i + 1;
                if( i == -1 ){
                    from = 0;
                    to = 192;
                }
                var texture = load_texture_file( file );
                var sprite = Sprite.Create( texture, new Rect(0,0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50 );
                if( from < 0 ){
                    shadows.Add( new ShadowDefinition( from + 192, to + 192, sprite )  );
                }
                shadows.Add( new ShadowDefinition( from, to, sprite )  );
            }
        }

        foreach( ShadowDefinition shadow in shadows ){
            Logger.LogInfo( $"{shadow.from} -> {shadow.to}" );
        }

        return true;
    }

    [HarmonyPatch(typeof(RREnemy), "UpdateMovement")]
    [HarmonyPrefix]
    public static void UpdateEnemyShadow( ref RREnemy __instance, ref SpriteRenderer ____monsterShadow ){
        if (__instance.IsDying) return;
        if (!(bool)____monsterShadow) return;
        var next_beat = __instance.NextActionRowTrueBeatNumber % 1f;
        foreach( ShadowDefinition shadow in shadows ){
            if( (next_beat > shadow.from) && (next_beat < shadow.to) ){
                ____monsterShadow.sprite = shadow.sprite;
            }
        }
        return;
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(ShadowPlugin));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        shadows_path = System.IO.Path.Join( Application.persistentDataPath, "CustomShadows" );

        Shared.BugSplatAccessor.Instance.BugSplat.ShouldPostException = ex => false;
    }

}

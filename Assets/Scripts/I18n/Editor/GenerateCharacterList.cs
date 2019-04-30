
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEditor;

public sealed class CharacterSetExporter
{
    [MenuItem("Tools/Generate Character Set", false, 101)]
    public static void ExportCharacterSet()
    {
        Type[] translations = new Type[]{
                typeof(EnglishLanguage),
            };

        var chars = new HashSet<char>();
        chars.Add('0');
        chars.Add('1');
        chars.Add('2');
        chars.Add('3');
        chars.Add('4');
        chars.Add('5');
        chars.Add('6');
        chars.Add('7');
        chars.Add('8');
        chars.Add('9');
        chars.Add(',');
        chars.Add('.');
        chars.Add(':');
        chars.Add(';');
        chars.Add('\'');
        chars.Add('_');
        chars.Add('+');
        chars.Add('-');
        chars.Add('%');
        chars.Add('?');
        chars.Add(' ');
        chars.Add('\u2026'); // Ellipsis

        foreach (var t in translations) {
            const BindingFlags flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
            var language = (ILanguage)Activator.CreateInstance(t);
            foreach (var prop in typeof(ILanguage).GetProperties(flags)) {
                if (prop.Name == "Id" || prop.Name == "CultureInfo")
                    continue;
                var value = prop.GetValue(language).ToString();
                foreach (var ch in value)
                    chars.Add(ch);
            }
        }

        foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(InventoryItemSpec).Name}")) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var spec = AssetDatabase.LoadAssetAtPath<InventoryItemSpec>(path);
            foreach (var ch in spec.NameEN)
                chars.Add(ch);
            foreach (var ch in spec.NameRU)
                chars.Add(ch);
            foreach (var ch in spec.NameES)
                chars.Add(ch);
        }

        chars.Remove('\n');

        List<char> charList = chars.ToList();
        charList.Sort();

        StringBuilder builder = new StringBuilder();
        foreach (var ch in charList)
            builder.Append(ch);

        byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
        File.WriteAllBytes("Assets/Fonts/CharacterList.txt", bytes);

        AssetDatabase.Refresh();
    }
}

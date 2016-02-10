using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Collectible)), CanEditMultipleObjects]
public class CollectibleEditor : Editor
{
    private static readonly string[] _filterAll = new string[] { "m_KeyType", "efficiencyPercentage", "weaponType" };
    private static readonly string[] _filterKey = new string[] { "m_KeyType" };
    private static readonly string[] _filterEfficiency = new string[] { "efficiencyPercentage", "weaponType" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Collectible col = target as Collectible;
        if (col.getType() == Collectible.Type.Weapon)
        {
            DrawPropertiesExcluding(serializedObject, _filterKey);
            if (col.GetComponent<CollectibleWeapon>() == null)
                col.gameObject.AddComponent<CollectibleWeapon>();
        }
        else
        {
            if (col.getType() == Collectible.Type.Key)
                DrawPropertiesExcluding(serializedObject, _filterEfficiency);
            else
                DrawPropertiesExcluding(serializedObject, _filterAll);
            if (col.GetComponent<CollectibleWeapon>() == null)
                DestroyImmediate(col.GetComponent<CollectibleWeapon>());
        }

        serializedObject.ApplyModifiedProperties();
    }
}

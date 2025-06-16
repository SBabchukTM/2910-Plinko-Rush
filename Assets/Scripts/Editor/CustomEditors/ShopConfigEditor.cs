using Runtime.Application.ShopSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.CustomEditors
{
    [CustomEditor(typeof(ShopSetup))]
    public class ShopConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var shopConfig = (ShopSetup)target;

            if (GUILayout.Button("Assign Item IDs based on list index."))
                AssignUniqueItemIDs(shopConfig);
        }

        private void AssignUniqueItemIDs(ShopSetup shopSetup)
        {
            if (shopSetup.ShopItems == null || shopSetup.ShopItems.Count == 0)
                return;

            var id = 0;

            foreach (var item in shopSetup.ShopItems)
            {
                var serializedItem = new SerializedObject(item);
                var itemIdProperty = serializedItem.FindProperty("_itemID");

                itemIdProperty.intValue = id;
                id++;

                serializedItem.ApplyModifiedProperties();
            }

            Debug.Log("Item IDs have been successfully updated.");
        }
    }
}
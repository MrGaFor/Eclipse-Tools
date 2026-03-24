#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;

public class TypeWithComponentIconDrawer : OdinValueDrawer<Type>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Type type = this.ValueEntry.SmartValue;
        Texture icon = null;

        if (type != null)
        {
            icon = ComponentIconSystem.GetTypeIcon(type);
        }

        GUIContent content = new GUIContent(label.text, icon);
        this.CallNextDrawer(content);
    }
}
#endif
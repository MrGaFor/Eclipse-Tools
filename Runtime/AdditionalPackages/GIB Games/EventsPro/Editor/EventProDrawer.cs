﻿#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Events.EventProAttributes;
using Object = UnityEngine.Object;

namespace UnityEditorInternal
{
    /// <summary>
    /// Responsible for drawing an 'Unity Events Pro' attribute in Inspector
    /// </summary>
    [CustomPropertyDrawer(typeof(EventProBase), true)]
    public class EventProDrawer : PropertyDrawer
    {
        class State
        {
            internal ReorderableList m_ReorderableList;
            public int lastSelectedIndex;
        }

        private const string kNoFunctionString = "No Function";

        // Persistent Listener Paths
        internal const string kInstancePath = "m_Target";
        internal const string kInstanceTypePath = "m_TargetAssemblyTypeName";
        internal const string kMethodNamePath = "m_MethodName";
        internal const string kCallStatePath = "m_CallState";
        internal const string kArgumentsPath = "m_Arguments";
        internal const string kModesPath = "m_Modes";
        internal const string kModePath = "m_Mode";

        //ArgumentCache paths
        internal const string kFloatArgument = "m_FloatArgument";
        internal const string kIntArgument = "m_IntArgument";
        internal const string kObjectArgument = "m_ObjectArgument";
        internal const string kStringArgument = "m_StringArgument";
        internal const string kBoolArgument = "m_BoolArgument";
        internal const string kEnumArgument = "m_EnumArgument";
        internal const string kVector2Argument = "m_Vector2Argument";
        internal const string kVector2IntArgument = "m_Vector2IntArgument";
        internal const string kVector3Argument = "m_Vector3Argument";
        internal const string kVector3IntArgument = "m_Vector3IntArgument";
        internal const string kVector4Argument = "m_Vector4Argument";
        internal const string kLayerMaskArgument = "m_LayerMaskArgument";
        internal const string kColorArgument = "m_ColorArgument";
        internal const string kQuaternionArgument = "m_QuaternionArgument";
        internal const string kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";


        //property path splits and separators
        private const float kSpacing = 5;
        private const int kExtraSpacing = 9;
        private const string kDotString = ".";
        private const string kArrayDataString = "Array.data[";
        private static readonly char[] kDotSeparator = { '.' };
        private static readonly char[] kClosingSquareBraceSeparator = { ']' };

        // uss names
        internal const string kUssClassName = "unity-event";
        internal const string kLeftColumnClassName = kUssClassName + "__left-column";
        internal const string kRightColumnClassName = kUssClassName + "__right-column";
        internal const string kContainerClassName = kUssClassName + "__container";
        internal const string kHeaderClassName = "unity-list-view__header";
        internal const string kListViewScrollViewClassName = kUssClassName + "__list-view-scroll-view";
        internal const string kListViewItemClassName = kUssClassName + "__list-view-item";

        private string m_Text;
        private EventProBase m_DummyEvent;
        private SerializedProperty m_Prop;
        private SerializedProperty m_ListenersArray;

        private ReorderableList m_ReorderableList;
        private int m_LastSelectedIndex;

        private readonly Dictionary<string, State> m_States = new Dictionary<string, State>();

        internal string GetEventParams(EventProBase evt)
        {
            var methodInfo = evt.FindMethod(evt, "Invoke", EP_PersistentListenerMode.EventDefined);
            var types = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

            var sb = new StringBuilder();

            sb.Append(" (");
            for (int i = 0; i < types.Length; i++)
            {
                sb.Append(GetTypeName(types[i]));
                if (i < types.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");

            return sb.ToString();
        }

        private State GetState(SerializedProperty property)
        {
            State state = null;
            string key = property.propertyPath;
            m_States.TryGetValue(key, out state);

            // ensure the cached SerializedProperty is synchronized (case 974069)
            if (state == null || state.m_ReorderableList.serializedProperty.serializedObject != property.serializedObject)
            {
                if (state == null)
                    state = new State();

                SerializedProperty listenersArray = property.FindPropertyRelative("m_PersistentCalls.m_Calls");
                state.m_ReorderableList =
                    new ReorderableList(property.serializedObject, listenersArray, true, true, true, true)
                    {
                        drawHeaderCallback = DrawEventHeader,
                        drawElementCallback = DrawEventListener,
                        elementHeightCallback = GetElementHeight,
                        onSelectCallback = OnSelectEvent,
                        onReorderCallback = OnReorderEvent,
                        onAddCallback = OnAddEvent,
                        onRemoveCallback = OnRemoveEvent
                    };

                SetupReorderableList(state.m_ReorderableList);

                m_States[key] = state;
            }
            return state;
        }

        private State RestoreState(SerializedProperty property)
        {
            State state = GetState(property);

            m_ListenersArray = state.m_ReorderableList.serializedProperty;
            m_ReorderableList = state.m_ReorderableList;
            m_LastSelectedIndex = state.lastSelectedIndex;
            m_ReorderableList.index = m_LastSelectedIndex;

            return state;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RestoreState(property);

            return m_ReorderableList == null ? 0f : m_ReorderableList.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.backgroundColor = GIB.EditorUtilities.ToolkitSettings.EPColor;

            m_Prop = property;
            m_Text = label.text;

            if (property != null)
            {
                State state = RestoreState(property);

                OnGUI(position);
                state.lastSelectedIndex = m_LastSelectedIndex;
            }

            GUI.backgroundColor = Color.white;
        }

        private void OnGUI(Rect position)
        {
            if (m_ListenersArray == null || !m_ListenersArray.isArray)
                return;

            m_DummyEvent = GetDummyEvent(m_Prop);
            if (m_DummyEvent == null)
                return;

            if (m_ReorderableList == null) return;

            int oldIdentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            m_ReorderableList.DoList(position);
            EditorGUI.indentLevel = oldIdentLevel;
        }

        private void DrawEventHeader(Rect rect)
        {
            string label = (string.IsNullOrEmpty(m_Text) ? "EventPro" : m_Text) + GetEventParams(m_DummyEvent);
            float buttonWidth = 15f;

            rect.width -= buttonWidth;
            rect.height = EditorGUIUtility.singleLineHeight;

            Rect trashButtonRect = new Rect(rect.x + rect.width, rect.y, buttonWidth, rect.height);
            GUIContent trashButton = EditorGUIUtility.IconContent("TreeEditor.Trash");
            trashButton.tooltip = "Clear event list";

            TooltipAttribute tooltipAttr = GetAttribute<TooltipAttribute>(fieldInfo, typeof(TooltipAttribute));

            EditorGUI.LabelField(rect, new GUIContent(label, tooltipAttr != null ? tooltipAttr.tooltip : ""));
            if (GUI.Button(trashButtonRect, trashButton, ReorderableList.defaultBehaviours.preButton))
                RemoveAllEvents();
        }

        private void DrawEventListener(Rect rect, int index, bool isActive, bool isFocused)
        {
            var pListener = m_ListenersArray.GetArrayElementAtIndex(index);

            if (pListener == null) return;

            rect.y++;
            Rect[] subRects = GetRowRects(rect);
            Rect enabledRect = subRects[0];
            Rect goRect = subRects[1];
            Rect functionRect = subRects[2];
            Rect argNameRect = subRects[3];
            Rect argRect = subRects[4];

            // find the current event target...
            var listenerTarget = pListener.FindPropertyRelative(kInstancePath);
            var callState = pListener.FindPropertyRelative(kCallStatePath);
            var methodName = pListener.FindPropertyRelative(kMethodNamePath);
            var arguments = pListener.FindPropertyRelative(kArgumentsPath);
            var modes = pListener.FindPropertyRelative(kModesPath);

            Color c = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            EditorGUI.PropertyField(enabledRect, callState, GUIContent.none);

            EditorGUI.BeginChangeCheck();
            {
                GUI.Box(goRect, GUIContent.none);
                EditorGUI.PropertyField(goRect, listenerTarget, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    methodName.stringValue = null;
                    arguments.arraySize = 0;
                    modes.arraySize = 0;
                }
            }

            BuildArgumentsField(pListener, argNameRect, argRect, m_DummyEvent);

            var desiredTypes = new Type[arguments.arraySize];

            for (int i = 0; i < arguments.arraySize; i++)
            {
                var argument = arguments.GetArrayElementAtIndex(i);
                var objArgument = argument.FindPropertyRelative(kObjectArgumentAssemblyTypeName);

                if (objArgument != null)
                {
                    var desiredArgTypeName = objArgument.stringValue;

                    if (!string.IsNullOrEmpty(desiredArgTypeName))
                        desiredTypes[i] = Type.GetType(desiredArgTypeName, false) ?? typeof(Object);
                }
                else
                    desiredTypes[i] = typeof(Object);
            }

            using (new EditorGUI.DisabledScope(listenerTarget.objectReferenceValue == null))
            {
                EditorGUI.BeginProperty(functionRect, GUIContent.none, methodName);
                {
                    GUIContent buttonContent;
                    if (EditorGUI.showMixedValue)
                    {
                        buttonContent = new GUIContent("\u2014", "Mixed Values");
                    }
                    else
                    {
                        var buttonLabel = new StringBuilder();
                        if (listenerTarget.objectReferenceValue == null || string.IsNullOrEmpty(methodName.stringValue))
                        {
                            buttonLabel.Append(kNoFunctionString);
                        }
                        else if (!IsListenerValid(listenerTarget.objectReferenceValue, methodName.stringValue, desiredTypes))
                        {
                            var instanceString = "UnknownComponent";
                            var instance = listenerTarget.objectReferenceValue;
                            if (instance != null)
                                instanceString = instance.GetType().Name;

                            buttonLabel.Append($"<Missing {instanceString}.{methodName.stringValue}>");
                        }
                        else
                        {
                            buttonLabel.Append(listenerTarget.objectReferenceValue.GetType().Name);

                            if (!string.IsNullOrEmpty(methodName.stringValue))
                            {
                                buttonLabel.Append(".");
                                if (methodName.stringValue.StartsWith("set_"))
                                    buttonLabel.Append(methodName.stringValue.Substring(4));
                                else
                                    buttonLabel.Append(methodName.stringValue);
                            }
                        }
                        buttonContent = new GUIContent(buttonLabel.ToString());
                    }

                    if (GUI.Button(functionRect, buttonContent, EditorStyles.popup))
                        BuildPopupList(listenerTarget.objectReferenceValue, m_DummyEvent, pListener).DropDown(functionRect);
                }
                EditorGUI.EndProperty();
            }

            GUI.backgroundColor = c;
        }

        private float GetElementHeight(int index)
        {
            var pListener = m_ListenersArray.GetArrayElementAtIndex(index);
            // fix for bug
            if (pListener == null) return 21;
            var arguments = pListener.FindPropertyRelative(kArgumentsPath);

            var height = m_ReorderableList.elementHeight;

            if (arguments.arraySize <= 0 || arguments == null) return height;

            height += EditorGUIUtility.singleLineHeight;
            if (arguments.arraySize <= 1) return height;

            var spacing = kExtraSpacing + (arguments.arraySize - 1) * 2;

            height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight * (arguments.arraySize + 1) + EditorGUIUtility.standardVerticalSpacing + spacing;

            return height;
        }

        private void OnSelectEvent(ReorderableList list)
        {
            m_LastSelectedIndex = list.index;
        }

        private void OnReorderEvent(ReorderableList list)
        {
            if (m_LastSelectedIndex != list.index)
            {
                int from = m_LastSelectedIndex;
                int to = list.index;

                if (from < to)
                {
                    while (from < to)
                    {
                        m_ListenersArray.MoveArrayElement(from, from++);
                    }
                }
                else
                {
                    while (from > to)
                    {
                        m_ListenersArray.MoveArrayElement(from, from--);
                    }
                }
            }

            m_LastSelectedIndex = list.index;
        }

        private void OnAddEvent(ReorderableList list)
        {
            if (m_ListenersArray.hasMultipleDifferentValues)
            {
                foreach (Object targetObject in m_ListenersArray.serializedObject.targetObjects)
                {
                    SerializedObject tempSerializedObject = new SerializedObject(targetObject);
                    SerializedProperty listenerArrayProperty = tempSerializedObject.FindProperty(m_ListenersArray.propertyPath);
                    listenerArrayProperty.arraySize += 1;
                    tempSerializedObject.ApplyModifiedProperties();
                }
                m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
                m_ListenersArray.serializedObject.Update();
                list.index = list.serializedProperty.arraySize - 1;
            }
            else
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
            }

            m_LastSelectedIndex = list.index;

            SerializedProperty pListener = m_ListenersArray.GetArrayElementAtIndex(list.index);

            SerializedProperty callState = pListener.FindPropertyRelative(kCallStatePath);
            SerializedProperty listenerTarget = pListener.FindPropertyRelative(kInstancePath);
            SerializedProperty methodName = pListener.FindPropertyRelative(kMethodNamePath);
            SerializedProperty modes = pListener.FindPropertyRelative(kModesPath);
            SerializedProperty arguments = pListener.FindPropertyRelative(kArgumentsPath);

            callState.enumValueIndex = (int)UnityEventCallState.RuntimeOnly;
            listenerTarget.objectReferenceValue = null;
            methodName.stringValue = null;
            modes.arraySize = 0;
            arguments.arraySize = 0;
        }

        private void OnRemoveEvent(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            m_LastSelectedIndex = list.index;
        }

        private void RemoveAllEvents()
        {
            if (m_ListenersArray == null || !m_ListenersArray.isArray)
                return;

            m_ListenersArray.arraySize = 0;

            m_ListenersArray.serializedObject.ApplyModifiedProperties();

            // Exiting the GUI is necessary to prevent out of bounds errors from occuring this frame
            GUIUtility.ExitGUI();
        }

        private EventProBase GetDummyEvent(SerializedProperty prop)
        {
            //Use the SerializedProperty path to iterate through the fields of the inspected targetObject
            Object tgtobj = prop.serializedObject.targetObject;
            if (tgtobj == null)
                return new EventPro();

            string propPath = prop.propertyPath;
            Type ft = tgtobj.GetType();
            while (propPath.Length != 0)
            {
                //we could have a leftover '.' if the previous iteration handled an array element
                if (propPath.StartsWith("."))
                    propPath = propPath.Substring(1);

                var splits = propPath.Split(new[] { '.' }, 2);
                var newField = ft.GetField(splits[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (newField == null)
                    break;

                ft = newField.FieldType;

                if (ft.IsArray)
                    ft = ft.GetElementType();

                if (ft.IsGenericType && typeof(List<>) == ft.GetGenericTypeDefinition())
                    ft = ft.GetGenericArguments()[0];

                //the last item in the property path could have been an array element
                //bail early in that case
                if (splits.Length == 1)
                    break;

                propPath = splits[1];
                if (propPath.StartsWith("Array.data["))
                    propPath = propPath.Split(new[] { ']' }, 2)[1];
            }

            if (ft.IsSubclassOf(typeof(EventProBase)))
                return Activator.CreateInstance(ft) as EventProBase;

            return new EventPro();
        }



        private void SetupReorderableList(ReorderableList list) =>
            list.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing + kExtraSpacing;

        private Rect[] GetRowRects(Rect rect)
        {
            Rect[] rects = new Rect[5];

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;

            Rect enabledRect = rect;
            enabledRect.width *= 0.3f;

            Rect gameObjectRect = enabledRect;
            gameObjectRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect functionRect = rect;
            functionRect.xMin = gameObjectRect.xMax + kSpacing;

            Rect argNameRect = gameObjectRect;
            argNameRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect argRect = functionRect;
            argRect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;

            rects[0] = enabledRect;
            rects[1] = gameObjectRect;
            rects[2] = functionRect;
            rects[3] = argNameRect;
            rects[4] = argRect;

            return rects;
        }

        private void BuildArgumentsField(SerializedProperty listener, Rect argNameRect, Rect argRect, EventProBase dummyEvent)
        {
            // figure out the signature of this delegate...
            // The property at this stage points to the 'container' and has the field name
            Type delegateType = dummyEvent.GetType();

            // check out the signature of invoke as this is the callback!
            MethodInfo delegateMethod = delegateType.GetMethod("Invoke");
            var delegateArgumentsTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            var listenerTarget = listener.FindPropertyRelative(kInstancePath);
            var methodName = listener.FindPropertyRelative(kMethodNamePath);
            var arguments = listener.FindPropertyRelative(kArgumentsPath);
            var modes = listener.FindPropertyRelative(kModesPath);

            if (arguments.arraySize == 0
                || modes.arraySize == 0)
                return;

            MethodInfo method = null;
            if (listenerTarget.objectReferenceValue != null)
                method = GetMethodInfo(listenerTarget, methodName, arguments);

            if (method != null)
            {
                var customInspectorAttr = GetAttribute<CustomInspectorAttribute>(method, typeof(CustomInspectorAttribute));

                if (customInspectorAttr != null)
                {
                    var methodBuilder = EventProBase.GetValidMethodInfo(listenerTarget.objectReferenceValue, customInspectorAttr.methodName, new Type[] { typeof(SerializedProperty), typeof(Rect), typeof(Rect) });

                    methodBuilder.Invoke(listenerTarget.objectReferenceValue, new object[] { arguments, argNameRect, argRect });
                }
                else
                {
                    ParameterInfo[] parameters = method.GetParameters();

                    var hasLayerAttr = GetAttribute<LayerAttribute>(method, typeof(LayerAttribute)) != null;
                    var methodSliderAttr = GetAttribute<SliderAttribute>(method, typeof(SliderAttribute));
                    var methodIntSliderAttr = GetAttribute<IntSliderAttribute>(method, typeof(IntSliderAttribute));

                    for (int i = 0; i < arguments.arraySize; i++)
                    {
                        var argument = arguments.GetArrayElementAtIndex(i);
                        var modeEnum = GetMode(modes.GetArrayElementAtIndex(i));
                        var assembly = argument.FindPropertyRelative(kObjectArgumentAssemblyTypeName);

                        SliderAttribute paramSliderAttr = null;
                        IntSliderAttribute paramIntSliderAttr = null;

                        //only allow argument if we have a valid target / method
                        if (listenerTarget.objectReferenceValue == null || string.IsNullOrEmpty(methodName.stringValue))
                            modeEnum = EP_PersistentListenerMode.Void;
                        else if (!hasLayerAttr && i < parameters.Length)
                            hasLayerAttr = parameters[i].IsDefined(typeof(LayerAttribute), false);

                        if (methodSliderAttr == null || methodIntSliderAttr == null)
                        {
                            var attrs = parameters[i].GetCustomAttributes(false);
                            foreach (var attr in attrs)
                            {
                                if (methodSliderAttr == null && attr is SliderAttribute)
                                    paramSliderAttr = (SliderAttribute)attr;

                                if (methodIntSliderAttr == null && attr is IntSliderAttribute)
                                    paramIntSliderAttr = (IntSliderAttribute)attr;

                                if (attr is CustomInspectorAttribute)
                                    customInspectorAttr = (CustomInspectorAttribute)attr;
                            }
                        }

#if UNITY_2020_2_OR_NEWER
                        argument = modeEnum switch
                        {
                            EP_PersistentListenerMode.Float => argument.FindPropertyRelative(kFloatArgument),
                            EP_PersistentListenerMode.Int => argument.FindPropertyRelative(kIntArgument),
                            EP_PersistentListenerMode.Object => argument.FindPropertyRelative(kObjectArgument),
                            EP_PersistentListenerMode.String => argument.FindPropertyRelative(kStringArgument),
                            EP_PersistentListenerMode.Bool => argument.FindPropertyRelative(kBoolArgument),
                            EP_PersistentListenerMode.Vector2 => argument.FindPropertyRelative(kVector2Argument),
                            EP_PersistentListenerMode.Vector2Int => argument.FindPropertyRelative(kVector2IntArgument),
                            EP_PersistentListenerMode.Vector3 => argument.FindPropertyRelative(kVector3Argument),
                            EP_PersistentListenerMode.Vector3Int => argument.FindPropertyRelative(kVector3IntArgument),
                            EP_PersistentListenerMode.Vector4 => argument.FindPropertyRelative(kVector4Argument),
                            EP_PersistentListenerMode.LayerMask => argument.FindPropertyRelative(kLayerMaskArgument),
                            EP_PersistentListenerMode.Color => argument.FindPropertyRelative(kColorArgument),
                            EP_PersistentListenerMode.Quaternion => argument.FindPropertyRelative(kQuaternionArgument),
                            _ => argument.FindPropertyRelative(kIntArgument)
                        };
#else
                        SerializedProperty _result;

                        switch (modeEnum)
                        {
                            case EP_PersistentListenerMode.Float:
                                _result = argument.FindPropertyRelative(kFloatArgument);
                                break;
                            case EP_PersistentListenerMode.Int:
                                _result = argument.FindPropertyRelative(kIntArgument);
                                break;
                            case EP_PersistentListenerMode.Object:
                                _result = argument.FindPropertyRelative(kObjectArgument);
                                break;
                            case EP_PersistentListenerMode.String:
                                _result = argument.FindPropertyRelative(kStringArgument);
                                break;
                            case EP_PersistentListenerMode.Bool:
                                _result = argument.FindPropertyRelative(kBoolArgument);
                                break;
                            case EP_PersistentListenerMode.Vector2:
                                _result = argument.FindPropertyRelative(kVector2Argument);
                                break;
                            case EP_PersistentListenerMode.Vector2Int:
                                _result = argument.FindPropertyRelative(kVector2IntArgument);
                                break;
                            case EP_PersistentListenerMode.Vector3:
                                _result = argument.FindPropertyRelative(kVector3Argument);
                                break;
                            case EP_PersistentListenerMode.Vector3Int:
                                _result = argument.FindPropertyRelative(kVector3IntArgument);
                                break;
                            case EP_PersistentListenerMode.Vector4:
                                _result = argument.FindPropertyRelative(kVector4Argument);
                                break;
                            case EP_PersistentListenerMode.LayerMask:
                                _result = argument.FindPropertyRelative(kLayerMaskArgument);
                                break;
                            case EP_PersistentListenerMode.Color:
                                _result = argument.FindPropertyRelative(kColorArgument);
                                break;
                            case EP_PersistentListenerMode.Quaternion:
                                _result = argument.FindPropertyRelative(kQuaternionArgument);
                                break;
                            default:
                                _result = argument.FindPropertyRelative(kIntArgument);
                                break;
                        }

                        argument = _result;
#endif

                        if (argument == null)
                            continue;

                        if (customInspectorAttr != null)
                        {
                            var methodBuilder = EventProBase.GetValidMethodInfo(listenerTarget.objectReferenceValue, customInspectorAttr.methodName, new Type[] { typeof(SerializedProperty), typeof(Rect), typeof(Rect) });

                            methodBuilder.Invoke(listenerTarget.objectReferenceValue, new object[] { argument, argNameRect, argRect });
                        }
                        else
                        {
                            if (i < parameters.Length)
                                EditorGUI.LabelField(argNameRect, new GUIContent(string.Format("{0}:", parameters[i].Name)));

                            switch (modeEnum)
                            {
                                case EP_PersistentListenerMode.EventDefined:
                                    {
                                        //if (i < delegateArgumentsTypes.Length && arguments.arraySize > delegateArgumentsTypes.Length)
                                        if (i < delegateArgumentsTypes.Length)
                                        {
                                            var type = Type.GetType(assembly.stringValue, false) ?? typeof(Object);

                                            if (type == delegateArgumentsTypes[i])
                                                EditorGUI.LabelField(argRect, "Dynamic " + GetTypeName(type));
                                        }

                                        break;
                                    }
                                case EP_PersistentListenerMode.Enum:
                                    {
                                        Type enumType = Type.GetType(assembly.stringValue, false);
                                        string[] names = Enum.GetNames(enumType);

                                        argument.intValue = EditorGUI.Popup(argRect, argument.intValue, names);
                                        break;
                                    }
                                case EP_PersistentListenerMode.Int when hasLayerAttr:
                                    argument.intValue = EditorGUI.LayerField(argRect, argument.intValue);
                                    break;
                                case EP_PersistentListenerMode.Int when methodIntSliderAttr != null:
                                    EditorGUI.IntSlider(argRect, argument, methodIntSliderAttr.minValue, methodIntSliderAttr.maxValue, GUIContent.none);
                                    break;
                                case EP_PersistentListenerMode.Int when paramIntSliderAttr != null:
                                    EditorGUI.IntSlider(argRect, argument, paramIntSliderAttr.minValue, paramIntSliderAttr.maxValue, GUIContent.none);
                                    break;
                                case EP_PersistentListenerMode.Int:
                                    EditorGUI.PropertyField(argRect, argument, GUIContent.none);
                                    break;
                                case EP_PersistentListenerMode.Float when (methodSliderAttr != null || paramSliderAttr != null):
                                    {
                                        if (methodSliderAttr != null)
                                            EditorGUI.Slider(argRect, argument, methodSliderAttr.minValue, methodSliderAttr.maxValue, GUIContent.none);
                                        else if (paramSliderAttr != null)
                                            EditorGUI.Slider(argRect, argument, paramSliderAttr.minValue, paramSliderAttr.maxValue, GUIContent.none);
                                        break;
                                    }
                                case EP_PersistentListenerMode.Vector2:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector2Field(argRect, GUIContent.none, argument.vector2Value);
                                        if (EditorGUI.EndChangeCheck())
                                            argument.vector2Value = result;
                                        break;
                                    }
                                case EP_PersistentListenerMode.Vector2Int:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector2IntField(argRect, GUIContent.none, argument.vector2IntValue);
                                        if (EditorGUI.EndChangeCheck())
                                            argument.vector2IntValue = result;
                                        break;
                                    }
                                case EP_PersistentListenerMode.Vector3:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector3Field(argRect, GUIContent.none, argument.vector3Value);
                                        if (EditorGUI.EndChangeCheck())
                                            argument.vector3Value = result;
                                        break;
                                    }
                                case EP_PersistentListenerMode.Vector3Int:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector3IntField(argRect, GUIContent.none, argument.vector3IntValue);
                                        if (EditorGUI.EndChangeCheck())
                                            argument.vector3IntValue = result;
                                        break;
                                    }
                                case EP_PersistentListenerMode.Vector4:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector4Field(argRect, GUIContent.none, argument.vector4Value);
                                        if (EditorGUI.EndChangeCheck())
                                            argument.vector4Value = result;
                                        break;
                                    }
                                case EP_PersistentListenerMode.Quaternion:
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var result = EditorGUI.Vector4Field(argRect, GUIContent.none, new Vector4(argument.quaternionValue.x, argument.quaternionValue.y, argument.quaternionValue.z, argument.quaternionValue.w));
                                        if (EditorGUI.EndChangeCheck())
                                            argument.quaternionValue = new Quaternion(result.x, result.y, result.z, result.w);
                                        break;
                                    }
                                case EP_PersistentListenerMode.Object:
                                    {
                                        GUIStyle style = new GUIStyle();

                                        var desiredArgTypeName = assembly == null ? string.Empty : assembly.stringValue;
                                        var desiredType = typeof(Object);

                                        if (!string.IsNullOrEmpty(desiredArgTypeName))
                                            desiredType = Type.GetType(desiredArgTypeName, false) ?? typeof(Object);

                                        EditorGUI.BeginChangeCheck();

                                        EditorStyles.objectField.normal.textColor =
                                                argument.objectReferenceValue == null ? new Color(1f, .5f, 0) : Color.white;

                                        var result = EditorGUI.ObjectField(argRect, GUIContent.none, argument.objectReferenceValue, desiredType, true);
                                        EditorStyles.objectField.normal.textColor = Color.white;
                                        if (EditorGUI.EndChangeCheck())
                                            argument.objectReferenceValue = result;
                                        break;
                                    }
                                default:
                                    {
                                        if (modeEnum != EP_PersistentListenerMode.Void)
                                            EditorGUI.PropertyField(argRect, argument, GUIContent.none);
                                        break;
                                    }
                            }
                        }

                        customInspectorAttr = null;

                        argNameRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
        }

        private GenericMenu BuildPopupList(Object target, EventProBase dummyEvent, SerializedProperty listener)
        {
            //special case for components... we want all the game objects targets there!
            var targetToUse = target;
            if (targetToUse is Component)
                targetToUse = (target as Component).gameObject;

            // find the current event target...
            var methodName = listener.FindPropertyRelative(kMethodNamePath);

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(kNoFunctionString),
                string.IsNullOrEmpty(methodName.stringValue),
                ClearEventFunction,
                new UnityEventFunction(listener, null, null, null));

            if (targetToUse == null)
                return menu;

            menu.AddSeparator("");

            // figure out the signature of this delegate...
            // The property at this stage points to the 'container' and has the field name
            Type delegateType = dummyEvent.GetType();

            // check out the signature of invoke as this is the callback!
            MethodInfo delegateMethod = delegateType.GetMethod("Invoke");
            var delegateArgumentsTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            GeneratePopUpForType(menu, targetToUse, false, listener, delegateArgumentsTypes);

            if (targetToUse is GameObject gameObject)
            {
                Component[] comps = gameObject.GetComponents<Component>();
                var duplicateNames = comps.Where(c => c != null).Select(c => c.GetType().Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                foreach (Component comp in comps)
                {
                    if (comp == null)
                        continue;

                    GeneratePopUpForType(menu, comp, duplicateNames.Contains(comp.GetType().Name), listener, delegateArgumentsTypes);
                }
            }

            return menu;
        }

        private void GeneratePopUpForType(GenericMenu menu, Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
        {
            var methods = new List<ValidMethodMap>();
            string targetName = useFullTargetName ? target.GetType().FullName : target.GetType().Name;

            bool didAddDynamic = false;

            // skip 'void' event defined on the GUI as we have a void prebuilt type!
            if (delegateArgumentsTypes.Length != 0)
            {
                GetMethodsForTargetAndMode(target, methods, true, delegateArgumentsTypes);
                if (methods.Count > 0)
                {
                    menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " + string.Join(", ", delegateArgumentsTypes.Select(e => GetTypeName(e)).ToArray())));
                    AddMethodsToMenu(menu, listener, methods, targetName, delegateArgumentsTypes);
                    didAddDynamic = true;
                }
            }

            methods.Clear();

            GetMethodsForTargetAndMode(target, methods);

            if (methods.Count > 0)
            {
                if (didAddDynamic)
                    // AddSeperator doesn't seem to work for sub-menus, so we have to use this workaround instead of a proper separator for now.
                    menu.AddItem(new GUIContent(targetName + "/ "), false, null);
                if (delegateArgumentsTypes.Length != 0)
                    menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));

                AddMethodsToMenu(menu, listener, methods, targetName);
            }
        }

        private void GetMethodsForTargetAndMode(Object target, List<ValidMethodMap> methods, bool isDynamic = false, Type[] delegateArgumentsTypes = null)
        {
            IEnumerable<ValidMethodMap> newMethods = CalculateMethodMap(target, delegateArgumentsTypes, isDynamic);

            methods.AddRange(newMethods);
        }

        private IEnumerable<ValidMethodMap> CalculateMethodMap(Object target, Type[] types, bool isDynamic)
        {
            var validMethods = new List<ValidMethodMap>();
            if (target == null || (isDynamic && types == null))
                return validMethods;

            // find the methods on the behaviour that match the signature
            Type componentType = target.GetType();
            var componentMethods = componentType.GetMethods().Where(x => !x.IsSpecialName).ToList();

            var wantedProperties = componentType.GetProperties().AsEnumerable();
            wantedProperties = wantedProperties.Where(x => x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null);
            componentMethods.AddRange(wantedProperties.Select(x => x.GetSetMethod()));

            foreach (var componentMethod in componentMethods)
            {
                var componentParameters = componentMethod.GetParameters();

                // Don't show obsolete methods.
                if (componentMethod.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
                    continue;

                if (componentMethod.ReturnType != typeof(void))
                    continue;

                if (isDynamic)
                {
                    if (types.Length > componentParameters.Length)
                        continue;

                    var modes = new EP_PersistentListenerMode[componentParameters.Length];

                    // if the argument types do not match, no match
                    bool parametersMatch = true;
                    for (int i = 0; i < types.Length; i++)
                    {
                        if (componentParameters[i].ParameterType.IsAssignableFrom(types[i]))
                            modes[i] = EP_PersistentListenerMode.EventDefined;
                        else
                        {
                            modes[i] = GetMode(componentParameters[i].ParameterType);
                            parametersMatch = false;
                        }
                    }

                    for (int i = types.Length; i < componentParameters.Length; i++)
                    {
                        modes[i] = GetMode(componentParameters[i].ParameterType);
                    }

                    // valid dynamic method
                    if (parametersMatch)
                    {
                        var vmm = new ValidMethodMap
                        {
                            target = target,
                            methodInfo = componentMethod,
                            modes = modes,
                            parameters = componentParameters,
                            isDynamic = isDynamic
                        };
                        validMethods.Add(vmm);
                    }
                }
                else
                {
                    if (HasInvalidParameter(componentParameters))
                        continue;

                    var vmm = new ValidMethodMap
                    {
                        target = target,
                        methodInfo = componentMethod,
                        modes = GetModes(componentParameters),
                        parameters = componentParameters,
                        isDynamic = isDynamic
                    };
                    validMethods.Add(vmm);
                }
            }

            return validMethods;
        }

        public bool IsListenerValid(Object uObject, string methodName, Type[] argumentTypes)
        {
            if (uObject == null || string.IsNullOrEmpty(methodName))
                return false;

            return EventProBase.GetValidMethodInfo(uObject, methodName, argumentTypes) != null;
        }

        private bool HasInvalidParameter(ParameterInfo[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsPrimitive
                    || parameter.ParameterType.IsEnum
                    || parameter.ParameterType.IsValueType
                    || parameter.ParameterType == typeof(string)
                    || parameter.ParameterType == typeof(GameObject)
                    || typeof(Component).IsAssignableFrom(parameter.ParameterType))
                {
                    continue;
                }

                if (parameter.IsOut
                    || parameter.ParameterType.IsArray
                    || parameter.ParameterType.IsGenericType
                    || parameter.ParameterType.IsGenericTypeDefinition
                    || parameter.ParameterType.IsGenericParameter
                    || parameter.ParameterType.IsInterface
                    || parameter.ParameterType == typeof(object)
                    || parameter.ParameterType == typeof(Object)
                    || parameter.ParameterType == typeof(IEnumerator)
                    || parameter.ParameterType == typeof(Coroutine)
                    || parameter.ParameterType == typeof(UnityAction)
                    || typeof(ICollection).IsAssignableFrom(parameter.ParameterType)
                    || typeof(ICollection<>).IsAssignableFrom(parameter.ParameterType))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, string targetName, Type[] delegateArgumentsTypes = null)
        {
            // Note: sorting by a bool in OrderBy doesn't seem to work for some reason, so using numbers explicitly.
            IEnumerable<ValidMethodMap> orderedMethods = methods.OrderBy(e => e.methodInfo.Name.StartsWith("set_") ? 0 : 1).ThenBy(e => e.methodInfo.Name);
            foreach (var validMethod in orderedMethods)
                AddFunctionsForScript(menu, listener, validMethod, targetName, delegateArgumentsTypes);
        }

        private void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method, string targetName, Type[] delegateArgumentsTypes)
        {
            // find the current event target...
            var listenerTarget = listener.FindPropertyRelative(kInstancePath).objectReferenceValue;
            var methodName = listener.FindPropertyRelative(kMethodNamePath).stringValue;
            var modes = listener.FindPropertyRelative(kModesPath);

            var args = new StringBuilder();
            var parameters = method.methodInfo.GetParameters();
            var count = parameters.Length;
            for (int index = 0; index < count; index++)
            {
                var methodArg = parameters[index];
                args.Append(GetTypeName(methodArg.ParameterType));

                if (index < count - 1)
                    args.Append(", ");
            }

            var isCurrentlySet = listenerTarget == method.target
                && methodName == method.methodInfo.Name
                && CompareModes(modes, method.modes);

            string path = GetFormattedMethodName(targetName, method.methodInfo, args.ToString(), method.isDynamic, delegateArgumentsTypes);

            menu.AddItem(new GUIContent(path),
                isCurrentlySet,
                SetEventFunction,
                new UnityEventFunction(listener, method.target, method.methodInfo, method.modes));
        }

        private bool CompareModes(SerializedProperty modes1, EP_PersistentListenerMode[] modes2)
        {
            if (modes1 == null || modes2 == null)
                return false;

            if (modes1.arraySize == 1 && modes2.Length == 0)
                if (modes1.GetArrayElementAtIndex(0).enumValueIndex == (int)EP_PersistentListenerMode.Void)
                    return true;

            if (modes1.arraySize != modes2.Length)
                return false;

            for (int i = 0; i < modes1.arraySize; i++)
            {
                if (modes1.GetArrayElementAtIndex(i).enumValueIndex != (int)modes2[i])
                    return false;
            }

            return true;
        }

        private EP_PersistentListenerMode GetMode(SerializedProperty mode)
        {
            switch (mode.type)
            {
                case "bool":
                    return EP_PersistentListenerMode.Bool;
                case "float":
                    return EP_PersistentListenerMode.Float;
                case "int":
                    return EP_PersistentListenerMode.Int;
                case "string":
                    return EP_PersistentListenerMode.String;
                case "Enum":
                    return (EP_PersistentListenerMode)mode.enumValueIndex;
                default:
                    return EP_PersistentListenerMode.Void;
            }
        }

        private EP_PersistentListenerMode GetMode(Type type)
        {
            if (type == typeof(int))
                return EP_PersistentListenerMode.Int;
            else if (type == typeof(float))
                return EP_PersistentListenerMode.Float;
            else if (type == typeof(string))
                return EP_PersistentListenerMode.String;
            else if (type == typeof(bool))
                return EP_PersistentListenerMode.Bool;
            else if (type.IsEnum)
                return EP_PersistentListenerMode.Enum;
            else if (type == typeof(Vector2))
                return EP_PersistentListenerMode.Vector2;
            else if (type == typeof(Vector2Int))
                return EP_PersistentListenerMode.Vector2Int;
            else if (type == typeof(Vector3))
                return EP_PersistentListenerMode.Vector3;
            else if (type == typeof(Vector3Int))
                return EP_PersistentListenerMode.Vector3Int;
            else if (type == typeof(Vector4))
                return EP_PersistentListenerMode.Vector4;
            else if (type == typeof(LayerMask))
                return EP_PersistentListenerMode.LayerMask;
            else if (type == typeof(Color))
                return EP_PersistentListenerMode.Color;
            else if (type == typeof(Quaternion))
                return EP_PersistentListenerMode.Quaternion;
            else
                return EP_PersistentListenerMode.Object;
        }

        private EP_PersistentListenerMode[] GetModes(ParameterInfo[] parameters)
        {
            if (parameters == null)
                return new EP_PersistentListenerMode[0];

            EP_PersistentListenerMode[] modes = new EP_PersistentListenerMode[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                modes[i] = GetMode(parameters[i].ParameterType);

            return modes;
        }

        private string GetTypeName(Type t)
        {
            if (t == typeof(int))
                return "int";
            if (t == typeof(float))
                return "float";
            if (t == typeof(string))
                return "string";
            if (t == typeof(bool))
                return "bool";
            return t.Name;
        }

        private string GetFormattedMethodName(string targetName, MethodInfo method, string args, bool dynamic, Type[] delegateArgumentsTypes)
        {
            if (dynamic)
            {
                if (method.Name.StartsWith("set_"))
                    return $"{targetName}/{method.Name.Substring(4)}";
                else
                {
                    var builder = new StringBuilder();

                    var count = delegateArgumentsTypes.Length;
                    for (int i = 0; i < count; i++)
                    {
                        builder.Append("[dynamic]");

                        if (i < count - 1)
                            builder.Append(", ");
                    }

                    var parameters = method.GetParameters();
                    count = parameters.Length;

                    if (count - delegateArgumentsTypes.Length > 0)
                        builder.Append(", ");

                    for (int i = delegateArgumentsTypes.Length; i < count; i++)
                    {
                        builder.Append(GetTypeName(parameters[i].ParameterType));

                        if (i < count - 1)
                            builder.Append(", ");
                    }

                    return $"{targetName}/{method.Name} ({builder.ToString()})";
                }
            }
            else
            {
                if (method.Name.StartsWith("set_"))
                    return $"{targetName}/{args} {method.Name.Substring(4)}";
                else
                    return $"{targetName}/{method.Name} ({args})";
            }
        }

        private MethodInfo GetMethodInfo(SerializedProperty listenerTarget, SerializedProperty methodName, SerializedProperty arguments)
        {
            var desiredTypes = new Type[arguments.arraySize];
            for (int i = 0; i < arguments.arraySize; i++)
            {
                var argument = arguments.GetArrayElementAtIndex(i);
                var objArgument = argument.FindPropertyRelative(kObjectArgumentAssemblyTypeName);

                if (objArgument != null)
                {
                    var desiredArgTypeName = objArgument.stringValue;

                    if (!string.IsNullOrEmpty(desiredArgTypeName))
                        desiredTypes[i] = Type.GetType(desiredArgTypeName, false) ?? typeof(Object);
                }
                else
                    desiredTypes[i] = typeof(Object);
            }

            return EventProBase.GetValidMethodInfo(listenerTarget.objectReferenceValue, methodName.stringValue, desiredTypes);
        }

        private static T GetAttribute<T>(MemberInfo memberInfo, Type attributeType, bool inherit = false)
        {
            if (memberInfo != null)
            {
                var attrs = memberInfo.GetCustomAttributes(attributeType, inherit);

                if (attrs.Length > 0)
                    return (T)attrs[0];
            }

            return default(T);
        }

        private static T GetAttribute<T>(ParameterInfo parameterInfo, Type attributeType, bool inherit)
        {
            if (parameterInfo != null)
            {
                var attrs = parameterInfo.GetCustomAttributes(attributeType, inherit);

                if (attrs.Length > 0)
                    return (T)attrs[0];
            }

            return default(T);
        }

        private void SetEventFunction(object source)
        {
            ((UnityEventFunction)source).Assign();
        }

        private void ClearEventFunction(object source)
        {
            ((UnityEventFunction)source).Clear();
        }

        private struct ValidMethodMap
        {
            public Object target;
            public MethodInfo methodInfo;
            public EP_PersistentListenerMode[] modes;
            public ParameterInfo[] parameters;
            public bool isDynamic;
        }

        struct UnityEventFunction
        {
            public readonly SerializedProperty m_Listener;
            public readonly Object m_Target;
            public readonly MethodInfo m_Method;
            public readonly EP_PersistentListenerMode[] m_Modes;

            public UnityEventFunction(SerializedProperty listener, Object target, MethodInfo method, EP_PersistentListenerMode[] modes)
            {
                m_Listener = listener;
                m_Target = target;
                m_Method = method;
                m_Modes = modes;
            }

            public void Assign()
            {
                // find the current event target...
                var listenerTarget = m_Listener.FindPropertyRelative(kInstancePath);
                var methodName = m_Listener.FindPropertyRelative(kMethodNamePath);
                var arguments = m_Listener.FindPropertyRelative(kArgumentsPath);
                var modes = m_Listener.FindPropertyRelative(kModesPath);

                listenerTarget.objectReferenceValue = m_Target;
                methodName.stringValue = m_Method.Name;

                var argParams = m_Method.GetParameters();

                arguments.arraySize = argParams.Length;
                modes.arraySize = argParams.Length;

                if (modes.arraySize == 0)
                {
                    modes.InsertArrayElementAtIndex(0);
                    modes.GetArrayElementAtIndex(0).enumValueIndex = (int)EP_PersistentListenerMode.Void;
                }

                int defaultValue = 0;
                var methodLayerAttr = GetAttribute<LayerAttribute>(m_Method, typeof(LayerAttribute));
                if (methodLayerAttr != null)
                    defaultValue = methodLayerAttr.defaultValue;

                for (int i = 0; i < argParams.Length; i++)
                {
                    modes.GetArrayElementAtIndex(i).enumValueIndex = (int)m_Modes[i];

                    var argument = arguments.GetArrayElementAtIndex(i);

                    var fullArgumentType = argument.FindPropertyRelative(kObjectArgumentAssemblyTypeName);

                    fullArgumentType.stringValue = argParams[i].ParameterType.AssemblyQualifiedName;

                    var paramLayerAttr = GetAttribute<LayerAttribute>(argParams[i], typeof(LayerAttribute), false);
                    if (paramLayerAttr != null)
                        defaultValue = paramLayerAttr.defaultValue;

                    switch (m_Modes[i])
                    {
                        case EP_PersistentListenerMode.Object:
                            argument.FindPropertyRelative(kObjectArgument).objectReferenceValue = null;
                            break;
                        case EP_PersistentListenerMode.Int:
                            argument.FindPropertyRelative(kIntArgument).intValue = defaultValue;
                            break;
                        case EP_PersistentListenerMode.Float:
                            argument.FindPropertyRelative(kFloatArgument).floatValue = 0f;
                            break;
                        case EP_PersistentListenerMode.String:
                            argument.FindPropertyRelative(kStringArgument).stringValue = null;
                            break;
                        case EP_PersistentListenerMode.Bool:
                            argument.FindPropertyRelative(kBoolArgument).boolValue = false;
                            break;
                        case EP_PersistentListenerMode.Enum:
                            argument.FindPropertyRelative(kIntArgument).intValue = 0;
                            break;
                        case EP_PersistentListenerMode.Vector2:
                            argument.FindPropertyRelative(kVector2Argument).vector2Value = Vector2.zero;
                            break;
                        case EP_PersistentListenerMode.Vector2Int:
                            argument.FindPropertyRelative(kVector2IntArgument).vector2IntValue = Vector2Int.zero;
                            break;
                        case EP_PersistentListenerMode.Vector3:
                            argument.FindPropertyRelative(kVector3Argument).vector3Value = Vector3.zero;
                            break;
                        case EP_PersistentListenerMode.Vector3Int:
                            argument.FindPropertyRelative(kVector3IntArgument).vector3IntValue = Vector3Int.zero;
                            break;
                        case EP_PersistentListenerMode.Vector4:
                            argument.FindPropertyRelative(kVector4Argument).vector4Value = Vector4.zero;
                            break;
                        case EP_PersistentListenerMode.LayerMask:
                            argument.FindPropertyRelative(kLayerMaskArgument).intValue = 0;
                            break;
                        case EP_PersistentListenerMode.Color:
                            argument.FindPropertyRelative(kColorArgument).colorValue = Color.clear;
                            break;
                        case EP_PersistentListenerMode.Quaternion:
                            argument.FindPropertyRelative(kQuaternionArgument).quaternionValue = Quaternion.Euler(Vector3.zero);
                            break;
                        case EP_PersistentListenerMode.EventDefined:
                        case EP_PersistentListenerMode.Void:
                        default:
                            break;
                    }
                }

                m_Listener.serializedObject.ApplyModifiedProperties();
            }

            public void Clear()
            {
                // find the current event target
                m_Listener.FindPropertyRelative(kMethodNamePath).stringValue = null;
                m_Listener.FindPropertyRelative(kArgumentsPath).arraySize = 0;
                m_Listener.FindPropertyRelative(kModesPath).arraySize = 0;

                m_Listener.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
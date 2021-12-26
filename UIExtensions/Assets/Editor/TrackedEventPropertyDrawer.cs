using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(TrackedUnityEvent))]
public class TrackedEventPropertyDrawer : PropertyDrawer {

    Color mainColorDark = new Color(0.25f, 0.25f, 0.25f);
    Color bgColorDark = new Color(0.2f, 0.2f, 0.2f);
    Color outlineColorDark = new Color(0.14f, 0.14f, 0.14f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty listenersCount = property.FindPropertyRelative("listenersCount");
        if (listenersCount == null) { Debug.LogWarning("Couldnt get actions"); }

        SerializedProperty actionObjects = property.FindPropertyRelative("actionObjects");
        if (actionObjects == null) { Debug.LogWarning("Couldnt get actionObjects"); }

        SerializedProperty selectedCompIndex = property.FindPropertyRelative("selectedCompIndex");
        SerializedProperty selectedFuncIndex = property.FindPropertyRelative("selectedFuncIndex");

        Debug.Log(listenersCount.intValue);
        for (int i = 0; i < listenersCount.intValue; i++) {
            Rect paddedRect = new Rect(position.x + 5, position.y, position.width, position.height * 0.75f);
            Rect btnRect = new Rect(position.x + position.width - 75, position.y + position.height * 0.75f, 75, position.height * 0.2f);
            Rect addBTNRect = new Rect(btnRect.position, new Vector2(btnRect.width / 2, btnRect.height));
            Rect subBTNRect = new Rect(new Vector2(btnRect.x + (btnRect.width / 2), btnRect.y), new Vector2(btnRect.width / 2, btnRect.height));

            //Main BG rect
            EditorGUI.DrawRect(paddedRect, outlineColorDark);
            EditorGUI.DrawRect(paddedRect.PadUniformCentered(2), bgColorDark);

            //+/- BTN rect
            EditorGUI.DrawRect(btnRect, outlineColorDark);
            EditorGUI.DrawRect(btnRect.PadUniformCentered(2), bgColorDark);

            //Add listener btn
            GUI.Button(addBTNRect, "+");
            GUI.Button(subBTNRect, "-");

            Rect leftRect = new Rect(paddedRect.x, paddedRect.y, paddedRect.width / 2, EditorGUIUtility.singleLineHeight);
            leftRect = leftRect.PadRect(new Rect(0, 0, 2, 2));

            Object newObj = (GameObject)EditorGUI.ObjectField(leftRect, GUIContent.none, actionObjects.GetArrayElementAtIndex(i).serializedObject.targetObject, typeof(GameObject), true);
            if (actionObjects.GetArrayElementAtIndex(i).objectReferenceValue == null || !newObj.Equals(actionObjects.GetArrayElementAtIndex(i).objectReferenceValue)) {
                selectedCompIndex.GetArrayElementAtIndex(i).intValue = 0;
                selectedFuncIndex.GetArrayElementAtIndex(i).intValue = 0;
                actionObjects.GetArrayElementAtIndex(i).objectReferenceValue = newObj;
            }

            //drop down based on functions attached to eventObj public scripts/components functions
            //get all components/scripts on eventObj
            //get all public funcs on eventObj

            //try get eventObject as GameObject
            GameObject eventGO = (GameObject)actionObjects.GetArrayElementAtIndex(i).objectReferenceValue;

            if (eventGO != null) {
                Rect rightRect = new Rect(paddedRect.x + (paddedRect.width / 2), paddedRect.y, paddedRect.width / 2, EditorGUIUtility.singleLineHeight);
                rightRect = rightRect.PadRect(new Rect(0, 0, 2, 2));

                //Get all components
                List<Component> eventObjectComponents = new List<Component>();
                eventObjectComponents.AddRange(eventGO.GetComponents(typeof(MonoBehaviour)));

                //Get funcs + func names on components
                List<MethodInfo[]> componentFuncs = new List<MethodInfo[]>();
                List<List<ParameterInfo[]>> componentFuncParams = new List<List<ParameterInfo[]>>();
                List<string[]> componentFuncNames = new List<string[]>();

                foreach (MonoBehaviour mb in eventObjectComponents) {
                    MethodInfo[] funcs = mb.GetScriptFunctions(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    componentFuncs.Add(funcs);

                    List<ParameterInfo[]> paramInfo = new List<ParameterInfo[]>();

                    string[] compFuncNames = new string[funcs.Length];
                    for (int j = 0; j < funcs.Length; j++) {
                        compFuncNames[j] = funcs[j].Name;

                        paramInfo.Add(funcs[j].GetParameters());
                    }

                    componentFuncParams.Add(paramInfo);

                    componentFuncNames.Add(compFuncNames);
                }

                //dropdown
                string[] dropdownOptions = new string[eventObjectComponents.Count + 1];
                dropdownOptions[0] = "No function"; //actual dropdown items start at index 1 as 0 is no func

                //Set func dropdown items
                for (int j = 0; j < eventObjectComponents.Count; j++) {
                    dropdownOptions[j + 1] = eventObjectComponents[j].GetType().Name;
                }

                selectedCompIndex.GetArrayElementAtIndex(i).intValue = EditorGUI.Popup(rightRect, selectedCompIndex.GetArrayElementAtIndex(i).intValue, dropdownOptions);

                if (selectedCompIndex.GetArrayElementAtIndex(i).intValue > 0) {
                    //Func dropdown
                    selectedFuncIndex.GetArrayElementAtIndex(i).intValue = EditorGUI.Popup(
                        new Rect(paddedRect.x, paddedRect.y + EditorGUIUtility.singleLineHeight, paddedRect.width, EditorGUIUtility.singleLineHeight),
                        selectedFuncIndex.GetArrayElementAtIndex(i).intValue,
                        componentFuncNames[selectedCompIndex.GetArrayElementAtIndex(i).intValue - 1]);

                    //Get parameters
                    ParameterInfo[] paramsInfo = componentFuncParams[selectedCompIndex.GetArrayElementAtIndex(i).intValue - 1][selectedFuncIndex.GetArrayElementAtIndex(i).intValue];
                    //need to setup scroll for params or increase height of main rect
                    for (int j = 0; j < paramsInfo.Length; j++) {
                        DrawDynamicPropertyField(new Rect(new Vector2(paddedRect.x, paddedRect.y + (EditorGUIUtility.singleLineHeight * (j + 2))), paddedRect.size).PadRect(0, 0, 5, 5), paramsInfo[j].ParameterType, paramsInfo[j].Name);
                    }
                }
            }
        }
        EditorGUI.EndProperty();
    }

    //This is painful. There's got to be another way
    void DrawDynamicPropertyField(Rect mainRect, Type propType, string propName) {
        Rect left = new Rect(mainRect.position, new Vector2(mainRect.width / 2, EditorGUIUtility.singleLineHeight));
        Rect right = new Rect(new Vector2(mainRect.position.x + (mainRect.width / 2), mainRect.position.y), new Vector2(mainRect.width / 2, EditorGUIUtility.singleLineHeight));

        EditorGUI.LabelField(left, $"{propName} ({propType.Name})");

        //oh the pain
        if (propType.IsEquivalentTo(typeof(string))) { EditorGUI.TextField(right, ""); }
        if (propType.IsEquivalentTo(typeof(int))) { EditorGUI.IntField(right, 0); }
        if (propType.IsEquivalentTo(typeof(float))) { EditorGUI.FloatField(right, 0); }
        if (propType.IsEquivalentTo(typeof(bool))) { EditorGUI.Toggle(right, false); }
    }
}

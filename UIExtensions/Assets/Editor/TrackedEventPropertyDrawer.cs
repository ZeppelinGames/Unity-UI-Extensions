using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(TrackedUnityEvent))]
public class TrackedEventPropertyDrawer : PropertyDrawer {

    [SerializeField] private Object eventObject;
    [SerializeField] private int ddSelectedIndex = 0;
    [SerializeField] private int funcSelectedIndex = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        Rect paddedRect = new Rect(position.x + 5, position.y, position.width, position.height * 0.9f);

        //fix rect padding
        EditorGUI.DrawRect(paddedRect, new Color().GrayscaleColor(0.14f));

        EditorGUI.DrawRect(paddedRect.PadUniformCentered(2), new Color().GrayscaleColor(0.2f));

        Rect leftRect = new Rect(paddedRect.x, paddedRect.y, paddedRect.width / 2, EditorGUIUtility.singleLineHeight);
        leftRect = leftRect.PadRect(new Rect(0, 0, 2, 2));

        Object newObj = EditorGUI.ObjectField(leftRect, GUIContent.none, eventObject, typeof(GameObject), true);
        if (eventObject == null || !newObj.Equals(eventObject)) {
            ddSelectedIndex = 0;
            funcSelectedIndex = 0;
            eventObject = newObj;
        }

        //drop down based on functions attached to eventObj public scripts/components functions
        //get all components/scripts on eventObj
        //get all public funcs on eventObj

        //try get eventObject as GameObject
        GameObject eventGO = eventObject as GameObject;

        if (eventGO != null) {
            Rect rightRect = new Rect(paddedRect.x + (paddedRect.width / 2), paddedRect.y, paddedRect.width / 2, EditorGUIUtility.singleLineHeight);
            rightRect = rightRect.PadRect(new Rect(0, 0, 2, 2));

            //Get all components
            List<Component> eventObjectComponents = new List<Component>();
            eventObjectComponents.AddRange(eventGO.GetComponents(typeof(MonoBehaviour)));

            //Get funcs + func names on components
            List<MethodInfo[]> componentFuncs = new List<MethodInfo[]>();
            List<string[]> componentFuncNames = new List<string[]>();
            foreach (MonoBehaviour mb in eventObjectComponents) {
                MethodInfo[] funcs = mb.GetScriptFunctions(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                componentFuncs.Add(funcs);

                string[] compFuncNames = new string[funcs.Length];
                for (int i = 0; i < funcs.Length; i++) {
                    compFuncNames[i] = funcs[i].Name;
                }

                componentFuncNames.Add(compFuncNames);
            }


            string[] dropdownOptions = new string[eventObjectComponents.Count + 1];
            dropdownOptions[0] = "No function";

            for (int i = 0; i < eventObjectComponents.Count; i++) { dropdownOptions[i + 1] = eventObjectComponents[i].GetType().Name; }

            ddSelectedIndex = EditorGUI.Popup(rightRect, ddSelectedIndex, dropdownOptions);

            if (ddSelectedIndex > 0) {
                funcSelectedIndex = EditorGUI.Popup(new Rect(paddedRect.x,paddedRect.y + EditorGUIUtility.singleLineHeight,paddedRect.width,paddedRect.height), funcSelectedIndex, componentFuncNames[ddSelectedIndex - 1]);
            }
        }
    }
}

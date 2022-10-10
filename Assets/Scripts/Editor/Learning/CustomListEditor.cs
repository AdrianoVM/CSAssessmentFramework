namespace Learning
{
    using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
 
[CustomEditor(typeof(CustomList))]
 
public class CustomListEditor : Editor {
 
    enum displayFieldType {DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields}
    displayFieldType DisplayFieldType;
 
    CustomList t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;
 
    void OnEnable(){
        t = (CustomList)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("MyList"); // Find the List in our script and create a refrence of it
    }
 
    public override void OnInspectorGUI(){
        //Update our list
   
        GetTarget.Update();
   
        //Choose how to display the list<> Example purposes only
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("",DisplayFieldType);
   
        //Resize our list
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("Define the list size with a number");
        ListSize = ThisList.arraySize;
        ListSize = EditorGUILayout.IntField ("List Size", ListSize);
   
        if(ListSize != ThisList.arraySize){
            while(ListSize > ThisList.arraySize){
                ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
            }
            while(ListSize < ThisList.arraySize){
                ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
            }
        }
   
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("Or");
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
   
        //Or add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new item with a button");
   
        if(GUILayout.Button("Add New")){
            t.MyList.Add(new CustomList.MyClass());
        }
   
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
   
        //Display our list to the inspector window
 
        for(int i = 0; i < ThisList.arraySize; i++){
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyInt = MyListRef.FindPropertyRelative("AnInt");
            SerializedProperty MyFloat = MyListRef.FindPropertyRelative("AnFloat");
            SerializedProperty MyVect3 = MyListRef.FindPropertyRelative("AnVector3");
            SerializedProperty MyGO = MyListRef.FindPropertyRelative("AnGO");
            SerializedProperty MyArray = MyListRef.FindPropertyRelative("AnIntArray");
 
       
            // Display the property fields in two ways.
       
            if(DisplayFieldType == 0){// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
                //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
                EditorGUILayout.LabelField("Automatic Field By Property Type");
                EditorGUILayout.PropertyField(MyGO);
                EditorGUILayout.PropertyField(MyInt);
                EditorGUILayout.PropertyField(MyFloat);
                EditorGUILayout.PropertyField(MyVect3);
 
                // Array fields with remove at index
                EditorGUILayout.Space ();
                EditorGUILayout.Space ();
                EditorGUILayout.LabelField("Array Fields");
 
                if(GUILayout.Button("Add New Index",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                    MyArray.InsertArrayElementAtIndex(MyArray.arraySize);
                    MyArray.GetArrayElementAtIndex(MyArray.arraySize -1).intValue = 0;
                }
 
                for(int a = 0; a < MyArray.arraySize; a++){
                    EditorGUILayout.PropertyField(MyArray.GetArrayElementAtIndex(a));
                    if(GUILayout.Button("Remove  (" + a.ToString() + ")",GUILayout.MaxWidth(100),GUILayout.MaxHeight(15))){
                        MyArray.DeleteArrayElementAtIndex(a);
                    }
                }
            }else{
                //Or
           
                //2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
                EditorGUILayout.LabelField("Customizable Field With GUI");
                MyGO.objectReferenceValue = EditorGUILayout.ObjectField("My Custom Go", MyGO.objectReferenceValue, typeof(GameObject), true);
                MyInt.intValue = EditorGUILayout.IntField("My Custom Int",MyInt.intValue);
                MyFloat.floatValue = EditorGUILayout.FloatField("My Custom Float",MyFloat.floatValue);
                MyVect3.vector3Value = EditorGUILayout.Vector3Field("My Custom Vector 3",MyVect3.vector3Value);
 
 
                // Array fields with remove at index
                EditorGUILayout.Space ();
                EditorGUILayout.Space ();
                EditorGUILayout.LabelField("Array Fields");
 
                if(GUILayout.Button("Add New Index",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                    MyArray.InsertArrayElementAtIndex(MyArray.arraySize);
                    MyArray.GetArrayElementAtIndex(MyArray.arraySize -1).intValue = 0;
                }
 
                for(int a = 0; a < MyArray.arraySize; a++){
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("My Custom Int (" + a.ToString() + ")",GUILayout.MaxWidth(120));
                    MyArray.GetArrayElementAtIndex(a).intValue = EditorGUILayout.IntField("",MyArray.GetArrayElementAtIndex(a).intValue, GUILayout.MaxWidth(100));
                    if(GUILayout.Button("-",GUILayout.MaxWidth(15),GUILayout.MaxHeight(15))){
                        MyArray.DeleteArrayElementAtIndex(a);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
       
            EditorGUILayout.Space ();
       
            //Remove this index from the List
            EditorGUILayout.LabelField("Remove an index from the List<> with a button");
            if(GUILayout.Button("Remove This Index (" + i.ToString() + ")")){
                ThisList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
        }
   
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}
}
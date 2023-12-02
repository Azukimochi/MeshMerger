using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.Core;

public class MeshMergerGUI : EditorWindow
{
    private const string Title = "MeshMerger";
    
    private Vector2 _scrollPosition = Vector2.zero;
    
    
    [SerializeField]
    private GameObject[] _gameObjects;
    
    [MenuItem("Tools/MeshMerger")]
    public static void CreateWindow() => GetWindow<MeshMergerGUI>(Title);

    private void OnGUI()
    {
           
        EditorGUILayout.LabelField(Title, EditorStyles.boldLabel);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Select GameObjects to merge:");
        
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty("_gameObjects");
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Merge Meshes"))
        {
            string savePath = EditorUtility.SaveFilePanelInProject( "MergedMesh", "asset", "", "Assets");
            MeshMergerProcess.MergeMeshes(_gameObjects, savePath);
        }
    }
}

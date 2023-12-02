using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshMergerProcess : MonoBehaviour
{
    static public void MergeMeshes(GameObject[] gameObjects, string savePath)
    {
        // 新しいメッシュを作成
        Mesh combinedMesh = new Mesh();
        List<Material> materials = new List<Material>();

        // CombineInstance配列とマテリアルリストを作成
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var gameObject in gameObjects)
        {
            if (gameObject == null) continue;

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            MeshFilter mf = gameObject.GetComponent<MeshFilter>();

            if (mf == null || meshRenderer == null) continue;

            Material[] localMats = meshRenderer.sharedMaterials;
            for (int materialIndex = 0; materialIndex < localMats.Length; materialIndex++)
            {
                materials.Add(localMats[materialIndex]);

                CombineInstance ci = new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    subMeshIndex = materialIndex,
                    transform = gameObject.transform.localToWorldMatrix
                };
                combineInstances.Add(ci);
            }
        }

        // メッシュを統合
        combinedMesh.CombineMeshes(combineInstances.ToArray(), false);

        // 新しいメッシュにマテリアルを適用
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
        var renderer = combinedObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = materials.ToArray();

        // 統合したメッシュをアセットとして保存
        AssetDatabase.CreateAsset(combinedMesh, savePath + ".asset");

        // プレハブとして保存
        PrefabUtility.SaveAsPrefabAsset(combinedObject, savePath + ".prefab");

        // オブジェクトを削除（オプション）
        DestroyImmediate(combinedObject);
    }
}

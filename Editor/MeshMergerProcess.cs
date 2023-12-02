using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class MeshMergerProcess : MonoBehaviour
{
    static public void MergeMeshes(GameObject[] gameObjects, string savePath)
    {
        // 新しいメッシュを作成
        Mesh combinedMesh = new Mesh();

        // マテリアルとそれに関連するCombineInstanceを格納するディクショナリ
        Dictionary<Material, List<CombineInstance>> materialToCombineInstances = new Dictionary<Material, List<CombineInstance>>();

        foreach (var gameObject in gameObjects)
        {
            if (gameObject == null) continue;

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            MeshFilter mf = gameObject.GetComponent<MeshFilter>();

            if (mf == null || meshRenderer == null) continue;

            Material[] localMats = meshRenderer.sharedMaterials;
            for (int materialIndex = 0; materialIndex < localMats.Length; materialIndex++)
            {
                if (!materialToCombineInstances.ContainsKey(localMats[materialIndex]))
                {
                    materialToCombineInstances[localMats[materialIndex]] = new List<CombineInstance>();
                }

                CombineInstance ci = new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    subMeshIndex = materialIndex,
                    transform = gameObject.transform.localToWorldMatrix
                };
                materialToCombineInstances[localMats[materialIndex]].Add(ci);
            }
        }

        // 統合されたメッシュとマテリアルのリスト
        List<Mesh> combinedMeshes = new List<Mesh>();
        List<Material> materials = new List<Material>();

        foreach (var materialCombineInstances in materialToCombineInstances)
        {
            Mesh combinedMeshForMaterial = new Mesh();
            combinedMeshForMaterial.CombineMeshes(materialCombineInstances.Value.ToArray(), true);
            combinedMeshes.Add(combinedMeshForMaterial);
            materials.Add(materialCombineInstances.Key);
        }

        // 統合されたメッシュを組み合わせる
        CombineInstance[] finalCombine = new CombineInstance[combinedMeshes.Count];
        for (int i = 0; i < combinedMeshes.Count; i++)
        {
            finalCombine[i].mesh = combinedMeshes[i];
            finalCombine[i].transform = Matrix4x4.identity;
        }

        combinedMesh.CombineMeshes(finalCombine, false, false);

        // 新しいメッシュにマテリアルを適用
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
        var renderer = combinedObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = materials.ToArray();

        // 統合したメッシュをアセットとして保存
        AssetDatabase.CreateAsset(combinedMesh, savePath + ".asset");
        PrefabUtility.SaveAsPrefabAsset(combinedObject, savePath + ".prefab");

        // オブジェクトを削除（オプション）
        DestroyImmediate(combinedObject);
    }
}

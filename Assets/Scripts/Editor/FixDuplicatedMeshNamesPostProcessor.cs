using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

public class FixDuplicateMeshNamesPostProcessor : AssetPostprocessor
{
    private void OnPostprocessModel(GameObject go)
    {
        // Padrão para encontrar meshes com nome bolt1, bolt2, etc.
        // Também captura bolt_1, bolt01, etc.
        Regex boltPattern = new Regex(@"bolt\d+", RegexOptions.IgnoreCase);

        // Encontra todos os MeshFilter na hierarquia
        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            string meshName = mf.sharedMesh.name;

            // Verifica se é um bolt (ou qualquer padrão que queira tratar)
            if (boltPattern.IsMatch(meshName) || meshName.Contains("bolt"))
            {
                // Gera um nome único baseado no caminho completo do objeto
                string uniqueName = GenerateUniqueName(mf, meshName);

                // Cria uma cópia única da mesh
                Mesh uniqueMesh = Object.Instantiate(mf.sharedMesh);
                uniqueMesh.name = uniqueName;
                uniqueMesh.hideFlags = HideFlags.HideAndDontSave;

                // Substitui pela mesh única
                mf.sharedMesh = uniqueMesh;

                // Log opcional para debug (descomente se quiser ver o que foi alterado)
                // Debug.Log($"Mesh renomeada: {meshName} -> {uniqueName} no objeto {mf.gameObject.name}");
            }
        }

        // Também processa SkinnedMeshRenderer se houver
        SkinnedMeshRenderer[] skinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            if (smr.sharedMesh == null) continue;

            string meshName = smr.sharedMesh.name;

            if (boltPattern.IsMatch(meshName) || meshName.Contains("bolt"))
            {
                string uniqueName = GenerateUniqueName(smr, meshName);

                Mesh uniqueMesh = Object.Instantiate(smr.sharedMesh);
                uniqueMesh.name = uniqueName;
                uniqueMesh.hideFlags = HideFlags.HideAndDontSave;

                smr.sharedMesh = uniqueMesh;
            }
        }
    }

    private string GenerateUniqueName(Component component, string originalName)
    {
        // Usa o caminho completo do objeto + instance ID para garantir unicidade
        string path = GetGameObjectPath(component.gameObject);
        string sanitizedPath = path.Replace("/", "_").Replace(" ", "");

        // Limita o tamanho para não ficar enorme
        if (sanitizedPath.Length > 50)
        {
            sanitizedPath = sanitizedPath.Substring(sanitizedPath.Length - 50);
        }

        return $"{originalName}_{sanitizedPath}_{component.GetInstanceID()}";
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
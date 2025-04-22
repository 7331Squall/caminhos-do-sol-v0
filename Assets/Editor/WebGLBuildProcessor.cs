using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;

public class WebGLBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const string Placeholder = "{{OUTPUT}}";

    public void OnPostprocessBuild(BuildReport report) {
        if (report.summary.platform != BuildTarget.WebGL) return;

        // Pega o nome da pasta final da build
        string outputFolder = Path.GetFileName(report.summary.outputPath.TrimEnd(Path.DirectorySeparatorChar));

        // Caminho do index.html gerado pela Unity
        string indexPath = Path.Combine(report.summary.outputPath, "index.html");
        if (File.Exists(indexPath)) {
            string html = File.ReadAllText(indexPath);
            html = html.Replace(Placeholder, outputFolder);
            File.WriteAllText(indexPath, html);
            Debug.Log($"[BuildProcessor] Substituído '{Placeholder}' por '{outputFolder}' no index.html");
        } else { Debug.LogWarning("[BuildProcessor] index.html não encontrado na pasta de build."); }
    }
}
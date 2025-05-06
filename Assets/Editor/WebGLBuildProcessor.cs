using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;

public class WebGLBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const string OutputPH = "{{OUTPUT}}";
    private const string OutputEX = "{{EXTENSION}}";

    public void OnPostprocessBuild(BuildReport report) {
        if (report.summary.platform != BuildTarget.WebGL) return;

        // Pega o nome da pasta final da build
        string outputFolder = Path.GetFileName(report.summary.outputPath.TrimEnd(Path.DirectorySeparatorChar));
        string extension = GetCompressionExtension();

        // Caminho do index.html gerado pela Unity
        string indexPath = Path.Combine(report.summary.outputPath, "index.html");
        if (File.Exists(indexPath)) {
            string html = File.ReadAllText(indexPath);
            html = html.Replace(OutputPH, outputFolder).Replace(OutputEX, extension);
            File.WriteAllText(indexPath, html);
            Debug.Log($"[BuildProcessor] Substituído '{OutputPH}' por '{outputFolder}' no index.html");
            Debug.Log($"[BuildProcessor] Substituído '{OutputEX}' por '{extension}' no index.html");
        } else { Debug.LogWarning("[BuildProcessor] index.html não encontrado na pasta de build."); }
    }

    string GetCompressionExtension() {
        switch (PlayerSettings.WebGL.compressionFormat) {
            case WebGLCompressionFormat.Gzip:   return ".gz";
            case WebGLCompressionFormat.Brotli: return ".br";
            case WebGLCompressionFormat.Disabled:
            default: return "";
        }
    }
}
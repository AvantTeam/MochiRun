#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class BuildAnnotations : IPreprocessBuildWithReport {
    public int callbackOrder { get { return 0; } }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report) {
        // Do the preprocessing here
        //throw new System.NotImplementedException();
        BuildTarget target = report.summary.platform;
        string path = report.summary.outputPath;
        Debug.Log("----T: " + target.ToString() + " PATH: " + path + "----");

    }
}
#endif
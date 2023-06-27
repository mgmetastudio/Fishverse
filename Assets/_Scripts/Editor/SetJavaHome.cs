#if UNITY_EDITOR
using UnityEditor;

public static class SetJavaHome
{
    [InitializeOnLoadMethod]
    private static void SetJavaHomePath()
    {
        string unityJavaHome = EditorPrefs.GetString("JdkPath");

        if (unityJavaHome == null || unityJavaHome == string.Empty)
            unityJavaHome = @"D:\DevSoft\UnityEngine\UnityEditors\2022.1.19f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK";

        if (unityJavaHome != null && unityJavaHome != string.Empty)
        {
            string javaHome = System.Environment.GetEnvironmentVariable("JAVA_HOME");

            if (javaHome != unityJavaHome)
            {
                System.Environment.SetEnvironmentVariable("JAVA_HOME", unityJavaHome);
                UnityEngine.Debug.Log("JAVA_HOME set to: " + unityJavaHome);
            }
        }
    }
}
#endif

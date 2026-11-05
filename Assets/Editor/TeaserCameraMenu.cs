using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Tools > Add Teaser Camera From Scene View: drops a fixed CinemachineCamera where the Scene view is looking and
/// appends it to the open scene's TeaserRig detail shots, so it gets an F-key and renders on the Display 2 RecordCam.
/// Move it like any object afterwards (GameObject > Align With View snaps it to the Scene view again).
/// </summary>
public static class TeaserCameraMenu
{
    [MenuItem("Tools/Add Teaser Camera From Scene View")]
    private static void Add()
    {
        var rig = Object.FindAnyObjectByType<TeaserRig>(FindObjectsInactive.Include);
        var view = SceneView.lastActiveSceneView;
        if (!rig || !view)
        {
            EditorUtility.DisplayDialog("Teaser camera", "Needs a scene with a TeaserRig and an open Scene view.", "OK");
            return;
        }

        var so = new SerializedObject(rig);
        var shots = so.FindProperty("detailShots");
        var parent = rig.transform.Find("DetailShots") ?? rig.transform;

        var go = new GameObject($"Custom {shots.arraySize + 1}");
        Undo.RegisterCreatedObjectUndo(go, "Add teaser camera");
        go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(view.camera.transform.position, view.camera.transform.rotation);
        var cam = go.AddComponent<CinemachineCamera>();
        cam.Lens.FieldOfView = 35;
        cam.Priority.Value = 100;
        // Record channel only, so the gameplay view on Display 1 never switches to it.
        cam.OutputChannel = OutputChannels.Channel03;

        shots.arraySize++;
        shots.GetArrayElementAtIndex(shots.arraySize - 1).objectReferenceValue = cam;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(rig.gameObject.scene);
        Selection.activeGameObject = go;
        var key = shots.arraySize <= 11 ? $"F{shots.arraySize}" : "[ / ]";
        Debug.Log($"Teaser camera: added {go.name} ({key} in Play Mode). Save the scene to keep it.");
    }
}

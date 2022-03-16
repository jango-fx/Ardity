using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SerialController))]
public class SerialControllerEditor : Editor
{
    SerialController serialController;
    List<string> portsList = new List<string>();
    List<string> portNames = new List<string>();
    int portID;

    SerializedProperty _baudRate = null;
    SerializedProperty _messageListener = null;
    SerializedProperty _reconnectionDelay = null;
    SerializedProperty _messageQueueSize = null;
    SerializedProperty _queueBehaviour = null;
    SerializedProperty _dropOldMessage = null;
    SerializedProperty _dtrEnable = null;
    SerializedProperty _rtsEnable = null;

    void Awake()
    {
        serialController = (SerialController)target;

        RefreshPortList();
        int editorID = portsList.IndexOf(serialController.portName);
        if (editorID >= 0)
            portID = editorID;
        else
            Debug.Log("[SERIAL]: ›" + serialController.portName + "‹ not found");

        _baudRate = serializedObject.FindProperty("baudRate");
        _messageListener = serializedObject.FindProperty("messageListener");
        _reconnectionDelay = serializedObject.FindProperty("reconnectionDelay");
        _messageQueueSize = serializedObject.FindProperty("messageQueueSize");
        _queueBehaviour = serializedObject.FindProperty("queueBehaviour");
        _dropOldMessage = serializedObject.FindProperty("dropOldMessage");
        _dtrEnable = serializedObject.FindProperty("dtrEnable");
        _rtsEnable = serializedObject.FindProperty("rtsEnable");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Refresh Ports List")) { RefreshPortList(); }

        GUIContent portSelection = new GUIContent("Select Serial Port: ");
        portID = EditorGUILayout.Popup(portSelection, portID, portNames.ToArray());
        serialController.portName = portsList[portID];

        serializedObject.Update();
        EditorGUILayout.PropertyField(_baudRate);
        EditorGUILayout.PropertyField(_messageListener);
        EditorGUILayout.PropertyField(_reconnectionDelay);
        EditorGUILayout.PropertyField(_messageQueueSize);
        EditorGUILayout.PropertyField(_queueBehaviour);
        EditorGUILayout.PropertyField(_dropOldMessage);
        EditorGUILayout.PropertyField(_dtrEnable);
        EditorGUILayout.PropertyField(_rtsEnable);
        serializedObject.ApplyModifiedProperties();
    }

    void RefreshPortList()
    {
        portsList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
        portNames = new List<string>(portsList);

        for (int i = portNames.Count - 1; i >= 0; i--)
        {
// Filter false ports under macOS. (Same for Linux ?)
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (!portNames[i].StartsWith("/dev/tty."))
            {
                portsList.RemoveAt(i);
                portNames.RemoveAt(i);
            }
            else
#endif
            {
                // Simplify names (under macOS).
                string[] split = portNames[i].Split('/');
                split = split[split.Length - 1].Split('.');
                portNames[i] = "[" + i + "]: " + split[split.Length - 1];
            }
        }

        Debug.Log(GetPortNames());
    }

    string GetPortNames()
    {
        string msg = "[SERIAL]: found " + portsList.Count + " ports.";
        for (int i = 0; i < portsList.Count; i++)
        {
            msg += "\n\t" + portNames[i];
        }
        return msg;
    }
}
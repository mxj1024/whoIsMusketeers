using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventBool : UnityEvent<bool> { }
[System.Serializable]
public class UnityEventInt : UnityEvent<int> { }
[System.Serializable]
public class UnityEventFloat : UnityEvent<float> { }
[System.Serializable]
public class UnityEventString : UnityEvent<string> { }
[System.Serializable]
public class UnityEventGameObject : UnityEvent<GameObject> { }

[System.Serializable]
public class UnityEventBoolSender : UnityEvent<bool, GameObject> { }
[System.Serializable]
public class UnityEventIntSender : UnityEvent<int, GameObject> { }
[System.Serializable]
public class UnityEventFloatSender : UnityEvent<float, GameObject> { }
[System.Serializable]
public class UnityEventStringSender : UnityEvent<string, GameObject> { }
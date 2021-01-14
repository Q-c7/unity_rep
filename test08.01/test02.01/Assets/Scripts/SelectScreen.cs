using System.Linq;
using UnityEngine;

public class SelectScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var args = Storage.GetArgs();
        var monitor = int.Parse((from arg in args where arg.Key == "--monitor" select arg).FirstOrDefault().Value);
        var camera = gameObject.GetComponent<Camera>();
        Display.displays[monitor - 1].Activate();
        camera.SetTargetBuffers(Display.displays[monitor - 1].colorBuffer, Display.displays[monitor - 1].depthBuffer);
        camera.targetDisplay = monitor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

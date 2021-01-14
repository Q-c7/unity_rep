using UnityEngine;
using UnityEngine.UI;

namespace AsImpL
{
    namespace Examples
    {
        /// <summary>
        /// Demonstrate how to load a model with ObjectImporter.
        /// </summary>
        public class AsImpLSample : MonoBehaviour
        {
            public InputField inputPath;
            //[SerializeField]
            //private string filePath = "models/OBJ_test/objtest_zup.obj";
            [SerializeField]
            public string objectName = "MyObject";
            [SerializeField]
            private ImportOptions importOptions = new ImportOptions();

            [SerializeField]
            private PathSettings pathSettings;

            private ObjectImporter objImporter;


            private void Awake()
            {
                inputPath.text = pathSettings.RootPath + inputPath.text;
                objImporter = gameObject.GetComponent<ObjectImporter>();
                if (objImporter == null)
                {
                    objImporter = gameObject.AddComponent<ObjectImporter>();
                }
            }


            public void ImportModel()
            {
                objImporter.ImportModelAsync(objectName, inputPath.text, null, importOptions);
                
            }


            private void OnValidate()
            {
                if(pathSettings==null)
                {
                    pathSettings = PathSettings.FindPathComponent(gameObject);
                }
            }

        }
    }
}

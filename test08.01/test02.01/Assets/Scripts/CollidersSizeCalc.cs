using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType { None, Up, Down, Forward, Backward, Left, Right };
public enum ColliderTarget { Bridge, MainTrolley, AuxTrolley, MtMh, MtAh, AtMh, AtAh };

public class CollidersSizeCalc : MonoBehaviour
{
    // Start is called before the first frame update

    public bool blockResizeWithHookPosition; //Колайдер не изменяет размер в высоту при перемещении крюка вверх/низ
    public bool isWarning = true;
    public bool isWeightColliders = false; //Колайдер должен изменять размер при изменении размера груза
    public bool blockAllResizes = false; //Запрещено любое изменение размера, кроме глобального размера настроек расстояний и аварий
    //public float addition; //Добавляет эти значения к размерам коллайдера
    public Vector3 sizeAdditions; //Поправки, связанные с размером груза
    public Vector3 posCorrections;
    public ColliderType type;
    public ColliderTarget target;
    private Vector3 cubeInitialPosition;
    private Vector3 cubeInitialScale;
    private Vector3 colliderInitialSize;
    private BoxCollider colliderObj;
    public float zMaxPoint = 29000f;
    private Vector3 colliderInitialPosition;
   
    void Start()
    {
        if(gameObject != null)
        {
            cubeInitialPosition = gameObject.transform.parent.position;
            cubeInitialScale = gameObject.transform.localScale;
            colliderObj = gameObject.GetComponent<BoxCollider>();
            colliderInitialSize = colliderObj.size;
            colliderInitialPosition = gameObject.transform.localPosition;
            //Debug.Log(rightColliderInitialPosition);
            //scaleYCoeff = rightCollider.
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject != null)
        {
            var collider = colliderObj;
            if(collider != null)
            {
                var size = collider.size;
                size.z = (Globals.scaleZ * (isWarning ? Globals.warningDistance : Globals.alarmDistance) + sizeAdditions.z) / collider.transform.lossyScale.z;
                //size.y = colliderInitialSize.y + sizeAdditions.y;
                //size.x = colliderInitialSize.x + sizeAdditions.x + 0.5f;
                collider.size = size;
                var center = collider.center;
                center.z = size.z / 2.0f;
                //center.y = -sizeAdditions.y / 2;
                collider.center = center;
            }

            if (blockAllResizes) return;

            Vector3 pos = gameObject.transform.position;
            Vector3 scale = cubeInitialScale;


            //scale += sizeAdditions / collider.transform.lossyScale.z;
            float dY = cubeInitialPosition.y - gameObject.transform.parent.position.y + sizeAdditions.y;
            if (!blockResizeWithHookPosition)
            {
                
                //Debug.Log(dY);
                scale.y = cubeInitialScale.y + dY / (gameObject.transform.parent.localScale.y);
                pos.y = cubeInitialPosition.y - dY / 2;
                //gameObject.transform.localScale = scale;
            }
            else
            {
                scale.y = sizeAdditions.y > 0 ? sizeAdditions.y / gameObject.transform.parent.localScale.y : cubeInitialScale.y;
                /*if(!isHook)
                    pos.y = cubeInitialPosition.y - gameObject.transform.localScale.y / 2;*/
            }
            
            //gameObject.transform.localPosition = cubeInitialPosition + new Vector3(posCorrections.x, 0, posCorrections.z);
            //float dX = posCorrections.x;// - gameObject.transform.parent.position.x;
            //float dZ = posCorrections.z;// - gameObject.transform.parent.position.z;
            //pos.x = cubeInitialPosition.x + dX * gameObject.transform.parent.localScale.x;
            //pos.z = cubeInitialPosition.z + dZ * gameObject.transform.parent.localScale.z;
            gameObject.transform.position = pos;
            float localY = 0.0f;
            if (isWeightColliders && type != ColliderType.Down && type != ColliderType.Up)
            {
                localY = colliderInitialPosition.y - gameObject.transform.localScale.y / 4;
            }else
            {
                localY = gameObject.transform.localPosition.y;
            }
            if(isWeightColliders)
                gameObject.transform.localPosition = new Vector3(posCorrections.x / gameObject.transform.parent.localScale.x, localY, posCorrections.z);
            scale.x = /*cubeInitialScale.x + */sizeAdditions.x / (gameObject.transform.parent.localScale.x);
            if (scale.x < 1.0f) scale.x = 1.0f;
            gameObject.transform.localScale = scale;

            //gameObject.transform.localScale += new Vector3(0, sizeAdditions.y);

        }
    }
}

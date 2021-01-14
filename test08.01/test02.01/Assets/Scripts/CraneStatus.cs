using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneStatus : MonoBehaviour
{
    public CraneConnection connection;
    public Image powerOnImage;
    public Image mainContactorImage;
    public Image generalFaultImage;
    public Image weightLimiterImage;
    public Image bridgeSensorImage;
    public Image mainTrolleySensorImage;
    public Image auxTrolleySensorImage;
    public Image mtMhSensorImage;
    public Image mtAhSensorImage;
    public Image atMhSensorImage;
    public Image atAhSensorImage;
    private Color on;
    private Color off;
    private Color disconnected;

    // Start is called before the first frame update
    void Start()
    {
        on = new Color(0, 1, 0);
        off = new Color(1, 0, 0);
        disconnected = new Color(0.5f, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(connection != null)
        {
            if (powerOnImage != null)
            {
                if (connection.connected)
                {
                    if(powerOnImage != null)
                        powerOnImage.color = connection.status.statuses.powerOn ? on : off;
                    if(mainContactorImage != null)
                        mainContactorImage.color = connection.status.statuses.mainContactorOn ? on : off;
                    if(generalFaultImage != null)
                        generalFaultImage.color = connection.status.statuses.generalFault ? on : off;
                    if(weightLimiterImage != null)
                        weightLimiterImage.color = connection.status.statuses.weightLimit ? on : off;
                    if(bridgeSensorImage != null)
                        bridgeSensorImage.color = connection.status.sensors.bridge ? on : off;
                    if(mainTrolleySensorImage != null)
                        mainTrolleySensorImage.color = connection.status.sensors.mainTrolley ? on : off;
                    if(auxTrolleySensorImage != null)
                        auxTrolleySensorImage.color = connection.status.sensors.auxTrolley ? on : off;
                    if(mtMhSensorImage != null)
                        mtMhSensorImage.color = connection.status.sensors.mtMainHoist ? on : off;
                    if(mtAhSensorImage != null)
                        mtAhSensorImage.color = connection.status.sensors.mtAuxHoist ? on : off;
                    if(atMhSensorImage != null)
                        atMhSensorImage.color = connection.status.sensors.atMainHoist ? on : off;
                    if(atAhSensorImage != null)
                        atAhSensorImage.color = connection.status.sensors.atAuxHoist ? on : off;
                }
                else
                {
                    if (powerOnImage != null)
                        powerOnImage.color = disconnected;
                    if (mainContactorImage != null)
                        mainContactorImage.color = disconnected;
                    if (generalFaultImage != null)
                        generalFaultImage.color = disconnected;
                    if (weightLimiterImage != null)
                        weightLimiterImage.color = disconnected;
                    if (bridgeSensorImage != null)
                        bridgeSensorImage.color = disconnected;
                    if (mainTrolleySensorImage != null)
                        mainTrolleySensorImage.color = disconnected;
                    if (auxTrolleySensorImage != null)
                        auxTrolleySensorImage.color = disconnected;
                    if (mtMhSensorImage != null)
                        mtMhSensorImage.color = disconnected;
                    if (mtAhSensorImage != null)
                        mtAhSensorImage.color = disconnected;
                    if (atMhSensorImage != null)
                        atMhSensorImage.color = disconnected;
                    if (atAhSensorImage != null)
                        atAhSensorImage.color = disconnected;
                }
                    
            }
        }
    }
}

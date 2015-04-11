using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackGraph : MonoBehaviour
{

    public enum trackValue
    {
        off,
        xAxis,
        yAxis,
        xSpeed,
        ySpeed,
        LeftRight,
        UpDown
    }


    public bool averageMode;

    public List<GameObject> trackedObjects;
    public int selector;
    public trackValue trackedValueType;
    public Character.Animals whoIs = Character.Animals.CHICKEN;
    private Character.Animals lastWhoIs;
    public int graphPoints;
    public float lineWidth = 0.5f;
    public Material lineMaterial;
    public Color lineColor;
    public Vector2 scale, offset;

    float lastValue;
    float[] data;
    float average;
    bool isCharacter;
    Character character;

    LineRenderer lineRenderer;

    GameObject UICamera;

    float initialPositionZ;
    // Use this for initialization
    void Start()
    {
         GameObject go = new GameObject();

         UICamera = GameObject.Find("UiCamera");

        if (UICamera)
        {
        go.transform.parent = UICamera.transform;
        initialPositionZ = UICamera.transform.localPosition.z;
        go.transform.localPosition = new Vector3(0, 0, -initialPositionZ);
        }
        else
        {
             go.transform.parent = this.transform;
        }

        go.layer = LayerMask.NameToLayer("UI");
        go.transform.localPosition = Vector3.zero;
        lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.SetVertexCount(graphPoints);

        data = new float[graphPoints];
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetWidth(lineWidth, lineWidth);
        lineRenderer.material = lineMaterial;
        lineRenderer.SetColors(lineColor, lineColor);

       Character[] trackedcharacter = FindObjectsOfType<Character>();

       
       for (int i = 0; i < trackedcharacter.Length; i++)
       {
           trackedObjects.Add(trackedcharacter[i].gameObject);

       }

          
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackedValueType == trackValue.off)
        {
            lineRenderer.enabled = false;
            return;
        }
        else
        {
            lineRenderer.enabled = true;
        }


        if (whoIs != lastWhoIs)
        {
            isCharacter = false;
            character = null;
            for(int i = 0; i < trackedObjects.Count ;i++ ){
                if (trackedObjects[i].GetComponent<Character>().CharacterSprite == whoIs)
                {
                    selector = i;
                    character = trackedObjects[i].GetComponent<Character>();
                    isCharacter = true;
                } 
            }
            lastWhoIs = whoIs;
        }
   

       

        average = 0;
        for (int i = 0; i < graphPoints-1; i++)
        {
            average += data[i];
            data[i] = data[i + 1];
        }
       
        float newData;

        switch (trackedValueType)
	{
            case trackValue.yAxis:
            newData = trackedObjects[selector].transform.position.y;
                break;
            case trackValue.xAxis:
                newData = trackedObjects[selector].transform.position.x;
                break;
            case trackValue.xSpeed:
                newData = lastValue - trackedObjects[selector].transform.position.x;
                lastValue = trackedObjects[selector].transform.position.x;
                break;
            case trackValue.ySpeed:
                newData = -(lastValue - trackedObjects[selector].transform.position.y);
                lastValue = trackedObjects[selector].transform.position.y;
                break;
            case trackValue.LeftRight:
                if (isCharacter)
                {
                    if (character.left > 0)
                        newData = -character.left;
                    else if(character.right>0)
                        newData = character.right;
                    else
                        newData = 0f;
                
                }
                else  newData = 0f; 

                    break;
            case trackValue.UpDown:
                    if (isCharacter)
                    {
                        if (character.up > 0)
                           newData = character.up;
                        else if (character.down > 0)
                            newData = -character.down;
                        else
                            newData = 0f;
                    }
                 else  newData = 0f;
                    break;
            default:
                newData = 0;
                break;
	}

        data[graphPoints - 1] = newData;

        average = average / graphPoints;

        for (int i = 0; i < graphPoints; i++)
        {
            if (averageMode)
            {
                lineRenderer.SetPosition(i, new Vector3(i * scale.x + offset.x, (data[i] - average) * scale.y + offset.y, -initialPositionZ));
            }
            else
            {
                lineRenderer.SetPosition(i, new Vector3(i * scale.x + offset.x, (data[i]) * scale.y + offset.y, -initialPositionZ));
            }
        }

       

	}
}

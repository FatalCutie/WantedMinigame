using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Face : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameManager gm;
    [SerializeField] private SpriteRenderer sr;
    public enum FaceID { FACE1, FACE2, FACE3, FACE4 };
    public bool movementLocked = true;
    public FaceID faceID;
    public float x, y = 1;
    #endregion

    void FixedUpdate()
    {
        //Movement unlocked and initilized by FaceAI
        if (!movementLocked)
        {
            transform.position += new Vector3(x, y, 0);

            //Out of bounds checks
            if (transform.position.x > 9.8) transform.position = new Vector3(-9.8f, transform.position.y, 0);
            else if (transform.position.x < -9.8) transform.position = new Vector3(9.8f, transform.position.y, 0); //this might not work
            if (transform.position.y >= 5.5) transform.position -= new Vector3(0, 11f, 0);
            else if (transform.position.y <= -5.5) transform.position += new Vector3(0, 11f, 0); //this might not work
        }

        //Magic numbers are already adjusted
        //Magic number: y: -5.5, 5.5
        //X: -11, 7.5 
    }

    //Initilizes FaceID as well as sprite
    //This needs to take into account there can only be 1 wanted
    public void Initilize(FaceAI.Modifier modifier)
    {
        gm = FindObjectOfType<GameManager>();
        FaceID wantedID = gm.wanted;
        faceID = (FaceID)Random.Range(0, 4);

        if (modifier == FaceAI.Modifier.MOVING) ConfigFaceMovement(-5, -5);

        //If wanted is generated mark it as so, if duplicate wanted is generated then reroll
        if (faceID == wantedID && !gm.wantedGenerated) gm.wantedGenerated = true;
        else
        {
            while (faceID == wantedID)
            {
                faceID = (FaceID)Random.Range(0, 4);
            }
        }

        //Assign appropriate sprite based off ID
        switch (faceID)
        {
            case FaceID.FACE1:
                sr.sprite = gm.spriteList[0];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE2:
                sr.sprite = gm.spriteList[1];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE3:
                sr.sprite = gm.spriteList[2];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE4:
                sr.sprite = gm.spriteList[3];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
        }

    }

    //Manual override to create a Wanted face
    public void InitilizeAsWanted()
    {
        faceID = gm.wanted;
        switch (faceID)
        {
            case FaceID.FACE1:
                sr.sprite = gm.spriteList[0];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE2:
                sr.sprite = gm.spriteList[1];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE3:
                sr.sprite = gm.spriteList[2];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
            case FaceID.FACE4:
                sr.sprite = gm.spriteList[3];
                if (!gm.faces.Contains(this)) gm.faces.Add(this);
                break;
        }
    }

    public void ConfigFaceMovement(float xx, float yy)
    {
        this.x = xx * 0.01f;
        this.y = yy * 0.01f;
        movementLocked = false;
    }
}

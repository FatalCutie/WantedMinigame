using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEngine;

public class FaceAI : MonoBehaviour
{
    private GameManager gm;
    public GameObject emptyFacePrefab;
    public GameObject facesContainer;
    public int facesToGenerate;
    [SerializeField] private bool fullScreenGeneration = true;
    [SerializeField] private GameObject screenBorderHolder;
    private bool firstRun = true;
    public int roundsWon;

    void Awake(){
        screenBorderHolder.SetActive(false);
    }

    void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.F)) roundsWon++;
    }

    public void InitilizeNewRound(){
        //Clear faces from previous round (if applicable)
        if(firstRun) {
            firstRun = !firstRun;
        } else ClearPreviousFaces();

        SetGameBorders(); //Adjust play field based on parameters
        GenerateRoundDetails(); //Determine difficulty, generate faces
    }

    //Determines pattern, number of faces for round
    void GenerateRoundDetails(){
        int points = roundsWon * 10;
            if(points < 30)
            {
                GenerateFacesInSquare(); //Tutorial rounds
            } 
            //TODO: Further increase algorithm complexity: spend points for difficulty?
            //lower % of options being picked too many times in a row?
            else{
                GenerateFaces();
            }

    }

    //Generates faces in a random scatter pattern
    void GenerateFaces(){
        for(int i = 0; i < facesToGenerate; i++){
            //Generate a new vector 3 within camera view
            //Between X -10 > 7.5, Y -4.5 > 4.5,
            float x, y;
            //TODO: Keep list of Vector2s to prevent overlapping?
            if(fullScreenGeneration){
                x = UnityEngine.Random.Range(-8.8f, 8.8f);
                y = UnityEngine.Random.Range(-4.5f, 4.5f);
            } else{ //Easy Difficulty, less of a playing field
                //Not updated
                x = UnityEngine.Random.Range(-5.5f, 5.5f);
                y = UnityEngine.Random.Range(-4.5f, 4.5f);
            }
            GameObject go = Instantiate(emptyFacePrefab, new Vector3(x, y, 0f), Quaternion.identity, facesContainer.transform);
            Face goFace = go.GetComponent<Face>();
            //goFace.ConfigFaceMovement(.02f, .02f,fullScreenGeneration);
            goFace.Initilize();
        }
    }

    //Generates faces in a square for first few rounds
    void GenerateFacesInSquare(){
        switch(roundsWon){
            case 0: 
                //Generate 2x2 square
                for(int i = 0; i < 2; i++){
                    float startingY = -0.5f + 1 * i;
                    for(int j = 0; j < 2; j++){
                        float startingX = -0.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize();
                    }
                }
                return;
            case 1:
                //Generate 4x4
                    for(int i = 0; i < 4; i++){
                    float startingY = -1.5f + 1 * i;
                    for(int j = 0; j < 4; j++){
                        float startingX = -1.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize();
                    }
                }
                return;
            case 2:
                //Generate 8x8
                for(int i = 0; i < 8; i++){
                    float startingY = -3.5f + 1 * i;
                    for(int j = 0; j < 8; j++){
                        float startingX = -3.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize();
                    }
                }
                return;
        }
    }

    //Generate Faces in rectangles that move vertically
    void GenerateFacesInMovingRectangles(){

    }

    //Wipes all faces from previous round (and list)
    void ClearPreviousFaces(){
        try{
            foreach(Face f in gm.faces){
                Destroy(f.gameObject);
            } 
            gm.faces.Clear();
        }catch(MissingReferenceException){}
    }

    //If generating smaller screen of faces, have borders
    void SetGameBorders(){
        screenBorderHolder.SetActive(!fullScreenGeneration);
    }
}

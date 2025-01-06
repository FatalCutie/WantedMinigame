using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    public FaceAI fAI;
    private float timer = 10;
    public TextMeshProUGUI timerText;
    public List<Sprite> spriteList;
    private bool paused = false;
    public Face.FaceID wanted;
    public Image wantedSpriteRenderer;
    public List<Face> faces;
    [HideInInspector] public bool wantedGenerated = false; //this will flip whenever a wanted is generated
    public GameObject incorrectScore;
    public GameObject correctScore;
    public GameObject playAgainUI;
    [SerializeField] private GameObject littleStarHolder;
    [SerializeField] private GameObject bigStar;
    public List<Image> stars;
    #endregion

    void Awake(){
    }

    void Start(){
        fAI = FindObjectOfType<FaceAI>();
        timerText.text = $"{(int)timer}";
        playAgainUI.SetActive(false);
        FindObjectOfType<AudioManager>().Play("bgm");
        StartNewRound();
        InitilizeStars();
    }

    void Update(){
        //if a head is clicked
        if (Input.GetMouseButtonDown(0) && Clicked()) { 
            //Do Something
        }

        //Timer controls
        if(timer >= 0 && !paused){
            timer -= Time.deltaTime;
            timerText.text = $"{(int)Math.Ceiling(timer)}";
        if(timer > 99){ //clamp timer value
            timer = 99f;
        }
        } else if (timer < 0){
            timer = 0;
            StartCoroutine("GameOver");
        }

        //Debug cheat code
        if(Input.GetKeyDown(KeyCode.KeypadPlus)) timer += 10;
    }

    void StartNewRound(){
        GenerateNewWanted(); //Select a new wanted face
        wantedGenerated = false;
        //Run AI to determine new faces
        //Generate faces under list
        //Initilize faces with info and movement data
        fAI.InitilizeNewRound();
        DoubleCheckForWanted(); //Double check wanted is generated for insurance
    }

    //Generate a new wanted, run DisplayWanted to update sprite ingame
    void GenerateNewWanted(){
        wanted =(Face.FaceID)UnityEngine.Random.Range(0, 4);
        DisplayNewWanted();
    }

    //Updates wanted sprite with current sprite
    void DisplayNewWanted(){
        switch(wanted){
            case Face.FaceID.FACE1:
                wantedSpriteRenderer.sprite = spriteList[0];
                break;
            case Face.FaceID.FACE2:
                wantedSpriteRenderer.sprite = spriteList[1];
                break;
            case Face.FaceID.FACE3:
                wantedSpriteRenderer.sprite = spriteList[2];
                break;
            case Face.FaceID.FACE4:
                wantedSpriteRenderer.sprite = spriteList[3];
                break;
        }
    }

    //Checks if a head is clicked
    GameObject Clicked(){
        if(!paused){ //Disable click detection between rounds
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            //If what was clicked is a face
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Face")) {
                GameObject suspect = hit.collider.gameObject;
                //Reward or punish player based on if click was correct/incorrect
                if(CheckIfWantedWasClicked(suspect)){
                    StartCoroutine(CorrectFaceClicked(suspect));
                } else StartCoroutine(IncorrectFaceClicked(suspect));
                return hit.collider.gameObject;
            }
            return null;
        }
        return null;

    }

    //Check if wanted was click, play sound effect
    bool CheckIfWantedWasClicked(GameObject suspect){
        if(suspect.GetComponent<Face>().faceID == wanted){
            return true;
        }
        return false;
    }

    IEnumerator CorrectFaceClicked(GameObject suspect){
        FindObjectOfType<AudioManager>().Play("correct");
        //Instantiate score indicator
        GameObject timeIncreaseImage = Instantiate(correctScore, 
        new Vector3(suspect.transform.position.x, suspect.transform.position.y + 1, suspect.transform.position.z),
        Quaternion.identity);
        //Adjust timer as needed
        timer += 5;
        paused = true;
        foreach(Face f in faces){
            f.movementLocked = true;
            if(f.faceID != wanted) f.gameObject.SetActive(false); //hide incorrect faces
            if(f.faceID == wanted) f.movementLocked = true;
        }
        yield return new WaitForSeconds(2);
        //Transition to next round
        UpdateUIOnCorrectGuess();
        Destroy(timeIncreaseImage);
        StartNewRound();
        paused = false;
        //ransition to next round
    }

    IEnumerator IncorrectFaceClicked(GameObject suspect){
        FindObjectOfType<AudioManager>().Play("incorrect");
        //Instantiate score indicator
        GameObject timeDecreaseImage = Instantiate(incorrectScore, 
        new Vector3(suspect.transform.position.x, suspect.transform.position.y + 1, suspect.transform.position.z),
        Quaternion.identity);
        //Adjust timer as needed
        timer -= 10;
        yield return new WaitForSeconds(2);
        Destroy(timeDecreaseImage);
    }

    IEnumerator GameOver(){
        //paused = true; //I don't think this code does anything but I'm leaving it tagged in case something fucks up and I'm like
        //"Man, what happened!!?"
        try{
            foreach(Face f in faces){
                if(f.faceID != wanted) Destroy(f.gameObject); //destroy incorrect faces
                if(f.faceID == wanted) f.movementLocked = true;
            } 
        }catch(MissingReferenceException){}

        //Toggle play again UI
        FindObjectOfType<AudioManager>().Stop("bgm");
        yield return new WaitForSeconds(2);
        playAgainUI.SetActive(true);
    }

    //Makes sure a wanted was generated: If not replace a random head with wanted
    void DoubleCheckForWanted(){
        foreach(Face f in faces){
            if(f.faceID == wanted){
                return;
            } 
        }
        int i = faces.Count;
        int j = UnityEngine.Random.Range(0, i);
        faces[j].InitilizeAsWanted();
    }

    void UpdateUIOnCorrectGuess(){
        fAI.roundsWon++;
        if(fAI.roundsWon % 5 == 0){
            foreach(Image i in stars) i.gameObject.SetActive(false); //Hide all previous stars
            if(!bigStar.activeSelf) bigStar.SetActive(true); //Reveal big star if not revealed already
            bigStar.GetComponentInChildren<TextMeshProUGUI>().text = $"{fAI.roundsWon}"; //Update big star text to multiple of 5
        } else {
            //Reveal the appropriate star for point
            stars[fAI.roundsWon % 5 - 1].gameObject.SetActive(true);
        }
    }

    void InitilizeStars(){
        //Populate list
        foreach(Transform t in littleStarHolder.transform){
            stars.Add(t.GetComponent<Image>());
        }
        //Hide stars if not hidden already
        bigStar.SetActive(false);
        foreach(Image i in stars) i.gameObject.SetActive(false);
    }
}

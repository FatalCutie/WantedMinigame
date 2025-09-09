using System.Collections.Generic;
using UnityEngine;

public class FaceAI : MonoBehaviour
{
    #region Variables
    private GameManager gm;
    public GameObject emptyFacePrefab;
    public GameObject facesContainer;
    //public int facesToGenerate;
    [SerializeField] private GameObject screenBorderHolder;
    private bool firstRun = true;
    public int roundsWon;
    public enum Modifier { NONE, HELIX, RANDOM }
    public Modifier roundModifier = Modifier.NONE;

    [Header("Play Area Bounds")]
    public float halfWidth = 9.8f;
    public float halfHeight = 5.5f;
    #endregion

    void Awake()
    {
        screenBorderHolder.SetActive(false);
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) roundsWon++;
    }

    public void InitilizeNewRound()
    {
        //Clear faces from previous round (if applicable)
        if (firstRun)
        {
            firstRun = !firstRun;
        }
        else ClearPreviousFaces();

        GenerateRoundDetails();
    }

    //Determines pattern, number of faces for round
    void GenerateRoundDetails()
    {
        int points = roundsWon * 10;
        if (points < 30)
        {
            GenerateFacesInSquare(); //Tutorial rounds
        }
        else
        {
            int faces = DecideNumberOfFaces(points);
            roundModifier = RollRoundModifier();
            GenerateFacesRandom(faces, roundModifier); //Hardcoded for now
        }
    }

    private void DecideDifficultyModifier()
    {
        int points = roundsWon * 10;
    }

    Modifier RollRoundModifier()
    {
        return Modifier.HELIX;
    }
    //Temp
    int DecideNumberOfFaces(int points)
    {
        if (points > 350) points = 250;
        return points / 2;
    }

    //Generates faces in a random scatter pattern
    void GenerateFacesRandom(int facesToGenerate, Modifier modifier)
    {
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < facesToGenerate; i++)
        {
            Vector2 spawnPos = FindValidSpawnPosition(usedPositions);
            if (spawnPos != new Vector2(0, 0)) //only spawn if a visible spot was found
            {
                GameObject go = Instantiate(emptyFacePrefab, new Vector3(spawnPos.x, spawnPos.y, 0f), Quaternion.identity, facesContainer.transform);
                Face goFace = go.GetComponent<Face>();
                goFace.Initilize(modifier);
                usedPositions.Add(spawnPos);
            }
            else
            {
                Debug.LogWarning("Could not find valid spawn position for face #" + i);
            }
        }
    }

    //Make sure faces don't spawn over eachother
    Vector2 FindValidSpawnPosition(List<Vector2> usedPositions)
    {
        float minDistance = .85f;
        int maxAttempts = 30;
        Vector2 spawnPos = Vector2.zero;
        bool valid = false;
        int attempts = 0;

        float halfSizeX = emptyFacePrefab.transform.localScale.x / 2f;
        float halfSizeY = emptyFacePrefab.transform.localScale.y / 2f;

        while (!valid && attempts < maxAttempts)
        {
            attempts++;

            //X always fully inside horizontal bounds
            float x = UnityEngine.Random.Range(-halfWidth + halfSizeX, halfWidth - halfSizeX);

            //Y can spawn within vertical bounds including the offscreen margins
            float y = UnityEngine.Random.Range(-halfHeight - halfSizeY, halfHeight + halfSizeY);

            spawnPos = new Vector2(x, y);

            //check overlap with existing faces
            valid = true;
            foreach (Vector2 existing in usedPositions)
            {
                if (Vector2.Distance(existing, spawnPos) < minDistance)
                {
                    valid = false;
                    break;
                }
            }
        }

        return valid ? spawnPos : Vector2.zero;
    }

    //Generates faces in a square for first few rounds
    void GenerateFacesInSquare()
    {
        switch (roundsWon)
        {
            case 0:
                //Generate 2x2 square
                for (int i = 0; i < 2; i++)
                {
                    float startingY = -0.5f + 1 * i; //.5 is hard coded. adjust if bigger faces are used
                    for (int j = 0; j < 2; j++)
                    {
                        float startingX = -0.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize(Modifier.NONE);
                    }
                }
                return;
            case 1:
                //Generate 4x4
                for (int i = 0; i < 4; i++)
                {
                    float startingY = -1.5f + 1 * i;
                    for (int j = 0; j < 4; j++)
                    {
                        float startingX = -1.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize(Modifier.NONE);
                    }
                }
                return;
            case 2:
                //Generate 8x8
                for (int i = 0; i < 8; i++)
                {
                    float startingY = -3.5f + 1 * i;
                    for (int j = 0; j < 8; j++)
                    {
                        float startingX = -3.5f + 1 * j;
                        GameObject go = Instantiate(emptyFacePrefab, new Vector3(startingX, startingY, 0f), Quaternion.identity, facesContainer.transform);
                        go.GetComponent<Face>().Initilize(Modifier.NONE);
                    }
                }
                return;
        }
    }

    //Generate Faces in rectangles that move vertically
    void GenerateFacesInMovingRectangles()
    {

    }

    //Wipes all faces from previous round (and list)
    void ClearPreviousFaces()
    {
        try
        {
            foreach (Face f in gm.faces)
            {
                Destroy(f.gameObject);
            }
            gm.faces.Clear();
        }
        catch (MissingReferenceException) { }
    }
}

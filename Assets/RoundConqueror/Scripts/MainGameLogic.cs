using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainGameLogic : MonoBehaviour
{

    Vector3 planetTouchedPos = new Vector3(0, 0, 0);
    [SerializeField] Camera camera;
    bool planetTouched;
    GameObject initialPlanetTouched;
    GameObject lastPlanetTouched;
    bool initialRestTime;
    [SerializeField] GameObject planetContainer;


    //TOTAL NUMBER OF SHIPS INFO
    [SerializeField] Text FriendlyTotalShipsText;
    [SerializeField] Text EnemyTotalShipsText;
    int totalFriendlyShips;
    int totalEnemyShips;



    //LINE DRAWINGS
    public float lineWidth = 0.02f;
    public Vector3 lineStartingPoint;
    public Vector3 lineLastPoint;
    public List<Vector3> linePlanetPositions = new List<Vector3>();
    public List<GameObject> planetsTouched = new List<GameObject>();
    [SerializeField] Material lineMat;


    //ENEMY RESERVED LOGIC
    //public List<ShipLogic> enemyFleets = new List<ShipLogic>();
    public List<PlanetLogic> enemyPlanets = new List<PlanetLogic>();
    public List<PlanetLogic> notEnemyPlanets = new List<PlanetLogic>();
    public List<PlanetLogic> friendlyPlanets = new List<PlanetLogic>();
    bool enemyDecisionMaking = false;

    //UI LOGIC
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject endGameUI;
    [SerializeField] GameObject pauseUI;



    void Start()
    {
        checkPlanetPossesion();
        Invoke("initialTime", 5f);

    }

    // Update is called once per frame
    void Update()
    {
        touchScreenLogic();
        if (!enemyDecisionMaking && initialRestTime) StartCoroutine(EnemyMovementsLogic());
        checkEnding();

    }
    void initialTime()
    {
        initialRestTime = true;
    }
    private void checkEnding()
    {
        if (friendlyPlanets.Count <= 0)
        {
            //Lose screen
        }
        else if (enemyPlanets.Count <= 0)
        {
            //win screen
            gameUI.GetComponent<Animator>().Play("GameLevelComplete");
            endGameUI.SetActive(true);
            endGameUI.GetComponent<Animator>().Play("LevelComplete");


        }
    }

    public void checkPlanetPossesion()
    {
        enemyPlanets.Clear();
        notEnemyPlanets.Clear();
        friendlyPlanets.Clear();


        foreach (Transform child in planetContainer.transform)
        {
            if (child.GetComponent<PlanetLogic>().enemyControlled)
            {
                enemyPlanets.Add(child.GetComponent<PlanetLogic>());
            }
            else if (!child.GetComponent<PlanetLogic>().enemyControlled)
            {
                if (child.GetComponent<PlanetLogic>().playerControlled) friendlyPlanets.Add(child.GetComponent<PlanetLogic>());
                notEnemyPlanets.Add(child.GetComponent<PlanetLogic>());

            }

        }

    }

    private void touchScreenLogic()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector2.zero);
                if (hit.collider != null)
                {
                    print(hit.collider.tag);


                    if (hit.collider.tag == "FriendlyPlanet")
                    {
                        print("pinché planeta");
                        lineStartingPoint = hit.collider.transform.position; //we get the starting point of the line
                        linePlanetPositions.Add(lineStartingPoint);
                        planetsTouched.Add(hit.collider.gameObject);
                        planetTouchedPos = hit.collider.transform.position;
                        planetTouched = true;
                        hit.collider.GetComponent<PlanetLogic>().createdLine = true;
                        initialPlanetTouched = hit.collider.gameObject;
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && planetTouched)
            {
                Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                RaycastHit2D raycastHit = Physics2D.Raycast(raycastPos, Vector2.zero);
                print("hola estoy en el moved");


                Vector2 touchPos = Input.touches[0].position;
                Vector3 touchPosinWorldSpace = camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, camera.nearClipPlane));
                Vector2 touchPos2D = touchPosinWorldSpace;
                Debug.DrawLine(planetTouchedPos, touchPos2D, Color.white, 2.5f);

                //LINE
                lineLastPoint = touchPos2D; //we get the last point of the line

                if (!initialPlanetTouched.GetComponent<LineRenderer>())
                {
                    LineRenderer lineFromPlanetToPos = initialPlanetTouched.AddComponent<LineRenderer>();
                    lineFromPlanetToPos.material = lineMat;
                    lineFromPlanetToPos.positionCount = 2;
                    lineFromPlanetToPos.SetPositions(new Vector3[] { lineStartingPoint, lineLastPoint });
                    lineFromPlanetToPos.startWidth = lineWidth;
                    lineFromPlanetToPos.endWidth = lineWidth;
                }
                else
                {
                    if (raycastHit.collider != null)
                    {

                        if (raycastHit.collider.CompareTag("FriendlyPlanet"))

                        {
                            if (!raycastHit.collider.GetComponent<PlanetLogic>().createdLine) //if we have not selected this planet
                            {
                                print("añadiendo planeta");

                                raycastHit.collider.GetComponent<PlanetLogic>().createdLine = true;
                                planetsTouched.Add(raycastHit.collider.gameObject);
                                linePlanetPositions.Add(raycastHit.collider.transform.position); //We add the new planet to our list
                                LineRenderer newLineFromPlanetToPos = raycastHit.collider.gameObject.AddComponent<LineRenderer>();
                                newLineFromPlanetToPos.material = lineMat;
                                newLineFromPlanetToPos.positionCount = 2;
                                newLineFromPlanetToPos.startWidth = lineWidth;
                                newLineFromPlanetToPos.endWidth = lineWidth;
                                newLineFromPlanetToPos.SetPositions(new Vector3[] { raycastHit.collider.transform.position, lineLastPoint });
                            }
                        }
                    }

                    for (int i = 0; i < linePlanetPositions.Count; i++)
                    {
                        planetsTouched[i].GetComponent<LineRenderer>().SetPositions(new Vector3[] { linePlanetPositions[i], lineLastPoint });
                    }

                    /* LineRenderer lineFromPlanetToPos = initialPlanetTouched.GetComponent<LineRenderer>();
                     lineFromPlanetToPos.SetPositions(new Vector3[] { lineStartingPoint, lineLastPoint });*/

                }




            }

            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit2D raycastHit = Physics2D.Raycast(raycast.origin, raycast.direction);

                if (raycastHit.collider != null)
                {


                    if ((raycastHit.collider.CompareTag("FriendlyPlanet") || raycastHit.collider.CompareTag("NeutralPlanet") || raycastHit.collider.CompareTag("EnemyPlanet"))
                        && planetTouched
                        && raycastHit.collider != initialPlanetTouched.GetComponent<CircleCollider2D>())
                    {
                        print("hola estoy enviando naves");

                        lastPlanetTouched = raycastHit.collider.gameObject;
                        for (int i = 0; i < linePlanetPositions.Count; i++)
                        {
                            PlanetLogic initialPlanetLogic = planetsTouched[i].GetComponent<PlanetLogic>();
                            CircleCollider2D targetCollider = lastPlanetTouched.GetComponent<CircleCollider2D>();

                            //if(!initialPlanetLogic.enemyControlled) initialPlanetLogic.sendShips(lastPlanetTouched.transform.position, targetCollider); //We send ships to the target planet
                            if (!initialPlanetLogic.enemyControlled) initialPlanetLogic.sendShips(lastPlanetTouched.transform, targetCollider); //We send ships to the target planet

                        }
                    }
                }

                for (int i = 0; i < linePlanetPositions.Count; i++)
                {
                    planetsTouched[i].GetComponent<PlanetLogic>().createdLine = false;
                    //Destroy(   [i].GetComponent<LineRenderer>());
            }
            planetsTouched.Clear();
            linePlanetPositions.Clear();
            planetTouched = false;
        }
    }
    }




    //ENEMY TURNS LOGIC
    IEnumerator EnemyMovementsLogic()
    {
        enemyDecisionMaking = true;
        //If we currently have any fleets moving do nothing
        /*if (enemyFleets.Count >= 1)
        {
            yield re;
        }*/

        //Find my strongest Planet
        PlanetLogic source = null;
        float sourceScore = 0;
        foreach (PlanetLogic planet in enemyPlanets)
        {
            float score = planet.enemyShips / (1 + planet.GrowthRate);
            if (score > sourceScore && planet.enemyShips > 8)
            {
                sourceScore = score;
                source = planet;
            }
        }

        //Then find the weakest friendly or neutral planet
        PlanetLogic dest = null;
        float destScore = 0;
        foreach (PlanetLogic planet in notEnemyPlanets)
        {
            float score = (1 + 1 / planet.GrowthRate) / planet.ships;
            if (score > destScore)
            {
                destScore = score;
                dest = planet;
            }
        }
        if (source != null && dest != null &&
            ((source.GetComponent<PlanetLogic>().enemyShips > dest.GetComponent<PlanetLogic>().ships * 1.75f && dest.GetComponent<PlanetLogic>().playerControlled) ||
            (source.GetComponent<PlanetLogic>().enemyShips > dest.GetComponent<PlanetLogic>().ships * 1.15f && dest.GetComponent<PlanetLogic>().neutralControlled)))
        {

            print(source.GetComponent<PlanetLogic>().enemyShips * 2);
            print(dest.GetComponent<PlanetLogic>().ships);
            source.sendEnemyShips(dest.transform, dest.GetComponent<CircleCollider2D>());
            yield return new WaitForSeconds(5f);
        }
        enemyDecisionMaking = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
    public void ContinueGame()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        //enable the scripts again
    }
    public void restartLevel()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void exitGame()
    {
        Application.Quit();
    }
}

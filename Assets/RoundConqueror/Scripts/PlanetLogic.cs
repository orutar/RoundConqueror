using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLogic : MonoBehaviour
{

    [SerializeField] GameObject shipGO;
    [SerializeField] GameObject enemyShipGO;

    [SerializeField] Material friendlyPlanetMat;
    [SerializeField] Material neutralPlanetMat;
    [SerializeField] Material enemyPlanetMat;

    [SerializeField] MainGameLogic mainGameLogic;

    Renderer currentRenderer;
    TextMesh numberOfShips;

    public bool playerControlled;
    public bool enemyControlled;
    public bool neutralControlled;

    bool shipBuilding;
    public int ships;
    public int enemyShips;
    public float GrowthRate;

    public bool createdLine;



    void Start()
    {
        GrowthRate = transform.localScale.x - 0.5f; //the growth is relative to the size
        numberOfShips = GetComponentInChildren<TextMesh>();
        currentRenderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {

        posesionLogic();
        if (playerControlled && !shipBuilding) StartCoroutine(shipsCreation());
        if (enemyControlled && !shipBuilding) StartCoroutine(enemyShipsCreation());

        shipsText();

    }

    private void shipsText()
    {
        if (playerControlled || neutralControlled) numberOfShips.text = ships.ToString();
        else if (enemyControlled) numberOfShips.text = enemyShips.ToString();
    }

    void posesionLogic()
    {
        if (playerControlled)
        {
            currentRenderer.material = friendlyPlanetMat;
            gameObject.tag = "FriendlyPlanet";
        }
        else if (neutralControlled)
        {
            currentRenderer.material = neutralPlanetMat;
            gameObject.tag = "NeutralPlanet";
        }
        else if (enemyControlled)
        {
            currentRenderer.material = enemyPlanetMat;
            gameObject.tag = "EnemyPlanet";
        }

    }

    public void sendShips(Transform targetPosition, CircleCollider2D targetCollider)
    {

        if (targetCollider == GetComponent<CircleCollider2D>()) return;
        ships -= ships / 2;

        for (int i = 0; i < ships; i++)
        {
            Vector2 direction = (targetPosition.position - transform.position).normalized;

            float angle = i * Mathf.PI * 2f / ships;
            Vector2 planetPos = transform.position;
            Vector2 radiusStart = new Vector2(Mathf.Cos(angle) * transform.localScale.x / 2, Mathf.Sin(angle) * transform.localScale.x / 2) + direction / 2; ;
            Vector2 finalPos = planetPos + radiusStart;

            GameObject newShip = Instantiate(shipGO, finalPos, Quaternion.identity);
            Vector3 targetRelativePostition = new Vector3(newShip.transform.position.x,
                                        newShip.transform.position.y,
                                        targetPosition.transform.position.z);
            newShip.transform.LookAt(targetRelativePostition);
           // newShip.GetComponent<LookAtTarget>().Target = targetPosition;
            //newShip.GetComponent<MoveUp>().enabled = true;

            //newShip.GetComponent<ShipLogic>().targetPlanetCollider = targetCollider;

        }



        /*Vector2 thisPlanet2DPos = transform.position;
        Vector2 direction = (targetPosition - thisPlanet2DPos).normalized;
        newShip.GetComponent<Rigidbody>().AddForce(direction * 50);*/

    }
    public void sendEnemyShips(Transform targetPosition, CircleCollider2D targetCollider)
    {
        enemyShips -= enemyShips / 2;


        for (int i = 0; i < enemyShips; i++)
        {
           /* Vector2 direction = (targetPosition.position - transform.position).normalized;

            float angle = i * Mathf.PI * 2f / enemyShips;
            Vector2 planetPos = transform.position;
            Vector2 radiusStart = new Vector2(Mathf.Cos(angle) * transform.localScale.x / 4, Mathf.Sin(angle) * transform.localScale.x / 4) + direction / 2;
            Vector2 finalPos = planetPos + radiusStart;
            GameObject newShip = Instantiate(enemyShipGO, finalPos, Quaternion.identity);
            newShip.GetComponent<ShipLogic>().enemyShip = true;
            newShip.GetComponent<ShipLogic>().targetPlanetCollider = targetCollider;

            newShip.GetComponent<LookAtTarget>().Target = targetPosition;
            newShip.GetComponent<MoveUp>().enabled = true;
            Vector3 targetRelativePostition = new Vector3(newShip.transform.position.x,
                                        newShip.transform.position.y,
                                        targetPosition.transform.position.z);
            newShip.transform.LookAt(targetRelativePostition);

            float x = i;
            //newShip.transform.position = transform.position + new Vector3(0, x, 0);

            newShip.GetComponent<ShipLogic>().targetPlanetCollider = targetCollider;
            mainGameLogic.enemyFleets.Add(newShip.GetComponent<ShipLogic>());*/

        }

    }

    IEnumerator shipsCreation()
    {
        shipBuilding = true;
        ships += 1;

        yield return new WaitForSeconds(1 / GrowthRate);
        shipBuilding = false;

    }
    IEnumerator enemyShipsCreation()
    {
        shipBuilding = true;
        enemyShips += 1;

        yield return new WaitForSeconds(1 / GrowthRate);
        shipBuilding = false;

    }
}

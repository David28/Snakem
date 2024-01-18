using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleAbbilityController : MonoBehaviour
{
    public int type = 1; //0 for offensive, 1 for defensive
    public AppleController player;
    //Offensive
    public GameObject slashPrefab;
    public float slashOffset = 0.5f;
    public float slashArcRadius = 1f;

    //Defensive
    public GameObject obstaclePrefab;
    public GameObject pointPrefab;
    void Start()
    {
        player = GetComponent<AppleController>();
    }

    void Update()
    {
        Vector3 direction = player.shootDirection;
        Vector3 spawnPosition = transform.position + direction;
        if (type == 1 && player.GetApplePowerDown())
        {
            spawnPosition.x = Mathf.Floor(spawnPosition.x) + 0.5f;
            spawnPosition.y = Mathf.Floor(spawnPosition.y) + 0.5f;
            pointPrefab.transform.position = spawnPosition;
        }
        else
        {
            spawnPosition = transform.position + direction * slashOffset;
            pointPrefab.transform.position = spawnPosition;
        }
        if (player.amDead)
        {
            return;
        }
        if (player.GetApplePowerRelease())
        {
            if (type == 1)
            {
                Build();
            }
            else
            {
                Slash();
            }
        }
    }

    private void Build()
    {
        if (player.ate < 3) return;
        Vector3 direction = player.shootDirection;
        Vector3 spawnPosition = transform.position + direction;
        spawnPosition.x = Mathf.Floor(spawnPosition.x) + 0.5f;
        spawnPosition.y = Mathf.Floor(spawnPosition.y) + 0.5f;
        if (GameObject.FindObjectOfType<GameManager>().IsOccupied(spawnPosition)) return;
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity).SetActive(true);
        player.SetAte(0);
    }

    private void Slash()
    {
        if (player.ate < 3) return;
        player.SetAte(0);
        //rotate according to player direction
        Vector3 direction = player.shootDirection;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //rotate around player
        slashPrefab.transform.rotation = rotation;
        slashPrefab.transform.position = transform.position + direction * slashOffset;
        slashPrefab.SetActive(true);
    }
}

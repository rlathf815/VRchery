using UnityEngine;

public class ArcherController : MonoBehaviour
{
    public GameObject ArrowPrefab;                  // The Arrow to shoot
    public float MaximalShootRange = 100f;          // Maximal distance to shoot
    public float MinimalShootRange = 4f;            // Minimal distance to shoot
    [Range(0,10)]
    public float SpreadFactor = 0.5f;               // Accuracy
    [Range(0f,0.4f)]
    public float SpreadFactorDistanceImpact = 0.1f; // Impact of the distance (from shooter to target) on the accuracy
    public float HeightMultiplier = 2f;             // Changes the parabola-height of the flightpath (Arrows fly in a higher arc)
    public float ArrowFlightSpeed = 6f;             // Speed of the Arrow
    public float ArrowLifeTime = 120f;              // Time until the Arrow gets destroyed (in seconds)  
    [Space]
    public GameObject Target;
    public bool UseTarget;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            // Shoot an Arrow every fixed tick while the key is pressed
            TryToShoot();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Shoot a single Arrow
            TryToShoot();
        }
    }

    private void TryToShoot()
    {
        if (UseTarget && Target != null)
        {
            ShootArrow(Target.transform.position);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Dont let the ray collide with objects tagged 'Projectile'
                if (!hit.collider.CompareTag("Projectile"))
                {
                    ShootArrow(hit.point);
                }
            }
        }
    }

    private void ShootArrow(Vector3 targetPos)
    {
        // Only allow to shoot targets within minimum and maximum range
        var distance = Vector3.Distance(transform.position, targetPos);
        if (distance >= MinimalShootRange && distance <= MaximalShootRange)
        {
            // Calculate the spread-range relative to the distance
            float spreadFactorByDistance = SpreadFactor * (1f + (SpreadFactorDistanceImpact * distance));

            // Calculate inaccurate target (somewhere around the original target)
            Vector3 inaccurateTarget = (Random.insideUnitSphere * spreadFactorByDistance) + targetPos;

            // Create a new Arrow
            var Arrow = Instantiate(ArrowPrefab, transform.position, transform.rotation);

            // Name the Arrow "Arrow" to remove the default name with "(Clone)"
            Arrow.name = "Arrow";

            // Tell the Arrow to go shwoooosh
            Arrow.GetComponent<ArrowController>().Shoot(inaccurateTarget, gameObject, ArrowFlightSpeed, HeightMultiplier, ArrowLifeTime);
        }
    }
}
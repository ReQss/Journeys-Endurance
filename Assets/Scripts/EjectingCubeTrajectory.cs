// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EjectingCubeTrajectory : MonoBehaviour
// {
//     // Start is called before the first frame update
//     EjectingCube ejectingCube;
//     LineRenderer lineRenderer;
//     public int numOfPoints = 50;
//     public float timeBetweenPoint = 0.1f;
//     public LayerMask collidableLayers;


//     void Start()
//     {
//         ejectingCube = GetComponent<EjectingCube>();
//         lineRenderer = GetComponent<LineRenderer>();
//     }

//     // Update is called once per frame
//     public float colliderRadius = 0;
//     public float timeToBreak = 0;
//     void Update()
//     {
//         if (ejectingCube.GetCanBuildOn() == true) return;//show trajectory only for new gadget
//         lineRenderer.positionCount = numOfPoints;
//         List<Vector3> points = new List<Vector3>();
//         Vector3 startingPosition = ejectingCube.ejectStart.position;
//         Vector3 startingVelocity = ejectingCube.ejectStart.transform.forward * ejectingCube.ejectStrength;
//         for (float t = 0; t < numOfPoints; t += timeBetweenPoint)
//         {
//             Vector3 newPoint = startingPosition + t * startingVelocity;
//             // second motion equation 
//             newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t;
//             points.Add(newPoint);
//             if (t > timeToBreak)
//             {
//                 var colliders = Physics.OverlapSphere(newPoint, colliderRadius, collidableLayers);

//                 if (colliders.Length > 0)
//                 {
//                     lineRenderer.positionCount = points.Count;
//                     break;
//                 }
//             }
//         }
//         lineRenderer.SetPositions(points.ToArray());
//     }
// }

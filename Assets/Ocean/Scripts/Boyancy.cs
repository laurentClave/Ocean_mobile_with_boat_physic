using UnityEngine;
using System.Collections.Generic;

public class Boyancy : MonoBehaviour
{
	
	private Ocean ocean;
	private float mag = 1f;
	private List<Vector3> blobs;
	private float ax = 2.0f;
	private float ay = 2.0f;
	private bool engine = false;
	private List<float> sinkForces;

	[Header("Boyancy :")]
	public float ypos = 0.0f;
	[SerializeField] private float dampCoeff = .2f;
	[SerializeField] private bool sink = false;
	[SerializeField] private float sinkForce = 3;

    protected virtual void Start ()
	{
        ocean = Ocean.Singleton;
		
		GetComponent<Rigidbody>().centerOfMass = new Vector3 (0.0f, -1f, 0.0f);
	
		Vector3 bounds = GetComponent<BoxCollider> ().size;
		float length = bounds.z;
		float width = bounds.x;

		blobs = new List<Vector3> ();

		int i = 0;
		float xstep = 1.0f / (ax - 1f);
		float ystep = 1.0f / (ay - 1f);
	
		sinkForces = new List<float>();
		
		float totalSink = 0;

		for (int x=0; x<ax; x++) {
			for (int y=0; y<ay; y++) {		
				blobs.Add (new Vector3 ((-0.5f + x * xstep) * width, 0.0f, (-0.5f + y * ystep) * length) + Vector3.up * ypos);
				
				float force =  Random.Range(0f,1f);
				
				force = force * force;
				
				totalSink += force;
				
				sinkForces.Add(force);
				i++;
			}		
		}
		
		// normalize the sink forces
		for (int j=0; j< sinkForces.Count; j++)
		{
			sinkForces[j] = sinkForces[j] / totalSink * sinkForce;
		}
		
	}


    protected virtual void FixedUpdate()
	{
        if (ocean != null)
        { 
            int index = 0;
            foreach (Vector3 blob in blobs) {

                Vector3 wpos = transform.TransformPoint (blob);
                float damp = GetComponent<Rigidbody>().GetPointVelocity (wpos).y;

                float buyancy = mag * (wpos.y);

                if (ocean.enabled)
                    buyancy = mag * (wpos.y - ocean.GetWaterHeightAtLocation (wpos.x, wpos.z));
			
			    if (sink)
			    {
				    buyancy = Mathf.Max(buyancy, -3) + sinkForces[index++] ;
			    }

				Rigidbody rigidbody =  GetComponent<Rigidbody>();
				rigidbody.AddForceAtPosition (-Vector3.up * (buyancy + dampCoeff * damp), wpos);
		    }
        }
    }
	
	public void Sink(bool isActive)
	{
	    sink = isActive;	
	}


}

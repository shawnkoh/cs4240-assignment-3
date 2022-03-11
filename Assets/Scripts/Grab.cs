using UnityEngine;

/// <summary>
/// DEPRECATED: ONLY FOR USE ON TESTING SCENE 'ianAudioScene' and 'samplescene'
/// Lets the player grab objects around them. Attached to the L and R hands.
/// <author>ian-from-dover</author>
/// feel free to add your authorship here
/// </summary>
public class Grab : MonoBehaviour
{
    public EventChannel pigeonPickup;
    public EventChannel gunEquipped;
    public OVRInput.Controller Controller; // placed on L and R
    public LayerMask grabMask; // only obj in this layer can be grabbed
    public string buttonName;
    public float grabRadius = 1; // range of sphere cast
    public Gun shootingGun;

    private GameObject _grabbedObject;
    private bool _grabbing;
    

    void Update()
    {
        if (!_grabbing && Input.GetAxis(buttonName) == 1)
        {
            GrabObject();
        }

        if (_grabbing && Input.GetAxis(buttonName) < 1)
        {
            DropObject();
        }
    }

    void GrabObject()
    {
        // Check current hand side of controller
        // Left hand : grab pigeon and ammo only
        // Right hand : grab gun only
        OVRPlugin.Handedness handedness = OVRPlugin.GetDominantHand();

        _grabbing = true;

        RaycastHit[] hits;

        // only react for objects in the correct layers
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0.0f, grabMask);
    
        if (hits.Length > 0)
        {
            int closestHit = 0;

            for (int i = 0; i < hits.Length; i++)
            {
                // can't figure out why grabbing nearest object is buggy
                // if (hits[i].collider != null) Debug.Log(hits[i].collider.name + " has been hit. Its distance is: " + hits[i].distance);

                if ((hits[i]).distance < hits[closestHit].distance)
                {
                    closestHit = i;
                    Debug.Log(hits[i].collider.name + " has been selected. Its distance is: " + hits[i].distance);

                }
            }

            _grabbedObject = hits[closestHit].transform.gameObject;

            if (_grabbedObject.tag == "Gun")
            {
                shootingGun.canShoot = true;
            }

            _grabbedObject.GetComponent<Rigidbody>().isKinematic = true; // gravity dont work on obj while it is held
            _grabbedObject.transform.position = transform.position;
            _grabbedObject.transform.parent = transform; // makes obj child of ctrler so they move tgt
            pigeonPickup.Publish();
        }
    }

    void DropObject()
    {
        _grabbing = false;

        if (_grabbedObject != null)
        {
            if (_grabbedObject.tag == "Gun")
            {
                shootingGun.canShoot = false;
            }

            _grabbedObject.transform.parent = null; // makes obj child of ctrler so they move tgt

            _grabbedObject.GetComponent<Rigidbody>().isKinematic = false; // gravity dont work on obj while it is held

            _grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(Controller); 
            _grabbedObject.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);

            _grabbedObject = null; // makes obj child of ctrler so they move tgt
            gunEquipped.Publish();
        }
    }
}

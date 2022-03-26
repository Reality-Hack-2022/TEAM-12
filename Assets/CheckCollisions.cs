using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckCollisions : MonoBehaviour
{
    public GameObject EnjoyZonePrefab;
    public AudioClip EnjoyZoneSound;

    public GameObject FocusZonePrefab;
    public AudioClip FocusZoneSound;

    public GameObject HeartZonePrefab;
    public AudioClip HeartZoneSound;
    public float projectileSpeed = 5 ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("heart"))
        {
            AudioSource.PlayClipAtPoint(HeartZoneSound, transform.position);
            GameObject spawnedObject = PhotonNetwork.Instantiate(HeartZonePrefab.name, transform.position, transform.rotation) as GameObject;


            spawnedObject.AddComponent<Rigidbody>();
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

            spawnedObject.GetComponent<Rigidbody>().useGravity = false;
            spawnedObject.GetComponent<Rigidbody>().isKinematic = false;

        }
        if (other.gameObject.CompareTag("focus"))
        {
            AudioSource.PlayClipAtPoint(FocusZoneSound, transform.position);
            GameObject spawnedObject = PhotonNetwork.Instantiate(FocusZonePrefab.name, transform.position, transform.rotation) as GameObject;

            spawnedObject.AddComponent<Rigidbody>();
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

            spawnedObject.AddComponent<Rigidbody>();
            spawnedObject.GetComponent<Rigidbody>().useGravity = false;
            spawnedObject.GetComponent<Rigidbody>().isKinematic = false;

        }
        if (other.gameObject.CompareTag("enjoyment"))
        {
            AudioSource.PlayClipAtPoint(EnjoyZoneSound, transform.position);
            GameObject spawnedObject = PhotonNetwork.Instantiate(EnjoyZonePrefab.name, transform.position, transform.rotation) as GameObject;

            spawnedObject.AddComponent<Rigidbody>();
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

            spawnedObject.AddComponent<Rigidbody>();
            spawnedObject.GetComponent<Rigidbody>().useGravity = false;
            spawnedObject.GetComponent<Rigidbody>().isKinematic = false;

        }
    }
}

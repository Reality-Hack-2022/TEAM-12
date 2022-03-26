using System.Collections;
using System.Collections.Generic;
using MRTK;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine;


public class SpawnEmojiandSound : MonoBehaviour
{
    public GameObject EnjoymentPrefab;
    public AudioClip EnjoymentSound;

    public GameObject ZonePrefab;
    public AudioClip ZoneSound;

    public GameObject FocusPrefab;
    public AudioClip FocusSound;

    public GameObject HeartPrefab;
    public AudioClip HeartSound;



    public GameObject EnjoyZonePrefab;
    public AudioClip EnjoyZoneSound;

    public GameObject FocusZonePrefab;
    public AudioClip FocusZoneSound;

    public GameObject HeartZonePrefab;
    public AudioClip HeartZoneSound;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (gameObject.CompareTag("zone") && other.gameObject.CompareTag("heart"))
    //    {
    //        AudioSource.PlayClipAtPoint(HeartZoneSound, transform.position);
    //        GameObject spawnedObject = PhotonNetwork.Instantiate(HeartZonePrefab.name, transform.position, transform.rotation) as GameObject;

            
    //        spawnedObject.AddComponent<Rigidbody>();
    //        spawnedObject.GetComponent<Rigidbody>().useGravity = false;
    //        spawnedObject.GetComponent<Rigidbody>().isKinematic = false;

    //    }
    //    if (other.gameObject.CompareTag("focus"))
    //    {
    //        AudioSource.PlayClipAtPoint(FocusZoneSound, transform.position);

    //    }
    //    if (other.gameObject.CompareTag("enjoyment"))
    //    {
    //        AudioSource.PlayClipAtPoint(EnjoyZoneSound, transform.position);

    //    }
    //}

    public void spawnEnjoyment()
    {
        //Instantiate(EnjoymentPrefab, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(EnjoymentSound, transform.position);

        //Instantiate(lootPrefab, transform.position, transform.rotation);

        //Destroy(gameObject, 0.5f);
        GameObject obj1 = GameObject.FindGameObjectWithTag("NetworkRoom");


        GameObject spawnedObject = PhotonNetwork.Instantiate(EnjoymentPrefab.name, transform.position, transform.rotation) as GameObject;
        
        gameObject.SetActive(false);

        //Debug.Log("Inside Spawn Cube Spawn from menu");
        spawnedObject.AddComponent<Rigidbody>();
        spawnedObject.GetComponent<Rigidbody>().useGravity = false;
        spawnedObject.GetComponent<Rigidbody>().isKinematic = true;

        //spawnedObject.AddComponent<nearinteractable>
    }

    public void spawnZone()
    {
        //Instantiate(EnjoymentPrefab, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(ZoneSound, transform.position);

        //Instantiate(lootPrefab, transform.position, transform.rotation);

        //Destroy(gameObject, 0.5f);

        GameObject spawnedObject = PhotonNetwork.Instantiate(ZonePrefab.name, transform.position, transform.rotation) as GameObject;
        gameObject.SetActive(false);

        //Debug.Log("Inside Spawn Cube Spawn from menu");
        spawnedObject.AddComponent<Rigidbody>();
        spawnedObject.GetComponent<Rigidbody>().useGravity = false;
        spawnedObject.GetComponent<Rigidbody>().isKinematic = true;


        //spawnedObject.AddComponent<nearinteractable>
    }
    public void spawnFocus()
    {
        //Instantiate(EnjoymentPrefab, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(FocusSound, transform.position);

        //Instantiate(lootPrefab, transform.position, transform.rotation);

        //Destroy(gameObject, 0.5f);

        GameObject spawnedObject = PhotonNetwork.Instantiate(FocusPrefab.name, transform.position, transform.rotation) as GameObject;
        gameObject.SetActive(false);

        //Debug.Log("Inside Spawn Cube Spawn from menu");
        spawnedObject.AddComponent<Rigidbody>();
        spawnedObject.GetComponent<Rigidbody>().useGravity = false;
        spawnedObject.GetComponent<Rigidbody>().isKinematic = true;


        //spawnedObject.AddComponent<nearinteractable>
    }
    public void spawnHeart()
    {
        //Instantiate(EnjoymentPrefab, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(HeartSound, transform.position);

        //Instantiate(lootPrefab, transform.position, transform.rotation);

        //Destroy(gameObject, 0.5f);

        GameObject spawnedObject = PhotonNetwork.Instantiate(HeartPrefab.name, transform.position, transform.rotation) as GameObject;
        gameObject.SetActive(false);

        //Debug.Log("Inside Spawn Cube Spawn from menu");
        spawnedObject.AddComponent<Rigidbody>();
        spawnedObject.GetComponent<Rigidbody>().useGravity = false;
        spawnedObject.GetComponent<Rigidbody>().isKinematic = true;


        //spawnedObject.AddComponent<nearinteractable>
    }




}

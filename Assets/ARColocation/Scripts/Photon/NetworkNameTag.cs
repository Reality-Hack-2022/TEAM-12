using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NetworkNameTag : MonoBehaviour
{
    [SerializeField]
    private Text _nameTagText;
    // Start is called before the first frame update
    void Start()
    {
        _nameTagText.text = PhotonNetwork.NickName;
    }
}

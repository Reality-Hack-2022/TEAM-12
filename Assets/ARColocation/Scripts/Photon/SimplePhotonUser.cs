﻿
using Photon.Pun;
using UnityEngine;

namespace Colocation
{
	public class SimplePhotonUser : MonoBehaviour
	{
		[SerializeField]
		private PhotonView pv;

		private string username;

		private void Start()
		{
			pv = GetComponent<PhotonView>();

			if (!pv.IsMine) return;

			username = "User" + PhotonNetwork.NickName;
			pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
		}

		[PunRPC]
		private void PunRPC_SetNickName(string nName)
		{
			gameObject.name = nName;
		}

	}
}
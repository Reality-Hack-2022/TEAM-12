using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


	public class NetworkManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
	{
	
		[SerializeField] private GameObject _photonPlayerPrefab;
		[SerializeField] private GameObject _photonControllerPrefab;
		private GameObject _cachedLocalPlayer;
		private GameObject _cachedLocalPlayerController;
		
		private Player[] photonPlayers;
		private int playersInRoom;
		private int myNumberInRoom;
		
		/// <summary>
		/// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
		/// </summary>
		private void Start()
		{
			// Allow prefabs not in a Resources folder
			if (PhotonNetwork.PrefabPool is DefaultPool pool)
			{
				if (_photonPlayerPrefab != null) pool.ResourceCache.Add(_photonPlayerPrefab.name, _photonPlayerPrefab);
				if (_photonControllerPrefab != null) pool.ResourceCache.Add(_photonControllerPrefab.name, _photonControllerPrefab);
			}
			PhotonNetwork.AutomaticallySyncScene = false;
			StartNetwork();
		}

		/// <summary>
		/// Connect to Photon network
		/// </summary>
		private void StartNetwork()
		{
			PhotonNetwork.ConnectUsingSettings();
		}

		/// <summary>
		/// Join the room or create one if it doesn't exist
		/// </summary>
		private void JoinOrCreateRoom()
		{
			var randomUserId = Random.Range(0, 999999);
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.AuthValues = new AuthenticationValues { UserId = randomUserId.ToString() };
			PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
			var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
			PhotonNetwork.JoinRandomOrCreateRoom(roomName: "Colocation Room", roomOptions: roomOptions);
		}


		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		public void OnEnable()
		{
			
			PhotonNetwork.AddCallbackTarget(this);
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled.
		/// </summary>
		public void OnDisable()
		{
			
			PhotonNetwork.RemoveCallbackTarget(this);
		}
		/// <summary>
		/// Create a player object on all connected devices
		/// </summary>
		private void CreatePlayer()
		{
			Debug.Log("Created Player!");
			_cachedLocalPlayer = PhotonNetwork.Instantiate(_photonPlayerPrefab.name, Vector3.zero, Quaternion.identity);
#if PLATFORM_LUMIN
			_cachedLocalPlayerController = PhotonNetwork.Instantiate(_photonControllerPrefab.name, Vector3.zero, Quaternion.identity);
#endif
			var props = new Hashtable();
			props.Add("playerViewId", PhotonView.Get(_cachedLocalPlayer).ViewID);
#if PLATFORM_LUMIN
			props.Add("controllerViewId", PhotonView.Get(_cachedLocalPlayerController).ViewID);
#endif
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
			
		}




	#region IConnectionCallbacks
		/// <inheritdoc />
		public void OnConnected() { }

		/// <inheritdoc />
		public void OnConnectedToMaster()
		{
			PhotonNetwork.NickName = "User " + PhotonNetwork.CountOfPlayers;
			JoinOrCreateRoom();
		}

		/// <inheritdoc />
		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.LogWarning("User disconnected from Photon. Reason: " + cause);
		}

		/// <inheritdoc />
		public void OnRegionListReceived(RegionHandler regionHandler) { }

		/// <inheritdoc />
		public void OnCustomAuthenticationFailed(string debugMessage) { }

		/// <inheritdoc />
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

	#endregion

	#region IMatchmakingCallbacks

		/// <inheritdoc />
		public void OnFriendListUpdate(List<FriendInfo> friendList) { }

		/// <inheritdoc />
		public void OnCreatedRoom() { }

		/// <inheritdoc />
		public void OnCreateRoomFailed(short returnCode, string message)
		{
			Debug.LogError($"Failed to create room. return code [{returnCode}] - {message} ");
		}

		/// <inheritdoc />
		public void OnJoinedRoom()
		{

			Debug.Log("\nPhotonLobby.OnJoinedRoom()");
			Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
			Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
			Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));

			photonPlayers = PhotonNetwork.PlayerList;
			playersInRoom = photonPlayers.Length;
			myNumberInRoom = playersInRoom;
			PhotonNetwork.NickName = myNumberInRoom.ToString();
			CreatePlayer();
		}

		/// <inheritdoc />
		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.LogError($"Failed to join room. return code [{returnCode}] - {message} ");
		}

		/// <inheritdoc />
		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.LogError($"Failed to join random room. return code [{returnCode}] - {message} ");
		}

		/// <inheritdoc />
		public void OnLeftRoom() { }

	#endregion

	#region IInRoomCallbacks

		/// <inheritdoc />
		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			photonPlayers = PhotonNetwork.PlayerList;
			playersInRoom++;
		}
		/// <inheritdoc />
		public void OnPlayerLeftRoom(Player otherPlayer) { }

		/// <inheritdoc />
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

		/// <inheritdoc />
		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

		/// <inheritdoc />
		public void OnMasterClientSwitched(Player newMasterClient) { }
		

	#endregion



	}


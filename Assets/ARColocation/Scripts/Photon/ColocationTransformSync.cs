
using Colocation.Utilities;
using Photon.Pun;

using UnityEngine;

namespace Colocation
{
    public class ColocationTransformSync : MonoBehaviourPun, IPunObservable
    {

        [SerializeField] private Transform _referenceTransform;
        [SerializeField] private bool _syncPosition=true;
        [SerializeField] private bool _syncRotation = true;
        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;




        void Awake()
        {
            AutoObsereveComponent();
            if (_referenceTransform == null)
            {
                _referenceTransform = FindObjectOfType<ReferencePoint>().transform;
            }
        }

        void AutoObsereveComponent()
        {
            if (!photonView.ObservedComponents.Contains(this))
                photonView.ObservedComponents.Add(this);
        }

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (_syncPosition)
                {
                    if (_referenceTransform)
                    {
                        Matrix4x4 pcfCoordinateSpace = Matrix4x4.TRS(_referenceTransform.position, _referenceTransform.rotation, Vector3.one);
                        Vector3 relPosition = Matrix4x4.Inverse(pcfCoordinateSpace).MultiplyPoint3x4(transform.position);
                        stream.SendNext(relPosition);
                    }
                    else
                    {
                        stream.SendNext(transform.position);
                    }
                }

                if (_syncRotation)
                {
                    if (_referenceTransform)
                    {
                        Quaternion relOrientation = Quaternion.Inverse(_referenceTransform.rotation) * transform.rotation;
                        stream.SendNext(relOrientation);
                    }
                    else
                    {
                        stream.SendNext(transform.rotation);

                    }

                }
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
            }
        }

        private void Start()
        {
            networkLocalPosition = transform.position;
            networkLocalRotation = transform.rotation;
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                var trans = transform;

                if (_referenceTransform)
                {
                    trans.rotation = _referenceTransform.rotation * networkLocalRotation;

                }
                else
                {
                    trans.rotation = networkLocalRotation;
                }
                
                if (_referenceTransform)
                {
                    
                    Matrix4x4 pcfCoordinateSpace = new Matrix4x4();
                    pcfCoordinateSpace.SetTRS(_referenceTransform.position, _referenceTransform.rotation, Vector3.one);
                    trans.position = pcfCoordinateSpace.MultiplyPoint3x4(networkLocalPosition);
                }
                else
                {
                    trans.position = networkLocalPosition;
                }

        
            }

        }
    }
}
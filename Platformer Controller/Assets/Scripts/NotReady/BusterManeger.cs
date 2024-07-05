//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BusterManeger : MonoBehaviour
//{
//    [SerializeField] private List<GameObject> Busters = new List<GameObject>();

//    private GameObject _buster;

//    private void Update()
//    {
//        foreach (GameObject buster in Busters)
//        {
//            _buster = buster;
//            var isActive = buster.GetComponent<Buster>().isActive;
//            if (!isActive)
//            {
//                StartCoroutine(KD());
//            }
//        }
//    }

//    IEnumerator KD()
//    {
//        _buster.gameObject.SetActive(false);
//        yield return new WaitForSeconds(1);
//        _buster.gameObject.SetActive(true);
//    }
//}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class GameBattleFollowCamera : MonoBehaviour
{

    private void Update()
    {
        transform.localPosition = new Vector3( GameCameraManager.instance.PosXReal ,
            GameCameraManager.instance.PosYReal , transform.localPosition.z );
    }
}

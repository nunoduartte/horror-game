using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfiguration : MonoBehaviour
{
    public enum Race { B, W };
    public enum Gender { F, M };

    public string playerGender;
    public string playerName;
    public string playerRace;

    public void RandomCustomize()
    {
        int blackOrWhite = Random.Range(0, 1000);
        int femaleOrMale = Random.Range(0, 1000);
        switch (blackOrWhite % 2)
        {
            case 0:
                this.playerRace = Race.B.ToString();
                break;
            case 1:
                this.playerRace = Race.W.ToString();
                break;
        }

        switch (femaleOrMale % 2)
        {
            case 0:
                this.playerGender = Gender.F.ToString();
                break;
            case 1:
                this.playerGender = Gender.M.ToString();
                break;
        }
    }
}

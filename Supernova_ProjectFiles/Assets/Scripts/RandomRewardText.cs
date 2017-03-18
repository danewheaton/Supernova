using UnityEngine;
using System.Collections;

public class RandomRewardText : MonoBehaviour
{
    TextMesh rewardText;

	void Start ()
    {
        rewardText = GetComponent<TextMesh>();
        int randomInt = Random.Range(0, 5);

        switch (randomInt)
        {
            case 0:
                rewardText.text = "NICE JOB!";
                break;
            case 1:
                rewardText.text = "AWESOME!";
                break;
            case 2:
                rewardText.text = "WAY TO GO!";
                break;
            case 3:
                rewardText.text = "SLICK MOVES!";
                break;
            case 4:
                rewardText.text = "GREAT!";
                break;
            default:
                break;
        }
    }
}

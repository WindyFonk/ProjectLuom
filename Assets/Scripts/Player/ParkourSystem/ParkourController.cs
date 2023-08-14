using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> actionList;

    EnviromentScan scanner;
    [SerializeField] Animator animator;
    MovementController playerController;


    bool inAction;
    private void Awake()
    {
        scanner = GetComponent<EnviromentScan>();
        playerController = GetComponent<MovementController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !inAction && !playerController.isCrouching)
        {
            var hitData = scanner.ObstacleCheck();

            if (hitData.forwardHitFound)
            {
                foreach (var action in actionList)
                {
                    if (action.checkPossible(hitData, transform))
                    {
                        StartCoroutine(VaultAction(action));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator VaultAction(ParkourAction action)
    {
        inAction = true;
        playerController.setControl(false);
        animator.CrossFade(action.animationName, 0.2f);

        yield return null;
        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.animationName))
        {
            Debug.Log("Wrong animation name");
        }


        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.targetRotation, 300f * Time.deltaTime);
            }

            if (action.EnableTargetMatching)
            {
                MatchTarget(action); 
            }

            yield return null;
        }

        playerController.setControl(true);
        inAction = false;
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget)
        {
            return;
        }

        animator.MatchTarget(action.MatchPosition, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchtWeightMask, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}

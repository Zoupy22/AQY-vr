using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RandomAnimationPlayer : MonoBehaviour
{
    private Animator animator; // 引用 Animator
    private string animationTriggerName = "start"; // 动画触发器的名称
    public float minWaitTime = 1.0f; // 最小随机等待时间
    public float maxWaitTime = 20.0f; // 最大随机等待时间

    void Start()
    {
        // 如果没有直接分配 Animator，在当前 GameObject 上自动查找
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
   public void StartPlayAnimationWithRandomDelay()
    {
        // 如果没有直接分配 Animator，在当前 GameObject 上自动查找
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            // 启动动画播放的循环协程
            StartCoroutine(PlayAnimationWithRandomDelay());
        }
        else
        {
            Debug.LogError("Animator component is not assigned or found!");
        }
    }
    private IEnumerator PlayAnimationWithRandomDelay()
    {

        // 随机等待一段时间
        float randomDelay = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(randomDelay); // 等待
                                                      // 触发动画
        animator.SetTrigger(animationTriggerName);
    }
}

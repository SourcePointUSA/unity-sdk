using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Scripts.Util
{
    public static class GraphicExtenstion
    {
        public static IEnumerator ChangeColor(this Graphic graphic, Color target)
        {
            Color currentColor = graphic.color;
            float time = 0f;
            while (graphic.color.r != target.r || graphic.color.g != target.g || graphic.color.b != target.b || graphic.color.a != target.a)
            {
                time += Time.deltaTime;
                graphic.color = Color.Lerp(currentColor, target, time * 5);
                yield return null;
            }
        }

        public static IEnumerator TriggerAnimation(this Animator animator, string trigger, Action onEndAction = null)
        {
            animator.SetTrigger(trigger);
            if (onEndAction != null)
            {
                if (animator != null && animator.GetCurrentAnimatorClipInfo(0).Length > 0)
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                onEndAction?.Invoke();
            }
        }
    }
}
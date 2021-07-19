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
            if (graphic != null)
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
        }

        public static IEnumerator TriggerAnimation(this Animator animator, string trigger, Action onEndAction = null)
        {
            if (animator != null)
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

        public static IEnumerator ChangeLocalPosition(this Transform trans, Vector3 target, float time = 0.5f)
        {
            Vector3 pos = trans.localPosition;
            float speed = 0;
            float rate = 1 / time;
            while (speed < 1)
            {
                speed += Time.deltaTime * rate;
                trans.localPosition = Vector3.Lerp(pos, target, speed);
                yield return null;
            }
        }
    }
}
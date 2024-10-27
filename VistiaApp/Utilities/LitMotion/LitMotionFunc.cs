using Cysharp.Threading.Tasks;
using LitMotion;
using System;
using UnityEngine;

public static class LitMotionFunc
{
    #region Transform
    public static MotionHandle ChangePosition(this Transform transform, Vector3 targetPosition, float duration)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .BindWithState(transform, (position, target) => target.position = position);
    }

    public static MotionHandle ChangePosition(this Transform transform, Vector3 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.position = position);
    }

    public static MotionHandle ChangePosition(this Transform transform, Vector3 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.position = position);
    }

    public static UniTask ChangePositionAsync(this Transform transform, Vector3 targetPosition, float duration)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .BindWithState(transform, (position, target) => target.position = position)
            .ToUniTask();
    }

    public static UniTask ChangePositionAsync(this Transform transform, Vector3 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.position = position)
            .ToUniTask();
    }

    public static UniTask ChangePositionAsync(this Transform transform, Vector3 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.position = position)
            .ToUniTask();
    }

    public static MotionHandle ChangePositionXTo(this Transform transform, float targetX, float duration)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static MotionHandle ChangePositionXTo(this Transform transform, float targetX, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static MotionHandle ChangePositionXTo(this Transform transform, float targetX, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static UniTask ChangePositionXToAsync(this Transform transform, float targetX, float duration)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionXToAsync(this Transform transform, float targetX, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionXToAsync(this Transform transform, float targetX, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.x, targetX, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static MotionHandle ChangePositionYTo(this Transform transform, float targetY, float duration)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static MotionHandle ChangePositionYTo(this Transform transform, float targetY, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static MotionHandle ChangePositionYTo(this Transform transform, float targetY, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static UniTask ChangePositionYToAsync(this Transform transform, float targetY, float duration)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionYToAsync(this Transform transform, float targetY, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionYToAsync(this Transform transform, float targetY, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.y, targetY, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static MotionHandle ChangePositionZTo(this Transform transform, float targetZ, float duration)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static MotionHandle ChangePositionZTo(this Transform transform, float targetZ, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static MotionHandle ChangePositionZTo(this Transform transform, float targetZ, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static UniTask ChangePositionZToAsync(this Transform transform, float targetZ, float duration)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static UniTask ChangePositionZToAsync(this Transform transform, float targetZ, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static UniTask ChangePositionZToAsync(this Transform transform, float targetZ, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.z, targetZ, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static MotionHandle ChangePositionXAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static MotionHandle ChangePositionXAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static MotionHandle ChangePositionXAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z));
    }

    public static UniTask ChangePositionXAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionXAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionXAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.x, transform.position.x + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.position = new Vector3(x, target.position.y, target.position.z))
            .ToUniTask();
    }

    public static MotionHandle ChangePositionYAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static MotionHandle ChangePositionYAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static MotionHandle ChangePositionYAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z));
    }

    public static UniTask ChangePositionYAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionYAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static UniTask ChangePositionYAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.y, transform.position.y + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.position = new Vector3(target.position.x, y, target.position.z))
            .ToUniTask();
    }

    public static MotionHandle ChangePositionZAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static MotionHandle ChangePositionZAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static MotionHandle ChangePositionZAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z));
    }

    public static UniTask ChangePositionZAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static UniTask ChangePositionZAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static UniTask ChangePositionZAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.position.z, transform.position.z + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.position = new Vector3(target.position.x, target.position.y, z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPosition(this Transform transform, Vector3 targetPosition, float duration)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .BindWithState(transform, (position, target) => target.localPosition = position);
    }

    public static MotionHandle ChangeLocalPosition(this Transform transform, Vector3 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.localPosition = position);
    }

    public static MotionHandle ChangeLocalPosition(this Transform transform, Vector3 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.localPosition = position);
    }

    public static UniTask ChangeLocalPositionAsync(this Transform transform, Vector3 targetPosition, float duration)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .BindWithState(transform, (position, target) => target.localPosition = position)
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionAsync(this Transform transform, Vector3 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.localPosition = position)
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionAsync(this Transform transform, Vector3 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (position, target) => target.localPosition = position)
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionXTo(this Transform transform, float targetX, float duration)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionXTo(this Transform transform, float targetX, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionXTo(this Transform transform, float targetX, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static UniTask ChangeLocalPositionXToAsync(this Transform transform, float targetX, float duration)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionXToAsync(this Transform transform, float targetX, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionXToAsync(this Transform transform, float targetX, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.x, targetX, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionYTo(this Transform transform, float targetY, float duration)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionYTo(this Transform transform, float targetY, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionYTo(this Transform transform, float targetY, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static UniTask ChangeLocalPositionYToAsync(this Transform transform, float targetY, float duration)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionYToAsync(this Transform transform, float targetY, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionYToAsync(this Transform transform, float targetY, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.y, targetY, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionZTo(this Transform transform, float targetZ, float duration)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static MotionHandle ChangeLocalPositionZTo(this Transform transform, float targetZ, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static MotionHandle ChangeLocalPositionZTo(this Transform transform, float targetZ, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static UniTask ChangeLocalPositionZToAsync(this Transform transform, float targetZ, float duration)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionZToAsync(this Transform transform, float targetZ, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionZToAsync(this Transform transform, float targetZ, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.z, targetZ, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionXAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionXAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionXAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z));
    }

    public static UniTask ChangeLocalPositionXAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionXAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionXAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.x, transform.localPosition.x + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (x, target) => target.localPosition = new Vector3(x, target.localPosition.y, target.localPosition.z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionYAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionYAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static MotionHandle ChangeLocalPositionYAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z));
    }

    public static UniTask ChangeLocalPositionYAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionYAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionYAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.y, transform.localPosition.y + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (y, target) => target.localPosition = new Vector3(target.localPosition.x, y, target.localPosition.z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalPositionZAmount(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static MotionHandle ChangeLocalPositionZAmount(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static MotionHandle ChangeLocalPositionZAmount(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z));
    }

    public static UniTask ChangeLocalPositionZAmountAsync(this Transform transform, float amount, float duration)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionZAmountAsync(this Transform transform, float amount, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static UniTask ChangeLocalPositionZAmountAsync(this Transform transform, float amount, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localPosition.z, transform.localPosition.z + amount, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (z, target) => target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z))
            .ToUniTask();
    }

    public static MotionHandle ChangeLocalScale(this Transform transform, Vector3 targetScale, float duration)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .BindWithState(transform, (scale, target) => target.localScale = scale);
    }

    public static MotionHandle ChangeLocalScale(this Transform transform, Vector3 targetScale, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (scale, target) => target.localScale = scale);
    }

    public static MotionHandle ChangeLocalScale(this Transform transform, Vector3 targetScale, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (scale, target) => target.localScale = scale);
    }

    public static UniTask ChangeLocalScaleAsync(this Transform transform, Vector3 targetScale, float duration)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .BindWithState(transform, (scale, target) => target.localScale = scale)
            .ToUniTask();
    }

    public static UniTask ChangeLocalScaleAsync(this Transform transform, Vector3 targetScale, float duration, Action onComplete)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (scale, target) => target.localScale = scale)
            .ToUniTask();
    }

    public static UniTask ChangeLocalScaleAsync(this Transform transform, Vector3 targetScale, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.localScale, targetScale, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (scale, target) => target.localScale = scale)
            .ToUniTask();
    }

    public static MotionHandle ChangeRotation(this Transform transform, Quaternion targetRotation, float duration)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation);
    }

    public static MotionHandle ChangeRotation(this Transform transform, Quaternion targetRotation, float duration, Action onComplete)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation);
    }

    public static MotionHandle ChangeRotation(this Transform transform, Quaternion targetRotation, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation);
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Quaternion targetRotation, float duration)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation)
            .ToUniTask();
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Quaternion targetRotation, float duration, Action onComplete)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation)
            .ToUniTask();
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Quaternion targetRotation, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.rotation, targetRotation, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = rotation)
            .ToUniTask();
    }

    public static MotionHandle ChangeRotation(this Transform transform, Vector3 targetRotation, float duration)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation));
    }

    public static MotionHandle ChangeRotation(this Transform transform, Vector3 targetRotation, float duration, Action onComplete)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation));
    }

    public static MotionHandle ChangeRotation(this Transform transform, Vector3 targetRotation, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation));
    }

    public static MotionHandle ChangeRotation(this Transform transform, Vector3 targetRotation, float duration, Action onComplete, int loop, LoopType loopType, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .WithLoops(loop, loopType)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation));
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Vector3 targetRotation, float duration)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation))
            .ToUniTask();
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Vector3 targetRotation, float duration, Action onComplete)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation))
            .ToUniTask();
    }

    public static UniTask ChangeRotationAsync(this Transform transform, Vector3 targetRotation, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(transform.rotation.eulerAngles, targetRotation, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(transform, (rotation, target) => target.rotation = Quaternion.Euler(rotation))
            .ToUniTask();
    }
    #endregion

    #region RectTransform
    public static MotionHandle ChangeSizeDelta(this RectTransform rectTransform, Vector2 targetSize, float duration)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size);
    }

    public static MotionHandle ChangeSizeDelta(this RectTransform rectTransform, Vector2 targetSize, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size);
    }

    public static MotionHandle ChangeSizeDelta(this RectTransform rectTransform, Vector2 targetSize, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size);
    }

    public static UniTask ChangeSizeDeltaAsync(this RectTransform rectTransform, Vector2 targetSize, float duration)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size)
            .ToUniTask();
    }

    public static UniTask ChangeSizeDeltaAsync(this RectTransform rectTransform, Vector2 targetSize, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size)
            .ToUniTask();
    }

    public static UniTask ChangeSizeDeltaAsync(this RectTransform rectTransform, Vector2 targetSize, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.sizeDelta, targetSize, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (size, target) => target.sizeDelta = size)
            .ToUniTask();
    }

    public static MotionHandle ChangeAnchoredPosition(this RectTransform rectTransform, Vector2 targetPosition, float duration)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position);
    }

    public static MotionHandle ChangeAnchoredPosition(this RectTransform rectTransform, Vector2 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position);
    }

    public static MotionHandle ChangeAnchoredPosition(this RectTransform rectTransform, Vector2 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position);
    }

    public static UniTask ChangeAnchoredPositionAsync(this RectTransform rectTransform, Vector2 targetPosition, float duration)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position)
            .ToUniTask();
    }

    public static UniTask ChangeAnchoredPositionAsync(this RectTransform rectTransform, Vector2 targetPosition, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position)
            .ToUniTask();
    }

    public static UniTask ChangeAnchoredPositionAsync(this RectTransform rectTransform, Vector2 targetPosition, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchoredPosition, targetPosition, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (position, target) => target.anchoredPosition = position)
            .ToUniTask();
    }

    public static MotionHandle ChangeAnchorMax(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax);
    }

    public static MotionHandle ChangeAnchorMax(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax);
    }

    public static MotionHandle ChangeAnchorMax(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax);
    }

    public static UniTask ChangeAnchorMaxAsync(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax)
            .ToUniTask();
    }

    public static UniTask ChangeAnchorMaxAsync(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax)
            .ToUniTask();
    }

    public static UniTask ChangeAnchorMaxAsync(this RectTransform rectTransform, Vector2 targetAnchorMax, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchorMax, targetAnchorMax, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMax, target) => target.anchorMax = anchorMax)
            .ToUniTask();
    }

    public static MotionHandle ChangeAnchorMin(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin);
    }

    public static MotionHandle ChangeAnchorMin(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin);
    }

    public static MotionHandle ChangeAnchorMin(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin);
    }

    public static UniTask ChangeAnchorMinAsync(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin)
            .ToUniTask();
    }

    public static UniTask ChangeAnchorMinAsync(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration, Action onComplete)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin)
            .ToUniTask();
    }

    public static UniTask ChangeAnchorMinAsync(this RectTransform rectTransform, Vector2 targetAnchorMin, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(rectTransform.anchorMin, targetAnchorMin, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(rectTransform, (anchorMin, target) => target.anchorMin = anchorMin)
            .ToUniTask();
    }
    #endregion

    #region Image
    public static MotionHandle ChangeColor(this UnityEngine.UI.Image image, Color targetColor, float duration)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeColor(this UnityEngine.UI.Image image, Color targetColor, float duration, Action onComplete)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeColor(this UnityEngine.UI.Image image, Color targetColor, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static UniTask ChangeColorAsync(this UnityEngine.UI.Image image, Color targetColor, float duration)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeColorAsync(this UnityEngine.UI.Image image, Color targetColor, float duration, Action onComplete)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeColorAsync(this UnityEngine.UI.Image image, Color targetColor, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(image.color, targetColor, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static MotionHandle ChangeAlpha(this UnityEngine.UI.Image image, float targetAlpha, float duration)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeAlpha(this UnityEngine.UI.Image image, float targetAlpha, float duration, Action onComplete)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeAlpha(this UnityEngine.UI.Image image, float targetAlpha, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color);
    }

    public static UniTask ChangeAlphaAsync(this UnityEngine.UI.Image image, float targetAlpha, float duration)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeAlphaAsync(this UnityEngine.UI.Image image, float targetAlpha, float duration, Action onComplete)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeAlphaAsync(this UnityEngine.UI.Image image, float targetAlpha, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        Color temp = image.color;
        temp.a = targetAlpha;
        return LMotion.Create(image.color, temp, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(image, (color, target) => target.color = color)
            .ToUniTask();
    }
    #endregion

    #region Text
    public static MotionHandle ChangeColor(this TMPro.TMP_Text text, Color targetColor, float duration)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeColor(this TMPro.TMP_Text text, Color targetColor, float duration, Action onComplete)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeColor(this TMPro.TMP_Text text, Color targetColor, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static UniTask ChangeColorAsync(this TMPro.TMP_Text text, Color targetColor, float duration)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeColorAsync(this TMPro.TMP_Text text, Color targetColor, float duration, Action onComplete)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeColorAsync(this TMPro.TMP_Text text, Color targetColor, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(text.color, targetColor, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static MotionHandle ChangeAlpha(this TMPro.TMP_Text text, float targetAlpha, float duration)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeAlpha(this TMPro.TMP_Text text, float targetAlpha, float duration, Action onComplete)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static MotionHandle ChangeAlpha(this TMPro.TMP_Text text, float targetAlpha, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color);
    }

    public static UniTask ChangeAlphaAsync(this TMPro.TMP_Text text, float targetAlpha, float duration)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeAlphaAsync(this TMPro.TMP_Text text, float targetAlpha, float duration, Action onComplete)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static UniTask ChangeAlphaAsync(this TMPro.TMP_Text text, float targetAlpha, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        Color temp = text.color;
        temp.a = targetAlpha;
        return LMotion.Create(text.color, temp, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (color, target) => target.color = color)
            .ToUniTask();
    }

    public static MotionHandle ChangeFontSize(this TMPro.TMP_Text text, int targetFontSize, float duration)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize);
    }

    public static MotionHandle ChangeFontSize(this TMPro.TMP_Text text, int targetFontSize, float duration, Action onComplete)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize);
    }

    public static MotionHandle ChangeFontSize(this TMPro.TMP_Text text, int targetFontSize, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize);
    }

    public static UniTask ChangeFontSizeAsync(this TMPro.TMP_Text text, int targetFontSize, float duration)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize)
            .ToUniTask();
    }

    public static UniTask ChangeFontSizeAsync(this TMPro.TMP_Text text, int targetFontSize, float duration, Action onComplete)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .WithOnComplete(onComplete)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize)
            .ToUniTask();
    }

    public static UniTask ChangeFontSizeAsync(this TMPro.TMP_Text text, int targetFontSize, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(text.fontSize, targetFontSize, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(text, (fontSize, target) => target.fontSize = fontSize)
            .ToUniTask();
    }
    #endregion

    #region Slider
    public static MotionHandle ChangeValue(this UnityEngine.UI.Slider slider, float targetValue, float duration)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .BindWithState(slider, (value, target) => target.value = value);
    }

    public static MotionHandle ChangeValue(this UnityEngine.UI.Slider slider, float targetValue, float duration, Action onComplete)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .WithOnComplete(onComplete)
            .BindWithState(slider, (value, target) => target.value = value);
    }

    public static MotionHandle ChangeValue(this UnityEngine.UI.Slider slider, float targetValue, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(slider, (value, target) => target.value = value);
    }

    public static UniTask ChangeValueAsync(this UnityEngine.UI.Slider slider, float targetValue, float duration)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .BindWithState(slider, (value, target) => target.value = value)
            .ToUniTask();
    }

    public static UniTask ChangeValueAsync(this UnityEngine.UI.Slider slider, float targetValue, float duration, Action onComplete)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .WithOnComplete(onComplete)
            .BindWithState(slider, (value, target) => target.value = value)
            .ToUniTask();
    }

    public static UniTask ChangeValueAsync(this UnityEngine.UI.Slider slider, float targetValue, float duration, Action onComplete, Ease easingType = Ease.Linear)
    {
        return LMotion.Create(slider.value, targetValue, duration)
            .WithEase(easingType)
            .WithOnComplete(onComplete)
            .BindWithState(slider, (value, target) => target.value = value)
            .ToUniTask();
    }
    #endregion
}
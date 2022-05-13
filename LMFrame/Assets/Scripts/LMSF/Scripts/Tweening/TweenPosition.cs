using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameObject对象位置Tween
/// </summary>
public class TweenPosition : UITweener
{
    public bool worldSpace = false;
    public bool is2D = false;
    public Vector3 from;
    public Vector3 to;

    Transform mTrans;
    RectTransform mRectTransform;

    public Transform cachedTransform { get { if(mTrans == null) mTrans = transform; return mTrans; } }

    public RectTransform cachedRectTransform { get { if(mRectTransform == null) mRectTransform = this.GetComponent<RectTransform>(); return mRectTransform; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>
    public Vector3 value
    {
        get
        {
            if(is2D)
            {
                return cachedRectTransform.anchoredPosition3D;
            }
            else
            {
                return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
            }
        }
        set
        {
            if(is2D)
            {
                cachedRectTransform.anchoredPosition3D = value;
            }
            else
            {
                if(worldSpace) cachedTransform.position = value;
                else cachedTransform.localPosition = value;
            }
        }
    }

    void Awake() { }

    /// <summary>
    /// Tween the value.
    /// </summary>
    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>
    static public TweenPosition Begin(GameObject go, float duration, Vector3 pos)
    {
        TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
        comp.from = comp.value;
        comp.to = pos;

        if(duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>
    static public TweenPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
    {
        TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
        comp.worldSpace = worldSpace;
        comp.from = comp.value;
        comp.to = pos;

        if(duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}

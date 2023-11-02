using DG.Tweening;
using Sources.cdreyer;
using UnityEngine;

namespace Sources.UI
{
    public class UIEndGame : MonoBehaviour
    {
        [SerializeField] ButtonBehaviour[] btns;

        void Start()
        {
            foreach (var b in btns)
            {
                b.transform.localScale = Vector3.zero;
                b.transform.DOScale(Vector3.one, 0.5f);
                b.AddListener(GameManager.GameManager.Instance.ReloadScene, InteractionType.ClickUp);
            };
        }
    }
}

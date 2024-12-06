using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YellowPanda.DevTools
{
    public class DraggableButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {

        [SerializeField] UnityEvent OnClick;

        Vector2 dragStart;
        Vector2 dragOffset;

        bool dragging;
        ButtonSnapType snapType;
        RectTransform rect;
        float speedY;
        Vector2 lastPosition;
        Vector2 pointerPosition;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            lastPosition = transform.position;
        }

        enum ButtonSnapType
        {
            Left,
            Right
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragStart = pointerPosition;
            dragOffset = (Vector2)transform.position - dragStart;
            dragging = true;
        }

        private void Update()
        {
            pointerPosition = Input.mousePosition;
            if (Input.touchCount > 0)
                pointerPosition = Input.GetTouch(0).position;

            if (dragging)
            {
                Vector2 targetPosition = pointerPosition + dragOffset;

                //Segue a posição do mouse ou do pointer
                Vector2 position = transform.position;
                position += (targetPosition - position) / 5f * Time.deltaTime * 60f;
                transform.position = position;

                speedY = rect.anchoredPosition.y - lastPosition.y; //Salva a velocidade Y do drag;
            }
            else
            {
                speedY += (0 - speedY) / 10f * Time.deltaTime * 60f; //Desacelera

                float width = rect.rect.width;
                if (snapType == ButtonSnapType.Left)
                {
                    Vector3 oldPosition = transform.position;
                    rect.anchorMin = new Vector2(0, 0.5f); //Garante que mudar a ancora não vai mudar a posição
                    rect.anchorMax = new Vector2(0, 0.5f);
                    transform.position = oldPosition;

                    Vector2 targetOffset = new Vector2(20, rect.offsetMin.y);
                    //Interpola a posição pra fazer snap no canto esquerdo
                    rect.offsetMin += (targetOffset - rect.offsetMin) / 5f * Time.deltaTime * 60f;
                    rect.offsetMax = new Vector2(rect.offsetMin.x + width, rect.offsetMax.y);
                }
                else if (snapType == ButtonSnapType.Right)
                {
                    Vector3 oldPosition = transform.position;
                    rect.anchorMin = new Vector2(1, 0.5f); //Garante que mudar a ancora não vai mudar a posição
                    rect.anchorMax = new Vector2(1, 0.5f);
                    transform.position = oldPosition;

                    Vector2 targetOffset = new Vector2(-20, rect.offsetMax.y);
                    //Interpola a posição pra fazer snap no canto direito
                    rect.offsetMax += (targetOffset - rect.offsetMax) / 5f * Time.deltaTime * 60f;
                    rect.offsetMin = new Vector2(rect.offsetMax.x - width, rect.offsetMin.y);
                }

                float newY = rect.anchoredPosition.y + speedY;

                //Garante que o botão não vai sair da tela sem querer
                if (newY < -Screen.height / 2f + rect.rect.height / 2f + 20) newY = -Screen.height / 2f + rect.rect.height / 2f + 20;
                if (newY > Screen.height / 2f - rect.rect.height / 2f - 20) newY = Screen.height / 2f - rect.rect.height / 2f - 20;

                rect.anchoredPosition = new Vector2( //Move Y baseado na velocidade que "jogou"
                            rect.anchoredPosition.x,
                            newY
                        );
            }

            lastPosition = rect.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;

            float mrx = pointerPosition.x / Screen.width;
            if (mrx < .5f)
            {
                snapType = ButtonSnapType.Left;
            }
            else
            {
                snapType = ButtonSnapType.Right;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!dragging)
            {
                OnClick?.Invoke();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}

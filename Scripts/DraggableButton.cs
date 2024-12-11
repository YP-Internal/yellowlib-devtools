using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YellowPanda.DevTools
{
    public class DraggableButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        enum ButtonSnapType
        {
            Left,
            Right
        }

        [SerializeField] UnityEvent OnClick;

        Vector2 _dragStart;
        Vector2 _dragOffset;

        bool _dragging;
        ButtonSnapType _snapType;
        RectTransform _rect;
        float _speedY;
        Vector2 _lastPosition;
        Vector2 _pointerPosition;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _lastPosition = transform.position;
        }

        private void Update()
        {
            _pointerPosition = Input.mousePosition;
            if (Input.touchCount > 0)
                _pointerPosition = Input.GetTouch(0).position;

            if (_dragging)
            {
                Vector2 targetPosition = _pointerPosition + _dragOffset;

                //Segue a posição do mouse ou do pointer
                Vector2 position = transform.position;
                position += (targetPosition - position) / 5f * Time.deltaTime * 60f;
                transform.position = position;

                _speedY = _rect.anchoredPosition.y - _lastPosition.y; //Salva a velocidade Y do drag;
            }
            else
            {
                _speedY += (0 - _speedY) / 10f * Time.deltaTime * 60f; //Desacelera

                float width = _rect.rect.width;
                if (_snapType == ButtonSnapType.Left)
                {
                    Vector3 oldPosition = transform.position;
                    _rect.anchorMin = new Vector2(0, 0.5f); //Garante que mudar a ancora não vai mudar a posição
                    _rect.anchorMax = new Vector2(0, 0.5f);
                    transform.position = oldPosition;

                    Vector2 targetOffset = new Vector2(20, _rect.offsetMin.y);
                    //Interpola a posição pra fazer snap no canto esquerdo
                    _rect.offsetMin += (targetOffset - _rect.offsetMin) / 5f * Time.deltaTime * 60f;
                    _rect.offsetMax = new Vector2(_rect.offsetMin.x + width, _rect.offsetMax.y);
                }
                else if (_snapType == ButtonSnapType.Right)
                {
                    Vector3 oldPosition = transform.position;
                    _rect.anchorMin = new Vector2(1, 0.5f); //Garante que mudar a ancora não vai mudar a posição
                    _rect.anchorMax = new Vector2(1, 0.5f);
                    transform.position = oldPosition;

                    Vector2 targetOffset = new Vector2(-20, _rect.offsetMax.y);
                    //Interpola a posição pra fazer snap no canto direito
                    _rect.offsetMax += (targetOffset - _rect.offsetMax) / 5f * Time.deltaTime * 60f;
                    _rect.offsetMin = new Vector2(_rect.offsetMax.x - width, _rect.offsetMin.y);
                }

                float newY = _rect.anchoredPosition.y + _speedY;

                //Garante que o botão não vai sair da tela sem querer
                if (newY < -Screen.height / 2f + _rect.rect.height / 2f + 20) newY = -Screen.height / 2f + _rect.rect.height / 2f + 20;
                if (newY > Screen.height / 2f - _rect.rect.height / 2f - 20) newY = Screen.height / 2f - _rect.rect.height / 2f - 20;

                _rect.anchoredPosition = new Vector2( //Move Y baseado na velocidade que "jogou"
                            _rect.anchoredPosition.x,
                            newY
                        );
            }

            _lastPosition = _rect.anchoredPosition;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStart = _pointerPosition;
            _dragOffset = (Vector2)transform.position - _dragStart;
            _dragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            float mrx = _pointerPosition.x / Screen.width;
            if (mrx < .5f)
            {
                _snapType = ButtonSnapType.Left;
            }
            else
            {
                _snapType = ButtonSnapType.Right;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_dragging)
            {
                OnClick?.Invoke();
            }
        }

        public void OnDrag(PointerEventData eventData) { }
    }
}

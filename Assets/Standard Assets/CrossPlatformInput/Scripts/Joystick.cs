using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

        //abenz
        public delegate void RollAction(Vector2 direction);
        public static event RollAction Roll;

        [Space(10)]
        [SerializeField]
        [Range(50, 150)]
        private int MovementRange = 100;

        [SerializeField]
        [Range(10, 50)]
        private int tapMargin;

        [SerializeField]
        [Range(0.1f, 1)]
        private float delayBetweenTapsToRoll;


        private int tapCount;
        private float lastDragTime;
        Vector3 lastDragPos;

        Vector3 m_StartPos;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        void OnEnable()
        {
            CreateVirtualAxes();
        }

        void Start()
        {
            m_StartPos = transform.position;
            //abenz
            tapCount = 0;
            lastDragPos = Vector3.zero;
            lastDragTime = 0;
        }

        //abenz
        void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Touch[] taps = Input.touches;
                if (taps.Length > 0)
                {
                    Touch lastTap = taps[taps.Length - 1];
                    if (InRistrictedArea(new Vector3(lastTap.position.x, lastTap.position.y, 0)))
                    {
                        if (lastTap.phase == TouchPhase.Began || lastTap.phase == TouchPhase.Stationary)
                        {
                            Vector3 newPos = new Vector3(lastTap.position.x - m_StartPos.x, lastTap.position.y - m_StartPos.y);
                            transform.position = Vector3.ClampMagnitude(newPos, MovementRange) + m_StartPos;
                            UpdateVirtualAxes(transform.position);
                        }
                        if (lastTap.phase == TouchPhase.Ended)
                        {
                            HandleRoll();
                            lastDragTime = Time.time;
                            lastDragPos = transform.position;

                            transform.position = m_StartPos;
                            UpdateVirtualAxes(m_StartPos);
                        }
                    }
                }

            }
            else // to test on pc
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 clickPos = Input.mousePosition;

                    if (InRistrictedArea(clickPos))
                    {
                        Vector3 newPos = new Vector3( clickPos.x - m_StartPos.x, clickPos.y - m_StartPos.y);
                        transform.position = Vector3.ClampMagnitude(newPos, MovementRange) + m_StartPos;
                        UpdateVirtualAxes(transform.position);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    HandleRoll();
                    lastDragTime = Time.time;
                    lastDragPos = transform.position;

                    transform.position = m_StartPos;
                    UpdateVirtualAxes(m_StartPos);
                }
            }
        }

        void UpdateVirtualAxes(Vector3 value)
        {
            var delta = m_StartPos - value;
            delta.y = -delta.y;
            delta /= MovementRange;
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(-delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
        }

        void CreateVirtualAxes()
        {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX)
            {
                m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;

            if (m_UseX)
            {
                int delta = (int)(data.position.x - m_StartPos.x);
                //abenz
                //delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
                newPos.x = delta;
            }

            if (m_UseY)
            {
                int delta = (int)(data.position.y - m_StartPos.y);
                //abenz
                //delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
                newPos.y = delta;
            }
            //abenz
            //transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
            transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;

            UpdateVirtualAxes(transform.position);
        }

        public void OnPointerUp(PointerEventData data)
        {
            //abenz
            HandleRoll();
            lastDragTime = Time.time;
            lastDragPos = transform.position;

            transform.position = m_StartPos;
            UpdateVirtualAxes(m_StartPos);
        }

        public void OnPointerDown(PointerEventData data)
        {
        }

        //abenz
        private void HandleRoll()
        {
            if (tapCount > 0)
            {
                if (SameDirectionAsLastPosition())
                {
                    tapCount++;
                    RollIfPossible();
                }
            }
            else
            {
                tapCount++;
            }
        }

        //abenz
        private void RollIfPossible()
        {
            if (Time.time - lastDragTime < delayBetweenTapsToRoll)
            {
                Roll(Vector3.ClampMagnitude(((Camera.main.ScreenToWorldPoint(transform.position) - Camera.main.ScreenToWorldPoint(m_StartPos)) *1000), MovementRange));
                tapCount = 0;
            }
            else
            {
                tapCount = 1;
            }
        }

        // abenz
        private bool SameDirectionAsLastPosition()
        {
            Vector3 oldPos = Camera.main.ScreenToWorldPoint(lastDragPos) - Camera.main.ScreenToWorldPoint(m_StartPos);
            Vector3 newPos = Camera.main.ScreenToWorldPoint(transform.position) - Camera.main.ScreenToWorldPoint(m_StartPos);

            if (oldPos == Vector3.zero)
            {
                return false;
            }

            if (Math.Sign(oldPos.x) == Math.Sign(newPos.x) && Math.Sign(oldPos.y) == Math.Sign(newPos.y))
            {
                return true;
            }
            return false;
        }

        //abenz
        private bool InRistrictedArea(Vector3 tap)
        {
            Vector3 vect = tap - m_StartPos;

            if (vect.magnitude < MovementRange + tapMargin )
            {
                return true;
            }
            return false;
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }
    }
}
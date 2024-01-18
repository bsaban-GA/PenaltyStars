using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public static TouchController Instance;

    public bool CanPlayerTouch = false;

    [Header("Force Info")]
    Vector2 _vecForce;
    [SerializeField] float FltMaxForceAmount = 5f;
    [SerializeField] float FltMinForceAmount = 0.5f;
    float _fltPlayerRotationAmount;

    [Header("Arrow Info")]
    [SerializeField] GameObject ObjArrowTrajectory;
    bool _isArrowSpawned = false;
    [SerializeField] float FltArrowSizer = 1f;
    GameObject _objArrow;
    Vector3 _vecArrowSizer = new Vector3(1f, 1f, 1f);

    [Header("Player Touch Info")]
    Vector2 _vecPlayerFirstTouchPos;
    Vector2 _vecPlayerTouchPos;
    Vector2 _vecPlayerSecondTouchPos;
    Vector2 _vecPlayerSecondTouchMomentPos;
    bool _isPlayerFirstTouchSaved = false;
    Touch _playerTouch;
    Touch _playerSecondTouch;
    GameObject _objSelectedPlayer;
    bool _hasPlayerSelected = false;
    bool _isFirstTouchAtLevel = true;
    bool _isPlayerSecondTouchSaved = false;

    void Awake() 
    {
        Instance = this;
    }

    void Update()
    {
        if(CanPlayerTouch)
            // TouchControl();

        MouseController();
    }

    void MouseController() {
        if(Input.GetMouseButton(0)) {
            _vecPlayerTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(_vecPlayerTouchPos, Vector2.zero);

            if(hit.collider != null && hit.collider.gameObject.tag == "friendlyPlayer") {
                _objSelectedPlayer = hit.collider.gameObject;

                if(!_isPlayerFirstTouchSaved) {
                    _vecPlayerFirstTouchPos = _vecPlayerTouchPos;
                    _isPlayerFirstTouchSaved = true;
                }

                if(!_hasPlayerSelected) {
                    _hasPlayerSelected = true;
                }

                if(_isFirstTouchAtLevel) {
                    _isFirstTouchAtLevel = false;
                    _objSelectedPlayer.GetComponent<PlayerSizer>().SetRingAsPasive();
                }
            }

            if(_hasPlayerSelected) {
                _vecForce = _vecPlayerTouchPos - _vecPlayerFirstTouchPos;

                // Debug.Log("Force magnitude is: " + _vecForce.magnitude);

                if(_vecForce.magnitude > FltMinForceAmount) {

                    if(!_isArrowSpawned) {
                        _objArrow = Instantiate(ObjArrowTrajectory, 
                                    _objSelectedPlayer.transform.position, 
                                    Quaternion.identity);

                        _isArrowSpawned = true;
                    }

                    if(_vecForce.magnitude > FltMaxForceAmount)
                        _vecForce = _vecForce / _vecForce.magnitude * FltMaxForceAmount;

                    _objArrow.transform.localScale = _vecArrowSizer * _vecForce.magnitude * FltArrowSizer;

                    //Rotate player towards force vector
                    _fltPlayerRotationAmount = Mathf.Atan2(_vecForce.y, _vecForce.x) * Mathf.Rad2Deg;

                    _objArrow.transform.rotation = Quaternion.Euler(0f, 0f, _fltPlayerRotationAmount - 180f);

                }
                else {
                    if(_objArrow != null) {
                        Destroy(_objArrow);
                        _isArrowSpawned = false;
                    }
                }
            }
        }
        else {
            _isPlayerFirstTouchSaved = false;
            _hasPlayerSelected = false;

            if(_isArrowSpawned) {
                CanPlayerTouch = false;
                Destroy(_objArrow);
                _isArrowSpawned = false;

                _objSelectedPlayer.GetComponent<Rigidbody2D>().AddForce(-_vecForce * 3f, ForceMode2D.Impulse);

                LevelController.Instance.CallGoalCountDown();

                _vecForce = Vector2.zero;
                ComboController.Instance.ShouldComboDecrease = false;
            }
        }
    }

    void TouchControl() {
        if(Input.touchCount > 0) {
            _playerTouch = Input.GetTouch(0);

            if(Input.touchCount == 2) {
                _playerSecondTouch = Input.GetTouch(1);
                
                _vecPlayerSecondTouchPos = Camera.main.ScreenToWorldPoint(_playerSecondTouch.position);

                if(!_isPlayerSecondTouchSaved) {
                    _isPlayerSecondTouchSaved = true;
                    _vecPlayerSecondTouchMomentPos = _vecPlayerSecondTouchPos;
                }
            }

            _vecPlayerTouchPos = Camera.main.ScreenToWorldPoint(_playerTouch.position);
            RaycastHit2D hit = Physics2D.Raycast(_vecPlayerTouchPos, Vector2.zero);

            if(hit.collider != null && hit.collider.gameObject.tag == "friendlyPlayer") {
                _objSelectedPlayer = hit.collider.gameObject;

                if(!_isPlayerFirstTouchSaved) {
                    _vecPlayerFirstTouchPos = _vecPlayerTouchPos;
                    _isPlayerFirstTouchSaved = true;
                }

                if(!_hasPlayerSelected) {
                    _hasPlayerSelected = true;
                }

                if(_isFirstTouchAtLevel) {
                    _isFirstTouchAtLevel = false;
                    _objSelectedPlayer.GetComponent<PlayerSizer>().SetRingAsPasive();
                }
            }

            if(_hasPlayerSelected) {
                if(_isPlayerSecondTouchSaved)
                    _vecForce = (_vecPlayerTouchPos - _vecPlayerFirstTouchPos) - (_vecPlayerSecondTouchPos - _vecPlayerSecondTouchMomentPos);
                else 
                    _vecForce = _vecPlayerTouchPos - _vecPlayerFirstTouchPos;
                // Debug.Log("Force magnitude is: " + _vecForce.magnitude);

                if(_vecForce.magnitude > FltMinForceAmount) {

                    if(!_isArrowSpawned) {
                        _objArrow = Instantiate(ObjArrowTrajectory, 
                                    _objSelectedPlayer.transform.position, 
                                    Quaternion.identity);

                        _isArrowSpawned = true;
                    }

                    if(_vecForce.magnitude > FltMaxForceAmount)
                        _vecForce = _vecForce / _vecForce.magnitude * FltMaxForceAmount;

                    _objArrow.transform.localScale = _vecArrowSizer * _vecForce.magnitude * FltArrowSizer;

                    //Rotate player towards force vector
                    _fltPlayerRotationAmount = Mathf.Atan2(_vecForce.y, _vecForce.x) * Mathf.Rad2Deg;

                    _objArrow.transform.rotation = Quaternion.Euler(0f, 0f, _fltPlayerRotationAmount - 180f);

                }
                else {
                    if(_objArrow != null) {
                        Destroy(_objArrow);
                        _isArrowSpawned = false;
                    }
                }
            }
        }
        else {
            _isPlayerFirstTouchSaved = false;
            _isPlayerSecondTouchSaved = false;
            _hasPlayerSelected = false;

            if(_isArrowSpawned) {
                CanPlayerTouch = false;
                Destroy(_objArrow);
                _isArrowSpawned = false;

                _objSelectedPlayer.GetComponent<Rigidbody2D>().AddForce(-_vecForce * 3f, ForceMode2D.Impulse);

                LevelController.Instance.CallGoalCountDown();

                _vecForce = Vector2.zero;
                ComboController.Instance.ShouldComboDecrease = false;
            }
        }
    }

}

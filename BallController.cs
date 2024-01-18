using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance;

    [SerializeField] Rigidbody2D RbBall;

    public bool ShouldBallStop = false;

    bool _isBallActive = false;

    bool _hasPlayerScored = false;

    [SerializeField] GameObject ObjGoalEffect;

    [Header("Ball Sprites & Renderer")]
    [SerializeField] SpriteRenderer SprBall;
    [SerializeField] Sprite[] SprArrBalls = new Sprite[2]; // 0 for normal ball and 1 for strike ball

    [Header("Trail")]
    [SerializeField] TrailRenderer TrlEffect;
    [SerializeField] Color[] ClrNoCombo = new Color[2];
    [SerializeField] Color[] ClrCombo = new Color[2];


    void Awake() 
    {
        Instance = this;
    }

    void Start() 
    {
        if(ComboController.Instance.GetComboPoint() > 1)
            ChangeBallSprite(1);
    }

    void Update() 
    {
        ControlBall();
    }

    //A method to control the ball via its speed
    void ControlBall() {
        if(RbBall.velocity.magnitude == 0f) {
            if(_isBallActive) {
                _isBallActive = false;

                //If player couldn't score at current level
                // if(!_hasPlayerScored) {
                //     HealthController.Instance.DecreaseHealth();
                //     LevelController.Instance.ResetLevel();

                //     Debug.Log("Yeni level gelmeli fakat gol olmadi");
                // }
            }
        }
        else {
            _isBallActive = true;
        }

        if(ShouldBallStop) {
            if(RbBall.velocity.magnitude > 0)
                RbBall.velocity = RbBall.velocity / 2f;
        }
    }

    IEnumerator CallNewLevel() {
        yield return new WaitForSeconds(0.5f);

        LevelController.Instance.ResetLevel();
        Debug.Log("yeni level gelmeli zira gol oldu");
    }


    public void ChangeBallSprite(int ballSprite) {
        SprBall.sprite = SprArrBalls[ballSprite];

        if(ballSprite == 0) {
            TrlEffect.startColor = ClrNoCombo[0];
            TrlEffect.endColor = ClrNoCombo[1];

            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[] {new GradientColorKey(ClrNoCombo[0], 0f), new GradientColorKey(ClrNoCombo[1], 0.85f)},
                new GradientAlphaKey[] {new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.725f, 0.85f)}
            );

            TrlEffect.colorGradient = gradient;
        }
        else {
            TrlEffect.startColor = ClrCombo[0];
            TrlEffect.endColor = ClrCombo[1];

            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[] {new GradientColorKey(ClrCombo[0], 0f), new GradientColorKey(ClrCombo[1], 0.85f)},
                new GradientAlphaKey[] {new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.725f, 0.85f)}
            );

            TrlEffect.colorGradient = gradient;
        }
    }

    public bool GetHasPlayerScored() {
        return _hasPlayerScored;
    }

    /* -------------------------------------------- */

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "goalEntrance") {
            if(!_hasPlayerScored) {
                StopCoroutine(LevelController.Instance.co);
                _isBallActive = false;
                _hasPlayerScored = true;
                RbBall.velocity = RbBall.velocity / 2f;
                ScoreController.Instance.IncreaseScore();

                AudioController.Instance.PlayGoalCheerSound();

                Instantiate(ObjGoalEffect, new Vector3(-13f, 0f, 0f), Quaternion.identity);

                // StartCoroutine(CallNewLevel());

            }
        }
    }

}

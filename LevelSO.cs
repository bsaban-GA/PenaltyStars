using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels", fileName = "New Level")]
public class LevelSO : ScriptableObject
{
    [SerializeField] List<Vector3> FriendlyPlayerPos;
    [SerializeField] List<Vector3> OpponentPlayerPos;
    [SerializeField] Vector3 BallPos;

    public List<Vector3> GetFriendlyPlayerPosList() {
        return FriendlyPlayerPos;
    }

    public List<Vector3> GetOpponentPlayerPosList() {
        return OpponentPlayerPos;
    }

    public Vector3 GetBallPos() {
        return BallPos;
    }
}

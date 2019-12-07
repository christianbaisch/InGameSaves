using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public List<float> livingTargetPositionX = new List<float>();
    public List<float> livingTargetPositionY = new List<float>();
    public List<float> livingTargetPositionZ = new List<float>();
    public List<int> livingTargetsTypes = new List<int>();

    public float playerX = 0f;
    public float playerY = 0f;
    public float playerZ = 0f;

    public float rotX = 0f;
    public float rotY = 0f;
    public float rotZ = 0f;

    public int health = 0;
    public int armor = 0;
    public int score = 0;

    public int pistolAmmo = 0;
    public int shotgunAmmo = 0;
    public int rifleAmmo = 0;

    public int waveCountdown = 0;
    public int enemiesLeft = 0;

}

/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class Game : MonoBehaviour
{

	private static Game singleton;
	[SerializeField]
	RobotSpawn[] spawns;
	[SerializeField]
	GameObject[] robotTypes;
	public int enemiesLeft;
	public GameObject gameOverPanel;
	public GameUI gameUI;
	public GameObject player;
	public int score;
	public int waveCountdown;
	public bool isGameOver;

	public Gun myGunP;
	public Gun myGunR;
	public Gun myGunS;

	[SerializeField]
    private GameObject menu;

	public bool isPaused = false;

	// 1
	void Start()
	{
		singleton = this;
		StartCoroutine("increaseScoreEachSecond");
		isGameOver = false;
		Time.timeScale = 1;
		waveCountdown = 30;
		enemiesLeft = 0;
		StartCoroutine("updateWaveTimer");
		SpawnRobots();
	}

	// 2
	private void SpawnRobots()
	{
		foreach (RobotSpawn spawn in spawns)
		{
			spawn.SpawnRobot();
			enemiesLeft++;
		}
		gameUI.SetEnemyText(enemiesLeft);
	}

	public void Pause()
    {
        menu.SetActive(true);
        Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        isPaused = true;
		player.GetComponent<FirstPersonController>().enabled = false;
		player.GetComponent<CharacterController>().enabled = false;
    }

    public void Unpause()
    {
        menu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        isPaused = false;
		player.GetComponent<FirstPersonController>().enabled = true;
		player.GetComponent<CharacterController>().enabled = true;
    }


	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (isPaused)
			{
				Unpause();
			}
			else if(!isGameOver)
			{
				Pause();
			}
		}
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();
		int i = 0;
		GameObject[] targets;

		targets = GameObject.FindGameObjectsWithTag("Robot");
		
		foreach (GameObject targetGameObject in targets)
		{
			Robot target = targetGameObject.GetComponent<Robot>();
			if (target.isDead == false)
			{
			save.livingTargetPositionX.Add(target.transform.position.x);
			save.livingTargetPositionY.Add(target.transform.position.y);
			save.livingTargetPositionZ.Add(target.transform.position.z);
			save.livingTargetsTypes.Add((int)target.GetComponent<Robot>().robotType);
			i++;
			}
		}
		
		Player playerscript = player.GetComponent<Player>();
		Gun gunPistolScript = myGunP.GetComponent<Gun>();

		save.pistolAmmo = playerscript.pistolCount();
		save.shotgunAmmo = playerscript.shotgunCount();
		save.rifleAmmo = playerscript.rifleCount();

		save.health = playerscript.health;
		save.armor = playerscript.armor;
		save.score = score;
		save.playerX = player.transform.position.x;
		save.playerY = player.transform.position.y;
		print("Y angle: " + save.playerY);
		save.playerZ = player.transform.position.z;

		save.rotX = player.transform.rotation.x;
		save.rotY = player.transform.eulerAngles.y;
		save.rotZ = player.transform.rotation.z;

		save.waveCountdown = waveCountdown;
		save.enemiesLeft = enemiesLeft;

		return save;
	}

	public void SaveGame()
	{
		// 1
		Save save = CreateSaveGameObject();

		// 2
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, save);
		file.Close();

		// 3
		//hits = 0;
		//shots = 0;
		//shotsText.text = "Shots: " + shots;
		//hitsText.text = "Hits: " + hits;

		//ClearRobots();
		//ClearBullets();
		Debug.Log("Game Saved");
	}

	public void LoadGame()
	{ 
		// 1
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			ClearBullets();
			ClearRobots();
			//RefreshRobots();

			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			// 4
			//shotsText.text = "Shots: " + save.shots;
			//hitsText.text = "Hits: " + save.hits;
			Player playerscript = player.GetComponent<Player>();
			playerscript.health = save.health;
			//save.ammo = shots;
			playerscript.armor = save.armor;
			score = save.score;

			Vector3 p = new Vector3(save.playerX, save.playerY, save.playerZ);
			Vector3 r = new Vector3(save.rotX, save.rotY, save.rotZ);
			player.transform.position = p;
			//player.transform.rotation = Quaternion.Euler(r);
			player.transform.rotation = Quaternion.AngleAxis (33, Vector3.forward);
			print("Y angle: " + save.playerY);
			print("Player Y angle: " + player.transform.eulerAngles);


			playerscript.resetPistolAmmo(save.pistolAmmo);
			playerscript.resetShotgunAmmo(save.shotgunAmmo);
			playerscript.resetRifleAmmo(save.rifleAmmo);

			// 3
			
			for (int i = 0; i < save.livingTargetPositionX.Count; i++)
			{
			Vector3 pos = new Vector3(save.livingTargetPositionX[i], save.livingTargetPositionY[i], save.livingTargetPositionZ[i]);
			GameObject robot = Instantiate(robotTypes[save.livingTargetsTypes[i]]);
			robot.transform.position = pos;
			//int position = save.livingTargetPositions[i];
			//Target target = targets[position].GetComponent<Target>();
			//target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
			//target.GetComponent<Target>().ResetDeathTimer();
			}

			waveCountdown = save.waveCountdown;
			enemiesLeft = save.enemiesLeft;

			gameUI.SetScoreText(score);
			gameUI.SetWaveText(waveCountdown);
			gameUI.SetEnemyText(enemiesLeft);
			gameUI.UpdateUI();

			Debug.Log("Game Loaded");

			Unpause();
		}
		else
		{
			Debug.Log("No game saved!");
		}
	}

	private void ClearRobots()
	{
		GameObject[] targets;

		targets = GameObject.FindGameObjectsWithTag("Robot");
		foreach( GameObject robot in targets)
		{
			Destroy(robot);
		}
	}

	private void ClearBullets()
	{
		GameObject[] targets;

		targets = GameObject.FindGameObjectsWithTag("RobotMissle");
		foreach( GameObject robotMissle in targets)
		{
			Destroy(robotMissle);
		}
	}




	private IEnumerator updateWaveTimer()
	{
		while (!isGameOver)
		{
			yield return new WaitForSeconds(1f);
			waveCountdown--;
			gameUI.SetWaveText(waveCountdown);

			// Spawn next wave and restart count down
			if (waveCountdown == 0)
			{
				SpawnRobots();
				waveCountdown = 30;
				gameUI.ShowNewWaveText();
			}
		}
	}

	IEnumerator increaseScoreEachSecond()
	{
		while (!isGameOver)
		{
			yield return new WaitForSeconds(1);
			score += 1;
			gameUI.SetScoreText(score);
		}
	}

	public void AddRobotKillToScore()
	{
		score += 10;
		gameUI.SetScoreText(score);
	}

	public static void RemoveEnemy()
	{
		singleton.enemiesLeft--;
		singleton.gameUI.SetEnemyText(singleton.enemiesLeft);

		// Give player bonus for clearing the wave before timer is done
		if (singleton.enemiesLeft == 0)
		{
			singleton.score += 50;
			singleton.gameUI.ShowWaveClearBonus();
		}
	}

	// 1
	public void OnGUI()
	{
		if (isGameOver && Cursor.visible == false)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	// 2
	public void GameOver()
	{
		isGameOver = true;
		Time.timeScale = 0;
		player.GetComponent<FirstPersonController>().enabled = false;
		player.GetComponent<CharacterController>().enabled = false;
		gameOverPanel.SetActive(true);
	}

	// 3
	public void RestartGame()
	{
		SceneManager.LoadScene(Constants.SceneBattle);
		gameOverPanel.SetActive(true);
	}

	// 4
	public void Exit()
	{
		Application.Quit();
	}

	// 5
	public void MainMenu()
	{
		SceneManager.LoadScene(Constants.SceneMenu);
	}

}

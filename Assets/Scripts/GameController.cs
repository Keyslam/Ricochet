using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	[SerializeField]
	private List<AudioClip> breaths = null;

	[SerializeField]
	private AudioSource mainTrack = null;
	[SerializeField]
	private AudioLowPassFilter mainTrackLowPass = null;

	[SerializeField]
	private AudioLowPassFilter rainLowPass = null;

	[SerializeField]
	private AudioSource gunshot = null;

	[SerializeField]
	private AudioSource breath = null;

	[SerializeField]
	private TMP_Text text = null;
	[SerializeField]
	private Image background = null;

	[SerializeField]
	private PlayerController playerController = null;
	[SerializeField]
	private CustomCursor customCursor = null;

	[SerializeField]
	private PostProcessProfile postProcessProfile = null;

	[SerializeField]
	private SpriteRenderer visorPrefab = null;

	private List<SpriteRenderer> visors = new List<SpriteRenderer>();

	[SerializeField]
	private GameObject bulletPrefab = null;

	[SerializeField]
	private GameObject bulletParticlePrefab = null;

	private GameObject bullet = null;
	private GameObject bulletParticle = null;

	[SerializeField]
	private CanvasGroup canvasGroup = null;

	[SerializeField]
	private ParticleSystem deathSystem = null;

	[SerializeField]
	private CanvasGroup titleGroup = null;

	[SerializeField]
	private Button startButton = null;

	private List<Vector2> hitPoints = null;
	private int target = 0;
	private Vector2 velocity = Vector2.zero;

	[SerializeField]
	private AudioSource[] layers = null;
	[SerializeField]
	private AudioLowPassFilter[] lowPassLayers = null;

	[SerializeField]
	private AudioSource hitSound = null;

	private Actor targetActor = null;

	private bool wasHit = false;

	[SerializeField]
	private Button restartButton = null;

	[SerializeField]
	private AudioSource introTrack = null;

	[SerializeField]
	private CanvasGroup endGroup = null;

	[SerializeField]
	private Button endButton = null;

	public void Awake()
	{
		for (int i = 0; i < 100; i++)
		{
			SpriteRenderer visor = Instantiate(visorPrefab, new Vector3(1000, 1000, 0), Quaternion.identity);
			visors.Add(visor);
		}


		Vignette vignette = postProcessProfile.GetSetting<Vignette>();
		vignette.intensity.Override(0.1f);
	}

	private bool aiming = false;
	private Vector3 gmousePos = Vector3.zero;

	private void FixedUpdate()
	{
		if (aiming)
		{
			gmousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			gmousePos.z = 0.0f;
		}
		DoShoot();

		if (bullet != null)
		{
			Vector2 targetPos = hitPoints[target];
			Vector2 delta = (targetPos - (Vector2)bullet.transform.position).normalized;

			velocity += delta * 50.0f;
			velocity = Vector2.ClampMagnitude(velocity, 25.0f);

			Vector2 deltaVelocity = velocity * Time.fixedDeltaTime;
			bullet.transform.position += new Vector3(deltaVelocity.x, deltaVelocity.y);

			bulletParticle.transform.position = bullet.transform.position;

			if (Vector2.Dot(velocity, delta) <= 0 || Vector2.Distance(bullet.transform.position, targetPos) <= 0.1f)
			{
				target++;
				velocity = Vector2.zero;
				if (target == hitPoints.Count)
				{
					target = 0;
					velocity = Vector2.zero;
					Destroy(bullet);
					bullet = null;
				}
			}
		}
	}

	private IEnumerator StartLayer(int index)
	{
		AudioSource audioSource = layers[index];

		float currTime = 0.0f;
		while (currTime < 0.5f)
		{
			float progress = currTime / 0.5f;
			audioSource.volume = Mathf.Lerp(0, 1, progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		audioSource.volume = 1.0f;
	}

	private IEnumerator StopAllMusic()
	{
		float currTime = 0.0f;
		while (currTime < 3.0f)
		{
			float progress = currTime / 3.0f;

			mainTrack.volume = Mathf.Lerp(1.0f, 0.0f, progress);
			foreach (AudioSource audioSource in layers)
				audioSource.volume = Mathf.Lerp(1.0f, 0.0f, progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}

		mainTrack.volume = 0.0f;
		foreach (AudioSource audioSource in layers)
			audioSource.volume = 0.0f;
	}

	public void OnEnable()
	{
		restartButton.onClick.AddListener(OnClick);
		startButton.onClick.AddListener(OnStart);
		endButton.onClick.AddListener(OnEnd);
	}

	public void OnDisable()
	{
		restartButton.onClick.RemoveListener(OnClick);
		startButton.onClick.RemoveListener(OnStart);
		endButton.onClick.RemoveListener(OnEnd);
	}

	public void OnEnd()
	{
		Application.Quit();
	}

	public bool started = false;
	public void OnStart()
	{
		if (started)
			return;

		started = true;

		StartCoroutine(StartMusic());
		StartCoroutine(GameRoutine());
	}

	private IEnumerator StartMusic()
	{
		introTrack.loop = false;

		while (introTrack.isPlaying)
			yield return null;

		mainTrack.Play();
		foreach (AudioSource audioSource in layers)
			audioSource.Play();
	}

	private IEnumerator ShowEnd()
	{
		float currTime = 0.0f;
		while (currTime < 1.0f)
		{
			float progress = currTime / 1.0f;
			endGroup.alpha = Mathf.Lerp(0.0f, 1.0f, progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		endGroup.alpha = 1.0f;
		endGroup.interactable = true;
		endGroup.blocksRaycasts = true;
	}

	private IEnumerator GameRoutine()
	{
		float currTime = 0.0f;
		while (currTime < 1.0f)
		{
			float progress = currTime / 1.0f;
			titleGroup.alpha = Mathf.Lerp(1.0f, 0, progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		titleGroup.alpha = 0.0f;
		titleGroup.interactable = false;
		titleGroup.blocksRaycasts = false;

		yield return StartCoroutine(ShowText("An assassins job is simple."));
		yield return StartCoroutine(ShowText("One target..."));
		yield return StartCoroutine(ShowText("One shot..."));
		yield return StartCoroutine(ShowText("One kill."));

		yield return StartCoroutine(Delay(0.5f));

		yield return StartCoroutine(DoLevel("Level_1"));

		yield return StartCoroutine(ShowText("Sometimes bystanders make it tricky."));
		

		yield return StartCoroutine(DoLevel("Level_2"));

		yield return StartCoroutine(ShowText("Or they hide behind an impenetrable wall."));
		

		yield return StartCoroutine(DoLevel("Level_3"));

		yield return StartCoroutine(ShowText("Regardless."));
		yield return StartCoroutine(ShowText("From the right angle..."));
		yield return StartCoroutine(ShowText("I'll ALWAYS take my shot."));
		StartCoroutine(StartLayer(0));

		yield return StartCoroutine(DoLevel("Level_4"));

		yield return StartCoroutine(ShowText("My targets.."));
		yield return StartCoroutine(ShowText("They deserve what's waiting for them."));
		StartCoroutine(StartLayer(1));

		yield return StartCoroutine(DoLevel("Level_5"));

		yield return StartCoroutine(ShowText("While they celebrate what has been won."));
		StartCoroutine(StartLayer(2));

		yield return StartCoroutine(DoLevel("Level_6"));

		yield return StartCoroutine(ShowText("Or prepare for the next."));
		StartCoroutine(StartLayer(3));

		yield return StartCoroutine(DoLevel("Level_7"));

		yield return StartCoroutine(ShowText("Useless training until the end..."));
		StartCoroutine(StartLayer(4));

		yield return StartCoroutine(DoLevel("Level_8"));

		yield return StartCoroutine(ShowText("Nobody will stand in my way."));
		yield return StartCoroutine(DoLevel("Level_Bonus"));

		yield return StartCoroutine(ShowText("I'll be coming for them."));
		yield return StartCoroutine(ShowText("All of them."));

		yield return StartCoroutine(DoLevel("Level_9"));

		yield return StartCoroutine(ShowText("These so called 'country protectors'."));

		yield return StartCoroutine(DoLevel("Level_10"));
		
		yield return StartCoroutine(ShowText("Because when you need to take the shot."));
		yield return StartCoroutine(ShowText("That one shot..."));
		yield return StartCoroutine(ShowText("The shot that matters most..."));

		yield return StartCoroutine(DoLevel("Level_11"));

		yield return StartCoroutine(StopAllMusic());

		yield return StartCoroutine(ShowText("You had better not hit my daughter.", 4.0f));

		yield return StartCoroutine(ShowEnd());
	}

	private IEnumerator DoLevel(string sceneName)
	{
		aiming = true;
		playerController.controllable = true;
		customCursor.active = true;

		wasHit = false;

		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

		yield return StartCoroutine(BackgroundFadeOut());

		yield return StartCoroutine(WaitForShoot());

		aiming = false;
		playerController.controllable = false;
		customCursor.active = false;

		yield return StartCoroutine(ShootRoutine());
		yield return StartCoroutine(BackgroundFadeIn());

		playerController.transform.position = new Vector3(0, -1.61f, 0);

		foreach (SpriteRenderer visor in visors)
			visor.color = new Color32(255, 255, 255, 255);

		AsyncOperation unload = SceneManager.UnloadSceneAsync(sceneName);

		if (wasHit)
		{
			yield return StartCoroutine(ShowText("Hit."));
		}
		else
		{
			yield return StartCoroutine(ShowText("Miss."));

			yield return StartCoroutine(WaitForRestart());
			yield return StartCoroutine(DoLevel(sceneName));
		}

		yield return unload;
	}

	private bool clicked = false;

	private void OnClick()
	{
		clicked = true;
	}

	private IEnumerator WaitForRestart()
	{
		restartButton.interactable = true;

		float currTime = 0.0f;
		while (currTime < 0.1f)
		{
			float progress = currTime / 0.1f;
			canvasGroup.alpha = Mathf.Lerp(0, 1, progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		canvasGroup.alpha = 1.0f;

		while (!clicked)
			yield return null;

		canvasGroup.alpha = 0.0f;
		clicked = false;
		restartButton.interactable = false;
	}

	private IEnumerator WaitForShoot()
	{
		while (!Input.GetMouseButtonDown(0))
			yield return null;
	}

	private IEnumerator TextFadeIn(string message, float time = 0.5f)
	{
		text.text = message;

		float currTime = 0.0f;
		while (currTime < time)
		{
			float progress = currTime / time;
			text.color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}

		text.color = new Color32(255, 255, 255, 255);
	}

	private IEnumerator TextFadeOut(float time = 0.5f)
	{
		float currTime = 0.0f;
		while (currTime < time)
		{
			float progress = currTime / time;
			text.color = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 0), progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}

		text.color = new Color32(255, 255, 255, 0);
	}

	private IEnumerator BackgroundFadeIn(float time = 0.5f)
	{
		float currTime = 0.0f;
		while (currTime < time)
		{
			float progress = currTime / time;
			background.color = Color32.Lerp(new Color32(38, 34, 38, 0), new Color32(38, 34, 38, 255), progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}

		background.color = new Color32(38, 34, 38, 255);
	}

	private IEnumerator BackgroundFadeOut(float time = 0.5f)
	{
		float currTime = 0.0f;
		while (currTime < time)
		{
			float progress = currTime / time;
			background.color = Color32.Lerp(new Color32(38, 34, 38, 255), new Color32(38, 34, 38, 0), progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}

		background.color = new Color32(38, 34, 38, 0);
	}

	private IEnumerator Delay(float time = 0.5f, bool unscaled = false)
	{
		float currTime = 0.0f;
		while (currTime < time)
		{
			currTime += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator ShowText(string message, float showTime = 2.5f, float waitTime = 1.0f, float fadeInTime = 0.5f, float fadeOutTime = 0.5f)
	{
		yield return StartCoroutine(TextFadeIn(message, fadeInTime));
		yield return StartCoroutine(Delay(showTime));
		yield return StartCoroutine(TextFadeOut(fadeOutTime));
		yield return StartCoroutine(Delay(waitTime));
	}

	private IEnumerator ShootRoutine()
	{
		playerController.controllable = false;
		customCursor.active = false;

		Vignette vignette = postProcessProfile.GetSetting<Vignette>();

		breath.PlayOneShot(breaths[Random.Range(0, breaths.Count - 1)]);

		

		float currTime = 0.0f;
		while (currTime < 0.8f)
		{
			float progress = currTime / 0.8f;

			//mainTrack.pitch = EaseOutQuad(1.0f, 0.5f, progress);
			float freq = EaseOutQuad(22000.0f, 600.0f, progress);
			mainTrackLowPass.cutoffFrequency = freq;
			foreach (AudioLowPassFilter audioSource in lowPassLayers)
				audioSource.cutoffFrequency = freq;


			rainLowPass.cutoffFrequency = EaseOutQuad(22000.0f, 600.0f, progress);
			Time.timeScale = EaseOutQuad(1.0f, 0.1f, progress);
			vignette.intensity.Override(EaseOutQuad(0.1f, 0.3f, progress));

			foreach (SpriteRenderer visor in visors)
				visor.color = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 0), progress);

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		mainTrackLowPass.cutoffFrequency = 600.0f;
		foreach (AudioLowPassFilter audioSource in lowPassLayers)
			audioSource.cutoffFrequency = 600.0f;
		rainLowPass.cutoffFrequency = 600.0f;
		//mainTrack.pitch = 0.5f;
		Time.timeScale = 0.1f;
		vignette.intensity.Override(0.3f);
		foreach (SpriteRenderer visor in visors)
			visor.color = new Color32(255, 255, 255, 0);

		BounceRay hit = BounceRay.Cast(playerController.transform.position, gmousePos - playerController.transform.position, 10.0f, LayerMask.GetMask("Bounceable"), LayerMask.GetMask("Actor"));
		hitPoints = hit.endPoints;

		targetActor = null;
		if (hit.hit != null && ((RaycastHit2D)hit.hit).collider != null)
		{
			Actor actor = ((RaycastHit2D)hit.hit).collider.GetComponent<Actor>();
			targetActor = actor;

			if (targetActor.isTarget)
				wasHit = true;
		}

		gunshot.Play();
		bullet = Instantiate(bulletPrefab, playerController.transform.position, Quaternion.identity);
		bulletParticle = Instantiate(bulletParticlePrefab, playerController.transform.position, Quaternion.identity);

		StartCoroutine(BulletKill());

		yield return StartCoroutine(Delay(0.2f, true));

		currTime = 0.0f;
		while (currTime < 0.5f)
		{
			float progress = currTime / 0.5f;


			//mainTrack.pitch = EaseOutQuad(0.5f, 1.0f, progress);
			float freq = EaseOutQuad(600.0f, 22000.0f, progress);
			mainTrackLowPass.cutoffFrequency = freq;
			foreach (AudioLowPassFilter audioSource in lowPassLayers)
				audioSource.cutoffFrequency = freq;
			rainLowPass.cutoffFrequency = EaseOutQuad(600.0f, 22000.0f, progress);
			Time.timeScale = EaseOutQuad(0.1f, 1.0f, progress);
			vignette.intensity.Override(EaseOutQuad(0.3f, 0.1f, progress));

			currTime += Time.unscaledDeltaTime;

			yield return null;
		}
		mainTrackLowPass.cutoffFrequency = 22000.0f;
		foreach (AudioLowPassFilter audioSource in lowPassLayers)
			audioSource.cutoffFrequency = 22000.0f;
		rainLowPass.cutoffFrequency = 22000.0f;
		//mainTrack.pitch = 1.0f;
		Time.timeScale = 1.0f;
		vignette.intensity.Override(0.1f);
	}

	public IEnumerator BulletKill()
	{
		while (bullet != null)
			yield return null;

		if (targetActor != null)
		{
			targetActor.Kill();
			deathSystem.transform.position = targetActor.transform.position;
			deathSystem.Play();
			hitSound.Play();
		}
	}

	public float EaseOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2) + start;
	}

	private void DoShoot()
	{
		BounceRay hit = BounceRay.Cast(playerController.transform.position, gmousePos - playerController.transform.position, 10.0f, LayerMask.GetMask("Bounceable"), LayerMask.GetMask("Actor"));

		hitPoints = hit.endPoints;

		Vector2 prevPoint = playerController.transform.position;

		int visorIndex = 0;
		foreach (Vector2 endPoint in hit.endPoints)
		{
			int count = Mathf.FloorToInt((endPoint - prevPoint).magnitude / 0.1f);

			for (int i = 0; i < count; i++)
			{
				visors[visorIndex].transform.position = Vector2.Lerp(prevPoint, endPoint, (float)i / count);
				visorIndex++;
			}

			for (int i = visorIndex; i < 100; i++)
			{
				visors[i].transform.position = new Vector3(1000, 1000, 0);
			}

			prevPoint = endPoint;
		}
	}
}

using UnityEngine;

public class BounceEntity : MonoBehaviour
{
	public event System.Action<BounceEntity> EntityClickedEvent;

	[SerializeField]
	private Rigidbody2D _rigidBody2D = null;

	[SerializeField]
	private SpriteRenderer _spriteRenderer = null;

	[SerializeField]
	private float _speed = 5f;

	public SpriteRenderer SpriteRenderer => _spriteRenderer;
	public Sprite Portrait => _spriteRenderer.sprite;

	public bool IsRunning
	{
		get; private set;
	}

	public void StartRunning()
	{
		if (!IsRunning)
		{
			IsRunning = true;
			RecalculateVelocity();
		}
	}

	public void StopRunning()
	{
		if (IsRunning)
		{
			IsRunning = false;
			_rigidBody2D.velocity = Vector2.zero;
		}
	}

	protected void Start()
	{
		StopRunning();
		StartRunning();
	}

	protected void FixedUpdate()
	{
		if (IsRunning)
		{
			Vector2 velocityNormalized = _rigidBody2D.velocity.normalized;
			if (Mathf.Abs(velocityNormalized.x) < 0.1f || Mathf.Abs(velocityNormalized.y) < 0.1f)
			{
				RecalculateVelocity();
			}
		}
	}

	protected void OnMouseDown()
	{
		EntityClickedEvent?.Invoke(this);
	}

	protected void RecalculateVelocity()
	{
		_rigidBody2D.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * _speed;
	}
}

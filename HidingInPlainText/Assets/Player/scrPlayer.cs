﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class scrPlayer : MonoBehaviour
{
	public Material OpenGL;

	public Vector2 AimPosition { get; private set; }
	public float AimRadius { get; private set; } // Fraction of the screen height that is the turning radius.
	public float TurnSpeed { get; private set; }

	public float Acceleration { get; private set; }
	public float SpeedLimit { get; private set; }


	void Start ()
	{
		AimPosition = Vector2.zero;
		AimRadius = 0.5f;
		TurnSpeed = 100.0f;

		Acceleration = 100.0f;
		SpeedLimit = 10.0f;

		Camera.main.GetComponent<scrCamera>().PostRender += PostRender;
	}

	void FixedUpdate ()
	{
		Aim ();
		Move ();

	}

	void Aim()
	{
		AimPosition += new Vector2(Input.GetAxis("Mouse X") / Screen.width, Input.GetAxis("Mouse Y") / Screen.height) * Settings.MouseSensitivity;

		if (AimPosition.magnitude > AimRadius)
		{
			AimPosition = AimPosition.normalized * AimRadius;
		}

		Vector3 rotationToAdd = Vector3.Slerp (Vector3.zero, new Vector3(-AimPosition.y, AimPosition.x).normalized, AimPosition.magnitude / AimRadius) * TurnSpeed * Time.deltaTime;
		transform.Rotate (rotationToAdd);
	}

	void Move()
	{
		Vector3 velocityToAdd = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
		velocityToAdd.Normalize();
		velocityToAdd *= Acceleration * Time.deltaTime;
		
		rigidbody.AddForce(velocityToAdd, ForceMode.VelocityChange);
		if (rigidbody.velocity.magnitude > SpeedLimit)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * SpeedLimit;
		}
	}

	void PostRender()
	{
		OpenGL.SetPass(0);
		GL.PushMatrix ();
		GL.LoadOrtho();
		GL.MultMatrix(scrCamera.ScreenMatrix);

		GL.Begin(GL.LINES);
		GL.Color (Color.white);

		// Draw the aim circle.
		Vector3 vertex = new Vector3(0.0f, AimRadius);
		for (int i = 1, vertexCount = 64; i <= vertexCount; ++i)
		{
			GL.Vertex(vertex);

			vertex = new Vector3(AimRadius * Mathf.Sin ((float)i / vertexCount * 2 * Mathf.PI), AimRadius * Mathf.Cos ((float)i / vertexCount * 2 * Mathf.PI));	
	
			GL.Vertex(vertex);
		}

		// Draw lines towards the aim position.
		for (int i = 0, vertexCount = 4; i < vertexCount; ++i)
		{
			GL.Vertex(new Vector3(AimRadius * Mathf.Sin ((float)i / vertexCount * 2 * Mathf.PI + Time.time * 0.1f), AimRadius * Mathf.Cos ((float)i / vertexCount * 2 * Mathf.PI + Time.time * 0.1f)));
			GL.Vertex(AimPosition);
		}


		GL.End ();
		GL.PopMatrix();
	}

}

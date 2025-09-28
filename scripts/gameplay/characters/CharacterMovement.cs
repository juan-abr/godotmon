using Godot;
using System;
using Game.Core;

namespace Game.Gameplay
{
	public partial class CharacterMovement : Node
	{
		[Signal] public delegate void AnimationEventHandler(string animationType);

		[ExportCategory("Nodes")]
		[Export] public Node2D Character;
		[Export] public CharacterInput CharacterInput;
		// Removed to address collision bug caused by fast inputs.
		//[Export] public CharacterCollisionRayCast CharacterCollisionRayCast;

		[ExportCategory("Movement")]
		[Export] public Vector2 TargetPosition = Vector2.Down;
		[Export] public bool IsWalking = false;
		[Export] public bool CollisionDetected = false;

		public override void _Ready()
		{
			CharacterInput.Walk += StartWalking;
			CharacterInput.Turn += Turn;

			// Removed to address collision bug caused by fast inputs.
			//CharacterCollisionRayCast.Collision += (value) => CollisionDetected = value;

			Core.Logger.Info("Loading character movement component ..");
		}

		public override void _Process(double delta)
		{
			Walk(delta);
		}

		public bool IsMoving()
		{
			return IsWalking;
		}

		public bool IsColliding()
		{
			return CollisionDetected;
		}

		private bool IsTargetOccupied(Vector2 targetPosition)
		{
			var spaceState = GetViewport().GetWorld2D().DirectSpaceState;

			Vector2 adjustedTargetPosition = targetPosition;
			adjustedTargetPosition.X += 8;
			adjustedTargetPosition.Y += 8;

			var query = new PhysicsPointQueryParameters2D
			{
				Position = adjustedTargetPosition,
				CollisionMask = 1,
				CollideWithAreas = true,
			};

			var result = spaceState.IntersectPoint(query);

			if (result.Count > 0)
			{
				foreach (var collision in result)
				{
					var collider = (Node)(GodotObject)collision["collider"];
					var colliderType = collider.GetType().Name;

					Core.Logger.Info(collider);

					switch (colliderType)
					{
						case "TileMapLayer":
							return true;
						default:
							return true;
					}
				}
			}

			return false;
		}

		public void StartWalking()
		{
			TargetPosition = Character.Position + CharacterInput.Direction * Globals.Instance.GRID_SIZE;

			if (!IsMoving() && !IsTargetOccupied(TargetPosition))
			{
				EmitSignal(SignalName.Animation, "walk");
				Core.Logger.Info($"Moving from {Character.Position} to {TargetPosition}");
				IsWalking = true;
			}
		}

		public void Walk(double delta)
		{
			if (IsWalking)
			{
				Character.Position = Character.Position.MoveToward(TargetPosition, (float)delta * Globals.Instance.GRID_SIZE * 4);

				if (Character.Position.DistanceTo(TargetPosition) < 1f)
				{
					StopWalking();
				}
			}
			else
			{
				EmitSignal(SignalName.Animation, "idle");
			}
		}

		public void StopWalking()
		{
			IsWalking = false;
			SnapPositionToGrid();
		}

		public void Turn()
		{
			EmitSignal(SignalName.Animation, "turn");
		}

		public void SnapPositionToGrid()
		{
			Character.Position = new Vector2(
				Mathf.Round(Character.Position.X / Globals.Instance.GRID_SIZE) * Globals.Instance.GRID_SIZE,
				Mathf.Round(Character.Position.Y / Globals.Instance.GRID_SIZE) * Globals.Instance.GRID_SIZE
			);
		}
	}
}

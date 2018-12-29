using UnityEngine;
using System;

namespace CGameDevToolkit.Framework
{
	public struct StateChangeEvent<T> where T: struct, IComparable, IConvertible, IFormattable
	{
		public GameObject Target;
		public StateMachine<T> TargetStateMachine;
		public T NewState;
		public T PreviousState;

		public StateChangeEvent(StateMachine<T> stateMachine)
		{
			Target = stateMachine.Target;
			TargetStateMachine = stateMachine;
			NewState = stateMachine.CurrentState;
			PreviousState = stateMachine.PreviousState;
		}
	}

	/// <summary>
	/// 简易状态机，使用 enum 类做作为泛型参数
	/// </summary>
	public class StateMachine<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		public bool IsTriggerEvent;
		public GameObject Target;
		public T CurrentState { get; protected set; }
		public T PreviousState { get; protected set; }

		public StateMachine(GameObject target, bool isTriggerEvent)
		{
			Target = target;
			IsTriggerEvent = isTriggerEvent;
		} 

		public virtual void ChangeState(T newState)
		{
			if (newState.Equals(CurrentState))
			{
				return;
			}

			PreviousState = CurrentState;
			CurrentState = newState;

			if (IsTriggerEvent)
			{
				EventManager.TriggerEvent (new StateChangeEvent<T> (this));
			}
		}

		public virtual void RestorePreviousState()
		{
			CurrentState = PreviousState;

			if (IsTriggerEvent)
			{
				EventManager.TriggerEvent (new StateChangeEvent<T> (this));
			}
		}	
	}
}
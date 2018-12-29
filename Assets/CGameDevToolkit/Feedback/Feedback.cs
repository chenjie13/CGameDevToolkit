using UnityEngine;
using System;

namespace CGameDevToolkit.Framework
{
    /// <summary>
    /// 用于触发各种反馈的类，如：音效，震屏，粒子特效等反馈
    /// </summary>
	[Serializable]
	public class Feedback
    {

        public virtual void Play(Vector3 position)
        {
        }
        
        public virtual void Stop()
		{
        }
	}
}

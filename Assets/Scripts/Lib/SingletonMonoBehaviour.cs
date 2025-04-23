using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				Debug.LogWarning(typeof(T) + " のインスタンスが存在しません。" +
					" シーンにアタッチされているGameObjectを調べてください。");
				return null;
			}

			return instance;
		}
	}

	virtual protected void Awake ()
	{
		if (instance == null)
		{
			instance = this as T;
		}
		// 他のGameObjectにアタッチされているか調べる.
		// アタッチされている場合は破棄する.
		else if (this != Instance)
		{
			Debug.LogWarning(
				typeof(T) +
				" は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました。" +
				" アタッチされているGameObjectは " + Instance.gameObject.name + " です。");
			Destroy(this.gameObject);
			return;
		}
	}

}

public abstract class DontDestroySingletonMonoBehaviour<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
	private static T instance;


	override protected void Awake ()
	{
		base.Awake();
		DontDestroyOnLoad(this.gameObject);
	}

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Pool Manager has a list of different pools for instantiating lots of different objects
/// without generating pikes
/// </summary>
public class PoolManager : MonoBehaviour
{
	private static PoolManager _instance;

	private static Dictionary<PoolIds, PoolData> poolList = new Dictionary<PoolIds, PoolData> ();
	
	public static PoolManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("PoolManager").AddComponent<PoolManager>();
			}
			return _instance;
		}
	}

	/// <summary>
	/// Clears all the pools in the manager
	/// </summary>
	public void clearPoolList()
	{
		foreach(KeyValuePair<PoolIds, PoolData> pair in poolList)
		{
			removePool(pair.Key);
		}
		poolList.Clear();
	}

	/// <summary>
	/// Creates a new pool
	/// </summary>
	/// <param name="poolId">The Pool id.</param>
	/// <param name="prefab">The prefab to instantiate all the instances.</param>
	/// <param name="numInstances">Number of instances.</param>
	public void createPool(PoolIds poolId, GameObject prefab, int numInstances)
	{
		PoolData poolData = new PoolData ();

		poolData.numInstances = numInstances;
		poolData.instanceList = new PoolInstance[numInstances];

		for (int i = 0; i < numInstances; i++)
		{
			poolData.instanceList[i] = instantiatePoolInstance(prefab);
		}

		poolList.Add (poolId, poolData);
	}

	/// <summary>
	/// Removes all the instances in a pool
	/// </summary>
	/// <param name="poolId">Pool id.</param>
	private void removePool(PoolIds poolId)
	{
		PoolData poolData = poolList[poolId];
		for (int i = 0; i < poolData.numInstances; i++)
		{
			GameObject.Destroy(poolData.instanceList[i].gameObject);
		}
	}

	/// <summary>
	/// Retrieve an instance that it is not being used from an specific pool
	/// </summary>
	/// <returns>The pool instance.</returns>
	/// <param name="poolId">Pool id.</param>
	public PoolInstance retrievePoolInstance(PoolIds poolId)
	{
		PoolData poolData = poolList[poolId];

		//Start from the last instance index and iterate through all the instances
		for (int i = poolData.instanceIndex; i < poolData.instanceIndex + poolData.numInstances; i++)
		{
			int index = (i < poolData.numInstances) ? i : i % poolData.numInstances;
			if (!poolData.instanceList[index].isAlive)
			{
				//Revive the instance and store the index as the last instance used
				poolData.instanceList[index].revive();
				poolData.instanceIndex = index;

				return poolData.instanceList[index];
			}
		}

		//If there are no more instances available, resize the pool and log a warning (so we know we need to make the pool bigger)
		resizePoolInstanceList (poolId);
		Debug.LogWarning("There are no instances availables in " + poolId + " (Resizing pool)");

		return retrievePoolInstance (poolId);
	}

	public void destroyInstance(PoolInstance instance)
	{
		instance.kill();
	}

	/// <summary>
	/// Resizes the instance list for a given pool (in case we need more instances that we expected)
	/// </summary>
	/// <param name="poolId">The id of the pool we want to resize.</param>
	private void resizePoolInstanceList(PoolIds poolId)
	{
		//Get the pool data and the prefab to instantiate
		PoolData poolData = poolList[poolId];
		GameObject prefab = poolData.instanceList[0].gameObject;

		//Calculate the new size and create the new array
		int newSize = Mathf.FloorToInt (poolData.numInstances * 1.20f);
		PoolInstance[] newInstanceList = new PoolInstance[newSize];

		//Move the references
		for (int i = 0; i < poolData.numInstances; i++)
		{
			newInstanceList[i] = poolData.instanceList[i];
		}
		//Create the new instances
		for (int i = poolData.numInstances; i < newSize; i++)
		{
			newInstanceList[i] = instantiatePoolInstance(prefab);
		}

		poolData.numInstances = newSize;
		poolData.instanceList = newInstanceList;
	}

	/// <summary>
	/// Instantiates a new pool instance
	/// </summary>
	/// <returns>The pool instance.</returns>
	/// <param name="prefab">The prefab for the instance.</param>
	private PoolInstance instantiatePoolInstance(GameObject prefab)
	{
		//The pool only operates with PoolInstance objects
		PoolInstance instance = (GameObject.Instantiate(prefab) as GameObject).GetComponent<PoolInstance>();
		
		instance.init();
		instance.transform.parent = transform;
		
		return instance;
	}

	private class PoolData
	{
		public int numInstances;
		public int instanceIndex = 0;

		public PoolInstance[] instanceList;
	}

	public enum PoolIds
	{
		Hex,
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckpointsManager : MonoBehaviour {
    public List<GameObject> Checkpoints;
    //private:
    private static CheckpointsManager Manager = null;
    private class CheckpointComparer
    {
        GameObject GOToFind;

        public CheckpointComparer(GameObject go)
        {
            GOToFind = go;
        }

        public bool Predicate(GameObject go1)
        {
            int id1 = GOToFind.GetInstanceID();
            int id2 = go1.GetInstanceID();
            return id1 == id2;
        }
    }

    private GameObject FindNextCheckpoint(GameObject currCP)
    {
        if (currCP == null)
            return Checkpoints[0];

        int index = Checkpoints.IndexOf(currCP);

        return Checkpoints[index + 1];
    }

    private static CheckpointsManager GetManager()
    {
        if (Manager == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("CheckpointManager");
            Manager = go.GetComponent<CheckpointsManager>();
            Assert.IsNotNull(Manager);
        }

        return Manager;
    }


    public static GameObject GetNextCheckpoint(GameObject currCP)
    {
        return GetManager().FindNextCheckpoint(currCP);
    }

}

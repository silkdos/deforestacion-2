using UnityEngine;
using System;
using System.Collections.Generic;

namespace Deforestation
{

    

    public class TreeTerrainController : MonoBehaviour
	{
		#region Properties
		public TreeInstance[] Trees => _trees;
		#endregion

		#region Fields
		[SerializeField] private Tree _treeDetectionPrefab;
		[SerializeField] private Tree _treePrefab;
		private TreeInstance[] _trees;
		Terrain _terrain;
        private List<Tree> _treeDetectors = new List<Tree>();
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            _terrain = Terrain.activeTerrain;

            TerrainData runtimeData = Instantiate(_terrain.terrainData);
            _terrain.terrainData = runtimeData;

            _trees = _terrain.terrainData.treeInstances;

            InitializeTrees();
        }

        private void InitializeTrees()
        {
            for (int i = _trees.Length - 1; i >= 0; i--)
            {
                TreeInstance tree = _trees[i];
                Vector3 treeWorldPos = TreeToWorldPosition(tree);
                Tree treeDetector = Instantiate(_treeDetectionPrefab, treeWorldPos, Quaternion.identity);
                treeDetector.transform.parent = transform;
                treeDetector.Index = i;

                _treeDetectors.Add(treeDetector);
            }
        }

        public GameObject DestroyTree(int i, Vector3 treeWorldPos)
		{
			//create tree
			Tree newTree = Instantiate(_treePrefab, treeWorldPos, Quaternion.identity);

			RemoveTreeFromTerrain(i);
			return newTree.gameObject;
		}
        #endregion

        #region Public Methods
        public Vector3 TreeToWorldPosition(TreeInstance tree)
		{
			return Vector3.Scale(tree.position, _terrain.terrainData.size) + _terrain.transform.position;
		}
        public void RemoveTreeFromTerrain(int index)
        {
            List<TreeInstance> trees = new List<TreeInstance>(_terrain.terrainData.treeInstances);
            trees.RemoveAt(index);
            _terrain.terrainData.treeInstances = trees.ToArray();

            _trees = _terrain.terrainData.treeInstances;

            // Destruir detector correspondiente
            Tree detectorToRemove = _treeDetectors.Find(t => t.Index == index);
            if (detectorToRemove != null)
            {
                _treeDetectors.Remove(detectorToRemove);
                Destroy(detectorToRemove.gameObject);
            }

            // Reasignar índices
            for (int i = 0; i < _treeDetectors.Count; i++)
            {
                _treeDetectors[i].Index = i;
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}

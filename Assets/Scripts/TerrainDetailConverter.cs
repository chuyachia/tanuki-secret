using UnityEngine;

public class TerrainDetailConverter:MonoBehaviour
{
	Terrain _terrain;
	

    private void Start() {
        _terrain = GetComponent<Terrain>();
        Convert();
    }

	void Convert()
	{
		var terrainData = _terrain.terrainData;
		var scaleX = terrainData.size.x / terrainData.detailWidth;
		var scaleZ = terrainData.size.z / terrainData.detailHeight;
		var targetDensity = Mathf.RoundToInt((1f - _terrain.detailObjectDensity) * 4);

		for (int d = 0; d < terrainData.detailPrototypes.Length; d++)
		{
			var detailPrototype = terrainData.detailPrototypes[d];
			var detailLayer = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, d);

			var accumulatingDensity = 0f;

			for (int x = 0; x < terrainData.detailWidth; x++)
			{
				for (int y = 0; y < terrainData.detailHeight; y++)
				{
					var layerDensity = detailLayer[y, x];
					if (layerDensity > 0)
						accumulatingDensity += layerDensity;
						
					if (accumulatingDensity > targetDensity)
					{
						var pos = new Vector3(x * scaleX + _terrain.transform.position.x, 0, y * scaleZ + _terrain.transform.position.z);
						pos.y = _terrain.SampleHeight(pos);
						var detail = Instantiate(terrainData.detailPrototypes[d].prototype, pos, Quaternion.Euler(0, Random.Range(0, 359), 0), transform);

						var scale = Random.Range(detailPrototype.minWidth, detailPrototype.maxWidth);
						detail.transform.localScale = Vector3.one * scale;

						accumulatingDensity = 0;
					}
				}
			}
		}
	}
}
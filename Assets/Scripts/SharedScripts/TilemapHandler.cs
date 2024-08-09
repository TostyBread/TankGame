using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHandler : MonoBehaviour
{
    public Tilemap destructableTilemap;
    public GameObject tileRemovedEffectPrefab; // Particle system prefab for tile removal

    public void RemoveTile(Vector3 worldPosition)
    {
        Vector3Int cellPosition = destructableTilemap.WorldToCell(worldPosition);

        // Play particle effect at the tile's position
        PlayTileRemovedEffect(destructableTilemap.GetCellCenterWorld(cellPosition));

        // Remove the tile from the tilemap
        destructableTilemap.SetTile(cellPosition, null);
    }

    private void PlayTileRemovedEffect(Vector3 position)
    {
        if (tileRemovedEffectPrefab != null)
        {
            // Instantiate the particle effect at the tile's position
            GameObject effect = Instantiate(tileRemovedEffectPrefab, position, Quaternion.identity);

            // Get the ParticleSystem component and set it to destroy after it completes
            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                float duration = particleSystem.main.duration;
                Destroy(effect, duration);
            }
            else
            {
                // If no ParticleSystem is found, destroy immediately
                Destroy(effect);
            }
        }
    }
}

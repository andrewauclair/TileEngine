using UnityEngine;

public class CPreviewHighlight : MonoBehaviour
{
	void OnDrawGizmos()
	{
		int t_nChunkSize = CChunkGenerator.msc_nChunkSize;

		Gizmos.color = Color.red;

		// Change to selection size?
		float t_rHalfChunk = .5f;// t_nChunkSize / 2.0f;
		float t_x = transform.position.x;
		float t_y = transform.position.y;

		Vector3 t_v3TopLeft = new Vector3(t_x - t_rHalfChunk, t_y + t_rHalfChunk, 0f);
		Vector3 t_v3TopRight = new Vector3(t_x + t_rHalfChunk, t_y + t_rHalfChunk, 0f);
		Vector3 t_v3BottomLeft = new Vector3(t_x - t_rHalfChunk, t_y - t_rHalfChunk, 0f);
		Vector3 t_v3BottomRight = new Vector3(t_x + t_rHalfChunk, t_y - t_rHalfChunk, 0f);

		Gizmos.DrawLine(t_v3TopLeft, t_v3TopRight);
		Gizmos.DrawLine(t_v3TopRight, t_v3BottomRight);
		Gizmos.DrawLine(t_v3BottomRight, t_v3BottomLeft);
		Gizmos.DrawLine(t_v3BottomLeft, t_v3TopLeft);
	}
}

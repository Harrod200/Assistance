using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000005 RID: 5
public class BlurUtility : MonoBehaviour
{
	// Token: 0x0600000A RID: 10 RVA: 0x000021B8 File Offset: 0x000003B8
	public static void BlurRenderTexture(ref CommandBuffer commandBuffer, RenderTexture target, int targetIterations)
	{
		if (!BlurUtility.s_blurMaterial)
		{
			BlurUtility.s_downsampleMaterial = new Material(Shader.Find("Hidden/TerraInvicta/DownsampleShader"));
			BlurUtility.s_blurMaterial = new Material(Shader.Find("Hidden/TerraInvicta/BlurShader"));
			BlurUtility.s_blurMaterial.SetInt("_KernelSize", 3);
			BlurUtility.s_blurMaterial.SetFloatArray("_KernelWeights", BlurUtility.s_kernelWeights3);
		}
		int num = (int)Mathf.Min(Mathf.Log((float)target.width, 2f) - 1f, (float)targetIterations);
		int width = target.width;
		int height = target.height;
		RenderTargetIdentifier[] array = new RenderTargetIdentifier[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)Mathf.Pow(2f, (float)(i + 1));
			int num3 = Shader.PropertyToID("BlurLevel" + i.ToString());
			array[i] = num3;
			commandBuffer.GetTemporaryRT(num3, Mathf.Max(2, width / num2), Mathf.Max(2, height / num2), 0, FilterMode.Bilinear, target.format);
		}
		RenderTargetIdentifier renderTargetIdentifier = target;
		for (int j = 0; j < num; j++)
		{
			commandBuffer.Blit(renderTargetIdentifier, array[j], BlurUtility.s_downsampleMaterial);
			renderTargetIdentifier = array[j];
		}
		for (int k = num - 2; k >= 0; k--)
		{
			commandBuffer.Blit(renderTargetIdentifier, array[k], BlurUtility.s_blurMaterial);
			renderTargetIdentifier = array[k];
		}
		commandBuffer.Blit(renderTargetIdentifier, target, BlurUtility.s_blurMaterial);
	}

	// Token: 0x04000004 RID: 4
	private static Material s_downsampleMaterial = null;

	// Token: 0x04000005 RID: 5
	private static Material s_blurMaterial = null;

	// Token: 0x04000006 RID: 6
	private static float[] s_kernelWeights3 = new float[] { 0.27901f, 0.44198f, 0.27901f };

	// Token: 0x04000007 RID: 7
	private static float[] s_kernelWeights5 = new float[] { 0.06136f, 0.24477f, 0.38774f, 0.24477f, 0.06136f };

	// Token: 0x04000008 RID: 8
	private static float[] s_kernelWeights7 = new float[] { 0.00598f, 0.060626f, 0.241843f, 0.383103f, 0.241843f, 0.060626f, 0.00598f };

	// Token: 0x04000009 RID: 9
	private static float[] s_kernelWeights9 = new float[] { 0.000229f, 0.005977f, 0.060598f, 0.241732f, 0.382928f, 0.241732f, 0.060598f, 0.005977f, 0.000229f };

	// Token: 0x0400000A RID: 10
	private static float[] s_kernelWeights11 = new float[]
	{
		3E-06f, 0.000229f, 0.005977f, 0.060598f, 0.24173f, 0.382925f, 0.24173f, 0.060598f, 0.005977f, 0.000229f,
		3E-06f
	};
}

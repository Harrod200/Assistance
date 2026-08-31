using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004B3 RID: 1203
	public static class TooltipExtensions
	{
		// Token: 0x06001B0A RID: 6922 RVA: 0x000924D8 File Offset: 0x000906D8
		public static void SetPosition(this Tooltip tooltip, TooltipTrigger trigger, Canvas canvas, Camera camera)
		{
			Vector3[] array = new Vector3[4];
			RectTransform rectTransform = trigger.gameObject.GetComponent<RectTransform>();
			if (trigger.tipPosition == TipPosition.CanvasTopMiddle || trigger.tipPosition == TipPosition.CanvasBottomMiddle)
			{
				rectTransform = canvas.gameObject.GetComponent<RectTransform>();
			}
			if (rectTransform != null)
			{
				rectTransform.GetWorldCorners(array);
			}
			else
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				if (TooltipManager.Instance.positionBounds == PositionBounds.Collider)
				{
					Collider component = trigger.GetComponent<Collider>();
					vector = component.bounds.center;
					vector2 = component.bounds.extents;
				}
				else
				{
					Renderer component2 = trigger.GetComponent<Renderer>();
					vector = component2.bounds.center;
					vector2 = component2.bounds.extents;
				}
				Vector3 vector3 = new Vector3(vector.x - vector2.x, vector.y - vector2.y, vector.z - vector2.z);
				Vector3 vector4 = new Vector3(vector.x - vector2.x, vector.y + vector2.y, vector.z - vector2.z);
				Vector3 vector5 = new Vector3(vector.x + vector2.x, vector.y + vector2.y, vector.z - vector2.z);
				Vector3 vector6 = new Vector3(vector.x + vector2.x, vector.y - vector2.y, vector.z - vector2.z);
				if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
				{
					array[0] = vector3;
					array[1] = vector4;
					array[2] = vector5;
					array[3] = vector6;
				}
				else
				{
					array[0] = camera.WorldToScreenPoint(vector3);
					array[1] = camera.WorldToScreenPoint(vector4);
					array[2] = camera.WorldToScreenPoint(vector5);
					array[3] = camera.WorldToScreenPoint(vector6);
				}
			}
			tooltip.SetPosition(trigger.tipPosition, trigger.tooltipStyle, array, tooltip.BackgroundImage, tooltip.RectTransform, canvas, camera, false, 0f);
			if (!TooltipManager.Instance.overflowProtection)
			{
				return;
			}
			Vector3[] array2 = new Vector3[4];
			tooltip.RectTransform.GetWorldCorners(array2);
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = RectTransformUtility.WorldToScreenPoint(camera, array2[i]);
				}
			}
			else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = RectTransformUtility.WorldToScreenPoint(null, array2[j]);
				}
			}
			Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			TooltipOverflow tooltipOverflow = new TooltipOverflow
			{
				BottomLeftCorner = !rect.Contains(array2[0]),
				TopLeftCorner = !rect.Contains(array2[1]),
				TopRightCorner = !rect.Contains(array2[2]),
				BottomRightCorner = !rect.Contains(array2[3])
			};
			float num = 0f;
			bool flag = false;
			if (tooltipOverflow.IsAny)
			{
				if (tooltipOverflow.BottomLeftCorner && tooltipOverflow.BottomRightCorner)
				{
					num = array2[0].y * -1f;
					flag = false;
				}
				else if (tooltipOverflow.TopLeftCorner && tooltipOverflow.TopRightCorner)
				{
					flag = true;
					num = array2[1].y - rect.height;
				}
			}
			if (tooltipOverflow.IsAny)
			{
				tooltip.SetPosition(tooltipOverflow.SuggestNewPosition(trigger.tipPosition), trigger.tooltipStyle, array, tooltip.BackgroundImage, tooltip.RectTransform, canvas, camera, flag, num);
			}
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x000928B0 File Offset: 0x00090AB0
		private static void SetPosition(this Tooltip tooltip, TipPosition tipPosition, TooltipStyle style, Vector3[] triggerCorners, Image bkgImage, RectTransform tooltipRectTrans, Canvas canvas, Camera camera, bool overflowTop = false, float overflowAmount = 0f)
		{
			Vector3 vector = Vector3.zero;
			Vector2 zero = Vector2.zero;
			bool flag = tipPosition == TipPosition.MouseBottomLeftCorner || tipPosition == TipPosition.MouseTopLeftCorner || tipPosition == TipPosition.MouseBottomRightCorner || tipPosition == TipPosition.MouseTopRightCorner || tipPosition == TipPosition.MouseTopMiddle || tipPosition == TipPosition.MouseLeftMiddle || tipPosition == TipPosition.MouseRightMiddle || tipPosition == TipPosition.MouseBottomMiddle;
			Vector3 vector2 = Input.mousePosition;
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				vector2.z = canvas.planeDistance;
				vector2 = camera.ScreenToWorldPoint(vector2);
			}
			switch (tipPosition)
			{
			case TipPosition.TopRightCorner:
			case TipPosition.MouseTopRightCorner:
				zero = new Vector2((float)(-1 * style.tipOffset), (float)(-1 * style.tipOffset));
				vector = (flag ? vector2 : triggerCorners[2]);
				tooltipRectTrans.pivot = new Vector2(0f, 0f);
				bkgImage.sprite = style.bottomLeftCorner;
				break;
			case TipPosition.BottomRightCorner:
			case TipPosition.MouseBottomRightCorner:
				zero = new Vector2((float)(-1 * style.tipOffset), (float)style.tipOffset);
				vector = (flag ? vector2 : triggerCorners[3]);
				tooltipRectTrans.pivot = new Vector2(0f, 1f);
				bkgImage.sprite = style.topLeftCorner;
				break;
			case TipPosition.TopLeftCorner:
			case TipPosition.MouseTopLeftCorner:
				zero = new Vector2((float)style.tipOffset, (float)(-1 * style.tipOffset));
				vector = (flag ? vector2 : triggerCorners[1]);
				tooltipRectTrans.pivot = new Vector2(1f, 0f);
				bkgImage.sprite = style.bottomRightCorner;
				break;
			case TipPosition.BottomLeftCorner:
			case TipPosition.MouseBottomLeftCorner:
				zero = new Vector2((float)style.tipOffset, (float)style.tipOffset);
				vector = (flag ? vector2 : triggerCorners[0]);
				tooltipRectTrans.pivot = new Vector2(1f, 1f);
				bkgImage.sprite = style.topRightCorner;
				break;
			case TipPosition.TopMiddle:
			case TipPosition.MouseTopMiddle:
				zero = new Vector2(0f, (float)(-1 * style.tipOffset));
				vector = (flag ? vector2 : (triggerCorners[1] + (triggerCorners[2] - triggerCorners[1]) / 2f));
				tooltipRectTrans.pivot = new Vector2(0.5f, 0f);
				bkgImage.sprite = style.topMiddle;
				break;
			case TipPosition.BottomMiddle:
			case TipPosition.MouseBottomMiddle:
				zero = new Vector2(0f, (float)style.tipOffset);
				vector = (flag ? vector2 : (triggerCorners[0] + (triggerCorners[3] - triggerCorners[0]) / 2f));
				tooltipRectTrans.pivot = new Vector2(0.5f, 1f);
				bkgImage.sprite = style.bottomMiddle;
				break;
			case TipPosition.RightMiddle:
			case TipPosition.MouseRightMiddle:
				zero = new Vector2((float)(-1 * style.tipOffset), 0f);
				vector = (flag ? vector2 : (triggerCorners[3] + (triggerCorners[2] - triggerCorners[3]) / 2f));
				if (overflowAmount > 0f)
				{
					if (overflowTop)
					{
						vector.y -= overflowAmount;
					}
					else
					{
						vector.y += overflowAmount;
					}
				}
				tooltipRectTrans.pivot = new Vector2(0f, 0.5f);
				bkgImage.sprite = style.rightMiddle;
				break;
			case TipPosition.LeftMiddle:
			case TipPosition.MouseLeftMiddle:
				zero = new Vector2((float)style.tipOffset, 0f);
				vector = (flag ? vector2 : (triggerCorners[0] + (triggerCorners[1] - triggerCorners[0]) / 2f));
				if (overflowAmount > 0f)
				{
					if (overflowTop)
					{
						vector.y -= overflowAmount;
					}
					else
					{
						vector.y += overflowAmount;
					}
				}
				tooltipRectTrans.pivot = new Vector2(1f, 0.5f);
				bkgImage.sprite = style.leftMiddle;
				break;
			case TipPosition.CanvasTopMiddle:
			{
				zero = new Vector2(0f, (float)(-1 * style.tipOffset));
				vector = triggerCorners[1] + (triggerCorners[2] - triggerCorners[1]) / 2f;
				tooltipRectTrans.pivot = new Vector2(0.5f, 1f);
				Vector2 vector3 = new Vector2(0.5f, 1f);
				tooltipRectTrans.anchorMax = vector3;
				tooltipRectTrans.anchorMin = vector3;
				bkgImage.sprite = style.topMiddle;
				break;
			}
			case TipPosition.CanvasBottomMiddle:
			{
				zero = new Vector2(0f, (float)style.tipOffset);
				vector = triggerCorners[0] + (triggerCorners[3] - triggerCorners[0]) / 2f;
				tooltipRectTrans.pivot = new Vector2(0.5f, 0f);
				Vector2 vector3 = new Vector2(0.5f, 0f);
				tooltipRectTrans.anchorMax = vector3;
				tooltipRectTrans.anchorMin = vector3;
				bkgImage.sprite = style.bottomMiddle;
				break;
			}
			}
			tooltip.GameObject.transform.position = vector;
			tooltipRectTrans.anchoredPosition += zero;
		}
	}
}

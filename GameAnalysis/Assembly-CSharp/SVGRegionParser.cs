using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class SVGRegionParser
{
	// Token: 0x0600004A RID: 74 RVA: 0x000039B8 File Offset: 0x00001BB8
	public void ParseSVG(string filepath, RegionOutlineCollection regionCollection)
	{
		bool flag = true;
		TIRegionOutline tiregionOutline = null;
		string text = null;
		bool flag2 = true;
		bool flag3 = false;
		Vector2 zero = Vector2.zero;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		regionCollection.regionOutlines.Clear();
		XmlTextReader xmlTextReader = new XmlTextReader(filepath);
		xmlTextReader.Read();
		while (xmlTextReader.Read())
		{
			if (xmlTextReader.NodeType != XmlNodeType.EndElement)
			{
				string name = xmlTextReader.Name;
				if (name != null)
				{
					if (!(name == "svg"))
					{
						if (!(name == "g"))
						{
							if (!(name == "path"))
							{
								if (!(name == "text"))
								{
									if (name != null)
									{
										if (name.Length == 0)
										{
											if (flag3)
											{
												string value = xmlTextReader.Value;
												if (tiregionOutline != null && !flag)
												{
													if (tiregionOutline.labelPositions == null)
													{
														tiregionOutline.labelPositions = new List<LabelPosition>();
													}
													tiregionOutline.labelPositions.Add(new LabelPosition
													{
														labelName = value,
														labelPosition = new CurvedPolyPoint(zero).NormalizeToRadial(this.width, this.height)
													});
												}
												flag3 = false;
											}
										}
									}
								}
								else
								{
									flag3 = true;
									string attribute = xmlTextReader.GetAttribute("x");
									string attribute2 = xmlTextReader.GetAttribute("y");
									if (attribute != "" && attribute2 != "")
									{
										zero = new Vector2(float.Parse(xmlTextReader.GetAttribute("x")), float.Parse(xmlTextReader.GetAttribute("y")));
									}
								}
							}
							else if (tiregionOutline != null && !flag && tiregionOutline != null)
							{
								CurvedPolyPoint[] array = this.ParsePath(xmlTextReader.GetAttribute("d"));
								if (tiregionOutline.poly2DList == null)
								{
									tiregionOutline.poly2DList = new List<CurvedPolygon>();
								}
								CurvedPolygon curvedPolygon = default(CurvedPolygon);
								curvedPolygon.data = array;
								tiregionOutline.poly2DList.Add(curvedPolygon);
								num += curvedPolygon.data.Length;
								num2++;
								num4 += curvedPolygon.data.Length;
								num3++;
							}
						}
						else if (xmlTextReader.GetAttribute("inkscape:groupmode") == "layer")
						{
							if (xmlTextReader.GetAttribute("style") != null && xmlTextReader.GetAttribute("style").Contains("display:none"))
							{
								flag = true;
							}
							else
							{
								flag = false;
								string attribute3 = xmlTextReader.GetAttribute("inkscape:label");
								if (string.IsNullOrEmpty(attribute3))
								{
									Log.Error("Parsing bad label:" + xmlTextReader.LineNumber.ToString() + " " + xmlTextReader.LinePosition.ToString(), Array.Empty<object>());
								}
								else
								{
									string[] array2 = attribute3.Split(new char[] { ':' });
									string text2;
									if (array2.Length == 1)
									{
										text = "UNK";
										text2 = array2[0];
									}
									else if (array2.Length > 1)
									{
										if (text != null && array2[0] != text)
										{
											Debug.Log(string.Concat(new string[]
											{
												"Parsed ",
												num3.ToString(),
												" polys with ",
												num4.ToString(),
												" Verts for nation ",
												text
											}));
											num4 = (num3 = 0);
										}
										text = array2[0];
										text2 = array2[1].TrimStart(Array.Empty<char>());
									}
									else
									{
										text = "UNK";
										text2 = "Undefined";
									}
									tiregionOutline = new TIRegionOutline();
									tiregionOutline.name = text + " - " + text2;
									tiregionOutline.nationTag = text;
									tiregionOutline.regionName = text2;
									regionCollection.regionOutlines.Add(tiregionOutline);
								}
							}
						}
					}
					else if (flag2)
					{
						string attribute4 = xmlTextReader.GetAttribute("width");
						if (attribute4 != null)
						{
							string text3 = Regex.Replace(attribute4, "[^0-9]", "");
							regionCollection.width = float.Parse(text3);
							this.width = float.Parse(text3);
							regionCollection.width = 6.2831855f;
						}
						string attribute5 = xmlTextReader.GetAttribute("height");
						if (attribute5 != null)
						{
							string text4 = Regex.Replace(attribute5, "[^0-9]", "");
							regionCollection.height = float.Parse(text4);
							this.height = float.Parse(text4);
							regionCollection.height = 3.1415927f;
						}
						regionCollection.collectionName = xmlTextReader.GetAttribute("sodipodi:docname");
						flag2 = false;
					}
				}
			}
		}
		regionCollection.regionOutlines.Sort((TIRegionOutline x, TIRegionOutline y) => x.name.CompareTo(y.name));
		Debug.Log(string.Concat(new string[]
		{
			"Parsed ",
			num3.ToString(),
			" polys with ",
			num4.ToString(),
			" Verts for nation ",
			text
		}));
		Debug.Log(string.Concat(new string[]
		{
			"Parsed ",
			num2.ToString(),
			" polys with ",
			num.ToString(),
			" total Verts"
		}));
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003EB8 File Offset: 0x000020B8
	private CurvedPolyPoint[] ParsePath(string svgLine)
	{
		List<object> list = this.CleanSVGLine(svgLine);
		string text = "";
		bool flag = false;
		object obj = null;
		bool flag2 = true;
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		Vector2 vector3 = default(Vector2);
		List<CurvedPolyPoint> list2 = new List<CurvedPolyPoint>();
		foreach (object obj2 in list)
		{
			if (obj2 is string)
			{
				text = obj2 as string;
				if (text == "z" && vector != vector3)
				{
					list2.Add(new CurvedPolyPoint(vector).NormalizeToRadial(this.width, this.height));
				}
			}
			else
			{
				if (obj2 is float? || obj2 is Vector2 || obj2 is Vector6d || obj2 is Vector7d)
				{
					if (text != null)
					{
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
						if (num <= 3742114125U)
						{
							if (num <= 3373006507U)
							{
								if (num != 3322673650U)
								{
									if (num != 3356228888U)
									{
										if (num != 3373006507U)
										{
											goto IL_054F;
										}
										if (!(text == "L"))
										{
											goto IL_054F;
										}
									}
									else if (!(text == "M"))
									{
										goto IL_054F;
									}
									vector3 = (Vector2)obj2;
									list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
									goto IL_054F;
								}
								if (!(text == "C"))
								{
									goto IL_054F;
								}
								vector3.x = (float)((Vector6d)obj2).v[4];
								vector3.y = (float)((Vector6d)obj2).v[5];
								list2.Add(new CurvedPolyPoint((Vector6d)obj2).NormalizeToRadial(this.width, this.height));
								goto IL_054F;
							}
							else if (num != 3440116983U)
							{
								if (num != 3540782697U)
								{
									if (num != 3742114125U)
									{
										goto IL_054F;
									}
									if (!(text == "Z"))
									{
										goto IL_054F;
									}
								}
								else
								{
									if (!(text == "V"))
									{
										goto IL_054F;
									}
									vector2 = vector3;
									vector3.x = vector2.x;
									vector3.y = (float)obj2;
									list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
									goto IL_054F;
								}
							}
							else
							{
								if (!(text == "H"))
								{
									goto IL_054F;
								}
								vector2 = vector3;
								vector3.y = vector2.y;
								vector3.x = (float)obj2;
								list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
								goto IL_054F;
							}
						}
						else if (num <= 3909890315U)
						{
							if (num != 3859557458U)
							{
								if (num != 3893112696U)
								{
									if (num != 3909890315U)
									{
										goto IL_054F;
									}
									if (!(text == "l"))
									{
										goto IL_054F;
									}
								}
								else if (!(text == "m"))
								{
									goto IL_054F;
								}
								vector2 = vector3;
								vector3 = (Vector2)obj2;
								vector3 = (Vector2)obj + vector2;
								list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
								goto IL_054F;
							}
							if (!(text == "c"))
							{
								goto IL_054F;
							}
							vector3.x += (float)((Vector6d)obj2).v[4];
							vector3.y += (float)((Vector6d)obj2).v[5];
							((Vector6d)obj2).v[0] = (double)vector3.x;
							((Vector6d)obj2).v[1] = (double)vector3.y;
							list2.Add(new CurvedPolyPoint((Vector6d)obj2).NormalizeToRadial(this.width, this.height));
							goto IL_054F;
						}
						else if (num != 3977000791U)
						{
							if (num != 4077666505U)
							{
								if (num != 4278997933U)
								{
									goto IL_054F;
								}
								if (!(text == "z"))
								{
									goto IL_054F;
								}
							}
							else
							{
								if (!(text == "v"))
								{
									goto IL_054F;
								}
								vector2 = vector3;
								vector3.x = vector2.x;
								vector3.y = (float)obj2 + vector2.y;
								list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
								goto IL_054F;
							}
						}
						else
						{
							if (!(text == "h"))
							{
								goto IL_054F;
							}
							vector2 = vector3;
							vector3.y = vector2.y;
							vector3.x = (float)obj2 + vector2.x;
							list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
							goto IL_054F;
						}
						if (vector != vector3)
						{
							list2.Add(new CurvedPolyPoint(vector3).NormalizeToRadial(this.width, this.height));
						}
						flag = true;
					}
					IL_054F:
					if (flag2)
					{
						flag2 = false;
						vector = vector3;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		return list2.ToArray();
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00004464 File Offset: 0x00002664
	private List<object> CleanSVGLine(string svgLine)
	{
		List<object> list = new List<object>(100);
		string text = null;
		svgLine = svgLine.Replace("Z", "z");
		svgLine = svgLine.Replace("e-", "NEGEXP").Replace("E-", "NEGEXP");
		svgLine = svgLine.Replace(",", " ").Replace("-", " -");
		svgLine = svgLine.Replace("NEGEXP", "e-");
		foreach (string text2 in this.commands)
		{
			svgLine = svgLine.Replace(text2, " " + text2 + " ");
		}
		svgLine = svgLine.Replace("  ", " ").Trim();
		svgLine = svgLine.Replace("  ", " ").Trim();
		string[] array2 = svgLine.Split(null);
		for (int j = 0; j < array2.Length; j++)
		{
			string command = this.GetCommand(array2[j]);
			if (command != null)
			{
				text = command;
				list.Add(command);
				j++;
			}
			if (text != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 3742114125U)
				{
					if (num <= 3440116983U)
					{
						if (num <= 3322673650U)
						{
							if (num != 3289118412U)
							{
								if (num != 3322673650U)
								{
									goto IL_050F;
								}
								if (!(text == "C"))
								{
									goto IL_050F;
								}
								goto IL_0454;
							}
							else
							{
								if (!(text == "A"))
								{
									goto IL_050F;
								}
								goto IL_04AE;
							}
						}
						else if (num != 3356228888U)
						{
							if (num != 3373006507U)
							{
								if (num != 3440116983U)
								{
									goto IL_050F;
								}
								if (!(text == "H"))
								{
									goto IL_050F;
								}
							}
							else
							{
								if (!(text == "L"))
								{
									goto IL_050F;
								}
								goto IL_0427;
							}
						}
						else
						{
							if (!(text == "M"))
							{
								goto IL_050F;
							}
							goto IL_0427;
						}
					}
					else if (num <= 3540782697U)
					{
						if (num != 3507227459U)
						{
							if (num != 3540782697U)
							{
								goto IL_050F;
							}
							if (!(text == "V"))
							{
								goto IL_050F;
							}
						}
						else
						{
							if (!(text == "T"))
							{
								goto IL_050F;
							}
							goto IL_0427;
						}
					}
					else if (num != 3557560316U)
					{
						if (num != 3591115554U)
						{
							if (num != 3742114125U)
							{
								goto IL_050F;
							}
							if (!(text == "Z"))
							{
								goto IL_050F;
							}
							continue;
						}
						else
						{
							if (!(text == "S"))
							{
								goto IL_050F;
							}
							goto IL_0427;
						}
					}
					else
					{
						if (!(text == "Q"))
						{
							goto IL_050F;
						}
						goto IL_0427;
					}
				}
				else if (num <= 3977000791U)
				{
					if (num <= 3859557458U)
					{
						if (num != 3826002220U)
						{
							if (num != 3859557458U)
							{
								goto IL_050F;
							}
							if (!(text == "c"))
							{
								goto IL_050F;
							}
							goto IL_0454;
						}
						else
						{
							if (!(text == "a"))
							{
								goto IL_050F;
							}
							goto IL_04AE;
						}
					}
					else if (num != 3893112696U)
					{
						if (num != 3909890315U)
						{
							if (num != 3977000791U)
							{
								goto IL_050F;
							}
							if (!(text == "h"))
							{
								goto IL_050F;
							}
						}
						else
						{
							if (!(text == "l"))
							{
								goto IL_050F;
							}
							goto IL_0427;
						}
					}
					else
					{
						if (!(text == "m"))
						{
							goto IL_050F;
						}
						goto IL_0427;
					}
				}
				else if (num <= 4077666505U)
				{
					if (num != 4044111267U)
					{
						if (num != 4077666505U)
						{
							goto IL_050F;
						}
						if (!(text == "v"))
						{
							goto IL_050F;
						}
					}
					else
					{
						if (!(text == "t"))
						{
							goto IL_050F;
						}
						goto IL_0427;
					}
				}
				else if (num != 4094444124U)
				{
					if (num != 4127999362U)
					{
						if (num != 4278997933U)
						{
							goto IL_050F;
						}
						if (!(text == "z"))
						{
							goto IL_050F;
						}
						continue;
					}
					else
					{
						if (!(text == "s"))
						{
							goto IL_050F;
						}
						goto IL_0427;
					}
				}
				else
				{
					if (!(text == "q"))
					{
						goto IL_050F;
					}
					goto IL_0427;
				}
				list.Add(float.Parse(array2[j++]));
				continue;
				IL_0427:
				list.Add(new Vector2(float.Parse(array2[j++]), float.Parse(array2[j++])));
				continue;
				IL_0454:
				list.Add(new Vector6d(float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++])));
				continue;
				IL_04AE:
				list.Add(new Vector7d(float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++]), float.Parse(array2[j++])));
				continue;
			}
			IL_050F:;
		}
		return list;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00004990 File Offset: 0x00002B90
	private string GetCommand(string token)
	{
		foreach (string text in this.commands)
		{
			if (token == text)
			{
				return token;
			}
		}
		return null;
	}

	// Token: 0x04000042 RID: 66
	private string[] commands = new string[]
	{
		"M", "m", "Z", "z", "L", "l", "H", "h", "V", "v",
		"C", "c", "S", "s", "Q", "q", "T", "t", "A", "a"
	};

	// Token: 0x04000043 RID: 67
	private float width;

	// Token: 0x04000044 RID: 68
	private float height;
}

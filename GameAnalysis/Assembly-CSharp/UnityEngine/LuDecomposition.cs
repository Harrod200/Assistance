using System;

namespace UnityEngine
{
	// Token: 0x020004EF RID: 1263
	public class LuDecomposition
	{
		// Token: 0x06001E38 RID: 7736 RVA: 0x0009E580 File Offset: 0x0009C780
		public LuDecomposition(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.LU = value.Clone();
			double[][] array = this.LU.Array;
			int rows = value.Rows;
			int columns = value.Columns;
			this.pivotVector = new int[rows];
			for (int i = 0; i < rows; i++)
			{
				this.pivotVector[i] = i;
			}
			this.pivotSign = 1;
			double[] array2 = new double[rows];
			for (int j = 0; j < columns; j++)
			{
				for (int k = 0; k < rows; k++)
				{
					array2[k] = array[k][j];
				}
				for (int l = 0; l < rows; l++)
				{
					double[] array3 = array[l];
					int num = Math.Min(l, j);
					double num2 = 0.0;
					for (int m = 0; m < num; m++)
					{
						num2 += array3[m] * array2[m];
					}
					array3[j] = (array2[l] -= num2);
				}
				int num3 = j;
				for (int n = j + 1; n < rows; n++)
				{
					if (Math.Abs(array2[n]) > Math.Abs(array2[num3]))
					{
						num3 = n;
					}
				}
				if (num3 != j)
				{
					for (int num4 = 0; num4 < columns; num4++)
					{
						double num5 = array[num3][num4];
						array[num3][num4] = array[j][num4];
						array[j][num4] = num5;
					}
					int num6 = this.pivotVector[num3];
					this.pivotVector[num3] = this.pivotVector[j];
					this.pivotVector[j] = num6;
					this.pivotSign = -this.pivotSign;
				}
				if ((j < rows) & (array[j][j] != 0.0))
				{
					for (int num7 = j + 1; num7 < rows; num7++)
					{
						array[num7][j] /= array[j][j];
					}
				}
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001E39 RID: 7737 RVA: 0x0009E77C File Offset: 0x0009C97C
		public bool NonSingular
		{
			get
			{
				for (int i = 0; i < this.LU.Columns; i++)
				{
					if (this.LU[i, i] == 0.0)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001E3A RID: 7738 RVA: 0x0009E7BC File Offset: 0x0009C9BC
		public double Determinant
		{
			get
			{
				if (this.LU.Rows != this.LU.Columns)
				{
					throw new ArgumentException("Matrix must be square.");
				}
				double num = (double)this.pivotSign;
				for (int i = 0; i < this.LU.Columns; i++)
				{
					num *= this.LU[i, i];
				}
				return num;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001E3B RID: 7739 RVA: 0x0009E81C File Offset: 0x0009CA1C
		public Matrix LowerTriangularFactor
		{
			get
			{
				int rows = this.LU.Rows;
				int columns = this.LU.Columns;
				Matrix matrix = new Matrix(rows, columns);
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < columns; j++)
					{
						if (i > j)
						{
							matrix[i, j] = this.LU[i, j];
						}
						else if (i == j)
						{
							matrix[i, j] = 1.0;
						}
						else
						{
							matrix[i, j] = 0.0;
						}
					}
				}
				return matrix;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001E3C RID: 7740 RVA: 0x0009E8B0 File Offset: 0x0009CAB0
		public Matrix UpperTriangularFactor
		{
			get
			{
				int rows = this.LU.Rows;
				int columns = this.LU.Columns;
				Matrix matrix = new Matrix(rows, columns);
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < columns; j++)
					{
						if (i <= j)
						{
							matrix[i, j] = this.LU[i, j];
						}
						else
						{
							matrix[i, j] = 0.0;
						}
					}
				}
				return matrix;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001E3D RID: 7741 RVA: 0x0009E92C File Offset: 0x0009CB2C
		public double[] PivotPermutationVector
		{
			get
			{
				int rows = this.LU.Rows;
				double[] array = new double[rows];
				for (int i = 0; i < rows; i++)
				{
					array[i] = (double)this.pivotVector[i];
				}
				return array;
			}
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x0009E968 File Offset: 0x0009CB68
		public Matrix Solve(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Rows != this.LU.Rows)
			{
				throw new ArgumentException("Invalid matrix dimensions.", "value");
			}
			if (!this.NonSingular)
			{
				throw new InvalidOperationException("Matrix is singular");
			}
			int columns = value.Columns;
			Matrix matrix = value.Submatrix(this.pivotVector, 0, columns - 1);
			int columns2 = this.LU.Columns;
			double[][] array = this.LU.Array;
			for (int i = 0; i < columns2; i++)
			{
				for (int j = i + 1; j < columns2; j++)
				{
					for (int k = 0; k < columns; k++)
					{
						Matrix matrix2 = matrix;
						int num = j;
						int num2 = k;
						matrix2[num, num2] -= matrix[i, k] * array[j][i];
					}
				}
			}
			for (int l = columns2 - 1; l >= 0; l--)
			{
				for (int m = 0; m < columns; m++)
				{
					Matrix matrix2 = matrix;
					int num2 = l;
					int num = m;
					matrix2[num2, num] /= array[l][l];
				}
				for (int n = 0; n < l; n++)
				{
					for (int num3 = 0; num3 < columns; num3++)
					{
						Matrix matrix2 = matrix;
						int num = n;
						int num2 = num3;
						matrix2[num, num2] -= matrix[l, num3] * array[n][l];
					}
				}
			}
			return matrix;
		}

		// Token: 0x040017F2 RID: 6130
		private Matrix LU;

		// Token: 0x040017F3 RID: 6131
		private int pivotSign;

		// Token: 0x040017F4 RID: 6132
		private int[] pivotVector;
	}
}

using System;

namespace UnityEngine
{
	// Token: 0x020004F2 RID: 1266
	public class QrDecomposition
	{
		// Token: 0x06001EB0 RID: 7856 RVA: 0x000A0104 File Offset: 0x0009E304
		public QrDecomposition(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.QR = value.Clone();
			double[][] array = this.QR.Array;
			int rows = value.Rows;
			int columns = value.Columns;
			this.Rdiag = new double[columns];
			for (int i = 0; i < columns; i++)
			{
				double num = 0.0;
				for (int j = i; j < rows; j++)
				{
					num = QrDecomposition.Hypotenuse(num, array[j][i]);
				}
				if (num != 0.0)
				{
					if (array[i][i] < 0.0)
					{
						num = -num;
					}
					for (int k = i; k < rows; k++)
					{
						array[k][i] /= num;
					}
					array[i][i] += 1.0;
					for (int l = i + 1; l < columns; l++)
					{
						double num2 = 0.0;
						for (int m = i; m < rows; m++)
						{
							num2 += array[m][i] * array[m][l];
						}
						num2 = -num2 / array[i][i];
						for (int n = i; n < rows; n++)
						{
							array[n][l] += num2 * array[n][i];
						}
					}
				}
				this.Rdiag[i] = -num;
			}
		}

		// Token: 0x06001EB1 RID: 7857 RVA: 0x000A0274 File Offset: 0x0009E474
		public Matrix Solve(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Rows != this.QR.Rows)
			{
				throw new ArgumentException("Matrix row dimensions must agree.");
			}
			if (!this.FullRank)
			{
				throw new InvalidOperationException("Matrix is rank deficient.");
			}
			int columns = value.Columns;
			Matrix matrix = value.Clone();
			int rows = this.QR.Rows;
			int columns2 = this.QR.Columns;
			double[][] array = this.QR.Array;
			for (int i = 0; i < columns2; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					double num = 0.0;
					for (int k = i; k < rows; k++)
					{
						num += array[k][i] * matrix[k, j];
					}
					num = -num / array[i][i];
					for (int l = i; l < rows; l++)
					{
						Matrix matrix2 = matrix;
						int num2 = l;
						int num3 = j;
						matrix2[num2, num3] += num * array[l][i];
					}
				}
			}
			for (int m = columns2 - 1; m >= 0; m--)
			{
				for (int n = 0; n < columns; n++)
				{
					Matrix matrix2 = matrix;
					int num3 = m;
					int num2 = n;
					matrix2[num3, num2] /= this.Rdiag[m];
				}
				for (int num4 = 0; num4 < m; num4++)
				{
					for (int num5 = 0; num5 < columns; num5++)
					{
						Matrix matrix2 = matrix;
						int num2 = num4;
						int num3 = num5;
						matrix2[num2, num3] -= matrix[m, num5] * array[num4][m];
					}
				}
			}
			return matrix.Submatrix(0, columns2 - 1, 0, columns - 1);
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001EB2 RID: 7858 RVA: 0x000A045C File Offset: 0x0009E65C
		public bool FullRank
		{
			get
			{
				int columns = this.QR.Columns;
				for (int i = 0; i < columns; i++)
				{
					if (this.Rdiag[i] == 0.0)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001EB3 RID: 7859 RVA: 0x000A0498 File Offset: 0x0009E698
		public Matrix UpperTriangularFactor
		{
			get
			{
				int columns = this.QR.Columns;
				Matrix matrix = new Matrix(columns, columns);
				double[][] array = matrix.Array;
				double[][] array2 = this.QR.Array;
				for (int i = 0; i < columns; i++)
				{
					for (int j = 0; j < columns; j++)
					{
						if (i < j)
						{
							array[i][j] = array2[i][j];
						}
						else if (i == j)
						{
							array[i][j] = this.Rdiag[i];
						}
						else
						{
							array[i][j] = 0.0;
						}
					}
				}
				return matrix;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001EB4 RID: 7860 RVA: 0x000A052C File Offset: 0x0009E72C
		public Matrix OrthogonalFactor
		{
			get
			{
				Matrix matrix = new Matrix(this.QR.Rows, this.QR.Columns);
				double[][] array = matrix.Array;
				double[][] array2 = this.QR.Array;
				for (int i = this.QR.Columns - 1; i >= 0; i--)
				{
					for (int j = 0; j < this.QR.Rows; j++)
					{
						array[j][i] = 0.0;
					}
					array[i][i] = 1.0;
					for (int k = i; k < this.QR.Columns; k++)
					{
						if (array2[i][i] != 0.0)
						{
							double num = 0.0;
							for (int l = i; l < this.QR.Rows; l++)
							{
								num += array2[l][i] * array[l][k];
							}
							num = -num / array2[i][i];
							for (int m = i; m < this.QR.Rows; m++)
							{
								array[m][k] += num * array2[m][i];
							}
						}
					}
				}
				return matrix;
			}
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x000A0664 File Offset: 0x0009E864
		private static double Hypotenuse(double a, double b)
		{
			if (Math.Abs(a) > Math.Abs(b))
			{
				double num = b / a;
				return Math.Abs(a) * Math.Sqrt(1.0 + num * num);
			}
			if (b != 0.0)
			{
				double num2 = a / b;
				return Math.Abs(b) * Math.Sqrt(1.0 + num2 * num2);
			}
			return 0.0;
		}

		// Token: 0x04001803 RID: 6147
		private Matrix QR;

		// Token: 0x04001804 RID: 6148
		private double[] Rdiag;
	}
}

using System;
using System.Globalization;
using System.IO;

namespace UnityEngine
{
	// Token: 0x020004F1 RID: 1265
	public class Matrix
	{
		// Token: 0x06001E86 RID: 7814 RVA: 0x0009F4A8 File Offset: 0x0009D6A8
		public Matrix(int rows, int columns)
		{
			this.rows = rows;
			this.columns = columns;
			this.data = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				this.data[i] = new double[columns];
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x0009F4F0 File Offset: 0x0009D6F0
		public Matrix(int rows, int columns, double value)
		{
			this.rows = rows;
			this.columns = columns;
			this.data = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				this.data[i] = new double[columns];
			}
			for (int j = 0; j < rows; j++)
			{
				this.data[j][j] = value;
			}
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x0009F550 File Offset: 0x0009D750
		public Matrix(double[][] value)
		{
			this.rows = value.Length;
			this.columns = value[0].Length;
			for (int i = 0; i < this.rows; i++)
			{
				if (value[i].Length != this.columns)
				{
					throw new ArgumentException("Argument out of range.");
				}
			}
			this.data = value;
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x0009F5A7 File Offset: 0x0009D7A7
		public override bool Equals(object obj)
		{
			return Matrix.Equals(this, (Matrix)obj);
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x0009F5B8 File Offset: 0x0009D7B8
		public static bool Equals(Matrix left, Matrix right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Rows != right.Rows || left.Columns != right.Columns)
			{
				return false;
			}
			for (int i = 0; i < left.Rows; i++)
			{
				for (int j = 0; j < left.Columns; j++)
				{
					if (left[i, j] != right[i, j])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x0009F628 File Offset: 0x0009D828
		public override int GetHashCode()
		{
			return this.Rows + this.Columns;
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001E8C RID: 7820 RVA: 0x0009F637 File Offset: 0x0009D837
		internal double[][] Array
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001E8D RID: 7821 RVA: 0x0009F63F File Offset: 0x0009D83F
		public int Rows
		{
			get
			{
				return this.rows;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001E8E RID: 7822 RVA: 0x0009F647 File Offset: 0x0009D847
		public int Columns
		{
			get
			{
				return this.columns;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001E8F RID: 7823 RVA: 0x0009F64F File Offset: 0x0009D84F
		public bool Square
		{
			get
			{
				return this.rows == this.columns;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001E90 RID: 7824 RVA: 0x0009F660 File Offset: 0x0009D860
		public bool Symmetric
		{
			get
			{
				if (this.Square)
				{
					for (int i = 0; i < this.rows; i++)
					{
						for (int j = 0; j <= i; j++)
						{
							if (this.data[i][j] != this.data[j][i])
							{
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
		}

		// Token: 0x17000452 RID: 1106
		public double this[int row, int column]
		{
			get
			{
				return this.data[row][column];
			}
			set
			{
				this.data[row][column] = value;
			}
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x0009F6C8 File Offset: 0x0009D8C8
		public Matrix Submatrix(int startRow, int endRow, int startColumn, int endColumn)
		{
			if (startRow > endRow || startColumn > endColumn || startRow < 0 || startRow >= this.rows || endRow < 0 || endRow >= this.rows || startColumn < 0 || startColumn >= this.columns || endColumn < 0 || endColumn >= this.columns)
			{
				throw new ArgumentException("Argument out of range.");
			}
			Matrix matrix = new Matrix(endRow - startRow + 1, endColumn - startColumn + 1);
			double[][] array = matrix.Array;
			for (int i = startRow; i <= endRow; i++)
			{
				for (int j = startColumn; j <= endColumn; j++)
				{
					array[i - startRow][j - startColumn] = this.data[i][j];
				}
			}
			return matrix;
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x0009F764 File Offset: 0x0009D964
		public Matrix Submatrix(int[] rowIndexes, int[] columnIndexes)
		{
			Matrix matrix = new Matrix(rowIndexes.Length, columnIndexes.Length);
			double[][] array = matrix.Array;
			for (int i = 0; i < rowIndexes.Length; i++)
			{
				for (int j = 0; j < columnIndexes.Length; j++)
				{
					if (rowIndexes[i] < 0 || rowIndexes[i] >= this.rows || columnIndexes[j] < 0 || columnIndexes[j] >= this.columns)
					{
						throw new ArgumentException("Argument out of range.");
					}
					array[i][j] = this.data[rowIndexes[i]][columnIndexes[j]];
				}
			}
			return matrix;
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x0009F7E4 File Offset: 0x0009D9E4
		public Matrix Submatrix(int i0, int i1, int[] c)
		{
			if (i0 > i1 || i0 < 0 || i0 >= this.rows || i1 < 0 || i1 >= this.rows)
			{
				throw new ArgumentException("Argument out of range.");
			}
			Matrix matrix = new Matrix(i1 - i0 + 1, c.Length);
			double[][] array = matrix.Array;
			for (int j = i0; j <= i1; j++)
			{
				for (int k = 0; k < c.Length; k++)
				{
					if (c[k] < 0 || c[k] >= this.columns)
					{
						throw new ArgumentException("Argument out of range.");
					}
					array[j - i0][k] = this.data[j][c[k]];
				}
			}
			return matrix;
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x0009F87C File Offset: 0x0009DA7C
		public Matrix Submatrix(int[] r, int j0, int j1)
		{
			if (j0 > j1 || j0 < 0 || j0 >= this.columns || j1 < 0 || j1 >= this.columns)
			{
				throw new ArgumentException("Argument out of range.");
			}
			Matrix matrix = new Matrix(r.Length, j1 - j0 + 1);
			double[][] array = matrix.Array;
			for (int i = 0; i < r.Length; i++)
			{
				for (int k = j0; k <= j1; k++)
				{
					if (r[i] < 0 || r[i] >= this.rows)
					{
						throw new ArgumentException("Argument out of range.");
					}
					array[i][k - j0] = this.data[r[i]][k];
				}
			}
			return matrix;
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x0009F914 File Offset: 0x0009DB14
		public Matrix Clone()
		{
			Matrix matrix = new Matrix(this.rows, this.columns);
			double[][] array = matrix.Array;
			for (int i = 0; i < this.rows; i++)
			{
				for (int j = 0; j < this.columns; j++)
				{
					array[i][j] = this.data[i][j];
				}
			}
			return matrix;
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x0009F96C File Offset: 0x0009DB6C
		public Matrix Transpose()
		{
			Matrix matrix = new Matrix(this.columns, this.rows);
			double[][] array = matrix.Array;
			for (int i = 0; i < this.rows; i++)
			{
				for (int j = 0; j < this.columns; j++)
				{
					array[j][i] = this.data[i][j];
				}
			}
			return matrix;
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001E99 RID: 7833 RVA: 0x0009F9C4 File Offset: 0x0009DBC4
		public double Norm1
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.columns; i++)
				{
					double num2 = 0.0;
					for (int j = 0; j < this.rows; j++)
					{
						num2 += Math.Abs(this.data[j][i]);
					}
					num = Math.Max(num, num2);
				}
				return num;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001E9A RID: 7834 RVA: 0x0009FA24 File Offset: 0x0009DC24
		public double InfinityNorm
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.rows; i++)
				{
					double num2 = 0.0;
					for (int j = 0; j < this.columns; j++)
					{
						num2 += Math.Abs(this.data[i][j]);
					}
					num = Math.Max(num, num2);
				}
				return num;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001E9B RID: 7835 RVA: 0x0009FA84 File Offset: 0x0009DC84
		public double FrobeniusNorm
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.rows; i++)
				{
					for (int j = 0; j < this.columns; j++)
					{
						num = Matrix.Hypotenuse(num, this.data[i][j]);
					}
				}
				return num;
			}
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x0009FAD0 File Offset: 0x0009DCD0
		public static Matrix Negate(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.Rows;
			int num2 = value.Columns;
			double[][] array = value.Array;
			Matrix matrix = new Matrix(num, num2);
			double[][] array2 = matrix.Array;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array2[i][j] = -array[i][j];
				}
			}
			return matrix;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x0009FB47 File Offset: 0x0009DD47
		public static Matrix operator -(Matrix value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return Matrix.Negate(value);
		}

		// Token: 0x06001E9E RID: 7838 RVA: 0x0009FB63 File Offset: 0x0009DD63
		public static bool operator ==(Matrix left, Matrix right)
		{
			return Matrix.Equals(left, right);
		}

		// Token: 0x06001E9F RID: 7839 RVA: 0x0009FB6C File Offset: 0x0009DD6C
		public static bool operator !=(Matrix left, Matrix right)
		{
			return !Matrix.Equals(left, right);
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x0009FB78 File Offset: 0x0009DD78
		public static Matrix Add(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			int num = left.Rows;
			int num2 = left.Columns;
			double[][] array = left.Array;
			if (num != right.Rows || num2 != right.Columns)
			{
				throw new ArgumentException("Matrix dimension do not match.");
			}
			Matrix matrix = new Matrix(num, num2);
			double[][] array2 = matrix.Array;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array2[i][j] = array[i][j] + right[i, j];
				}
			}
			return matrix;
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x0009FC2A File Offset: 0x0009DE2A
		public static Matrix operator +(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			return Matrix.Add(left, right);
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x0009FC5C File Offset: 0x0009DE5C
		public static Matrix Subtract(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			int num = left.Rows;
			int num2 = left.Columns;
			double[][] array = left.Array;
			if (num != right.Rows || num2 != right.Columns)
			{
				throw new ArgumentException("Matrix dimension do not match.");
			}
			Matrix matrix = new Matrix(num, num2);
			double[][] array2 = matrix.Array;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array2[i][j] = array[i][j] - right[i, j];
				}
			}
			return matrix;
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x0009FD0E File Offset: 0x0009DF0E
		public static Matrix operator -(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			return Matrix.Subtract(left, right);
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x0009FD40 File Offset: 0x0009DF40
		public static Matrix Multiply(Matrix left, double right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			int num = left.Rows;
			int num2 = left.Columns;
			double[][] array = left.Array;
			Matrix matrix = new Matrix(num, num2);
			double[][] array2 = matrix.Array;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array2[i][j] = array[i][j] * right;
				}
			}
			return matrix;
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x0009FDB8 File Offset: 0x0009DFB8
		public static Matrix operator *(Matrix left, double right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			return Matrix.Multiply(left, right);
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x0009FDD5 File Offset: 0x0009DFD5
		public static Matrix operator *(double left, Matrix right)
		{
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			return Matrix.Multiply(right, left);
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x0009FDF4 File Offset: 0x0009DFF4
		public static Matrix Multiply(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			int num = left.Rows;
			double[][] array = left.Array;
			if (right.Rows != left.columns)
			{
				throw new ArgumentException("Matrix dimensions are not valid.");
			}
			int num2 = right.Columns;
			Matrix matrix = new Matrix(num, num2);
			double[][] array2 = matrix.Array;
			int num3 = left.columns;
			double[] array3 = new double[num3];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					array3[j] = right[j, i];
				}
				for (int k = 0; k < num; k++)
				{
					double[] array4 = array[k];
					double num4 = 0.0;
					for (int l = 0; l < num3; l++)
					{
						num4 += array4[l] * array3[l];
					}
					array2[k][i] = num4;
				}
			}
			return matrix;
		}

		// Token: 0x06001EA8 RID: 7848 RVA: 0x0009FEF5 File Offset: 0x0009E0F5
		public static Matrix operator *(Matrix left, Matrix right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			return Matrix.Multiply(left, right);
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x0009FF26 File Offset: 0x0009E126
		public Matrix Solve(Matrix rightHandSide)
		{
			if (this.rows != this.columns)
			{
				return new QrDecomposition(this).Solve(rightHandSide);
			}
			return new LuDecomposition(this).Solve(rightHandSide);
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001EAA RID: 7850 RVA: 0x0009FF4F File Offset: 0x0009E14F
		public Matrix Inverse
		{
			get
			{
				return this.Solve(Matrix.Diagonal(this.rows, this.rows, 1.0));
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001EAB RID: 7851 RVA: 0x0009FF71 File Offset: 0x0009E171
		public double Determinant
		{
			get
			{
				return new LuDecomposition(this).Determinant;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001EAC RID: 7852 RVA: 0x0009FF80 File Offset: 0x0009E180
		public double Trace
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < Math.Min(this.rows, this.columns); i++)
				{
					num += this.data[i][i];
				}
				return num;
			}
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x0009FFC4 File Offset: 0x0009E1C4
		public static Matrix Diagonal(int rows, int columns, double value)
		{
			Matrix matrix = new Matrix(rows, columns);
			double[][] array = matrix.Array;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					array[i][j] = ((i == j) ? value : 0.0);
				}
			}
			return matrix;
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x000A0010 File Offset: 0x0009E210
		public override string ToString()
		{
			string text;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				for (int i = 0; i < this.rows; i++)
				{
					for (int j = 0; j < this.columns; j++)
					{
						stringWriter.Write(this.data[i][j].ToString() + " ");
					}
					stringWriter.WriteLine();
				}
				text = stringWriter.ToString();
			}
			return text;
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x000A0098 File Offset: 0x0009E298
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

		// Token: 0x04001800 RID: 6144
		private double[][] data;

		// Token: 0x04001801 RID: 6145
		private int rows;

		// Token: 0x04001802 RID: 6146
		private int columns;
	}
}

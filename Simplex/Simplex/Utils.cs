﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex
{
    public static class Utils
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static void PopulateLastRow<T>(this T[,] arr, T[] values)
        {
            var lastRow = arr.GetUpperBound(0);
            for (int i = 0; i < arr.GetLength(1); i++)
            {
                arr[lastRow, i] = values[i];
            }
        }


        public static double[,] AddColumn(double[,] original, double[] added, int index)
        {
            int lastRow = original.GetLength(0);
            int lastColumn = original.GetLength(1);
            double[,] result = new double[lastRow, lastColumn + 1];
            for (int i = 0; i < lastRow; i++)
            {
                for (int j = 0;  j <= lastColumn; j++)
                {
                    int k = j;
                    if(index < j)
                    {
                        k = j - 1;
                    }
                    if(j == index)
                    {
                        result[i, j] = added[i];
                    }else
                    {
                        result[i, j] = original[i, k];
                    }
                }
            }
            return result;
        }

        public static double[,] AddRow(double[,] original, double[] added)
        {
            int lastRow = original.GetUpperBound(0);
            int lastColumn = original.GetUpperBound(1);
            // Create new array.
            double[,] result = new double[lastRow + 2, lastColumn + 1];
            // Copy existing array into the new array.
            for (int i = 0; i <= lastRow; i++)
            {
                for (int x = 0; x <= lastColumn; x++)
                {
                    result[i, x] = original[i, x];
                }
            }
            // Add the new row.
            for (int i = 0; i < added.Length; i++)
            {
                result[lastRow + 1, i] = added[i];
            }
            return result;
        }

        public static double[,] Copy(double[,] original)
        {
            int lastRow = original.GetUpperBound(0);
            int lastColumn = original.GetUpperBound(1);
            // Create new array.
            double[,] result = new double[lastRow + 1, lastColumn + 1];
            // Copy existing array into the new array.
            for (int i = 0; i <= lastRow; i++)
            {
                for (int x = 0; x <= lastColumn; x++)
                {
                    result[i, x] = original[i, x];
                }
            }
            return result;
        }


        


        public static double[,] TrimColumnArray(int columnToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0), originalArray.GetLength(1)- 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }
            return result;
        }


        public static double[,] TrimArray(int rowToRemove, int columnToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }

        public static double[,] TrimRowArray(int rowToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0) - 1, originalArray.GetLength(1)];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }
            return result;
        }
    }
}

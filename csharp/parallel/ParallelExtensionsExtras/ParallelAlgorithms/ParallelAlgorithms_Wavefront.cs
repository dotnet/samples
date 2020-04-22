//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="numBlocksPerRow">Partition the matrix into this number of blocks along the rows.</param>
        /// <param name="numBlocksPerColumn">Partition the matrix into this number of blocks along the columns.</param>
        /// <param name="processBlock">The action to invoke for every block, supplied with the start and end indices of the rows and columns.</param>
        public static void Wavefront(
            int numRows, int numColumns,
            int numBlocksPerRow, int numBlocksPerColumn,
            Action<int, int, int, int> processBlock)
        {
            // Validate parameters
            if (numRows <= 0) throw new ArgumentOutOfRangeException(nameof(numRows));
            if (numColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numColumns));
            if (numBlocksPerRow <= 0 || numBlocksPerRow > numRows)
                throw new ArgumentOutOfRangeException(nameof(numBlocksPerRow));
            if (numBlocksPerColumn <= 0 || numBlocksPerColumn > numColumns)
                throw new ArgumentOutOfRangeException(nameof(numBlocksPerColumn));
            if (processBlock == null)
                throw new ArgumentNullException(nameof(processBlock));

            // Compute the size of each block
            int rowBlockSize = numRows / numBlocksPerRow;
            int columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront(numBlocksPerRow, numBlocksPerColumn, (row, column) =>
            {
                int start_i = row * rowBlockSize;
                int end_i = row < numBlocksPerRow - 1 ?
                    start_i + rowBlockSize : numRows;

                int start_j = column * columnBlockSize;
                int end_j = column < numBlocksPerColumn - 1 ?
                    start_j + columnBlockSize : numColumns;

                processBlock(start_i, end_i, start_j, end_j);
            });
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="processRowColumnCell">The action to invoke for every cell, supplied with the row and column indices.</param>
        public static void Wavefront(int numRows, int numColumns, Action<int, int> processRowColumnCell)
        {
            // Validate parameters
            if (numRows <= 0) throw new ArgumentOutOfRangeException(nameof(numRows));
            if (numColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numColumns));
            if (processRowColumnCell == null) throw new ArgumentNullException(nameof(processRowColumnCell));

            // Store the previous row of tasks as well as the previous task in the current row
            Task[] prevTaskRow = new Task[numColumns];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[2];

            // Create a task for each cell
            for (int row = 0; row < numRows; row++)
            {
                prevTaskInCurrentRow = null;
                for (int column = 0; column < numColumns; column++)
                {
                    // In-scope locals for being captured in the task closures
                    int j = row, i = column;

                    // Create a task with the appropriate dependencies.
                    Task curTask;
                    if (row == 0 && column == 0)
                    {
                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Factory.StartNew(() => processRowColumnCell(j, i));
                    }
                    else if (row == 0 || column == 0)
                    {
                        // Tasks in the left-most column depend only on the task above them, and
                        // tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[0] : prevTaskInCurrentRow;
                        curTask = antecedent.ContinueWith(p =>
                        {
                            p.Wait(); // Necessary only to propagate exceptions
                            processRowColumnCell(j, i);
                        });
                    }
                    else // row > 0 && column > 0
                    {
                        // All other tasks depend on both the tasks above and to the left
                        dependencies[0] = prevTaskInCurrentRow;
                        dependencies[1] = prevTaskRow[column];
                        curTask = Task.Factory.ContinueWhenAll(dependencies, ps =>
                        {
                            Task.WaitAll(ps); // Necessary only to propagate exceptions
                            processRowColumnCell(j, i);
                        });
                    }

                    // Keep track of the task just created for future iterations
                    prevTaskRow[column] = prevTaskInCurrentRow = curTask;
                }
            }

            // Wait for the last task to be done.
            prevTaskInCurrentRow.Wait();
        }
    }
}

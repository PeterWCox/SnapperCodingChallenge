﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnapperCodingChallenge.Core
{
    /// <summary>
    /// A class that models a target image e.g. starship, nuclear torpedo as a 2D array of characters.
    /// </summary>
    public class TargetImageTextFile : ITargetImage
    {
        public TargetImageTextFile(string name, string filePath, char blankCharacter)
        {
            this.Name = name;
            this.FilePath = filePath;
            this.GridRepresentation = ConvertTextFileInto2DArray(filePath).TrimArray(blankCharacter);
            this.InternalShapeCoordinatesOfTarget = ITargetImage.CalculateCoordinatesInsidePerimeterOfObject(this, blankCharacter);
            this.CentroidLocalCoordinates = CalculateLocalCoordinatesOfShapeCentroid();
    
            bool targetOK = ITargetImage.VerifyTargetHasADefinedShape(InternalShapeCoordinatesOfTarget);

            if (!targetOK)
            {
                throw new Exception("Target is not defined by a particular shape - please check input and try again.");
            }
        }

        /// <summary>
        /// The name of the target e.g. Starship, NuclearTorpedo
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The filepath of the textfile used to create the target.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The 2D multi-dimensional array that models the target, used for the business logic.
        /// </summary>
        public char[,] GridRepresentation { get; }

        /// <summary>
        /// The elements within the grid representation of the target, used as a comparison to determine whether a given
        /// slice of the SnapperImage contains a target - for example:
        /// 
        /// {
        ///   00X00
        ///   XXXXX
        ///   0X0X0
        ///   00X00
        /// }
        /// Yields [row,col] => 
        /// {0,2},
        /// {1,0},{1,2},{1,3},{1,4},{1,5}, 
        /// {2,1},{2,2}.{2,3},{2,4}
        /// {3,2}
        /// </summary>
        public List<Coordinate> InternalShapeCoordinatesOfTarget { get; }

        /// <summary>
        /// The coordinates {x,y} describing the centroid of the object. relative to the local {0,0} square.
        /// For example:
        /// 
        ///  0 1|2 3 
        /// 0X X X X
        /// 1X X X X  
        /// 2X X X X
        /// 3X X X X
        /// 4X X X X
        /// 
        /// The "centroid" of the shape {x,y} from 0,0 square is 1.5,2.5
        /// </summary>
        public Coordinate CentroidLocalCoordinates { get; }

        /// <summary>
        /// Takes a textfile and converts it into a 2D array of characters.
        /// </summary>
        /// <param name="filePath">The filepath for the textfile.</param>
        /// <returns></returns>
        private char[,] ConvertTextFileInto2DArray(string filePath)
        {
            //Open the text file and get an array of strings representing each line.
            string[] rows = File.ReadAllLines(filePath);

            //Set the dimensions of the 2D character array.
            int numberOfRows = rows.Length;
            int numberOfColumns = rows[0].Length;
            char[,] array = new char[numberOfRows, numberOfColumns];

            //For each row, convert to character array and set the elements of the 2d char array/

            for (int i = 0; i < numberOfRows; i++)
            {
                //Convert the ith row into a character array.                     
                char[] charArray = rows[i].ToCharArray();

                //Add each element in the char array to the row under consideration. 
                for (int j = 0; j < charArray.Length; j++)
                {
                    array[i, j] = charArray[j];
                }
            }

            return array;
        }

        /// <summary>
        /// Returns the local co-ordinates of the target, for example:
        /// 
        /// Local coords {x,y} => 0.5,1.5
        /// 
        ///   0 1  x=> 
        /// 0 X X
        /// 1 X X 
        /// 2 X X 
        /// 3 X X 
        /// 
        /// </summary>
        /// <returns></returns>
        public Coordinate CalculateLocalCoordinatesOfShapeCentroid()
        {
            int numberOfRows = GridRepresentation.GetLength(0);
            int numberOfColumns = GridRepresentation.GetLength(1);

            double numberOfColumnsDbl = Convert.ToDouble(numberOfColumns);
            double numberOfRowsDbl = Convert.ToDouble(numberOfRows);

            double x = (numberOfColumnsDbl - 1) / 2;
            double y = (numberOfRowsDbl - 1) / 2;

            return new Coordinate(x, y);
        }

        

       

        

        
    }
}

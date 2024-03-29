﻿using System;

namespace SnapperCodingChallenge.Core
{
    /// <summary>
    /// A class used to compare a piece of a snapper image with a defined target to determine whether a target
    /// indeed exists.
    /// </summary>
    public class Scan
    {
        public Scan(ISnapperImage snapperImage, ITargetImage targetImage, int horizontalOffset, 
            int verticalOffset, double minimumConfidenceInTargetPrecision)
        {
            SnapperImage = snapperImage;
            TargetImage = targetImage;
            this.HorizontalOffset = horizontalOffset;
            this.VerticalOffset = verticalOffset;
            MinimumConfidenceInTargetPrecision = minimumConfidenceInTargetPrecision;
            CentroidGlobalCoordinates = ITargetImage.CalculateGlobalCoordinatesOfShapeCentroid(TargetImage, HorizontalOffset, VerticalOffset);
            TopLHCornerGlobalCoordinates = new Coordinate(HorizontalOffset, VerticalOffset);
            ScanImageForTarget();
        }

        /// <summary>
        /// The snapper image to be scanned.
        /// </summary>
        public ISnapperImage SnapperImage { get; }

        /// <summary>
        /// The target we are scanning for.
        /// </summary>
        public ITargetImage TargetImage { get; }

        /// <summary>
        /// The horizontal offset from 0,0 for the snapper image which we're scanning for.
        /// </summary>
        public int HorizontalOffset { get; }

        /// <summary>
        /// The vertical offset from 0,0 for the snapper image which we're scanning for.
        /// </summary>
        public int VerticalOffset { get; }
        
        /// <summary>
        /// The minimum match in elements we need to determine a match e.g. 0.7 => 70% minimum match required.
        /// </summary>
        public double MinimumConfidenceInTargetPrecision { get; }

        //Properties

        /// <summary>
        /// The global coordinates of the centroid of the target slice.
        /// </summary>
        public Coordinate CentroidGlobalCoordinates { get; set; }

        /// <summary>
        /// The global coordinates of the centroid of the target slice.
        /// </summary>
        public Coordinate TopLHCornerGlobalCoordinates { get; set; }


        /// <summary>
        /// The number of matches when comparing ArrayA to Array B. For example:
        /// 
        /// 00100       00000
        /// 00100  vs   00100
        /// 01110       01100
        /// 11111       11001
        /// 
        /// Number of Elements compared = 10
        /// Number of Matches = 6
        /// Number of difference = 4
        /// 
        /// </summary>
        public double Matches { get; private set; }

        /// <summary>
        /// The number of differences when comparing ArrayA to Array B. For example:
        /// 
        /// 00100       00000
        /// 00100  vs   00100
        /// 01110       01100
        /// 11111       11001
        /// 
        /// Number of Elements compared = 10
        /// Number of Matches = 6
        /// Number of difference = 4
        /// 
        /// </summary>
        public double Differences { get; private set; }

        /// <summary>
        /// The accuracy of the match. For example:
        /// 
        /// 00100       00000
        /// 00100  vs   00100
        /// 01110       01100
        /// 11111       11001
        /// 
        /// Number of Elements compared = 10
        /// Number of Matches = 6
        /// Number of difference = 4
        /// Confidence = 6/10 = 60%
        /// 
        /// </summary>
        public double ConfidenceInTargetDetection { get; private set; }
       
        /// <summary>
        /// Returns whether a target was found.
        /// </summary>
        public bool TargetFound { get; private set; }

        //Methods
        private void ScanImageForTarget()
        {
            //Get a "Slice" of the SnapperImage based on a horiz+vert offset from (0,0) based on dimensions of target
            char[,] slice = GetSubArrayFromArray
                (SnapperImage.GridRepresentation, TargetImage.GridRepresentation, HorizontalOffset, VerticalOffset);

            foreach (Coordinate coordinate in TargetImage.InternalShapeCoordinatesOfTarget)
            {
                int x = Convert.ToInt32(coordinate.X);
                int y = Convert.ToInt32(coordinate.Y);

                if (slice[x, y] == TargetImage.GridRepresentation[x, y])
                {
                    Matches++;
                }
                else
                {
                    Differences++;
                }
            }

            ConfidenceInTargetDetection = Matches / (Matches + Differences);

            TargetFound = (ConfidenceInTargetDetection >= MinimumConfidenceInTargetPrecision) ? true : false;
        }

        private char[,] GetSubArrayFromArray(char[,] mainArray, char[,] targetArray, int horizontalOffset, int verticalOffset)
        {
            char[,] slice = new char[targetArray.GetLength(0), targetArray.GetLength(1)];

            for (int i = 0; i < slice.GetLength(0); i++)
            {
                for (int j = 0; j < slice.GetLength(1); j++)
                {
                    slice[i, j] = mainArray[i + verticalOffset, j + horizontalOffset];
                }
            }

            return slice;
        }

        public string ScanSummary()
        {
            if (TargetFound)
            {
                return $"Position {HorizontalOffset},{VerticalOffset} - {TargetImage.Name} found with centroid co-ordinates [X,Y] {CentroidGlobalCoordinates.X}," +
                    $"{CentroidGlobalCoordinates.Y} with a certainty of {100 * Math.Round(ConfidenceInTargetDetection, 2)}%!";
            }
            else
            {
                return $"Position {HorizontalOffset},{VerticalOffset} - {TargetImage.Name} NOT found with centroid co-ordinates [X,Y] {CentroidGlobalCoordinates.X}," +
                    $"{CentroidGlobalCoordinates.Y}.";
            }
        }

    }
}

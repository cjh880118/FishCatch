using System;
using UnityEngine;

namespace JHchoi.Module.Detection.CV
{
    public class WaterplayBuilder : IGraphBuilder<DetectionGraph>
    {
        public WaterplayBuilder()
        {

        }

        public DetectionGraph Build()
        {
            var bytesToMat = new BytesToMatNode();
            var matToCalibMat = new MatToCalibMatNode();
            var calibMatToWarpMat = new MatToWarpMatNode();
            var warpMatToDiffMat = new MatToDiffMatNode();
            var diffMatToBinaryMat = new MatToBinaryMatNode();
            var binaryMatToImageContours = new MatToImageContoursNode();
            var imageContoursToViewportContours = new ImageContoursToViewportContoursNode();

            return new DetectionGraph(bytesToMat)
                .AddNode(bytesToMat, matToCalibMat)
                .AddDepth()
                .AddNode(matToCalibMat, calibMatToWarpMat)
                .AddDepth()
                .AddNode(calibMatToWarpMat, warpMatToDiffMat)
                .AddDepth()
                .AddNode(warpMatToDiffMat, diffMatToBinaryMat)
                .AddDepth()
                .AddNode(diffMatToBinaryMat, binaryMatToImageContours)
                .AddDepth()
                .AddNode(binaryMatToImageContours, imageContoursToViewportContours);
        }
    }
}
using System;
using UnityEngine;

namespace JHchoi.Module.Detection
{
    public interface IGraphBuilder<TGraph> where TGraph : IGraph
    {
        TGraph Build();
    }
}
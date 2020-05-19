using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.UI.Event
{
    public class QuizMsg : Message
    {
        public MeshType MeshType;
        public PatternType PatternType;

        public QuizMsg(MeshType meshType, PatternType patternType)
        {
            MeshType = meshType;
            PatternType = patternType;
        }
    }
}
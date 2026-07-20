using System.Collections.Generic;

namespace DroneFactory.Model
{
    public class PieceDefinition
    {
        public PieceDefinition(string name, PieceKind kind, params PieceTag[] tags)
        {
            Name = name;
            Kind = kind;
            Tags = tags;
        }

        public string Name { get; }
        public PieceKind Kind { get; }
        public IReadOnlyList<PieceTag> Tags { get; }

        public bool HasTag(PieceTag tag)
        {
            for (int i = 0; i < Tags.Count; i++)
            {
                if (Tags[i] == tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

using System.Collections.Generic;
using DroneFactory.Model;

namespace DroneFactory.Data
{
    public class PieceCatalog
    {
        private readonly Dictionary<string, PieceDefinition> _pieces =
            new Dictionary<string, PieceDefinition>(System.StringComparer.Ordinal);

        private readonly List<string> _order = new List<string>();
        private readonly List<string> _stockableOrder = new List<string>();

        public PieceCatalog()
        {
            Add(new PieceDefinition("Hull_HG1", PieceKind.Hull, PieceTag.S));
            Add(new PieceDefinition("Hull_HF1", PieceKind.Hull));
            Add(new PieceDefinition("Hull_HS1", PieceKind.Hull, PieceTag.S));

            Add(new PieceDefinition("Core_CG1", PieceKind.Core, PieceTag.TwoD));
            Add(new PieceDefinition("Core_C3D1", PieceKind.Core, PieceTag.TwoD, PieceTag.ThreeD));

            Add(new PieceDefinition("Generator_GG1", PieceKind.Generator));
            Add(new PieceDefinition("Generator_GF1", PieceKind.Generator));
            Add(new PieceDefinition("Generator_GS1", PieceKind.Generator, PieceTag.S));

            Add(new PieceDefinition("Move_MF1", PieceKind.Move, PieceTag.F));
            Add(new PieceDefinition("Move_ML1", PieceKind.Move, PieceTag.L));
            Add(new PieceDefinition("Move_MS1", PieceKind.Move, PieceTag.S));
            Add(new PieceDefinition("Move_MM1", PieceKind.Move, PieceTag.M));
            Add(new PieceDefinition("Move_MU1", PieceKind.Move, PieceTag.M, PieceTag.L));
            Add(new PieceDefinition("Move_MS2", PieceKind.Move, PieceTag.M, PieceTag.S));

            Add(new PieceDefinition("Processor_PG1", PieceKind.Processor, PieceTag.TwoD));
            Add(new PieceDefinition("Processor_P3D1", PieceKind.Processor, PieceTag.ThreeD));
            Add(new PieceDefinition("Processor_PU1", PieceKind.Processor, PieceTag.TwoD, PieceTag.ThreeD));

            Add(new PieceDefinition("System_SG1", PieceKind.System, PieceTag.TwoD));
            Add(new PieceDefinition("System_S3D1", PieceKind.System, PieceTag.TwoD, PieceTag.ThreeD));
        }

        private void Add(PieceDefinition piece)
        {
            _pieces[piece.Name] = piece;
            _order.Add(piece.Name);

            // Systemes exclus du stock de pieces (hypothese reprise de l'etape 1)
            if (piece.Kind != PieceKind.System)
            {
                _stockableOrder.Add(piece.Name);
            }
        }

        public IReadOnlyList<string> PieceNames => _order;
        public IReadOnlyList<string> StockablePieceNames => _stockableOrder;

        public bool TryGet(string name, out PieceDefinition piece)
        {
            return _pieces.TryGetValue(name, out piece);
        }

        public PieceDefinition Get(string name)
        {
            return _pieces[name];
        }
    }
}

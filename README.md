# Drone Factory

Usine de drones : suivi de stock, calcul des besoins et generation des instructions
d'assemblage pour une commande de drones.

## Compiler

Depuis ce dossier (necessite le SDK .NET, multiplateforme) :

```bash
dotnet build
```

## Lancer

```bash
dotnet run
```

Une fois le programme lance, tape une commande puis appuie sur Entree.

Exemples d'entrees :

```text
STOCKS
NEEDED_STOCKS 1 DXF-1, 2 RDL-1
INSTRUCTIONS 1 DXF-1
VERIFY 1 DXF-1, 1 Cat
PRODUCE 1 DXF-1
ADD_TEMPLATE MDL-1, Hull_HS1, Core_C3D1, System_S3D1, Generator_GS1, Move_MS1, Processor_P3D1
```

Quitter avec `EXIT` ou `QUIT`.

## Design patterns (etape 2)

- **Command** (`Commands/`) : chaque instruction utilisateur (`STOCKS`, `NEEDED_STOCKS`,
  `INSTRUCTIONS`, `VERIFY`, `PRODUCE`, `ADD_TEMPLATE`) est une classe `ICommand`
  enregistree dans un `CommandRegistry`, plutot qu'un gros `if/else`.
- **Strategy** (`Categorization/`) : une regle par categorie de drone (`AerienRule`,
  `MarinRule`, `TerrestreRule`, `SubmersibleRule`), executees par `DroneCategorizer`.
- **Builder** (`Assembly/DroneAssemblyBuilder.cs`) : construit la sequence
  d'instructions d'assemblage d'un drone (`GET_OUT_STOCK`, `INSTALL`, `ASSEMBLE`, ...)
  en respectant l'ordre impose par le sujet.
- **Factory** (`DroneTemplateFactory.cs`) : construit et valide un `DroneTemplate` a
  partir de la liste de pieces d'`ADD_TEMPLATE` (identification du role de chaque
  piece + validation de categorie).

## Hypotheses de cette version

- Chaque piece demarre avec un stock de 10 exemplaires ; les drones demarrent a 0.
- Les systemes (`System_SG1`, `System_S3D1`) sont installes mais ne sont pas
  decomptes du stock, car le sujet les separe des pieces.
- `ADD_TEMPLATE TEMPLATE_NAME, Piece1, ..., PieceN` : la liste de pieces inclut le
  systeme a installer (au meme titre que la coque, le generateur, etc.) ; le role de
  chaque piece est deduit automatiquement du catalogue. Une reponse de succes
  `TEMPLATE_ADDED TEMPLATE_NAME` est renvoyee (non specifiee explicitement par le
  sujet).
- Categorie Submersible ("toutes les pieces de type (S)") : seules la coque, le
  generateur et le module de deplacement portent la dimension F/M/L/S dans le
  catalogue (le module principal, le systeme et le module de controle portent la
  dimension 2D/3D) ; la verification porte donc sur ces trois pieces.
- Un template est rejete si aucune des 4 categories (Aerien/Marin/Terrestre/
  Submersible) n'est satisfaite.

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
RECEIVE 5 Move_MF1, 2 DXF-1
PRODUCE 1 DXF-1 WITH 2 Move_ML1
GET_MOVEMENTS
GET_MOVEMENTS Hull_HF1
```

Quitter avec `EXIT` ou `QUIT`.

## Design patterns (etape 2)

- **Command** (`Commands/`) : chaque instruction utilisateur (`STOCKS`, `NEEDED_STOCKS`,
  `INSTRUCTIONS`, `VERIFY`, `PRODUCE`, `ADD_TEMPLATE`, `RECEIVE`, `GET_MOVEMENTS`) est
  une classe `ICommand` enregistree dans un `CommandRegistry`, plutot qu'un gros
  `if/else`.
- **Strategy** (`Categorization/`) : une regle par categorie de drone (`AerienRule`,
  `MarinRule`, `TerrestreRule`, `SubmersibleRule`), executees par `DroneCategorizer`.
- **Builder** (`Assembly/DroneAssemblyBuilder.cs`) : construit la sequence
  d'instructions d'assemblage d'un drone (`GET_OUT_STOCK`, `INSTALL`, `ASSEMBLE`, ...)
  en respectant l'ordre impose par le sujet, y compris pour plusieurs generateurs ou
  modules de deplacement.
- **Factory** (`DroneTemplateFactory.cs`) : construit et valide un `DroneTemplate` a
  partir de la liste de pieces d'`ADD_TEMPLATE` (identification du role de chaque
  piece + validation de construction + validation de categorie).

## Modules complementaires (etape 3)

- **5.1.1 Recevoir du stock** : `RECEIVE ARGS` ajoute des pieces ou des drones au
  stock (`Commands/ReceiveCommand.cs`).
- **5.1.2 Contraintes de construction** : un drone accepte 1 a 2 generateurs et 1 a 3
  modules de deplacement ; a partir de 2 modules de deplacement, il faut exactement 2
  generateurs. Regle centralisee dans `Data/ConstructionRules.cs`, reutilisee par
  `ADD_TEMPLATE` et par `WITH`/`WITHOUT`/`REPLACE`.
- **Modification de drones** (`WITH`/`WITHOUT`/`REPLACE`, section 5.2.1) : gere dans
  `Commands/OrderParser.cs` (analyse) et `Commands/OrderCalculator.cs`
  (`ApplyModifications`, application des deltas de pieces + revalidation de la regle
  de construction si un generateur/module de deplacement est touche).
- **Tracabilite des flux** (`GET_MOVEMENTS`, section 5.2.3) : `StockRepository`
  journalise chaque mouvement de stock (`RemovePieces`/`AddPiece`/`AddDrones`
  prennent desormais un motif, ex. `PRODUCE`, `RECEIVE`) ; `GetMovementsCommand`
  affiche l'historique complet ou filtre sur une liste d'elements.

## Hypotheses de cette version

- Chaque piece demarre avec un stock de 10 exemplaires ; les drones demarrent a 0.
- Les systemes (`System_SG1`, `System_S3D1`) sont installes mais ne sont pas
  decomptes du stock, car le sujet les separe des pieces. `RECEIVE`/`WITH`/`WITHOUT`
  refusent donc les noms de systeme avec une erreur claire.
- `ADD_TEMPLATE TEMPLATE_NAME, Piece1, ..., PieceN` : la liste de pieces inclut le
  systeme a installer (au meme titre que la coque, le generateur, etc.) ; le role de
  chaque piece est deduit automatiquement du catalogue. Une reponse de succes
  `TEMPLATE_ADDED TEMPLATE_NAME` est renvoyee (non specifiee explicitement par le
  sujet). `RECEIVE` renvoie `STOCK_UPDATED`, comme `PRODUCE`.
- Categorie Submersible ("toutes les pieces de type (S)") : seules la coque, les
  generateurs et les modules de deplacement portent la dimension F/M/L/S dans le
  catalogue (le module principal, le systeme et le module de controle portent la
  dimension 2D/3D) ; la verification porte donc sur ces pieces.
- Un template est rejete si aucune des 4 categories (Aerien/Marin/Terrestre/
  Submersible) n'est satisfaite.
- Une regle de categorie portant sur "un module de deplacement (X)" est satisfaite
  des qu'**au moins un** des modules de deplacement du drone porte le tag X. Pour
  Submersible, "toutes les pieces (S)" exige que **tous** les generateurs et modules
  de deplacement portent le tag S.
- Assemblage a plusieurs generateurs/modules de deplacement (pas d'exemple dans le
  sujet) : chaque piece supplementaire est repliee dans l'accumulateur deja en place
  (`ASSEMBLE TMP1 TMP1 GeneratorK`), en gardant l'ordre existant (generateurs avant
  le module principal, deplacement apres la coque).
- `WITH`/`WITHOUT`/`REPLACE` s'appliquent une fois par ligne de commande (pas
  multiplies par la quantite de drones), et affectent `NEEDED_STOCKS`, `VERIFY` et
  `PRODUCE`. `INSTRUCTIONS` continue d'imprimer la sequence du template de base : le
  sujet ne montre aucun exemple de sortie `INSTRUCTIONS` avec modificateurs, ce cas
  n'est donc pas gere par cette version.
- Le separateur `;` est utilise des qu'un des mots-cles `WITH`/`WITHOUT`/`REPLACE`
  apparait dans les `ARGS` ; sinon `,` comme avant (retrocompatible).
- `RECEIVE` ne gere que pieces et drones, pas les "assemblages" (qui ne sont dans ce
  projet qu'une notation d'affichage temporaire, jamais un objet de stock).
- Format de `GET_MOVEMENTS` : une ligne par mouvement,
  `<+/-quantite> <nom> (<origine>)`, ex. `-1 Hull_HF1 (PRODUCE)`,
  `+5 Move_MF1 (RECEIVE)`. Le sujet n'impose pas de format precis.

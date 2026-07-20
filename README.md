# Drone Livrable 1

Ce dossier contient une version proof of concept du sujet "usine de drones".

## Compiler

Depuis ce dossier, copie-colle la commande complete sur une seule ligne :

```powershell
.\build.ps1
```

ou

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

L'executable est genere ici :

```text
bin\DroneFactory.exe
```

## Lancer

Puis lance le programme avec :

```powershell
.\bin\DroneFactory.exe
```

Une fois le programme lance, tape une commande puis appuie sur Entree.

Exemples d'entrees :

```text
STOCKS
NEEDED_STOCKS 1 DXF-1, 2 RDL-1
INSTRUCTIONS 1 DXF-1
VERIFY 1 DXF-1, 1 Cat
PRODUCE 1 DXF-1
```

## Hypotheses de cette version

- Chaque piece demarre avec un stock de 10 exemplaires.
- Les drones produits demarrent avec un stock de 0.
- Les systemes (`System_SG1`, `System_S3D1`) sont installes mais ne sont pas decomptes du stock, car le sujet les separe des pieces.
- Les instructions d'assemblage suivent l'ordre donne dans l'exemple du sujet.

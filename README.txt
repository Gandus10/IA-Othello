Auteur :
  - André Neto Da Silva
  - Gander Laurent

Cours :
  - Intelligence Artificiel

Superviseur :
  - Hatem Ghorbel

But du projet :

Implementer un IA permettant de jouer au jeu de l'Othello en respectant une interface fourni (IPlayable)

Othello (aussi connu sous le nom Reversi) est un jeu de société combinatoire abstrait opposant deux joueurs.

Il se joue sur un tablier unicolore de 64 cases, 8 sur 8, appelé othellier.
Les joueurs disposent de 64 pions bicolores, noirs d'un côté et blancs de l'autre.
En début de partie, quatre pions sont déjà placés au centre de l'othellier : deux noirs, en e4 et d5, et deux blancs, en d4 et e5.
Chaque joueur, noir et blanc, pose l'un après l'autre un pion de sa couleur sur l'othellier selon des règles précises.
Le jeu s'arrête quand les deux joueurs ne peuvent plus poser de pion. On compte alors le nombre de pions.
Le joueur ayant le plus grand nombre de pions de sa couleur sur l'othellier a gagné.

(source : wikipédia)

Implémentation de la fonction d'évaluation:
Nous avons  implementer notre IA de la manière dites de la mobilité, c'est à dire qu'on va réduire les possibilités de l'adversaire,
du fait qu'il ne lui reste que peux de possibilités et qu'ils ne soient pas bons

On doit regarder 2 chose pour évaluer un coup :
 - La mobilité
    A chaque tour, on va compter le nombres de coup possible en utilisant la fonction MovesPossible
 - Le score
    On va compter le score de chaque tiles en faisant state_de_la_tile * score_de_la_tile avec :
    - state = (
        Empty = 0,
        Anonymous(Noir) = -1,
        Nsa(Blanc) = 1
      )
    - score (
      {60, 5, 30, 27, 27, 30, 5, 60},
      {5, 0, 20, 20, 20, 20, 0, 5},
      {30, 20, 40, 30, 30, 40, 20, 30},
      {27, 20, 30, 40, 40, 30, 20, 27},
      {27, 20, 30, 40, 40, 30, 20, 27},
      {30, 20, 40, 30, 30, 40, 20, 30},
      {5, 0, 20, 20, 20, 20, 0, 5},
      {60, 5, 30, 27, 27, 30, 5, 60}
    )
  - nombre de tile utilisé
    on prend le score de la NSA (blanc) + le score d'anonymous (noirs)

  Ensuite on attribut un poids à chaque valeur
    weightMobility = 4
    weightScore = 6

  Et pour finir on retourne (64 - numPawns) * weightMobility * mobilité + numPawns * weightScore * score;

# PC Cleaner Pro v1.0

PC Cleaner Pro est un utilitaire de nettoyage système avancé écrit en C#. Il permet de libérer de l'espace disque en supprimant les fichiers inutiles et temporaires de votre système.

## Fonctionnalités

- **Affichage des informations du disque** :
  - Affiche les informations sur les disques durs, y compris l'espace total, l'espace utilisé et l'espace libre.
  - Affiche une barre de progression pour visualiser l'utilisation du disque.

- **Menu principal** :
  - Propose plusieurs options de nettoyage et d'analyse :
    1. Analyser l'espace récupérable.
    2. Nettoyer le cache des navigateurs.
    3. Nettoyer le cache Windows.
    4. Nettoyer les fichiers temporaires.
    5. Nettoyer la corbeille.
    6. Nettoyer les fichiers de mise à jour Windows.
    7. Nettoyer les profils obsolètes.
    8. Nettoyer les miniatures.
    9. Voir l'espace total récupérable.
    10. Tout nettoyer.
    11. Quitter.

- **Analyse de l'espace récupérable** :
  - Analyse différentes catégories de fichiers pour déterminer l'espace qui peut être récupéré :
    - Cache des navigateurs.
    - Cache Windows.
    - Fichiers temporaires.
    - Corbeille.
    - Fichiers de mise à jour Windows.
    - Miniatures.

- **Nettoyage** :
  - Nettoie les fichiers inutiles dans les catégories suivantes :
    - Cache des navigateurs (Chrome, Firefox, Edge).
    - Cache Windows (dossiers Temp, Prefetch, SoftwareDistribution).
    - Fichiers temporaires.
    - Corbeille.
    - Fichiers de mise à jour Windows.
    - Miniatures.
    - Profils utilisateurs obsolètes.

- **Affichage de l'espace récupérable** :
  - Affiche un récapitulatif de l'espace récupérable par catégorie après analyse.

- **Vérification des droits administrateur** :
  - Vérifie si l'application est exécutée avec des droits administrateur, nécessaires pour certaines opérations de nettoyage.

## Prérequis

- .NET 8.0
- Droits administrateur pour exécuter certaines opérations de nettoyage.

## Installation

1. Clonez le dépôt :
   
2. Ouvrez le projet dans Visual Studio 2022.

3. Compilez et exécutez le projet.

## Utilisation

1. Lancez l'application en tant qu'administrateur.
2. Sélectionnez une option dans le menu principal pour analyser ou nettoyer votre système.
3. Suivez les instructions affichées à l'écran.

## Contribuer

Les contributions sont les bienvenues ! Veuillez suivre les étapes suivantes pour contribuer :

1. Forkez le dépôt.
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/ma-fonctionnalité`).
3. Commitez vos modifications (`git commit -am 'Ajout de ma fonctionnalité'`).
4. Poussez votre branche (`git push origin feature/ma-fonctionnalité`).
5. Ouvrez une Pull Request.

## Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

---

Merci d'utiliser PC Cleaner Pro v1.0 ! Si vous avez des questions ou des suggestions, n'hésitez pas à ouvrir une issue sur GitHub.

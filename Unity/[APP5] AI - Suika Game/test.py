import neat
from mlagents_envs.environment import UnityEnvironment

env = UnityEnvironment(file_name="build/[APP5] AI - Suika Game")

# Définir une fonction d'évaluation (fitness function) pour évaluer les performances des individus
def eval_genomes(genomes, config):
    global ge, nets;
    nbCoup=10;
    ge=[]
    nets=[]
    for genome_id, genome in genomes:
        ge.append(genome)
        net = neat.nn.FeedForwardNetwork.create(genome, config)
        nets.append(net)
        genome.fitness = 0
        # Votre logique d'évaluation ici

    for i in range (0,len(ge)):
        for j in range (0,nbCoup+1):
            #Il faut récupérer les inputs
            #Faut réussir à appeler la fonction CollectObservations du GameManager

            #Il mettre les input dans le réseau de neuronnes
            output = nets[i].activate() #Prend en param les inputs

            #Lance l'action voulu par l'output 
            #On doit réussir à lancer la fonction OnActionReceived du gameManager


            #On récupère Reward et on met à jour fitness


        

def run(path="config.txt"):
    # Charger la configuration par défaut de NEAT
    config = neat.Config(neat.DefaultGenome, neat.DefaultReproduction,
                        neat.DefaultSpeciesSet, neat.DefaultStagnation,
                        path)  # Vous pouvez spécifier un fichier de configuration personnalisé

    # Créer une population initiale
    p = neat.Population(config)

    # Ajouter des rapports pour afficher des statistiques pendant l'évolution
    p.add_reporter(neat.StdOutReporter(True))  # Affiche des statistiques dans la console
    p.add_reporter(neat.StatisticsReporter())   # Collecte des statistiques sur la population

    # Évaluer la population sur 10 générations
    winner = p.run(eval_genomes, 10)

    # Utiliser le gagnant (meilleur individu) pour votre tâche spécifique

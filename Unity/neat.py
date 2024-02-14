import neat-python
from mlagents_envs.environment import UnityEnvironment


def run(config_path):
    config=neat.config.Config(
        neat.DefaultGenome,
        neat.DefaultReproduction,
        neat.DefaultSpeciesSet,
        neat.DefaultStagnation,
        config_path
    )

    pop=neat.Population(config)
    pop.run(eval_genome,50) #On veut run la fonction d'évolution 50 fois (c'est arbitraire)
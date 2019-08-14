import json
from deathinfo import *


def read_file(path):
    with open(path) as f:
        content = f.readlines()
    # you may also want to remove whitespace characters like `\n` at the end of each line
    content = [x.strip() for x in content]
    return content


def read_json(path):
    with open(path) as f:
        content = json.load(f)
        return content


def main():
    file = 'statistics.json'
    content = read_file(file)
    deathInfo = []
    for j in content:
        d = json.loads(j)
        deathInfo.append(DeathInfo(d['SimulationId'], d['DeathTime'], d['BehaviourName'], d['ActionName'], d['Floor'], d['InRoom']))

    for info in deathInfo:
        print(str(info.deathTimeMinute) + ' ' + str(info.deathTimeSeconds))

    victims = {}
    numberOfVictims = []
    allVictims = 0
    for info in deathInfo:
        allVictims += 1
        if info.deathTimeMinute in victims.keys():
            victims[info.deathTimeMinute] += 1
        else:
            victims[info.deathTimeMinute] = 1
    print(victims)
    numberOfVictims.append(allVictims/10)
    print(numberOfVictims)


main()

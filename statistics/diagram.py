import json
from deathinfo import *
import seaborn as sns
import matplotlib.pyplot as plt
import pandas as pd

sns.set(style="ticks", color_codes=True)

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


def read_statistics(file):
    content = read_file(file)
    deathInfo = []
    for j in content:
        d = json.loads(j)
        deathInfo.append(
            DeathInfo(d['SimulationId'], d['DeathTime'], d['BehaviourName'], d['ActionName'], d['Floor'], d['InRoom']))

    victims = {}
    numberOfVictims = []
    allVictims = 0
    for info in deathInfo:
        allVictims += 1
        if info.deathTimeMinute in victims.keys():
            victims[info.deathTimeMinute] += 1
        else:
            victims[info.deathTimeMinute] = 1
    numberOfVictims.append(allVictims / 10)
    return deathInfo


def main():
    #run-----------------------------------
    name = 'run4'
    fmt = '.json'
    filename = name + fmt
    run4 = read_statistics(filename)

    name = 'run7'
    fmt = '.json'
    filename = name + fmt
    run7 = read_statistics(filename)

    name = 'run10'
    fmt = '.json'
    filename = name + fmt
    run10 = read_statistics(filename)

    #hide-----------------------------------
    name = 'hide4'
    fmt = '.json'
    filename = name + fmt
    hide4 = read_statistics(filename)

    name = 'hide7'
    fmt = '.json'
    filename = name + fmt
    hide7 = read_statistics(filename)

    name = 'hide10'
    fmt = '.json'
    filename = name + fmt
    hide10 = read_statistics(filename)

    #fight-----------------------------------
    name = 'fight4'
    fmt = '.json'
    filename = name + fmt
    fight4 = read_statistics(filename)

    name = 'fight7'
    fmt = '.json'
    filename = name + fmt
    fight7 = read_statistics(filename)

    name = 'fight10'
    fmt = '.json'
    filename = name + fmt
    fight10 = read_statistics(filename)

    data = []

    # run
    d1 = get_sample(run4, 'run', 4)
    d2 = get_sample(run7, 'run', 7)
    d3 = get_sample(run10, 'run', 10)
    add_to_data(data, d1, d2, d3)

    # hide
    d1 = get_sample(hide4, 'hide', 4)
    d2 = get_sample(hide7, 'hide', 7)
    d3 = get_sample(hide10, 'hide', 10)
    add_to_data(data, d1, d2, d3)

    # fight
    d1 = get_sample(fight4, 'fight', 4)
    d2 = get_sample(fight7, 'fight', 7)
    d3 = get_sample(fight10, 'fight', 10)
    add_to_data(data, d1, d2, d3)

    df = pd.DataFrame(data, columns=['behaviour','floors','victims'])
    #print(df)
    colors = ["blue", "grey", "red"]
    sns.set_palette(sns.xkcd_palette(colors))
    sns.catplot(x="floors", y="victims", hue="behaviour", kind="bar", data=df)
    plt.show()


def add_to_data(data, d1, d2, d3):
    victims1 = d1[2]
    victims2 = d2[2]
    victims3 = d3[2]
    for i in range(0, 10):
        t1 = [d1[0], d1[1], victims1[i]]
        t2 = [d2[0], d2[1], victims2[i]]
        t3 = [d3[0], d3[1], victims3[i]]
        data.append(t1)
        data.append(t2)
        data.append(t3)

def get_sample(death_info, name, floors):
    victims = []
    for i in range(1,11):
        count = 0
        for di in death_info:
            if di.simulationId == i:
                count += 1
        victims.append(count)

    return [name, floors, victims]


main()

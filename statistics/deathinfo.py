
class DeathInfo:
    def __init__(self, simulationId, deathTime, behaviourName, actionName, floor, inRoom):
        self.simulationId = simulationId
        self.deathTimeSeconds = deathTime
        self.deathTimeMinute = int(int(deathTime) / 60) + 1
        self.behaviourName = behaviourName
        self.actionName = actionName
        self.floor = floor
        self.inRoom = inRoom

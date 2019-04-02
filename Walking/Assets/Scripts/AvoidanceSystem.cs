using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Fuzzy.Library;
using System;

public class AvoidanceSystem
{
    private MamdaniFuzzySystem avoidanceSystem;
    private Dictionary<FuzzyVariable, double> result;


    public void initAvoidanceSystem()
    {
        avoidanceSystem = new MamdaniFuzzySystem();

        //FuzzyTerm near = new FuzzyTerm("Near", new TrapezoidMembershipFunction(-5.0f, 0.0f, 2.0f, 3.0f));
        FuzzyTerm near = new FuzzyTerm("Near", new TriangularMembershipFunction(-0.2f, 0.0f, 0.2f));
        FuzzyTerm far = new FuzzyTerm("Far", new TrapezoidMembershipFunction(0.3f, 10.0f, 20.0f, 30.0f));

        FuzzyVariable rightDistance = new FuzzyVariable("RightDistance", 0.0f, 30.0f);
        FuzzyVariable veryRightDistance = new FuzzyVariable("VeryRightDistance", 0.0f, 30.0f);
        FuzzyVariable leftDistance = new FuzzyVariable("LeftDistance", 0.0f, 30.0f);
        FuzzyVariable veryLeftDistance = new FuzzyVariable("VeryLeftDistance", 0.0f, 30.0f);
        FuzzyVariable frontDistance = new FuzzyVariable("FrontDistance", 0.0f, 30.0f);

        rightDistance.Terms.Add(near);
        rightDistance.Terms.Add(far);

        veryRightDistance.Terms.Add(near);
        veryRightDistance.Terms.Add(far);

        leftDistance.Terms.Add(near);
        leftDistance.Terms.Add(far);

        veryLeftDistance.Terms.Add(near);
        veryLeftDistance.Terms.Add(far);

        frontDistance.Terms.Add(near);
        frontDistance.Terms.Add(far);

        FuzzyTerm fsVN = new FuzzyTerm("VeryNegative", new TriangularMembershipFunction(
        -20f, -15f, -10f));
        FuzzyTerm fsN = new FuzzyTerm("Negative", new TriangularMembershipFunction(
            -15f, -10f, -5f));
        FuzzyTerm fsLN = new FuzzyTerm("LittleNegative", new TriangularMembershipFunction(
            -5f, -2.5f, 0f));
        FuzzyTerm fsZero = new FuzzyTerm("Zero", new TriangularMembershipFunction(
            -1f, 0f, 1f));
        FuzzyTerm fsLP = new FuzzyTerm("LittlePositive", new TriangularMembershipFunction(
            0f, 2.5f, 5f));
        FuzzyTerm fsP = new FuzzyTerm("Positive", new TriangularMembershipFunction(
            5f, 10f, 15f));
        FuzzyTerm fsVP = new FuzzyTerm("VeryPositive", new TriangularMembershipFunction(
            10f, 15f, 20f));
        FuzzyTerm turnAround = new FuzzyTerm("TurnAround", new TriangularMembershipFunction(90f, 90f, 90f));

        FuzzyVariable angle = new FuzzyVariable("Angle", -20f, 20f);
        angle.Terms.Add(fsVN);
        angle.Terms.Add(fsN);
        angle.Terms.Add(fsLN);
        angle.Terms.Add(fsZero);
        angle.Terms.Add(fsLP);
        angle.Terms.Add(fsP);
        angle.Terms.Add(fsVP);
        angle.Terms.Add(turnAround);

        FuzzyTerm slower = new FuzzyTerm("Slower", new TriangularMembershipFunction(-0.2f, -0.1f, 0.0f));
        FuzzyTerm fastWalk = new FuzzyTerm("Faster", new TriangularMembershipFunction(0.0f, 0.1f, 0.2f));

        FuzzyVariable speedChange = new FuzzyVariable("SpeedChange", -0.1f, 0.1f);
        speedChange.Terms.Add(slower);
        speedChange.Terms.Add(fastWalk);

        FuzzyTerm stay = new FuzzyTerm("Stay", new TriangularMembershipFunction(-0.4f, MovementTargets.Stay, 0.4f));
        FuzzyTerm walk = new FuzzyTerm("Walk", new TriangularMembershipFunction(0.0f, MovementTargets.Walk, 0.4f));
        FuzzyTerm run = new FuzzyTerm("Run", new TriangularMembershipFunction(0.4f, MovementTargets.Run, 1.5f));

        FuzzyVariable movement = new FuzzyVariable("CurrentMovement", 0.0f, 15.0f);
        movement.Terms.Add(stay);
        movement.Terms.Add(walk);
        movement.Terms.Add(run);

        FuzzyVariable targetMovement = new FuzzyVariable("TargetMovement", 0.0f, 15.0f);
        targetMovement.Terms.Add(stay);
        targetMovement.Terms.Add(walk);
        targetMovement.Terms.Add(run);

        FuzzyTerm isStatic = new FuzzyTerm("Static", new ConstantMembershipFunction(1.0f));
        FuzzyVariable colliderType = new FuzzyVariable("ColliderType", 0.0f, 1.0f);
        colliderType.Terms.Add(isStatic);


        avoidanceSystem.Input.Add(leftDistance);
        avoidanceSystem.Input.Add(veryLeftDistance);
        avoidanceSystem.Input.Add(veryRightDistance);
        avoidanceSystem.Input.Add(rightDistance);
        avoidanceSystem.Input.Add(frontDistance);
        avoidanceSystem.Input.Add(movement);
        avoidanceSystem.Input.Add(targetMovement);
        avoidanceSystem.Input.Add(colliderType);

        avoidanceSystem.Output.Add(angle);
        avoidanceSystem.Output.Add(speedChange);

        try
        {
            RulesSystem.RulesForCrowdAvoidance(avoidanceSystem);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void Calculate(float rightDist, float veryRightDist, float leftDist, float veryLeftDist, float frontDist, float speed, float targetSpeed, bool isStatic = false)
    {
        FuzzyVariable right = avoidanceSystem.InputByName("RightDistance");
        FuzzyVariable veryRight = avoidanceSystem.InputByName("VeryRightDistance");
        FuzzyVariable left = avoidanceSystem.InputByName("LeftDistance");
        FuzzyVariable veryLeft = avoidanceSystem.InputByName("VeryLeftDistance");
        FuzzyVariable front = avoidanceSystem.InputByName("FrontDistance");
        FuzzyVariable currentMovement = avoidanceSystem.InputByName("CurrentMovement");
        FuzzyVariable targetMovement = avoidanceSystem.InputByName("TargetMovement");
        FuzzyVariable colliderType = avoidanceSystem.InputByName("ColliderType");

        Dictionary<FuzzyVariable, double> inputValues = new Dictionary<FuzzyVariable, double>();
        inputValues.Add(right, rightDist);
        inputValues.Add(left, leftDist);
        inputValues.Add(veryLeft, veryLeftDist);
        inputValues.Add(veryRight, veryRightDist);
        inputValues.Add(front, frontDist);
        inputValues.Add(currentMovement, speed);
        inputValues.Add(targetMovement, targetSpeed);
        inputValues.Add(colliderType, isStatic ? 1 : 0);

        result = avoidanceSystem.Calculate(inputValues);
    }

    public float GetOutputValue(string output)
    {
        if (result == null || (result != null && result.Keys.Count == 0)) return -999.0f;
        FuzzyVariable value = avoidanceSystem.OutputByName(output);
        return float.IsNaN((float)result[value]) ? -999.0f : (float)result[value];
    }
}
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
        FuzzyTerm near = new FuzzyTerm("Near", new TrapezoidMembershipFunction(-5.0f, 0.0f, 2f, 3.0f));
        FuzzyTerm far = new FuzzyTerm("Far", new TrapezoidMembershipFunction(1.0f, 10.0f, 20.0f, 30.0f));

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
        -50f, -35f, -25f));
        FuzzyTerm fsN = new FuzzyTerm("Negative", new TriangularMembershipFunction(
            -35f, -25f, -15f));
        FuzzyTerm fsLN = new FuzzyTerm("LittleNegative", new TriangularMembershipFunction(
            -15f, -7.5f, -3f));
        FuzzyTerm fsZero = new FuzzyTerm("Zero", new TriangularMembershipFunction(
            -3f, 0f, 3f));
        FuzzyTerm fsLP = new FuzzyTerm("LittlePositive", new TriangularMembershipFunction(
            3f, 7.5f, 15f));
        FuzzyTerm fsP = new FuzzyTerm("Positive", new TriangularMembershipFunction(
            15f, 25f, 35f));
        FuzzyTerm fsVP = new FuzzyTerm("VeryPositive", new TriangularMembershipFunction(
            25f, 35f, 50f));

        FuzzyVariable angle = new FuzzyVariable("Angle", -40f, 40f);
        angle.Terms.Add(fsVN);
        angle.Terms.Add(fsN);
        angle.Terms.Add(fsLN);
        angle.Terms.Add(fsZero);
        angle.Terms.Add(fsLP);
        angle.Terms.Add(fsP);
        angle.Terms.Add(fsVP);

        FuzzyTerm slower = new FuzzyTerm("Slower", new TriangularMembershipFunction(-1.0f, -0.5f, 0.0f));
        FuzzyTerm fastWalk = new FuzzyTerm("Faster", new TriangularMembershipFunction(0.0f, 0.5f, 1.0f));

        FuzzyVariable speedChange = new FuzzyVariable("SpeedChange", -0.2f, 0.2f);
        speedChange.Terms.Add(slower);
        speedChange.Terms.Add(fastWalk);

        FuzzyTerm stay = new FuzzyTerm("Stay", new TriangularMembershipFunction(-1f, MovementTargets.Stay, 1f));
        FuzzyTerm walk = new FuzzyTerm("Walk", new TriangularMembershipFunction(0.0f, MovementTargets.Walk, 2.5f));
        FuzzyTerm run = new FuzzyTerm("Run", new TriangularMembershipFunction(3f, MovementTargets.Run, 7.5f));

        FuzzyVariable movement = new FuzzyVariable("CurrentMovement", 0.0f, 15.0f);
        movement.Terms.Add(stay);
        movement.Terms.Add(walk);
        movement.Terms.Add(run);

        FuzzyVariable targetMovement = new FuzzyVariable("TargetMovement", 0.0f, 15.0f);
        targetMovement.Terms.Add(stay);
        targetMovement.Terms.Add(walk);
        targetMovement.Terms.Add(run);


        avoidanceSystem.Input.Add(leftDistance);
        avoidanceSystem.Input.Add(veryLeftDistance);
        avoidanceSystem.Input.Add(veryRightDistance);
        avoidanceSystem.Input.Add(rightDistance);
        avoidanceSystem.Input.Add(frontDistance);
        avoidanceSystem.Input.Add(movement);
        avoidanceSystem.Input.Add(targetMovement);

        avoidanceSystem.Output.Add(angle);
        avoidanceSystem.Output.Add(speedChange);

        try
        {
            //angle
            MamdaniFuzzyRule rule1 = avoidanceSystem.ParseRule("if (FrontDistance is Far) then (Angle is Zero)");

            MamdaniFuzzyRule rule2 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Near) and (VeryRightDistance is Far) then (Angle is Negative)");
            MamdaniFuzzyRule rule3 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Far) and (VeryRightDistance is Near) then (Angle is LittleNegative)");

            MamdaniFuzzyRule rule4 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is Positive)");
            MamdaniFuzzyRule rule5 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittlePositive)");

            MamdaniFuzzyRule rule6 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is Positive)");
            MamdaniFuzzyRule rule7 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is VeryPositive)");

            MamdaniFuzzyRule rule8 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittleNegative)");
            MamdaniFuzzyRule rule9 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is LittlePositive)");

            MamdaniFuzzyRule rule10 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) then (Angle is Negative)");
            MamdaniFuzzyRule rule11 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is Positive)");

            MamdaniFuzzyRule rule12 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Far) then (Angle is VeryNegative)");
            MamdaniFuzzyRule rule13 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Far) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is VeryPositive)");

            MamdaniFuzzyRule rule14 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Far) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is LittlePositive)");
            MamdaniFuzzyRule rule15 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Near) then (Angle is LittleNegative)");

            MamdaniFuzzyRule rule16 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (RightDistance is Near) and (VeryRightDistance is Near) and (LeftDistance is Near) and (VeryLeftDistance is Near) then (Angle is Zero)");

            //speed
            MamdaniFuzzyRule rule17 = avoidanceSystem.ParseRule("if ((FrontDistance is Far) or (FrontDistance is slightly Near)) and (CurrentMovement is Stay) and (TargetMovement is Walk) then (SpeedChange is Faster)");
            MamdaniFuzzyRule rule18 = avoidanceSystem.ParseRule("if ((FrontDistance is Far) or (FrontDistance is slightly Near)) and (CurrentMovement is Stay) and (TargetMovement is Run) then (SpeedChange is Faster)");
            MamdaniFuzzyRule rule19 = avoidanceSystem.ParseRule("if ((FrontDistance is Far) or (FrontDistance is slightly Near)) and (CurrentMovement is Walk) and (TargetMovement is Run) then (SpeedChange is Faster)");

            MamdaniFuzzyRule rule20 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Run) and (TargetMovement is Walk) then (SpeedChange is Slower)");
            MamdaniFuzzyRule rule21 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Run) and (TargetMovement is Stay) then (SpeedChange is Slower)");
            MamdaniFuzzyRule rule22 = avoidanceSystem.ParseRule("if ((FrontDistance is Far)) and (CurrentMovement is Walk) and (TargetMovement is Stay) then (SpeedChange is Slower)");

            MamdaniFuzzyRule rule23 = avoidanceSystem.ParseRule("if (FrontDistance is Near) then (SpeedChange is Slower)");

            MamdaniFuzzyRule rule24 = avoidanceSystem.ParseRule("if (FrontDistance is Far) and (CurrentMovement is slightly Walk) and (TargetMovement is Walk) then (SpeedChange is Faster)");

            MamdaniFuzzyRule rule25 = avoidanceSystem.ParseRule("if (FrontDistance is Near) and (LeftDistance is Far) and (VeryLeftDistance is Far) and (RightDistance is Far) and (VeryRightDistance is Far) then (Angle is Positive)");

            avoidanceSystem.Rules.Add(rule1);
            avoidanceSystem.Rules.Add(rule2);
            avoidanceSystem.Rules.Add(rule3);
            avoidanceSystem.Rules.Add(rule4);
            avoidanceSystem.Rules.Add(rule5);
            avoidanceSystem.Rules.Add(rule6);
            avoidanceSystem.Rules.Add(rule7);
            avoidanceSystem.Rules.Add(rule8);
            avoidanceSystem.Rules.Add(rule9);
            avoidanceSystem.Rules.Add(rule10);
            avoidanceSystem.Rules.Add(rule11);
            avoidanceSystem.Rules.Add(rule12);
            avoidanceSystem.Rules.Add(rule13);
            avoidanceSystem.Rules.Add(rule14);
            avoidanceSystem.Rules.Add(rule15);
            avoidanceSystem.Rules.Add(rule16);
            avoidanceSystem.Rules.Add(rule17);
            avoidanceSystem.Rules.Add(rule18);
            avoidanceSystem.Rules.Add(rule19);
            avoidanceSystem.Rules.Add(rule20);
            avoidanceSystem.Rules.Add(rule21);
            avoidanceSystem.Rules.Add(rule22);
            avoidanceSystem.Rules.Add(rule23);
            avoidanceSystem.Rules.Add(rule24);
            avoidanceSystem.Rules.Add(rule25);

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void Calculate(float rightDist, float veryRightDist, float leftDist, float veryLeftDist, float frontDist, float speed, float targetSpeed)
    {
        FuzzyVariable right = avoidanceSystem.InputByName("RightDistance");
        FuzzyVariable veryRight = avoidanceSystem.InputByName("VeryRightDistance");
        FuzzyVariable left = avoidanceSystem.InputByName("LeftDistance");
        FuzzyVariable veryLeft = avoidanceSystem.InputByName("VeryLeftDistance");
        FuzzyVariable front = avoidanceSystem.InputByName("FrontDistance");
        FuzzyVariable currentMovement = avoidanceSystem.InputByName("CurrentMovement");
        FuzzyVariable targetMovement = avoidanceSystem.InputByName("TargetMovement");

        Dictionary<FuzzyVariable, double> inputValues = new Dictionary<FuzzyVariable, double>();
        inputValues.Add(right, rightDist);
        inputValues.Add(left, leftDist);
        inputValues.Add(veryLeft, veryLeftDist);
        inputValues.Add(veryRight, veryRightDist);
        inputValues.Add(front, frontDist);
        inputValues.Add(currentMovement, speed);
        inputValues.Add(targetMovement, targetSpeed);

        result = avoidanceSystem.Calculate(inputValues);
    }

    public float GetOutputValue(string output)
    {
        if (result == null || (result != null && result.Keys.Count == 0)) return -999.0f;
        FuzzyVariable value = avoidanceSystem.OutputByName(output);
        return float.IsNaN((float)result[value]) ? -999.0f : (float)result[value];
    }
}